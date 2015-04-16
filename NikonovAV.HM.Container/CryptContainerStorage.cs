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
        private Blowfish crypter;

        public long SizeDestInBlocks { get; set; }
        public long ContentSize { get; set; }
        public int BlockSize { get { return 8; } }

        public CryptContainerStorage(Stream unpacked, Stream container, Blowfish crypter = null)
        {
            unpackedData = unpacked;
            containerData = container;
            long sizeDestInBlocks = ((unpackedData.Length - 1) / BlockSize) + 1;
            containerData.SetLength((sizeDestInBlocks * BlockSize) + BlockSize);
            SizeDestInBlocks = sizeDestInBlocks;
        }

        public void Pack()
        {
            long unpackedDataSize = unpackedData.Length;
            int addsBytes = (int)(unpackedDataSize % (long)BlockSize);
            for (long i = 0; i < unpackedDataSize; i++)
            {
                int buf = unpackedData.ReadByte();

                containerData.WriteByte((byte)buf);
            }
            for (var i = 0; i < addsBytes; i++)
            {
                containerData.WriteByte(0);
            }
            if (crypter != null)
            {
                crypter.Encrypt(containerData);
            }
        }

        public void Extract()
        {
            if (crypter != null)
            {
                crypter.Decrypt(containerData);
            }
            containerData.Position = 0;
            unpackedData.Position = 0;
            byte[] sizeVariable = new byte[8];
            containerData.Read(sizeVariable, 0, 8);
            long unpackedDataSize = BitConverter.ToInt64(sizeVariable, 0);
            for (long i = 0; i < unpackedDataSize; i++)
            {
                int buf = containerData.ReadByte();

                unpackedData.WriteByte((byte)buf);
            }
        }
    }
}
