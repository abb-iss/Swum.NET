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
using System.Xml;
using System.Xml.Linq;
using NUnit.Framework;
using ABB.SrcML;
using ABB.Swum;
using ABB.Swum.Nodes;
using ABB.Swum.WordData;

namespace ABB.Swum.Tests
{
    /// <summary>
    /// Summary description for BaseVerbRuleTests
    /// </summary>
    [TestFixture]
    public class BaseVerbRuleTests
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

            //initialize swum stuff
            splitter = new ConservativeIdSplitter();
            tagger = new UnigramTagger();
            posData = new PCKimmoPartOfSpeechData();
        }

        [Test]
        public void TestInClass()
        {
            string testSrcML = "<function><type><name>int</name></type> <name><name>CBidMarkup</name><op:operator>::</op:operator><name>modifyBid</name></name><parameter_list>(<param><decl><type><name>bool</name></type> <name>Recalc</name></decl></param>)</parameter_list><block>{<return>return <expr><lit:literal type=\"number\">0</lit:literal></expr>;</return>}</block></function>";
            XElement xml = XElement.Parse(string.Format(srcMLFormat, testSrcML), LoadOptions.PreserveWhitespace);
            MethodContext mc = ContextBuilder.BuildMethodContext(xml.Descendants(SRC.Function).First());

            MethodDeclarationNode mdn = new MethodDeclarationNode("modifyBid", mc);
            BaseVerbRule rule = new BaseVerbRule(posData, tagger, splitter);
            Console.WriteLine("InClass(): {0}", rule.InClass(mdn));
            rule.ConstructSwum(mdn);
            Console.WriteLine(mdn.ToString());
        }
    }
}
