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
using System.Text.RegularExpressions;
using NUnit.Framework;


namespace ABB.Swum.Nodes.Tests
{
    [TestFixture]
    public class WordNodeTests
    {
        [Test]
        public void TestToString()
        {
            WordNode wn = new WordNode("Test", PartOfSpeechTag.Noun);
            Assert.AreEqual("Test(Noun)", wn.ToString());
        }

        [Test]
        public void TestConstructor_Default()
        {
            WordNode wn = new WordNode();
            Assert.AreEqual(string.Empty, wn.Text);
            Assert.AreEqual(PartOfSpeechTag.Unknown, wn.Tag);
            Assert.AreEqual(0.0, wn.Confidence);
            Assert.AreEqual(Location.None, wn.Location);
        }

        [Test]
        public void TestConstructor_AllParams()
        {
            WordNode wn = new WordNode("watermelon", PartOfSpeechTag.Noun, 5.7);
            Assert.AreEqual("watermelon", wn.Text);
            Assert.AreEqual(PartOfSpeechTag.Noun, wn.Tag);
            Assert.AreEqual(5.7, wn.Confidence);
            Assert.AreEqual(Location.None, wn.Location);
        }

        [Test]
        public void TestAllCaps()
        {
            WordNode wn = new WordNode("HoWdy");
            Assert.IsFalse(wn.IsAllCaps());

            wn = new WordNode("HOWDY");
            Assert.IsTrue(wn.IsAllCaps());
        }

        [Test]
        public void TestGetNewWord_Default()
        {
            WordNode wn = new WordNode("watermelon", PartOfSpeechTag.Noun);
            object obj = wn.GetNewWord();
            Assert.IsInstanceOf(wn.GetType(), obj);
            Assert.IsFalse(wn.Equals(obj));
            WordNode wn2 = new WordNode();
            Assert.IsTrue(WordNodesAreEqual(wn2, obj as WordNode));
        }

        [Test]
        public void TestGetNewWord_Parameters()
        {
            WordNode wn = new WordNode("watermelon", PartOfSpeechTag.Noun);
            object obj = wn.GetNewWord("beef", PartOfSpeechTag.Verb);
            Assert.IsInstanceOf(wn.GetType(), obj);
            
            WordNode wn2 = new WordNode("beef", PartOfSpeechTag.Verb);
            Assert.IsTrue(WordNodesAreEqual(wn2, obj as WordNode));
        }

        [Test]
        public void TestGetParse() {
            WordNode wn = new WordNode("cowboy", PartOfSpeechTag.Noun);
            var parse = wn.GetParse();
            Assert.AreEqual(1, parse.Size());
            Assert.AreEqual(wn, parse[0]);
        }

        [Test]
        public void TestParse() {
            var actual = WordNode.Parse("cowboy(Noun)");
            var expected = new WordNode("cowboy", PartOfSpeechTag.Noun);
            Assert.IsTrue(WordNodesAreEqual(actual, expected));
        }

        [Test]
        [ExpectedException(typeof(FormatException))]
        public void TestParse_BadFormat() {
            var actual = WordNode.Parse("mango (Verb)");
        }

        [Test]
        [ExpectedException(typeof(FormatException))]
        public void TestParse_BadPOS() {
            var actual = WordNode.Parse("mango(INTERJECTION)");
        }

        [Test]
        public void TestRoundTrip() {
            var wn = new WordNode("mangoes", PartOfSpeechTag.NounPlural);
            Assert.IsTrue(WordNodesAreEqual(wn, WordNode.Parse(wn.ToString())));
        }

        public static bool WordNodesAreEqual(WordNode wn1, WordNode wn2)
        {
            return wn1.Text == wn2.Text
                && wn1.Tag == wn2.Tag
                && wn1.Confidence == wn2.Confidence
                && wn1.Location == wn2.Location;
        }
    }
}
