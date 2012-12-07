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
    /// <summary>
    /// Summary description for MethodDeclarationNodeTests
    /// </summary>
    [TestFixture]
    public class MethodDeclarationNodeTests {

        [Test]
        public void TestConstructor_Name() {
            MethodDeclarationNode mdn = new MethodDeclarationNode("MyMethod");
            Assert.AreEqual("MyMethod", mdn.Name);
            Assert.IsNull(mdn.Context);
            Assert.AreEqual(MethodRole.Unknown, mdn.Role);
            Assert.IsNull(mdn.FormalParameters);
            Assert.IsNull(mdn.DeclaringClass);
            Assert.IsNull(mdn.ReturnType);
            Assert.IsNull(mdn.Action);
            Assert.IsNull(mdn.Theme);
            Assert.IsNull(mdn.SecondaryArguments);
            Assert.IsNull(mdn.UnknownArguments);
            Assert.IsFalse(mdn.IsReactive);
            Assert.IsFalse(mdn.IsConstructor);
            Assert.IsFalse(mdn.IsDestructor);
        }

        [Test]
        public void TestConstructor_Context() {
            MethodContext mc = new MethodContext("STATUS", false);
            MethodDeclarationNode mdn = new MethodDeclarationNode("MyMethod", mc);
            Assert.AreEqual("MyMethod", mdn.Name);
            Assert.AreEqual(mc, mdn.Context);
            Assert.AreEqual(MethodRole.Unknown, mdn.Role);
            Assert.IsNull(mdn.FormalParameters);
            Assert.IsNull(mdn.DeclaringClass);
            Assert.IsNull(mdn.ReturnType);
            Assert.IsNull(mdn.Action);
            Assert.IsNull(mdn.Theme);
            Assert.IsNull(mdn.SecondaryArguments);
            Assert.IsNull(mdn.UnknownArguments);
            Assert.IsFalse(mdn.IsReactive);
            Assert.IsFalse(mdn.IsConstructor);
            Assert.IsFalse(mdn.IsDestructor);
        }

        [Test]
        public void TestAddUnknownArgument() {
            MethodDeclarationNode mdn = new MethodDeclarationNode("MyMethod");
            TypeNode arg = new TypeNode("int", true);
            mdn.AddUnknownArgument(arg);
            Assert.IsNotNull(mdn.UnknownArguments);
            Assert.AreEqual(1, mdn.UnknownArguments.Count);
            Assert.AreEqual(arg, mdn.UnknownArguments[0]);

            VariableDeclarationNode vdn = new VariableDeclarationNode("foo");
            mdn.AddUnknownArgument(vdn);
            Assert.AreEqual(2, mdn.UnknownArguments.Count);
            Assert.AreEqual(arg, mdn.UnknownArguments[0]);
            Assert.AreEqual(vdn, mdn.UnknownArguments[1]);
        }

        [Test]
        public void TestAddUnknownArguments() {
            MethodDeclarationNode mdn = new MethodDeclarationNode("MyMethod");
            Node[] args = new Node[] { new TypeNode("int", true), new VariableDeclarationNode("bar") };
            mdn.AddUnknownArguments(args);
            Assert.IsNotNull(mdn.UnknownArguments);
            Assert.AreEqual(args.Length, mdn.UnknownArguments.Count);
            for(int i = 0; i < args.Length; i++) {
                Assert.AreEqual(args[i], mdn.UnknownArguments[i]);
            }

            List<Node> args2 = new List<Node>() { new VariableDeclarationNode("xyzzy"), new VariableDeclarationNode("myParam") };
            mdn.AddUnknownArguments(args2);
            Assert.AreEqual(args.Length + args2.Count, mdn.UnknownArguments.Count);
            for(int i = 0; i < mdn.UnknownArguments.Count; i++) {
                if(i < args.Length) {
                    Assert.AreEqual(args[i], mdn.UnknownArguments[i]);
                } else {
                    Assert.AreEqual(args2[i - args.Length], mdn.UnknownArguments[i]);
                }
            }
        }

        [Test]
        public void TestClearUnknownArguments_Null() {
            MethodDeclarationNode mdn = new MethodDeclarationNode("foo");
            mdn.ClearUnknownArguments();
            Assert.IsNotNull(mdn.UnknownArguments);
            Assert.AreEqual(0, mdn.UnknownArguments.Count);
        }

        [Test]
        public void TestClearUnknownArguments_ExistingArgs() {
            MethodDeclarationNode mdn = new MethodDeclarationNode("MyMethod");
            Node[] args = new Node[] { new TypeNode("int", true), new VariableDeclarationNode("bar") };
            mdn.AddUnknownArguments(args);
            Assert.AreEqual(2, mdn.UnknownArguments.Count);
            mdn.ClearUnknownArguments();
            Assert.AreEqual(0, mdn.UnknownArguments.Count);
        }

        [Test]
        public void TestAddSecondaryArgument() {
            MethodDeclarationNode mdn = new MethodDeclarationNode("MyMethod");
            VariableDeclarationNode vdn = new VariableDeclarationNode("foo");
            Assert.IsNull(mdn.SecondaryArguments);
            mdn.AddSecondaryArgument(vdn, new WordNode("to", PartOfSpeechTag.Preposition));
            Assert.AreEqual(1, mdn.SecondaryArguments.Count);
            var sec = mdn.SecondaryArguments[0];
            Assert.AreEqual(vdn, sec.Argument);
            Assert.AreEqual("to", sec.Preposition.Text);
            Assert.AreEqual(PartOfSpeechTag.Preposition, sec.Preposition.Tag);

            VariableDeclarationNode vdn2 = new VariableDeclarationNode("myParam");
            mdn.AddSecondaryArgument(vdn2, new WordNode("from", PartOfSpeechTag.Preposition));
            Assert.AreEqual(2, mdn.SecondaryArguments.Count);
            sec = mdn.SecondaryArguments[0];
            Assert.AreEqual(vdn, sec.Argument);
            Assert.AreEqual("to", sec.Preposition.Text);
            Assert.AreEqual(PartOfSpeechTag.Preposition, sec.Preposition.Tag);
            sec = mdn.SecondaryArguments[1];
            Assert.AreEqual(vdn2, sec.Argument);
            Assert.AreEqual("from", sec.Preposition.Text);
            Assert.AreEqual(PartOfSpeechTag.Preposition, sec.Preposition.Tag);
        }

        [Test]
        public void TestClearSecondaryArguments_Null() {
            MethodDeclarationNode mdn = new MethodDeclarationNode("foo");
            mdn.ClearSecondaryArguments();
            Assert.IsNotNull(mdn.SecondaryArguments);
            Assert.AreEqual(0, mdn.SecondaryArguments.Count);
        }

        [Test]
        public void TestClearSecondaryArguments_Null_ExistingArgs() {
            MethodDeclarationNode mdn = new MethodDeclarationNode("MyMethod");
            VariableDeclarationNode vdn = new VariableDeclarationNode("foo");
            mdn.AddSecondaryArgument(vdn, new WordNode("to", PartOfSpeechTag.Preposition));
            Assert.AreEqual(1, mdn.SecondaryArguments.Count);
            mdn.ClearSecondaryArguments();
            Assert.AreEqual(0, mdn.SecondaryArguments.Count);
        }

        [Test]
        public void TestCreateThemeFromPhrases() {
            MethodDeclarationNode mdn = new MethodDeclarationNode("MyMethod");
            PhraseNode pn1 = new PhraseNode(new string[] { "hello", "World" });
            PhraseNode pn2 = new PhraseNode(new string[] { "cowboy", "watermelon" });
            mdn.CreateThemeFromPhrases(pn1, pn2);
            Assert.AreSame(pn1, mdn.Theme);
            Assert.AreEqual(4, pn1.Size());
            Assert.AreEqual("hello", pn1[0].Text);
            Assert.AreEqual("World", pn1[1].Text);
            Assert.AreEqual("cowboy", pn1[2].Text);
            Assert.AreEqual("watermelon", pn1[3].Text);
        }

        [Test]
        public void TestCreateThemeFromPhrases_FirstNull() {
            MethodDeclarationNode mdn = new MethodDeclarationNode("MyMethod");
            PhraseNode pn1 = new PhraseNode(new string[] { "hello", "World" });
            mdn.CreateThemeFromPhrases(pn1, null);
            Assert.AreSame(pn1, mdn.Theme);
            Assert.AreEqual(2, pn1.Size());
            Assert.AreEqual("hello", pn1[0].Text);
            Assert.AreEqual("World", pn1[1].Text);
        }

        [Test]
        public void TestCreateThemeFromPhrases_SecondNull() {
            MethodDeclarationNode mdn = new MethodDeclarationNode("MyMethod");
            PhraseNode pn2 = new PhraseNode(new string[] { "cowboy", "watermelon" });
            mdn.CreateThemeFromPhrases(null, pn2);
            Assert.AreSame(pn2, mdn.Theme);
            Assert.AreEqual(2, pn2.Size());
            Assert.AreEqual("cowboy", pn2[0].Text);
            Assert.AreEqual("watermelon", pn2[1].Text);
        }

        [Test]
        public void TestAssignStructuralInformation() {
            var formals = new FormalParameterRecord[] { new FormalParameterRecord("SGVData*", false, "p"), new FormalParameterRecord("ASSchedule*", false, "p2") };
            MethodContext mc = new MethodContext("int", true, "MyClass", formals, true, false, false);
            MethodDeclarationNode mdn = new MethodDeclarationNode("MyMethod", mc);
            mdn.Parse(new NullSplitter());
            mdn.AssignStructuralInformation(new NullSplitter(), new NullTagger());
            Assert.AreEqual(Location.Name, mdn.ParsedName.Location);
            Assert.AreEqual("int", mdn.ReturnType.Name);
            Assert.IsTrue(mdn.ReturnType.IsPrimitive);
            Assert.AreEqual("MyClass", mdn.DeclaringClass.Name);
            Assert.AreEqual(2, mdn.FormalParameters.Count);
            Assert.AreEqual("SGVData*", mdn.FormalParameters[0].Type.Name);
            Assert.AreEqual("p", mdn.FormalParameters[0].Name);
            Assert.AreEqual("ASSchedule*", mdn.FormalParameters[1].Type.Name);
            Assert.AreEqual("p2", mdn.FormalParameters[1].Name);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestAssignStructuralInformation_NoContext() {
            MethodDeclarationNode mdn = new MethodDeclarationNode("MyMethod");
            mdn.AssignStructuralInformation(new NullSplitter(), new NullTagger());
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestAssignStructuralInformation_NullSplitter() {
            MethodDeclarationNode mdn = new MethodDeclarationNode("MyMethod");
            mdn.AssignStructuralInformation(null, new NullTagger());
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestAssignStructuralInformation_NullTagger() {
            MethodDeclarationNode mdn = new MethodDeclarationNode("MyMethod");
            mdn.AssignStructuralInformation(new NullSplitter(), null);
        }

        [Test]
        public void TestCreateEquivalenceFromUnknownArguments() {
            var arg1 = new VariableDeclarationNode("arg1");
            var arg2 = new VariableDeclarationNode("arg2");
            var arg3 = new VariableDeclarationNode("arg3");
            var arg4 = new VariableDeclarationNode("arg4");
            var mdn = new MethodDeclarationNode("MyMethod");
            mdn.AddUnknownArguments(new VariableDeclarationNode[] { arg1, arg2, arg3, arg4 });
            Assert.AreEqual(4, mdn.UnknownArguments.Count);

            var themeNode = new PhraseNode(new string[] { "Relevent", "Args" });
            EquivalenceNode equiv = mdn.CreateEquivalenceFromUnknownArguments(themeNode, new bool[] { true, false, true, false });
            Assert.AreEqual(3, equiv.EquivalentNodes.Count);
            Assert.IsTrue(equiv.EquivalentNodes.Contains(themeNode));
            Assert.IsTrue(equiv.EquivalentNodes.Contains(arg1));
            Assert.IsTrue(equiv.EquivalentNodes.Contains(arg3));
            Assert.AreEqual(2, mdn.UnknownArguments.Count);
            Assert.IsTrue(mdn.UnknownArguments.Contains(arg2));
            Assert.IsTrue(mdn.UnknownArguments.Contains(arg4));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestCreateEquivalenceFromUnknownArguments_NullNode() {
            var arg1 = new VariableDeclarationNode("arg1");
            var mdn = new MethodDeclarationNode("MyMethod");
            mdn.AddUnknownArguments(new VariableDeclarationNode[] { arg1 });
            EquivalenceNode equiv = mdn.CreateEquivalenceFromUnknownArguments(null, new bool[] { true });
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestCreateEquivalenceFromUnknownArguments_MissizedArray() {
            var arg1 = new VariableDeclarationNode("arg1");
            var mdn = new MethodDeclarationNode("MyMethod");
            mdn.AddUnknownArguments(new VariableDeclarationNode[] { arg1 });
            EquivalenceNode equiv = mdn.CreateEquivalenceFromUnknownArguments(new PhraseNode(), new bool[] { true, false });
        }

        [Test]
        public void TestToPlainString_DefaultContext() {
            var mdn = new MethodDeclarationNode("MyMethod", new MethodContext());
            var splitter = new ConservativeIdSplitter();
            mdn.Parse(splitter);
            mdn.AssignStructuralInformation(splitter, new UnigramTagger());
            Assert.AreEqual("My Method", mdn.ToPlainString());
        }

        [Test]
        public void TestToPlainString_Context() {
            var formals = new FormalParameterRecord[] { new FormalParameterRecord("SGVData*", false, "p"), new FormalParameterRecord("ASSchedule*", false, "p2") };
            MethodContext mc = new MethodContext("int", true, "MyClass", formals, true, false, false);
            MethodDeclarationNode mdn = new MethodDeclarationNode("CalcNewValue", mc);
            var splitter = new ConservativeIdSplitter();
            mdn.Parse(splitter);
            mdn.AssignStructuralInformation(splitter, new UnigramTagger());
            Assert.AreEqual("Calc New Value", mdn.ToPlainString());
        }

        [Test]
        public void TestToPlainString_NotParsed() {
            var formals = new FormalParameterRecord[] { new FormalParameterRecord("SGVData*", false, "p"), new FormalParameterRecord("ASSchedule*", false, "p2") };
            MethodContext mc = new MethodContext("int", true, "MyClass", formals, true, false, false);
            MethodDeclarationNode mdn = new MethodDeclarationNode("CalcNewValue", mc);
            Assert.AreEqual("CalcNewValue", mdn.ToPlainString());
        }

        [Test]
        public void TestGetParse() {
            MethodContext mc = new MethodContext("int", true, "MyClass", null, true, false, false);
            MethodDeclarationNode mdn = new MethodDeclarationNode("CalcNewValue", mc);
            UnigramSwumBuilder builder = new UnigramSwumBuilder();
            builder.ApplyRules(mdn);

            var parsedName = mdn.GetParse();
            Assert.AreEqual(3, parsedName.Size());
            Assert.AreEqual("Calc", parsedName[0].Text);
            Assert.AreEqual(PartOfSpeechTag.Verb, parsedName[0].Tag);
            Assert.AreEqual("New", parsedName[1].Text);
            Assert.AreEqual(PartOfSpeechTag.NounModifier, parsedName[1].Tag);
            Assert.AreEqual("Value", parsedName[2].Text);
            Assert.AreEqual(PartOfSpeechTag.NounIgnorable, parsedName[2].Tag);
        }


    }
}
