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
        public void Method1() 
        {
            MemoryStream original = null;
            MemoryStream inconteiner = null;
            CryptContainerStorage cryptContainerStorage = new CryptContainerStorage(original, inconteiner);

            cryptContainerStorage.Pack();
        }
    }
}
