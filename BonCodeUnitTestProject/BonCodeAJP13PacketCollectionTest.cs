using BonCodeAJP13;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace BonCodeUnitTestProject
{
    
    
    /// <summary>
    ///This is a test class for BonCodeAJP13PacketCollectionTest and is intended
    ///to contain all BonCodeAJP13PacketCollectionTest Unit Tests
    ///</summary>
    [TestClass()]
    public class BonCodeAJP13PacketCollectionTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for BonCodeAJP13PacketCollection Constructor
        ///</summary>
        [TestMethod()]
        public void BonCodeAJP13PacketCollectionConstructorTest()
        {
            BonCodeAJP13PacketCollection target = new BonCodeAJP13PacketCollection();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for Add
        ///</summary>
        [TestMethod()]
        public void AddTest()
        {
            BonCodeAJP13PacketCollection target = new BonCodeAJP13PacketCollection(); // TODO: Initialize to an appropriate value
            BonCodeAJP13Packet value = null; // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.Add(value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Contains
        ///</summary>
        [TestMethod()]
        public void ContainsTest()
        {
            BonCodeAJP13PacketCollection target = new BonCodeAJP13PacketCollection(); // TODO: Initialize to an appropriate value
            BonCodeAJP13Packet value = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.Contains(value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for IndexOf
        ///</summary>
        [TestMethod()]
        public void IndexOfTest()
        {
            BonCodeAJP13PacketCollection target = new BonCodeAJP13PacketCollection(); // TODO: Initialize to an appropriate value
            BonCodeAJP13Packet value = null; // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.IndexOf(value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Insert
        ///</summary>
        [TestMethod()]
        public void InsertTest()
        {
            BonCodeAJP13PacketCollection target = new BonCodeAJP13PacketCollection(); // TODO: Initialize to an appropriate value
            int index = 0; // TODO: Initialize to an appropriate value
            BonCodeAJP13Packet value = null; // TODO: Initialize to an appropriate value
            target.Insert(index, value);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Remove
        ///</summary>
        [TestMethod()]
        public void RemoveTest()
        {
            BonCodeAJP13PacketCollection target = new BonCodeAJP13PacketCollection(); // TODO: Initialize to an appropriate value
            BonCodeAJP13Packet value = null; // TODO: Initialize to an appropriate value
            target.Remove(value);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Item
        ///</summary>
        [TestMethod()]
        public void ItemTest()
        {
            BonCodeAJP13PacketCollection target = new BonCodeAJP13PacketCollection(); // TODO: Initialize to an appropriate value
            int index = 0; // TODO: Initialize to an appropriate value
            BonCodeAJP13Packet expected = null; // TODO: Initialize to an appropriate value
            BonCodeAJP13Packet actual;
            target[index] = expected;
            actual = target[index];
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
