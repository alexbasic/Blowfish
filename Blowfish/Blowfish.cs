using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NikonovAV.HM.BlowfishCrypt
{
    public class Blowfish
    {
        BlowfishContext context;
        BlowFeistel blowFeistel;

        public Blowfish(byte[] key)
        {
            context = new BlowfishContext(key);
            blowFeistel = new BlowFeistel();
            blowFeistel.CryptPkeys(context);
        }

        public ulong[] EncryptArray(ulong[] data)
        {
            long length = data.LongLength;
            ulong[] result = new ulong[length];
            for (long i = 0; i < length; i++)
            {
                UInt64 block = data[i];
                uint right = (uint)(block & 0x00000000FFFFFFFF);
                uint left = (uint)((block >> 32) & 0x00000000FFFFFFFF);
                blowFeistel.BlowfishEncrypt(context, ref left, ref right);
                ulong ecrypted = (ulong)left;
                data[i] = (ecrypted << 32) | right;
            }
            return result;
        }

        public ulong[] DecryptArray(ulong[] data)
        {
            long length = data.LongLength;
            ulong[] result = new ulong[length];
            for (long i = 0; i < length; i++)
            {
                UInt64 block = data[i];
                uint right = (uint)(block & 0x00000000FFFFFFFF);
                uint left = (uint)((block >> 32) & 0x00000000FFFFFFFF);
                blowFeistel.BlowfishDecrypt(context, ref left, ref right);
                ulong decrypted = (ulong)left;
                data[i] = (decrypted << 32) | right;
            }
            return result;
        }
    }
}
