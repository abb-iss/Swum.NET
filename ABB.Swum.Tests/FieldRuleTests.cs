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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using NUnit.Framework;
using ABB.SrcML;
using ABB.Swum;
using ABB.Swum.Nodes;
using ABB.Swum.WordData;

namespace ABB.Swum.Tests
{
    [TestFixture]
    public class FieldRuleTests
    {
        private static string srcMLFormat;
        private static ConservativeIdSplitter splitter;
        private static UnigramTagger tagger;
        private static PCKimmoPartOfSpeechData posData;

        [TestFixtureSetUp]
        public static void ClassSetup()
        {
            //construct the necessary srcML wrapper unit tags
            XmlNamespaceManager xnm = ABB.SrcML.SrcML.NamespaceManager;
            StringBuilder namespaceDecls = new StringBuilder();
            foreach (string prefix in xnm)
            {
                if (prefix != string.Empty && !prefix.StartsWith("xml", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (prefix.Equals("src", StringComparison.InvariantCultureIgnoreCase))
                    {
                        namespaceDecls.AppendFormat("xmlns=\"{0}\" ", xnm.LookupNamespace(prefix));
                    }
                    else
                    {
                        namespaceDecls.AppendFormat("xmlns:{0}=\"{1}\" ", prefix, xnm.LookupNamespace(prefix));
                    }
                }
            }
            srcMLFormat = string.Format("<unit {0}>{{0}}</unit>", namespaceDecls.ToString());

            splitter = new ConservativeIdSplitter();
            tagger = new UnigramTagger();
            posData = new PCKimmoPartOfSpeechData();
        }

        [Test]
        public void TestToString()
        {
            FieldRule rule = new FieldRule(null, null, null);
            Console.WriteLine(rule.ToString());
        }

        [Test]
        public void TestConstructSwum()
        {
            string testSrcML = "<class>class <name>foo</name> <block>{<private type=\"default\"><decl_stmt><decl><type><name>int</name></type> <name>a</name></decl>;</decl_stmt></private>}</block>;</class>";
            XElement xml = XElement.Parse(string.Format(srcMLFormat, testSrcML), LoadOptions.PreserveWhitespace);
            FieldContext fc = ContextBuilder.BuildFieldContext(xml.Descendants(SRC.Declaration).First());

            FieldDeclarationNode fdn = new FieldDeclarationNode("a", fc);
            FieldRule rule = new FieldRule(posData, tagger, splitter);
            rule.ConstructSwum(fdn);
            Console.WriteLine(fdn.ToString());
        }
    }
}
