using NikonovAV.HM.BlowfishCrypt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NikonovAV.HM.Container
{
    public class CryptContainerStorage
    {
        private Stream containerData;
        private Stream unpackedData;
        private ICrypter _crypter;

        //public long SizeDestInBlocks { get; set; }
        //public long ContentSize { get; set; }
        public int BlockSize { get { return 8; } }

        public CryptContainerStorage(Stream unpacked, Stream container, ICrypter crypter)
        {
            unpackedData = unpacked;
            containerData = container;
            _crypter = crypter;
        }

        public void Pack()
        {
            //TODO Усовершенствовать шифрование.
            // Шифровать не весь поток после упаковки, а шифровать и производить упаковку блоками по 64 бита
            //long sizeDestInBlocks = ((unpackedData.Length - 1) / BlockSize) + 1;
            //containerData.SetLength((sizeDestInBlocks * BlockSize) + BlockSize);
            //SizeDestInBlocks = sizeDestInBlocks;
            containerData.Position = 0;
            unpackedData.Position = 0;
            long unpackedDataSize = unpackedData.Length;
            int addsBytes = (int)(unpackedDataSize % (long)BlockSize);
            byte[] buffer = LongToArray(unpackedDataSize);
            containerData.Write(buffer, 0, 8);
            for (long i = 0; i < unpackedDataSize; i++)
            {
                int buf = unpackedData.ReadByte();

                containerData.WriteByte((byte)buf);
            }
            for (var i = 0; i < addsBytes; i++)
            {
                containerData.WriteByte(0);
            }
            if (_crypter != null)
            {
                _crypter.Encrypt(containerData);
            }
        }

        public void Extract()
        {
            //long sizeDestInBlocks = ((unpackedData.Length - 1) / BlockSize) + 1;
            //SizeDestInBlocks = sizeDestInBlocks;

            if (_crypter != null)
            {
                _crypter.Decrypt(containerData);
            }
            containerData.Position = 0;
            unpackedData.Position = 0;
            byte[] sizeVariable = new byte[8];
            containerData.Read(sizeVariable, 0, 8);
            long unpackedDataSize = ArrayToLong(sizeVariable);
            for (long i = 0; i < unpackedDataSize; i++)
            {
                int buf = containerData.ReadByte();

                unpackedData.WriteByte((byte)buf);
            }
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
