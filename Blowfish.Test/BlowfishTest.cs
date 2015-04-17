using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NikonovAV.HM.BlowfishCrypt;

namespace NikonovAV.HM.BlowfishCrypt.Test
{
    [TestClass]
    public class BlowfishTest
    {
        [TestMethod]
        public void EncryptDecryptBlock_NonCryptedKeys()
        {
            byte[] key = new byte[40];
            var expectedLeft = 10U;
            var expectedRight = 235U;
            var blowFeistel = new BlowFeistel();
            var context = new BlowfishContext(key);
            UInt32 left = expectedLeft;
            UInt32 right = expectedRight;
            blowFeistel.BlowfishEncrypt(context, ref left, ref right);
            blowFeistel.BlowfishDecrypt(context, ref left, ref right);
            Assert.AreEqual(left, expectedLeft);
            Assert.AreEqual(right, expectedRight);
        }

        [TestMethod]
        public void EncryptDecryptBlock_CryptedKeys()
        {
            byte[] key = new byte[56];
            var expectedLeft = 10U;
            var expectedRight = 235U;
            var blowFeistel = new BlowFeistel();
            Random rnd = new Random();
            rnd.NextBytes(key);
            var context = new BlowfishContext(key);
            UInt32 left = expectedLeft;
            UInt32 right = expectedRight;
            blowFeistel.CryptPkeys(context);
            blowFeistel.BlowfishEncrypt(context, ref left, ref right);
            blowFeistel.BlowfishDecrypt(context, ref left, ref right);
            Assert.AreEqual(left, expectedLeft);
            Assert.AreEqual(right, expectedRight);
        }

        [TestMethod]
        public void EncryptDecryptArray()
        {
            #region init
            int dataLength = 13107200;
            ulong[] dataArray = new ulong[dataLength];
            ulong[] originalDataArray = new ulong[dataLength];
            int keyLength = 56;
            byte[] key = new byte[keyLength];

            Random rnd = new Random();
            rnd.NextBytes(key);
            for(var i =0;i<dataLength;i++)
            {
                uint r = (uint)rnd.Next();
                ulong l = (uint)rnd.Next();
                dataArray[i] = (l << 32) | r;
                originalDataArray[i] = dataArray[i];
            }
            #endregion

            Blowfish blowfish = new Blowfish(key);

            blowfish.EncryptArray(dataArray);

            for (var i = 0; i < dataLength; i++)
            {
                Assert.IsFalse(dataArray[i] == originalDataArray[i]);
            }

            blowfish.DecryptArray(dataArray);

            for (var i = 0; i < dataLength; i++)
            {
                Assert.IsTrue(dataArray[i] == originalDataArray[i]);
            }
        }

        [TestMethod]
        public void EncryptDecryptArray_KeyDoNotMutate()
        {
            #region init
            int dataLength = 13107200;
            ulong[] dataArray = new ulong[dataLength];
            ulong[] originalDataArray = new ulong[dataLength];
            int keyLength = 56;
            byte[] key = new byte[keyLength];

            Random rnd = new Random();
            rnd.NextBytes(key);
            byte[] originalKey = new byte[keyLength];
            Array.Copy(key, originalKey, key.Length);
            for (var i = 0; i < dataLength; i++)
            {
                uint r = (uint)rnd.Next();
                ulong l = (uint)rnd.Next();
                dataArray[i] = (l << 32) | r;
                originalDataArray[i] = dataArray[i];
            }
            #endregion

            Blowfish blowfish = new Blowfish(key);

            blowfish.EncryptArray(dataArray);

            Blowfish blowfish2 = new Blowfish(key);

            blowfish2.DecryptArray(dataArray);

            for (var i = 0; i < key.Length; i++)
            {
                Assert.IsTrue(key[i] == originalKey[i]);
            }
        }
    }
}
