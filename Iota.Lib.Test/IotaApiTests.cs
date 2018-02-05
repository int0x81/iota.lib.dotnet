﻿using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Iota.Lib.Model;
using Iota.Lib.Exception;
using Iota.Lib.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Iota.Lib.Test
{
    [TestClass]
    public class IotaApiTests
    {
        const string NODE = "nodes.thetangle.org"; //Your test node; Please note that not all nodes accept the all requests
        const int PORT = 443;                      //Your test nodes's port
        const bool IS_SSL = true;                  //Your test node's encryption state
        const int TIMEOUT = 10000;                 //Limits the time a request should take in milliseconds

        readonly List<string> ADDRESSES = new List<string>()
        {
            "ADJGOINLDBYXPTHJTZCAMABXLNOUFVPRROSRT99RYCNMIBIVKLEKCBVMRWZCKMVLUIMZVHEUXZAJRGA9DNVCJURQMY",
            "NZ9CNGUKCHUQPRIYLV9OXN9KMDGAEFLDOWENIH9KHPVJGYIWUKKBFDZSZDPAOOFEECVITDXQXPEUAGOT9UBQYOQXCD",
            "JIY9BUVVUCTQHGYNZQPZQAMDHPLZMTYPJSEFCZKJ9VCWN9LZWCWROUCTNOOOMSSFUENASJCBJRBULHLBXLLJUWXCHA"
        };

        readonly List<string> RAW_TRANSACTIONS = new List<string>()
        {
            "BYSWEAUTWXHXZ9YBZISEK9LUHWGMHXCGEVNZHRLUWQFCUSDXZHOFHWHL9MQPVJXXZLIXPXPXF9KYEREFSKCPKYIIKPZVLHUTDFQKKVVBBN9ATTLPCNPJDWDEVIYYLGPZGCWXOBDXMLJC9VO9QXTTBLAXTTBFUAROYEGQIVB9MJWJKXJMCUPTWAUGFZBTZCSJVRBGMYXTVBDDS9MYUJCPZ9YDWWQNIPUAIJXXSNLKUBSCOIJPCLEFPOXFJREXQCUVUMKSDOVQGGHRNILCO9GNCLWFM9APMNMWYASHXQAYBEXF9QRIHIBHYEJOYHRQJAOKAQ9AJJFQ9WEIWIJOTZATIBOXQLBMIJU9PCGBLVDDVFP9CFFSXTDUXMEGOOFXWRTLFGV9XXMYWEMGQEEEDBTIJ9OJOXFAPFQXCDAXOUDMLVYRMRLUDBETOLRJQAEDDLNVIRQJUBZBO9CCFDHIX9MSQCWYAXJVWHCUPTRSXJDESISQPRKZAFKFRULCGVRSBLVFOPEYLEE99JD9SEBALQINPDAZHFAB9RNBH9AZWIJOTLBZVIEJIAYGMC9AZGNFWGRSWAXTYSXVROVNKCOQQIWGPNQZKHUNODGYADPYLZZZUQRTJRTODOUKAOITNOMWNGHJBBA99QUMBHRENGBHTH9KHUAOXBVIVDVYYZMSEYSJWIOGGXZVRGN999EEGQMCOYVJQRIRROMPCQBLDYIGQO9AMORPYFSSUGACOJXGAQSPDY9YWRRPESNXXBDQ9OZOXVIOMLGTSWAMKMTDRSPGJKGBXQIVNRJRFRYEZ9VJDLHIKPSKMYC9YEGHFDS9SGVDHRIXBEMLFIINOHVPXIFAZCJKBHVMQZEVWCOSNWQRDYWVAIBLSCBGESJUIBWZECPUCAYAWMTQKRMCHONIPKJYYTEGZCJYCT9ABRWTJLRQXKMWY9GWZMHYZNWPXULNZAPVQLPMYQZCYNEPOCGOHBJUZLZDPIXVHLDMQYJUUBEDXXPXFLNRGIPWBRNQQZJSGSJTTYHIGGFAWJVXWL9THTPWOOHTNQWCNYOYZXALHAZXVMIZE9WMQUDCHDJMIBWKTYH9AC9AFOT9DPCADCV9ZWUTE9QNOMSZPTZDJLJZCJGHXUNBJFUBJWQUEZDMHXGBPTNSPZBR9TGSKVOHMOQSWPGFLSWNESFKSAZY9HHERAXALZCABFYPOVLAHMIHVDBGKUMDXC9WHHTIRYHZVWNXSVQUWCR9M9RAGMFEZZKZ9XEOQGOSLFQCHHOKLDSA9QCMDGCGMRYJZLBVIFOLBIJPROKMHOYTBTJIWUZWJMCTKCJKKTR9LCVYPVJI9AHGI9JOWMIWZAGMLDFJA9WU9QAMEFGABIBEZNNAL9OXSBFLOEHKDGHWFQSHMPLYFCNXAAZYJLMQDEYRGL9QKCEUEJ9LLVUOINVSZZQHCIKPAGMT9CAYIIMTTBCPKWTYHOJIIY9GYNPAJNUJ9BKYYXSV9JSPEXYMCFAIKTGNRSQGUNIYZCRT9FOWENSZQPD9ALUPYYAVICHVYELYFPUYDTWUSWNIYFXPX9MICCCOOZIWRNJIDALWGWRATGLJXNAYTNIZWQ9YTVDBOFZRKO9CFWRPAQQRXTPACOWCPRLYRYSJARRKSQPR9TCFXDVIXLP9XVL99ERRDSOHBFJDJQQGGGCZNDQ9NYCTQJWVZIAELCRBJJFDMCNZU9FIZRPGNURTXOCDSQGXTQHKHUECGWFUUYS9J9NYQ9U9P9UUP9YMZHWWWCIASCFLCMSKTELZWUGCDE9YOKVOVKTAYPHDF9ZCCQAYPJIJNGSHUIHHCOSSOOBUDOKE9CJZGYSSGNCQJVBEFTZFJ9SQUHOASKRRGBSHWKBCBWBTJHOGQ9WOMQFHWJVEG9NYX9KWBTCAIXNXHEBDIOFO9ALYMFGRICLCKKLG9FOBOX9PDWNQRGHBKHGKKRLWTBEQMCWQRLHAVYYZDIIPKVQTHYTWQMTOACXZOQCDTJTBAAUWXSGJF9PNQIJ9AJRUMUVCPWYVYVARKR9RKGOUHHNKNVGGPDDLGKPQNOYHNKAVVKCXWXOQPZNSLATUJT9AUWRMPPSWHSTTYDFAQDXOCYTZHOYYGAIM9CELMZ9AZPWB9MJXGHOKDNNSZVUDAGXTJJSSZCPZVPZBYNNTUQABSXQWZCHDQSLGK9UOHCFKBIBNETK999999999999999999999999999999999999999999999999999999999999999999999999999999999NOXDXXKUDWLOFJLIPQIBRBMGDYCPGDNLQOLQS99EQYKBIU9VHCJVIPFUYCQDNY9APGEVYLCENJIOBLWNB999999999XKBRHUD99C99999999NKZKEKWLDKMJCI9N9XQOLWEPAYWSH9999999999999999999999999KDDTGZLIPBNZKMLTOLOXQVNGLASESDQVPTXALEKRMIOHQLUHD9ELQDBQETS9QFGTYOYWLNTSKKMVJAUXSIROUICDOXKSYZTDPEDKOQENTJOWJONDEWROCEJIEWFWLUAACVSJFTMCHHXJBJRKAAPUDXXVXFWP9X9999IROUICDOXKSYZTDPEDKOQENTJOWJONDEWROCEJIEWFWLUAACVSJFTMCHHXJBJRKAAPUDXXVXFWP9X9999"
        };

        const string TEST_SEED_01 = "SMRUKAKOPAKXQSIKVZWQGQNKZZWL9BGEFJCIEBRJDIAGWFHUKAOSWACNC9JFDU9WHAPZBEIGWBU9VTNZS";
        const string TEST_SEED_02 = "IHDEENZYITYVYSPKAURUZAQKGVJEREFDJMYTANNXXGPZ9GJWTEOJJ9IPMXOGZNQLSNMFDSQOTZAEETUEA";
        const string TEST_ADDRESS_WITHOUT_CHECKSUM_SECURITY_LEVEL_2 = "LXQHWNY9CQOHPNMKFJFIJHGEPAENAOVFRDIBF99PPHDTWJDCGHLYETXT9NPUVSNKT9XDTDYNJKJCPQMZC";
        const string TEST_MESSAGE = "JUSTANOTHERJOTATEST";
        const string TEST_TAG = "JOTASPAM9999999999999999999";
        const int MIN_WEIGHT_MAGNITUDE = 14;
        const int DEPTH = 9;

        IotaApi api = new IotaApi(NODE, PORT, IS_SSL);

        [TestMethod, Timeout(TIMEOUT)]
        public void TestGetInputs()
        {
            var inputs = api.GetInputs(TEST_SEED_01);
            Assert.IsTrue(InputValidator.IsArrayOfValidTransactionHashes(inputs));
        }

        [TestMethod]
        public void TestPrepareTransfers()
        {
            const int SECURITY_LEVEL = 3;
            string inputAddress = IotaApiUtils.CreateNewAddress(TEST_SEED_01, 1, SECURITY_LEVEL, false);
            string remainderAddress = IotaApiUtils.CreateNewAddress(TEST_SEED_01, 2, SECURITY_LEVEL, false);

            List <Transaction> outputs = new List<Transaction>
            {
                new Transaction
                {
                    Address = "JHYLDJCBBTSFGVTBONTIVOWURCWMWBGGVRTOAMTKKFHWJAJHKKPWEYTAVDXMUSJBIUYEVZMO9LXBWHTUZ",
                    Value = 3,
                    ObsoleteTag = "999999999999999999999999999",
                    Tag = "999999999999999999999999999",
                    Timestamp = 1515494426
                }
            };

            List<Transaction> inputs = new List<Transaction>
            {
                new Transaction
                {
                    Address = inputAddress,
                    Value = -5,
                    KeyIndex = 1,
                    SecurityLevel = 3,
                    ObsoleteTag = "999999999999999999999999999",
                    Tag = "999999999999999999999999999",
                    Timestamp = 1515494426
                }
            };

            var result = api.PrepareTransfers(TEST_SEED_01, outputs, SECURITY_LEVEL, inputs, remainderAddress);
            Assert.IsTrue(result.Count() == 5);
        }

        [TestMethod]
        public void TestGetNewAddresses()
        {
            int numberOfAddresses = 21;

            var addresses = api.GetNewAddresses(TEST_SEED_01, 0, numberOfAddresses);
            Assert.IsTrue(InputValidator.IsArrayOfValidAddress(addresses));
            Assert.IsTrue(addresses.Count() == numberOfAddresses);
        }

        [TestMethod]
        public void TestGetBundle()
        {
            var bundle = api.GetBundle(RAW_TRANSACTIONS[0]);
            Assert.IsTrue(bundle != null);
        }

        [TestMethod]
        public void TestGetTransfers()
        {
            var bundles = api.GetTransfers(TEST_SEED_01, 0, 5, 2);
            Assert.IsTrue(bundles.Count >= 0);
        }

        /// <summary>
        /// This test performs an actuall transfer including local proof-of-work
        /// </summary>
        [TestMethod]
        public void PROOF_OF_CONCEPT()
        {
            IPowService powService = new PearlDiver();
            const string SEED = "HAKHOVW9EQWPESUCKITYGLYWGCCOXYH9EOZITARIFJMARWB9SSNB9URZFFANPWEGNONPGEUDBENZRZW9R";
            string outgoingAddress = "DMDSWYIUUFDMHKIBQPP9LMCQNYQDFXXMPT9GWHXYZ9IQNEYJLSNASVXFFSZZKJAVHTFIDSZGIOXDURONWDTTBHVBWX";
            string inputAddress = IotaApiUtils.CreateNewAddress(SEED, 0, 2, true);
            Assert.AreEqual(inputAddress, "RQXWRWSRPKRFTCJQME9FPXEJMZXQHOEKYZRQCNYQADWTPBKPPSYZYADKBLRNOKUMQYYSLJJDBAJJWGBMWCBDTSU9CA");
            Transaction output = new Transaction(outgoingAddress, 2);
            Transaction input = new Transaction(inputAddress, -10, null, "IHATEJAVA", 0, 2);
            List<string> rawTransactions = api.PrepareTransfers(SEED, new List<Transaction> {output}, 2, new List<Transaction> {input}).ToList();
            Assert.IsTrue(rawTransactions.Count == 4);

            List<string> rawTransactionsWithNonce = new List<string>();
            foreach (string rawTransaction in rawTransactions)
            {
                var dummy = rawTransaction.Length;
                string rawTransactionWithNonce = powService.Execute(rawTransaction, 10);
                rawTransactionsWithNonce.Add(rawTransactionWithNonce);
            }
            Transaction transaction_01 = new Transaction(rawTransactionsWithNonce[0]);
            Transaction transaction_02 = new Transaction(rawTransactionsWithNonce[1]);
            Transaction transaction_03 = new Transaction(rawTransactionsWithNonce[2]);
            Transaction transaction_04 = new Transaction(rawTransactionsWithNonce[3]);

            var result = api.BroadcastTransactions(rawTransactionsWithNonce);
            Assert.IsTrue(result.StatusCode == System.Net.HttpStatusCode.OK);
        }
    }
}
