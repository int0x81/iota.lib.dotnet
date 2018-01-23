﻿using Iota.Lib.CSharp.Api.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Iota.Lib.CSharpTests
{
    [TestClass]
    public class ChecksumTest
    {
        private static string TEST_ADDRESS_WITHOUT_CHECKSUM =
            "ADJGOINLDBYXPTHJTZCAMABXLNOUFVPRROSRT99RYCNMIBIVKLEKCBVMRWZCKMVLUIMZVHEUXZAJRGA9D";

        private static string TEST_ADDRESS_WITH_CHECKSUM =
            "ADJGOINLDBYXPTHJTZCAMABXLNOUFVPRROSRT99RYCNMIBIVKLEKCBVMRWZCKMVLUIMZVHEUXZAJRGA9DNVCJURQMY";

        [TestMethod]
        public void ShouldAddChecksum()
        {
            string result = Checksum.AddChecksum(TEST_ADDRESS_WITHOUT_CHECKSUM);
            Assert.AreEqual(result.Length, TEST_ADDRESS_WITH_CHECKSUM.Length);
            Assert.AreEqual(result, TEST_ADDRESS_WITH_CHECKSUM);
        }

        [TestMethod]
        public void ShouldRemoveChecksum()
        {
            Assert.AreEqual(Checksum.RemoveChecksum(TEST_ADDRESS_WITH_CHECKSUM), TEST_ADDRESS_WITHOUT_CHECKSUM);
        }

        [TestMethod]
        public void TestAddressDoesNotIncludeChecksumShouldGenerateCorrectChecksum_02()
        {
            string address = "FAJIXQNBJHCCVBIW9PDYIAXAHWJZHHUNTOPLXTPGYIHYGUGTCTOWJSJZLJQZPBNL9FCRSFTENJLVSDMPD";

            Assert.AreEqual("FAJIXQNBJHCCVBIW9PDYIAXAHWJZHHUNTOPLXTPGYIHYGUGTCTOWJSJZLJQZPBNL9FCRSFTENJLVSDMPDETBRCTSI9", Checksum.AddChecksum(address));
        }
    }
}