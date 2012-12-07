/******************************************************************************
 * Copyright (c) 2012 ABB Group
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the Eclipse Public License v1.0
 * which accompanies this distribution, and is available at
 * http://www.eclipse.org/legal/epl-v10.html
 *
 * Contributors:
 *    Patrick Francis (ABB Group) - initial implementation and documentation
 *****************************************************************************/

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using ABB.Swum;
using NUnit.Framework;


namespace ABB.Swum.Tests
{
    /// <summary>
    /// Unit tests for ABB.Swum.ConservativeIdSplitter
    /// </summary>
    [TestFixture]
    public class ConservativeIdSplitterTests
    {
        public ConservativeIdSplitterTests()
        {
            splitter = new ConservativeIdSplitter();
        }

        private TestContext testContextInstance;
        private ConservativeIdSplitter splitter;

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
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [Test]
        public void AllSameCase_SingleWord()
        {
            //test lowercase word
            string[] actual = splitter.Split("lowercase");
            Assert.AreEqual("lowercase", actual[0]);
            Assert.AreEqual(1, actual.Length);

            //test uppercase word
            actual = splitter.Split("CONSTANTVALUE");
            Assert.AreEqual("CONSTANTVALUE", actual[0]);
            Assert.AreEqual(1, actual.Length);
        }

        [Test]
        public void AllSameCase_MultipleWords()
        {
            string[] actual = splitter.Split("lower_case");
            string[] expected = { "lower", "case" };
            Assert.AreEqual(expected.Length, actual.Length);
            for (int i = 0; i < actual.Length; i++)
                Assert.AreEqual(expected[i], actual[i]);

            actual = splitter.Split("HELLO_WORLD");
            expected[0] = "HELLO";
            expected[1] = "WORLD";
            Assert.AreEqual(expected.Length, actual.Length);
            for (int i = 0; i < actual.Length; i++)
                Assert.AreEqual(expected[i], actual[i]);

            actual = splitter.Split("howdy_water4melons999");
            string[] expected2 = { "howdy", "water", "4", "melons", "999" };
            Assert.AreEqual(expected2.Length, actual.Length);
            for (int i = 0; i < actual.Length; i++)
                Assert.AreEqual(expected2[i], actual[i]);
        }

        [Test]
        public void MixedCase()
        {
            string[] actual = splitter.Split("HelloWorld");
            string[] expected = { "Hello", "World" };
            Assert.AreEqual(expected.Length, actual.Length);
            for (int i = 0; i < actual.Length; i++)
                Assert.AreEqual(expected[i], actual[i]);

            actual = splitter.Split("first_secondThird");
            string[] expected2 = { "first", "second", "Third" };
            Assert.AreEqual(expected2.Length, actual.Length);
            for (int i = 0; i < actual.Length; i++)
                Assert.AreEqual(expected2[i], actual[i]);
        }
    }
}
