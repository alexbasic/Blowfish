using Microsoft.VisualStudio.TestTools.UnitTesting;
using NikonovAV.HM.BlowfishCrypt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NikonovAV.HM.Container.Test
{
    [TestClass]
    public class CryptContainerStorageTest
    {
        internal class MockCrypter : ICrypter
        {
            public ulong[] EncryptArray(ulong[] data)
            {
                return data;
            }

            public ulong[] DecryptArray(ulong[] data)
            {
                return data;
            }

            public void Encrypt(Stream stream)
            {
                //
            }

            public void Decrypt(Stream stream)
            {
                //
            }

            public ulong EncryptBlock(ulong dataBlock)
            {
                return dataBlock;
            }

            public ulong DecryptBlock(ulong dataBlock)
            {
                return dataBlock;
            }
        }

        [TestMethod]
        public void PackShouldBeCorrectSize() 
        {
            int originalSize = 100004;
            int expectedPackedSize = 100016;
            MemoryStream original = new MemoryStream(originalSize);
            byte randomData = 200;
            for (var i = 0; i < originalSize; i++)
            {
                original.WriteByte(randomData);
            }
            MemoryStream inconteiner = new MemoryStream();
            CryptContainerStorage cryptContainerStorage = new CryptContainerStorage(original, inconteiner, new MockCrypter());

            cryptContainerStorage.Pack();

            //размер запакованного = (размер исходного расширенный до крастности 8) + 8 байт 
            Assert.AreEqual(expectedPackedSize, inconteiner.Length);

            inconteiner.Position = 0;
            byte[] buffer = new byte[8];
            inconteiner.Read(buffer, 0, 8);
            long writedSizeVariable = ArrayToLong(buffer);
        }

        [TestMethod]
        public void PackShouldBeCorrect_WritedSizeVariable()
        {
            int originalSize = 100004;
            int expectedPackedSize = 100016;
            MemoryStream original = new MemoryStream(originalSize);
            byte randomData = 200;
            for (var i = 0; i < originalSize; i++)
            {
                original.WriteByte(randomData);
            }
            MemoryStream inconteiner = new MemoryStream();
            CryptContainerStorage cryptContainerStorage = new CryptContainerStorage(original, inconteiner, new MockCrypter());

            cryptContainerStorage.Pack();

            inconteiner.Position = 0;
            byte[] buffer = new byte[8];
            inconteiner.Read(buffer, 0, 8);
            long writedSizeVariable = ArrayToLong(buffer);

            Assert.AreEqual(originalSize, writedSizeVariable);
        }

        [TestMethod]
        public void ExtractShouldBeCorrectSize()
        {
            long originalSize = 100004;
            long packedSize = 100016;
            MemoryStream original = new MemoryStream();
            MemoryStream inconteiner = new MemoryStream((int)packedSize);
            byte randomData = 200;
            byte[] sizeValue = LongToArray(originalSize);
            inconteiner.Write(sizeValue, 0, 8);
            for (var i = 0; i < originalSize; i++)
            {
                inconteiner.WriteByte(randomData);
            }

            original = new MemoryStream(0);
            CryptContainerStorage cryptContainerStorageForUnpack = new CryptContainerStorage(original, inconteiner, new MockCrypter());

            cryptContainerStorageForUnpack.Extract();

            //Извлеченный должен быть как исходный
            Assert.AreEqual(originalSize, original.Length);
        }

        [TestMethod]
        public void ShouldBeCorrectPackAndExtractWithoutCrypter()
        {
            //init
            int originalSize = 100004;
            int expectedPackedSize = 100016;
            MemoryStream original = new MemoryStream(originalSize);
            Random rnd = new Random();
            for (var i = 0; i < originalSize; i++)
            {
                original.WriteByte((byte)rnd.Next(0, 255));
            }

            //encrypt
            MemoryStream inconteiner = new MemoryStream();
            CryptContainerStorage cryptContainerStorage = new CryptContainerStorage(original, inconteiner, new MockCrypter());
            cryptContainerStorage.Pack();

            //decrypt
            MemoryStream destination = new MemoryStream();
            CryptContainerStorage cryptContainerStorage2 = new CryptContainerStorage(destination, inconteiner, new MockCrypter());
            cryptContainerStorage2.Extract();

            //asserts
            Assert.AreEqual(original.Length, destination.Length);
            original.Position = 0;
            destination.Position = 0;
            for (var i = 0; i < originalSize; i++)
            {
                int originalByte = original.ReadByte();
                int destinationByte = destination.ReadByte();
                Assert.AreEqual(originalByte, destinationByte);
                Assert.AreNotEqual(originalByte, -1);
                Assert.AreNotEqual(destinationByte, -1);
            }
        }

        [TestMethod]
        public void ShouldBeCorrectPackAndExtract()
        {
            //init
            int originalSize = 16;//100004;
            int expectedPackedSize = 16;//100016;
            MemoryStream original = new MemoryStream(originalSize);
            Random rnd = new Random();
            for (var i = 0; i < originalSize; i++)
            {
                original.WriteByte((byte)rnd.Next(0, 255));
            }
            byte[] key = new byte[40];
            byte[] key2 = new byte[40];
            Random rnd2 = new Random();
            rnd2.NextBytes(key);
            Array.Copy(key, key2, key.Length);

            //encrypt
            var crypter1 = new Blowfish(key);
            MemoryStream inconteiner = new MemoryStream();
            CryptContainerStorage cryptContainerStorage = new CryptContainerStorage(original, inconteiner, crypter1);
            cryptContainerStorage.Pack();

            //decrypt
            var crypter2 = new Blowfish(key2);
            MemoryStream destination = new MemoryStream();
            inconteiner.Position = 0;
            CryptContainerStorage cryptContainerStorage2 = new CryptContainerStorage(destination, inconteiner, crypter2);
            cryptContainerStorage2.Extract();

            //asserts
            Assert.AreEqual(original.Length, destination.Length);
            original.Position = 0;
            destination.Position = 0;
            for (var i = 0; i < originalSize; i++)
            {
                int originalByte = original.ReadByte();
                int destinationByte = destination.ReadByte();
                Assert.AreEqual(originalByte, destinationByte);
                Assert.AreNotEqual(originalByte, -1);
                Assert.AreNotEqual(destinationByte, -1);
            }
        }

        [TestMethod]
        public void ShouldBeCrypt()
        {
            //init
            int originalSize = 100004;
            int expectedPackedSize = 100016;
            MemoryStream original = new MemoryStream(originalSize);
            Random rnd = new Random();
            for (var i = 0; i < originalSize; i++)
            {
                original.WriteByte((byte)rnd.Next(0, 255));
            }
            byte[] key = new byte[40];
            Random rnd2 = new Random();
            rnd2.NextBytes(key);

            //encrypt
            MemoryStream inconteiner = new MemoryStream();
            CryptContainerStorage cryptContainerStorage = new CryptContainerStorage(original, inconteiner, new Blowfish(key));
            cryptContainerStorage.Pack();

            original.Position = 0;
            inconteiner.Position = 8;
            int _originalByte = original.ReadByte();
            int _destinationByte = inconteiner.ReadByte();
            long counter = 0;
            for (var i = 0; i < originalSize-1; i++)
            {
                int originalByte = original.ReadByte();
                int destinationByte = inconteiner.ReadByte();
                if (originalByte == destinationByte) 
                {
                    counter++;
                }
            }

            Assert.IsTrue(((float)counter / (float)originalSize) <0.05);
        }

        [TestMethod]
        public void ShouldBecorrectWriteNotCrypt()
        {
            //init
            int originalSize = 100004;
            int expectedPackedSize = 100016;
            MemoryStream original = new MemoryStream(originalSize);
            Random rnd = new Random();
            for (var i = 0; i < originalSize; i++)
            {
                original.WriteByte((byte)rnd.Next(0, 255));
            }
            byte[] key = new byte[40];
            Random rnd2 = new Random();
            rnd2.NextBytes(key);

            //encrypt
            MemoryStream inconteiner = new MemoryStream();
            CryptContainerStorage cryptContainerStorage = new CryptContainerStorage(original, inconteiner, new MockCrypter());
            cryptContainerStorage.Pack();

            original.Position = 0;
            inconteiner.Position = 8;
            for (var i = 0; i < originalSize; i++)
            {
                int originalByte = original.ReadByte();
                int destinationByte = inconteiner.ReadByte();
                Assert.AreEqual(originalByte, destinationByte);
                Assert.AreNotEqual(originalByte, -1);
                Assert.AreNotEqual(destinationByte, -1);
            }
        }

        private byte[] LongToArray(long original) 
        {
            byte[] result = new byte[8];
            result[7] = (byte)(original & 0xFF);
            original >>= 8;
            result[6] = (byte)(original & 0xFF);
            original >>= 8;
            result[5] = (byte)(original & 0xFF);
            original >>= 8;
            result[4] = (byte)(original & 0xFF);
            original >>= 8;
            result[3] = (byte)(original & 0xFF);
            original >>= 8;
            result[2] = (byte)(original & 0xFF);
            original >>= 8;
            result[1] = (byte)(original & 0xFF);
            original >>= 8;
            result[0] = (byte)(original & 0xFF);
            return result;
        }

        private long ArrayToLong(byte[] longArrayValue)
        {
            long result =
                (((long)longArrayValue[0]) << 56) |
                (((long)longArrayValue[1]) << 48) |
                (((long)longArrayValue[2]) << 40) |
                (((long)longArrayValue[3]) << 32) |
                (((long)longArrayValue[4]) << 24) |
                (((long)longArrayValue[5]) << 16) |
                (((long)longArrayValue[6]) << 8) |
                (((long)longArrayValue[7]));
            return result;
        }
    }
}
