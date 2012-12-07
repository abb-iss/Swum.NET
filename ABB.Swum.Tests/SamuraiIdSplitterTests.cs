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
using NUnit.Framework;
using ABB.Swum;

namespace ABB.Swum.Tests
{
    [TestFixture]
    public class SamuraiIdSplitterTests
    {
        public static SamuraiIdSplitter Splitter;

        [TestFixtureSetUp]
        public static void ClassSetup()
        {
            Splitter = new SamuraiIdSplitter(@"TestFiles\NotepadPlusPlus6.2_words.count");
        }

        [Test]
        public void CamelCase()
        {
            string[] actual = Splitter.Split("HourlyBidMarkupWriter");
            string[] expected = { "Hourly", "Bid", "Markup", "Writer" };
            Assert.AreEqual(expected.Length, actual.Length);
            for (int i = 0; i < actual.Length; i++)
                Assert.AreEqual(expected[i], actual[i]);
        }

        [Test]
        public void CamelCase_Punctuation()
        {
            string[] actual = Splitter.Split("~PowerBid_Watcher");
            string[] expected = { "Power", "Bid", "Watcher"};
            Assert.AreEqual(expected.Length, actual.Length);
            for (int i = 0; i < actual.Length; i++)
                Assert.AreEqual(expected[i], actual[i]);
        }

        [Test]
        public void CamelCase_Numbers()
        {
            string[] actual = Splitter.Split("X11Certificate");
            string[] expected = { "X", "11", "Certificate" };
            Assert.AreEqual(expected.Length, actual.Length);
            for (int i = 0; i < actual.Length; i++)
                Assert.AreEqual(expected[i], actual[i]);
        }

        [Test]
        public void CaseChange_Beginning()
        {
            string[] actual = Splitter.Split("IOWriter");
            string[] expected = { "IO", "Writer" };
            Assert.AreEqual(expected.Length, actual.Length);
            for (int i = 0; i < actual.Length; i++)
                Assert.AreEqual(expected[i], actual[i]);
        }

        [Test]
        public void CaseChange_End()
        {
            string[] actual = Splitter.Split("AssignedXML");
            string[] expected = { "Assigned", "XML" };
            Assert.AreEqual(expected.Length, actual.Length);
            for (int i = 0; i < actual.Length; i++)
                Assert.AreEqual(expected[i], actual[i]);
        }

        [Test]
        public void CaseChange_Middle()
        {
            string[] actual = Splitter.Split("UpdatedASTVisitor");
            string[] expected = { "Updated", "AST", "Visitor" };
            Assert.AreEqual(expected.Length, actual.Length);
            for (int i = 0; i < actual.Length; i++)
                Assert.AreEqual(expected[i], actual[i]);
        }

        [Test]
        public void SameCase()
        {
            string[] actual = Splitter.Split("sparsematrix");
            string[] expected = { "sparse", "matrix" };
            Assert.AreEqual(expected.Length, actual.Length);
            for (int i = 0; i < actual.Length; i++)
                Assert.AreEqual(expected[i], actual[i]);
        }
    }
}
