using NikonovAV.HM.BlowfishCrypt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NikonovAV.HM.BlowfishCrypt
{
    public class BlowfishContext
    {
        private const int maxKeyLengthInBytes = 56; //size to 448bit
        public byte[] Key { get; set; }
        public UInt32[] PKeys { get; set; }
        public UInt32[][] SKeys { get; set; }

        public BlowfishContext(byte[] key)
        {
            if (key.Length > maxKeyLengthInBytes)
            {
                throw new ArgumentException(string.Format("Maximum key length should be {0} bit", maxKeyLengthInBytes));
            }
            Key = new byte[key.Length];
            Array.Copy(key, Key, key.Length);
            PKeys = MakePKeys();
            InitSKeys();
        }

        private UInt32[] MakePKeys()
        {
            UInt32[] result = new UInt32[Constants.PArray.Length];
            Array.Copy(Constants.PArray, result, Constants.PArray.Length);
            var keyLength = Key.Length;
            for (int i = 0, k = 0; i < 18; i++)
            {
                uint longKeyValue = 0;
                for (int j = 0; j < 4; j++, k++)
                {
                    longKeyValue = (longKeyValue << 8) | Key[k % keyLength];
                }
                result[i] ^= longKeyValue;
            }

            return result;
        }

        private void InitSKeys() 
        {
            SKeys = new UInt32[4][];
            SKeys[0] = new UInt32[256];
            SKeys[1] = new UInt32[256];
            SKeys[2] = new UInt32[256];
            SKeys[3] = new UInt32[256];

            Array.Copy(Constants.sbox0, SKeys[0], Constants.sbox0.Length);
            Array.Copy(Constants.sbox1, SKeys[1], Constants.sbox1.Length);
            Array.Copy(Constants.sbox2, SKeys[2], Constants.sbox2.Length);
            Array.Copy(Constants.sbox3, SKeys[3], Constants.sbox3.Length);
        }
    }
}
