using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NikonovAV.HM.BlowfishCrypt
{
    public interface ICrypter
    {
        ulong[] EncryptArray(ulong[] data);
        ulong[] DecryptArray(ulong[] data);
        void Encrypt(Stream stream);
        void Decrypt(Stream stream);
        ulong EncryptBlock(ulong dataBlock);
        ulong DecryptBlock(ulong dataBlock);
    }

    public class Blowfish : ICrypter
    {
        BlowfishContext context;
        BlowFeistel blowFeistel;

        public Blowfish(byte[] key)
        {
            context = new BlowfishContext(key);
            blowFeistel = new BlowFeistel();
            blowFeistel.CryptPkeys(context);
        }

        public void Encrypt(Stream stream)
        {
            AlignedVerif(stream);
            //TODO добавить код обработки
            long sizeInBlock = stream.Length / 8;
            byte[] buffer = new byte[8];
            stream.Position = 0;
            for (var i = 0; i < sizeInBlock; i++)
            {
                stream.Read(buffer, 0, 8);
                ulong data = (ulong)ArrayToLong(buffer);
                data = EncryptBlock(data);
                stream.Position = stream.Position - 8;
                stream.Write(LongToArray((long)data), 0, 8);
            }
        }

        public void Decrypt(Stream stream)
        {
            AlignedVerif(stream);
            //TODO добавить код обработки
            long sizeInBlock = stream.Length / 8;
            byte[] buffer = new byte[8];
            stream.Position = 0;
            for (var i = 0; i < sizeInBlock; i++)
            {
                stream.Read(buffer, 0, 8);
                ulong data = (ulong)ArrayToLong(buffer);
                data = DecryptBlock(data);
                stream.Position = stream.Position - 8;
                stream.Write(LongToArray((long)data), 0, 8);
            }
        }

        private void AlignedVerif(Stream stream)
        {
            int blockSize = 8;
            bool alignedSize = (stream.Length % blockSize) == 0;
            if (!alignedSize)
            {
                throw new InvalidDataException("Данные не выровнены блоками по 64 бита");
            }
        }

        public ulong EncryptBlock(ulong dataBlock)
        {
            uint right = (uint)(dataBlock & 0x00000000FFFFFFFF);
            uint left = (uint)((dataBlock >> 32) & 0x00000000FFFFFFFF);
            blowFeistel.BlowfishEncrypt(context, ref left, ref right);
            ulong ecrypted = (ulong)left;
            ulong resilt = (ecrypted << 32) | right;
            return resilt;
        }

        public ulong DecryptBlock(ulong dataBlock)
        {
            uint right = (uint)(dataBlock & 0x00000000FFFFFFFF);
            uint left = (uint)((dataBlock >> 32) & 0x00000000FFFFFFFF);
            blowFeistel.BlowfishDecrypt(context, ref left, ref right);
            ulong decrypted = (ulong)left;
            ulong resilt = (decrypted << 32) | right;
            return resilt;
        }

        public ulong[] EncryptArray(ulong[] data)
        {
            long length = data.LongLength;
            ulong[] result = new ulong[length];
            for (long i = 0; i < length; i++)
            {
                data[i] = EncryptBlock(data[i]);
            }
            return result;
        }

        public ulong[] DecryptArray(ulong[] data)
        {
            long length = data.LongLength;
            ulong[] result = new ulong[length];
            for (long i = 0; i < length; i++)
            {
                data[i] = DecryptBlock(data[i]);
            }
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
    }
}
