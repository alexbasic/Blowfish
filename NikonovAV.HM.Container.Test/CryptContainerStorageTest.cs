using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        [TestMethod]
        public void PackAndExtractShoulBeCorrectSize() 
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
            CryptContainerStorage cryptContainerStorage = new CryptContainerStorage(original, inconteiner);

            cryptContainerStorage.Pack();

            //размер запакованного = (размер исходного расширенный до крастности 8) + 8 байт 
            Assert.AreEqual(expectedPackedSize, inconteiner.Length);

            original = new MemoryStream(0);
            CryptContainerStorage cryptContainerStorageForUnpack = new CryptContainerStorage(original, inconteiner);

            cryptContainerStorageForUnpack.Extract();

            //Извлеченный должен быть как исходный
            Assert.AreEqual(originalSize, original.Length);
        }
    }
}
