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


namespace ABB.Swum.Nodes.Tests {
    [TestFixture]
    public class PhraseNodeTests {
        [Test]
        public void TestConstructor_EnumerableWordNodes() {
            var wn1 = new WordNode("Eat", PartOfSpeechTag.Verb);
            var wn2 = new WordNode("More", PartOfSpeechTag.NounModifier);
            var wn3 = new WordNode("Chicken", PartOfSpeechTag.Noun);

            var words = new WordNode[] {wn1, wn2, wn3};
            PhraseNode pn = new PhraseNode(words, Location.Name, true);

            Assert.AreEqual(words.Length, pn.Size());
            for(int i = 0; i < pn.Size(); i++) {
                Assert.AreEqual(words[i], pn[i]);
            }
            Assert.AreEqual(Location.Name, pn.Location);
        }

        [Test]
        public void TestConstructor_EnumerableWordNodesEmpty() {
            var words = new WordNode[] {};
            PhraseNode pn = new PhraseNode(words, Location.Name, true);

            Assert.AreEqual(words.Length, pn.Size());
            for(int i = 0; i < pn.Size(); i++) {
                Assert.AreEqual(words[i], pn[i]);
            }
            Assert.AreEqual(Location.Name, pn.Location);
        }

        [Test]
        public void TestConstructor_EnumerableStrings() {
            var words = new string[] {"Eat", "More", "chicken"};
            var pn = new PhraseNode(words);

            Assert.AreEqual(3, pn.Size());
            for(int i = 0; i < pn.Size(); i++) {
                Assert.AreEqual(words[i], pn[i].Text);
            }
            Assert.AreEqual(Location.None, pn.Location);
        }

        [Test]
        public void TestConstructor_EnumerableStringsEmpty() {
            var words = new string[] {};
            var pn = new PhraseNode(words);

            Assert.AreEqual(words.Length, pn.Size());
            for(int i = 0; i < pn.Size(); i++) {
                Assert.AreEqual(words[i], pn[i].Text);
            }
            Assert.AreEqual(Location.None, pn.Location);
        }

        [Test]
        public void TestConstructor_EnumerableStringsNull() {
            var pn = new PhraseNode(null);
            Assert.AreEqual(0, pn.Size());
            Assert.AreEqual(Location.None, pn.Location);
        }

        [Test]
        public void TestToString() {
            var wn1 = new WordNode("Eat", PartOfSpeechTag.Verb);
            var wn2 = new WordNode("More", PartOfSpeechTag.NounModifier);
            var wn3 = new WordNode("Chicken", PartOfSpeechTag.Noun);
            var words = new WordNode[] {wn1, wn2, wn3};
            PhraseNode pn = new PhraseNode(words, Location.Name, true);

            string expected = string.Format("{0} {1} {2}", wn1, wn2, wn3);
            Assert.AreEqual(expected, pn.ToString());
        }

        [Test]
        public void TestToString_EmptyPhrase() {
            var pn = new PhraseNode();
            Assert.AreEqual(string.Empty, pn.ToString());
        }

        [Test]
        public void TestGetPhrase() {
            var wn1 = new WordNode("Eat", PartOfSpeechTag.Verb);
            var wn2 = new WordNode("More", PartOfSpeechTag.NounModifier);
            var wn3 = new WordNode("Chicken", PartOfSpeechTag.Noun);
            var words = new WordNode[] {wn1, wn2, wn3};
            PhraseNode pn = new PhraseNode(words, Location.Name, true);

            var phrase = pn.GetPhrase();
            Assert.AreEqual(words.Length, phrase.Count);
            for(int i = 0; i < phrase.Count; i++) {
                Assert.AreEqual(pn[i], words[i]);
            }
        }

        [Test]
        public void TestGetParse() {
            var wn1 = new WordNode("Eat", PartOfSpeechTag.Verb);
            var wn2 = new WordNode("More", PartOfSpeechTag.NounModifier);
            var wn3 = new WordNode("Chicken", PartOfSpeechTag.Noun);
            var words = new WordNode[] {wn1, wn2, wn3};
            PhraseNode pn = new PhraseNode(words, Location.Name, true);

            var parse = pn.GetParse();
            Assert.AreEqual(pn, parse);
        }

        [Test]
        public void TestParse() {
            var wn1 = new WordNode("Eat", PartOfSpeechTag.Verb);
            var wn2 = new WordNode("More", PartOfSpeechTag.NounModifier);
            var wn3 = new WordNode("Chicken", PartOfSpeechTag.Noun);
            var words = new WordNode[] { wn1, wn2, wn3 };
            var expected = new PhraseNode(words, Location.Name, false);

            var actual = PhraseNode.Parse("Eat(Verb) More(NounModifier) Chicken(Noun)");
            Assert.IsTrue(PhraseNodesAreEqual(actual, expected));
        }

        [Test]
        public void TestRoundTrip() {
            var wn1 = new WordNode("Get", PartOfSpeechTag.Verb);
            var wn2 = new WordNode("Fixed", PartOfSpeechTag.NounModifier);
            var wn3 = new WordNode("Hydro", PartOfSpeechTag.NounModifier);
            var wn4 = new WordNode("Schedule", PartOfSpeechTag.Noun);
            var pn = new PhraseNode(new[] { wn1, wn2, wn3, wn4 }, Location.Name, false);
            Assert.IsTrue(PhraseNodesAreEqual(pn, PhraseNode.Parse(pn.ToString())));
        }

        public static bool PhraseNodesAreEqual(PhraseNode pn1, PhraseNode pn2) {
            if(pn1.Size() != pn2.Size()) {
                return false;
            }
            for(int i = 0; i < pn1.Size(); i++) {
                if(!WordNodeTests.WordNodesAreEqual(pn1[i], pn2[i])) {
                    return false;
                }
            }
            return true;
        }
    }
}
