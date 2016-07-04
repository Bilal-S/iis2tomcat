﻿using BonCodeAJP13.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Configuration;
using System.Collections.Specialized;
using System.Xml;

namespace BonCodeUnitTestProject
{
    
    
    /// <summary>
    ///This is a test class for BonCodeAJP13SettingProviderTest and is intended
    ///to contain all BonCodeAJP13SettingProviderTest Unit Tests
    ///</summary>
    [TestClass()]
    public class BonCodeAJP13SettingProviderTest
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
        ///A test for BonCodeAJP13SettingProvider Constructor
        ///</summary>
        [TestMethod()]
        public void BonCodeAJP13SettingProviderConstructorTest()
        {
            BonCodeAJP13SettingProvider target = new BonCodeAJP13SettingProvider();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for GetAppSettingsFilename
        ///</summary>
        [TestMethod()]
        public void GetAppSettingsFilenameTest()
        {
            BonCodeAJP13SettingProvider target = new BonCodeAJP13SettingProvider(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.GetAppSettingsFilename();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetAppSettingsPath
        ///</summary>
        [TestMethod()]
        public void GetAppSettingsPathTest()
        {
            BonCodeAJP13SettingProvider target = new BonCodeAJP13SettingProvider(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.GetAppSettingsPath();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetPropertyValues
        ///</summary>
        [TestMethod()]
        public void GetPropertyValuesTest()
        {
            BonCodeAJP13SettingProvider target = new BonCodeAJP13SettingProvider(); // TODO: Initialize to an appropriate value
            SettingsContext context = null; // TODO: Initialize to an appropriate value
            SettingsPropertyCollection props = null; // TODO: Initialize to an appropriate value
            SettingsPropertyValueCollection expected = null; // TODO: Initialize to an appropriate value
            SettingsPropertyValueCollection actual;
            actual = target.GetPropertyValues(context, props);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetValue
        ///</summary>
        [TestMethod()]
        [DeploymentItem("BonCodeAJP13.dll")]
        public void GetValueTest()
        {
            BonCodeAJP13SettingProvider_Accessor target = new BonCodeAJP13SettingProvider_Accessor(); // TODO: Initialize to an appropriate value
            SettingsProperty setting = null; // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.GetValue(setting);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Initialize
        ///</summary>
        [TestMethod()]
        public void InitializeTest()
        {
            BonCodeAJP13SettingProvider target = new BonCodeAJP13SettingProvider(); // TODO: Initialize to an appropriate value
            string name = string.Empty; // TODO: Initialize to an appropriate value
            NameValueCollection col = null; // TODO: Initialize to an appropriate value
            target.Initialize(name, col);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for SetPropertyValues
        ///</summary>
        [TestMethod()]
        public void SetPropertyValuesTest()
        {
            BonCodeAJP13SettingProvider target = new BonCodeAJP13SettingProvider(); // TODO: Initialize to an appropriate value
            SettingsContext context = null; // TODO: Initialize to an appropriate value
            SettingsPropertyValueCollection collection = null; // TODO: Initialize to an appropriate value
            target.SetPropertyValues(context, collection);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for ApplicationName
        ///</summary>
        [TestMethod()]
        public void ApplicationNameTest()
        {
            BonCodeAJP13SettingProvider target = new BonCodeAJP13SettingProvider(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            target.ApplicationName = expected;
            actual = target.ApplicationName;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Name
        ///</summary>
        [TestMethod()]
        public void NameTest()
        {
            BonCodeAJP13SettingProvider target = new BonCodeAJP13SettingProvider(); // TODO: Initialize to an appropriate value
            string actual;
            actual = target.Name;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SettingsXML
        ///</summary>
        [TestMethod()]
        [DeploymentItem("BonCodeAJP13.dll")]
        public void SettingsXMLTest()
        {
            BonCodeAJP13SettingProvider_Accessor target = new BonCodeAJP13SettingProvider_Accessor(); // TODO: Initialize to an appropriate value
            XmlDocument actual;
            actual = target.SettingsXML;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
