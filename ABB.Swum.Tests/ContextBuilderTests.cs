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
using System.IO;
using System.Xml;
using System.Xml.Linq;
using NUnit.Framework;
using ABB.Swum;
using ABB.SrcML;

namespace ABB.Swum.Tests {
    [TestFixture]
    public class ContextBuilderTests {
        private static string srcMLFormat;

        [TestFixtureSetUp]
        public static void ClassSetup() {
            //construct the necessary srcML wrapper unit tags
            XmlNamespaceManager xnm = ABB.SrcML.SrcML.NamespaceManager;
            StringBuilder namespaceDecls = new StringBuilder();
            foreach(string prefix in xnm) {
                if(prefix != string.Empty && !prefix.StartsWith("xml", StringComparison.InvariantCultureIgnoreCase)) {
                    if(prefix.Equals("src", StringComparison.InvariantCultureIgnoreCase)) {
                        namespaceDecls.AppendFormat("xmlns=\"{0}\" ", xnm.LookupNamespace(prefix));
                    } else {
                        namespaceDecls.AppendFormat("xmlns:{0}=\"{1}\" ", prefix, xnm.LookupNamespace(prefix));
                    }
                }
            }
            srcMLFormat = string.Format("<unit {0}>{{0}}</unit>", namespaceDecls.ToString());
        }

        #region ConstructTypeName tests

        [Test]
        public void TestConstructTypeName_MultipleNames() {
            string testSrcML = "<type><name>static</name> <name>int</name></type>";
            XElement xml = XElement.Parse(string.Format(srcMLFormat, testSrcML), LoadOptions.PreserveWhitespace);

            bool isPrimitive;
            Assert.AreEqual("int", ContextBuilder.ConstructTypeName(xml.Element(SRC.Type), out isPrimitive));
            Assert.AreEqual(true, isPrimitive);
        }

        [Test]
        public void TestConstructTypeName_Pointer() {
            string testSrcML = "<type><name>int</name><type:modifier>*</type:modifier></type>";
            XElement xml = XElement.Parse(string.Format(srcMLFormat, testSrcML), LoadOptions.PreserveWhitespace);

            bool isPrimitive;
            Assert.AreEqual("int*", ContextBuilder.ConstructTypeName(xml.Element(SRC.Type), out isPrimitive));
            Assert.AreEqual(true, isPrimitive);
        }

        [Test]
        public void TestConstructTypeName_Reference() {
            string testSrcML = "<type><name>int</name><type:modifier>&amp;</type:modifier></type>";
            XElement xml = XElement.Parse(string.Format(srcMLFormat, testSrcML), LoadOptions.PreserveWhitespace);

            bool isPrimitive;
            Assert.AreEqual("int&", ContextBuilder.ConstructTypeName(xml.Element(SRC.Type), out isPrimitive));
            Assert.AreEqual(true, isPrimitive);
        }

        [Test]
        public void TestConstructTypeName_MultipleModifiers() {
            string testSrcML = "<type><name>STATUS</name><type:modifier>*</type:modifier><type:modifier>&amp;</type:modifier></type>";
            XElement xml = XElement.Parse(string.Format(srcMLFormat, testSrcML), LoadOptions.PreserveWhitespace);

            bool isPrimitive;
            Assert.AreEqual("STATUS*&", ContextBuilder.ConstructTypeName(xml.Element(SRC.Type), out isPrimitive));
            Assert.AreEqual(false, isPrimitive);
        }

        [Test]
        public void TestConstructTypeName_Namespace() {
            string testSrcML = "<type><name><name>std</name><op:operator>::</op:operator><name>string</name></name></type>";
            XElement xml = XElement.Parse(string.Format(srcMLFormat, testSrcML), LoadOptions.PreserveWhitespace);

            bool isPrimitive;
            Assert.AreEqual("string", ContextBuilder.ConstructTypeName(xml.Element(SRC.Type), out isPrimitive));
            Assert.AreEqual(false, isPrimitive);
        }

        [Test]
        public void TestConstructTypeName_MultiplePrimitive() {
            string testSrcML = "<type><name>static</name> <name>unsigned</name> <name>short</name> <name>int</name></type>";
            XElement xml = XElement.Parse(string.Format(srcMLFormat, testSrcML), LoadOptions.PreserveWhitespace);

            bool isPrimitive;
            Assert.AreEqual("unsigned short int", ContextBuilder.ConstructTypeName(xml.Element(SRC.Type), out isPrimitive));
            Assert.AreEqual(true, isPrimitive);
        }

        [Test]
        public void TestConstructTypeName_MultiplePrimitiveWrongCase() {
            string testSrcML = "<type><name>static</name> <name>Unsigned</name> <name>char</name></type>";
            XElement xml = XElement.Parse(string.Format(srcMLFormat, testSrcML), LoadOptions.PreserveWhitespace);

            bool isPrimitive;
            Assert.AreEqual("char", ContextBuilder.ConstructTypeName(xml.Element(SRC.Type), out isPrimitive));
            Assert.AreEqual(true, isPrimitive);
        }

        [Test]
        public void TestConstructTypeName_ModifierOnNonPrimitive() {
            string testSrcML = "<type><name>unsigned</name> <name>BAR</name></type>";
            XElement xml = XElement.Parse(string.Format(srcMLFormat, testSrcML), LoadOptions.PreserveWhitespace);

            bool isPrimitive;
            Assert.AreEqual("BAR", ContextBuilder.ConstructTypeName(xml.Element(SRC.Type), out isPrimitive));
            Assert.AreEqual(false, isPrimitive);
        }

        [Test]
        public void TestConstructTypeName_ConstPrimitive() {
            string testSrcML = "<type><name>const</name> <name>int</name></type>";
            XElement xml = XElement.Parse(string.Format(srcMLFormat, testSrcML), LoadOptions.PreserveWhitespace);

            bool isPrimitive;
            Assert.AreEqual("int", ContextBuilder.ConstructTypeName(xml.Element(SRC.Type), out isPrimitive));
            Assert.AreEqual(true, isPrimitive);
        }

        [Test]
        public void TestConstructTypeName_ConstObject() {
            string testSrcML = "<type><name>const</name> <name>Foo</name></type>";
            XElement xml = XElement.Parse(string.Format(srcMLFormat, testSrcML), LoadOptions.PreserveWhitespace);

            bool isPrimitive;
            Assert.AreEqual("Foo", ContextBuilder.ConstructTypeName(xml.Element(SRC.Type), out isPrimitive));
            Assert.AreEqual(false, isPrimitive);
        }

        [Test]
        public void TestConstructTypeName_ConstPointer() {
            string testSrcML = "<type><name>Foo</name><type:modifier>*</type:modifier> <name>const</name></type>";
            XElement xml = XElement.Parse(string.Format(srcMLFormat, testSrcML), LoadOptions.PreserveWhitespace);

            bool isPrimitive;
            Assert.AreEqual("Foo*", ContextBuilder.ConstructTypeName(xml.Element(SRC.Type), out isPrimitive));
            Assert.AreEqual(false, isPrimitive);
        }

        [Test]
        public void TestConstructTypeName_PointerToConstObj() {
            string testSrcML = "<type><name>Foo</name> <name>const</name><type:modifier>*</type:modifier></type>";
            XElement xml = XElement.Parse(string.Format(srcMLFormat, testSrcML), LoadOptions.PreserveWhitespace);

            bool isPrimitive;
            Assert.AreEqual("Foo*", ContextBuilder.ConstructTypeName(xml.Element(SRC.Type), out isPrimitive));
            Assert.AreEqual(false, isPrimitive);
        }

        [Test]
        public void TestConstructTypeName_ConstPointerToConstObj() {
            string testSrcML = "<type><name>Foo</name> <name>const</name><type:modifier>*</type:modifier> <name>const</name></type>";
            XElement xml = XElement.Parse(string.Format(srcMLFormat, testSrcML), LoadOptions.PreserveWhitespace);

            bool isPrimitive;
            Assert.AreEqual("Foo*", ContextBuilder.ConstructTypeName(xml.Element(SRC.Type), out isPrimitive));
            Assert.AreEqual(false, isPrimitive);
        }
        
        [Test]
        public void TestConstructTypeName_PointerConstPointerToConstObj() {
            string testSrcML = "<type><name>Foo</name> <name>const</name><type:modifier>*</type:modifier> <name>const</name><type:modifier>*</type:modifier></type>";
            XElement xml = XElement.Parse(string.Format(srcMLFormat, testSrcML), LoadOptions.PreserveWhitespace);

            bool isPrimitive;
            Assert.AreEqual("Foo**", ContextBuilder.ConstructTypeName(xml.Element(SRC.Type), out isPrimitive));
            Assert.AreEqual(false, isPrimitive);
        }

        #endregion

        #region BuildMethodContext tests

        private bool MethodContextsAreEqual(MethodContext mc1, MethodContext mc2) {
            if(mc1 == null && mc2 == null) {
                return true;
            }
            if((mc1 == null) ^ (mc2 == null)) {
                return false;
            }
            if(mc1.DeclaringClass != mc2.DeclaringClass
               || mc1.IsConstructor != mc2.IsConstructor
               || mc1.IsDestructor != mc2.IsDestructor
               || mc1.IsStatic != mc2.IsStatic
               || mc1.IdType != mc2.IdType
               || mc1.IdTypeIsPrimitive != mc2.IdTypeIsPrimitive
               || mc1.FormalParameters.Count != mc2.FormalParameters.Count) {
                return false;
            }
            for(int i = 0; i < mc1.FormalParameters.Count; i++) {
                if(!mc1.FormalParameters[i].Equals(mc2.FormalParameters[i])) {
                    return false;
                }
            }
            return true;
        }

        [Test]
        public void TestBuildMethodContext_FunctionWithClassAndParams() {
            string testSrcML = "<function><type><name>int</name></type> <name><name>CBidMarkup</name><op:operator>::</op:operator><name>modifyBid</name></name><parameter_list>(<param><decl><type><name>bool</name></type> <name>Recalc</name></decl></param>, <param><decl><type><name>char</name><type:modifier>*</type:modifier></type> <name>Foo</name></decl></param>)</parameter_list><block>{ <return>return <expr><lit:literal type=\"number\">0</lit:literal></expr>;</return> }</block></function>";
            XElement xml = XElement.Parse(string.Format(srcMLFormat, testSrcML), LoadOptions.PreserveWhitespace);
            var formals = new FormalParameterRecord[] {new FormalParameterRecord("bool", true, "Recalc"), new FormalParameterRecord("char*", true, "Foo")};

            MethodContext mc1 = new MethodContext("int", true, "CBidMarkup", formals, false, false, false);
            MethodContext mc2 = ContextBuilder.BuildMethodContext(xml.Element(SRC.Function));
            Assert.IsTrue(MethodContextsAreEqual(mc1, mc2));
        }

        [Test]
        public void TestBuildMethodContext_FunctionNoParams() {
            string testSrcML = "<function><type><name>int</name></type> <name>GetZero</name><parameter_list>()</parameter_list> <block>{ <return>return <expr><lit:literal type=\"number\">0</lit:literal></expr>;</return> }</block></function>";
            XElement xml = XElement.Parse(string.Format(srcMLFormat, testSrcML), LoadOptions.PreserveWhitespace);

            MethodContext mc1 = new MethodContext("int", true, "", new List<FormalParameterRecord>(), false, false, false);
            MethodContext mc2 = ContextBuilder.BuildMethodContext(xml.Element(SRC.Function));
            Assert.IsTrue(MethodContextsAreEqual(mc1, mc2));
        }

        [Test]
        public void TestBuildMethodContext_StaticFunctionWithClassAndParams() {
            string testSrcML = "<function><type><name>static</name> <name>char</name></type> <name><name>CBidMarkup</name><op:operator>::</op:operator><name>run</name></name><parameter_list>(<param><decl><type><name>CGVDate</name> <type:modifier>&amp;</type:modifier></type> <name>CurrDate</name></decl></param>)</parameter_list> <block>{ <return>return <expr><lit:literal type=\"char\">'a'</lit:literal></expr>;</return> }</block></function>";
            XElement xml = XElement.Parse(string.Format(srcMLFormat, testSrcML), LoadOptions.PreserveWhitespace);
            var formals = new FormalParameterRecord[] {new FormalParameterRecord("CGVDate&", false, "CurrDate")};

            MethodContext mc1 = new MethodContext("char", true, "CBidMarkup", formals, true, false, false);
            MethodContext mc2 = ContextBuilder.BuildMethodContext(xml.Element(SRC.Function));
            Assert.IsTrue(MethodContextsAreEqual(mc1, mc2));
        }

        [Test]
        public void TestBuildMethodContext_Constructor() {
            string testSrcML = "<constructor><name><name>CBidMarkup</name><op:operator>::</op:operator><name>CBidMarkup</name></name><parameter_list>(<param><decl><type><name>SGVData</name> <type:modifier>*</type:modifier></type> <name>p</name></decl></param>, <param><decl><type><name>ASSchedule</name> <type:modifier>*</type:modifier></type> <name>p2</name></decl></param>)</parameter_list> <block>{ }</block></constructor>";
            XElement xml = XElement.Parse(string.Format(srcMLFormat, testSrcML), LoadOptions.PreserveWhitespace);
            var formals = new FormalParameterRecord[] {new FormalParameterRecord("SGVData*", false, "p"), new FormalParameterRecord("ASSchedule*", false, "p2")};

            MethodContext mc1 = new MethodContext("", false, "CBidMarkup", formals, false, true, false);
            MethodContext mc2 = ContextBuilder.BuildMethodContext(xml.Element(SRC.Constructor));
            Assert.IsTrue(MethodContextsAreEqual(mc1, mc2));
        }

        [Test]
        public void TestBuildMethodContext_Destructor() {
            string testSrcML = "<destructor><name><name>CBidMarkup</name><op:operator>::</op:operator>~<name>CBidMarkup</name></name><parameter_list>()</parameter_list> <block>{ }</block></destructor>";
            XElement xml = XElement.Parse(string.Format(srcMLFormat, testSrcML), LoadOptions.PreserveWhitespace);

            MethodContext mc1 = new MethodContext("", false, "CBidMarkup", new List<FormalParameterRecord>(), false, false, true);
            MethodContext mc2 = ContextBuilder.BuildMethodContext(xml.Element(SRC.Destructor));
            Assert.IsTrue(MethodContextsAreEqual(mc1, mc2));
        }

        [Test]
        public void TestBuildMethodContext_InlineFunction() {
            string testSrcML = "<class>class <name>CBidMarkup</name><block>{<private type=\"default\"> <function><type><name>int</name></type> <name>run</name><parameter_list>(<param><decl><type><name>CGVDate</name> <type:modifier>&amp;</type:modifier></type> <name>CurrDate</name></decl></param>)</parameter_list> <block>{ <return>return <expr><op:operator>-</op:operator><lit:literal type=\"number\">1</lit:literal></expr>;</return>}</block></function> </private>}</block>;</class>";
            XElement xml = XElement.Parse(string.Format(srcMLFormat, testSrcML), LoadOptions.PreserveWhitespace);
            var formals = new FormalParameterRecord[] {new FormalParameterRecord("CGVDate&", false, "CurrDate")};

            MethodContext mc1 = new MethodContext("int", true, "CBidMarkup", formals, false, false, false);
            MethodContext mc2 = ContextBuilder.BuildMethodContext(xml.Descendants(SRC.Function).First());
            Assert.IsTrue(MethodContextsAreEqual(mc1, mc2));
        }

        [Test]
        public void TestBuildMethodContext_InlineFunctionInStruct() {
            string testSrcML = "<struct>struct <name>MyStruct</name> <block>{<public type=\"default\"> <function><type><name>int</name></type> <name>foo</name><parameter_list>()</parameter_list> <block>{<return>return <expr><lit:literal type=\"number\">0</lit:literal></expr>;</return>}</block></function> </public>}</block>;</struct>";
            XElement xml = XElement.Parse(string.Format(srcMLFormat, testSrcML), LoadOptions.PreserveWhitespace);

            MethodContext mc1 = new MethodContext("int", true, "MyStruct", new List<FormalParameterRecord>(), false, false, false);
            MethodContext mc2 = ContextBuilder.BuildMethodContext(xml.Descendants(SRC.Function).First());
            Assert.IsTrue(MethodContextsAreEqual(mc1, mc2));
        }

        [Test]
        public void TestBuildMethodContext_GlobalFunction() {
            string testSrcML = "<function><type><name>int</name></type> <name>GetZero</name><parameter_list>()</parameter_list> <block>{<return>return <expr><lit:literal type=\"number\">0</lit:literal></expr>;</return>}</block></function>";
            XElement xml = XElement.Parse(string.Format(srcMLFormat, testSrcML), LoadOptions.PreserveWhitespace);

            MethodContext mc1 = new MethodContext("int", true, "", new List<FormalParameterRecord>(), false, false, false);
            MethodContext mc2 = ContextBuilder.BuildMethodContext(xml.Descendants(SRC.Function).First());
            Assert.IsTrue(MethodContextsAreEqual(mc1, mc2));
        }

        #endregion

        #region BuildFieldContext tests

        private bool FieldContextsAreEqual(FieldContext fc1, FieldContext fc2) {
            if(fc1 == null && fc2 == null) {
                return true;
            }
            if((fc1 == null) ^ (fc2 == null)) {
                return false;
            }
            if(fc1.DeclaringClass != fc2.DeclaringClass
               || fc1.IdType != fc2.IdType
               || fc1.IdTypeIsPrimitive != fc2.IdTypeIsPrimitive) {
                return false;
            }
            return true;
        }

        [Test]
        public void TestBuildFieldContext_Class() {
            string testSrcML = "<class>class <name>foo</name> <block>{<private type=\"default\"><decl_stmt><decl><type><name>int</name></type> <name>a</name></decl>;</decl_stmt></private>}</block>;</class>";
            XElement xml = XElement.Parse(string.Format(srcMLFormat, testSrcML), LoadOptions.PreserveWhitespace);

            FieldContext fc1 = new FieldContext("int", true, "foo");
            FieldContext fc2 = ContextBuilder.BuildFieldContext(xml.Descendants(SRC.Declaration).First());
            Assert.IsTrue(FieldContextsAreEqual(fc1, fc2));
        }

        [Test]
        public void TestBuildFieldContext_Struct() {
            string testSrcML = "<struct>struct <name>foo</name> <block>{<public type=\"default\"><decl_stmt><decl><type><name>int</name></type> <name>a</name></decl>;</decl_stmt></public>}</block>;</struct>";
            XElement xml = XElement.Parse(string.Format(srcMLFormat, testSrcML), LoadOptions.PreserveWhitespace);

            FieldContext fc1 = new FieldContext("int", true, "foo");
            FieldContext fc2 = ContextBuilder.BuildFieldContext(xml.Descendants(SRC.Declaration).First());
            Assert.IsTrue(FieldContextsAreEqual(fc1, fc2));
        }

        [Test]
        public void TestBuildFieldContext_Global() {
            string testSrcML = "<decl_stmt><decl><type><name>int</name></type> <name>a</name> =<init> <expr><lit:literal type=\"number\">42</lit:literal></expr></init></decl>;</decl_stmt>";
            XElement xml = XElement.Parse(string.Format(srcMLFormat, testSrcML), LoadOptions.PreserveWhitespace);

            FieldContext fc1 = new FieldContext("int", true, "");
            FieldContext fc2 = ContextBuilder.BuildFieldContext(xml.Descendants(SRC.Declaration).First());
            Assert.IsTrue(FieldContextsAreEqual(fc1, fc2));
        }

        #endregion
    }
}
