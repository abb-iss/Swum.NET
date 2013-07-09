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
using ABB.Swum.Nodes;
using ABB.SrcML;
using NUnit.Framework;

namespace ABB.Swum.Tests {
    [TestFixture]
    public class UnigramSwumBuilderTests {
        private SrcMLFileUnitSetup fileUnitSetup;
        private UnigramSwumBuilder builder;

        [TestFixtureSetUp]
        public void ClassSetup() {
            fileUnitSetup = new SrcMLFileUnitSetup(Language.CPlusPlus);
            builder = new UnigramSwumBuilder();
        }

        [Test]
        public void TestLeadingPrepositionRule_OnGetAccObject() {
            var xml = @"<function><type><name>LRESULT</name></type> <name><name>CMenuContainer</name><op:operator>::</op:operator><name>OnGetAccObject</name></name><parameter_list>( <param><decl><type><name>UINT</name></type> <name>uMsg</name></decl></param>, <param><decl><type><name>WPARAM</name></type> <name>wParam</name></decl></param>, <param><decl><type><name>LPARAM</name></type> <name>lParam</name></decl></param>, <param><decl><type><name>BOOL</name><type:modifier>&amp;</type:modifier></type> <name>bHandled</name></decl></param> )</parameter_list> <block>{
    <if>if <condition>(<expr><op:operator>(</op:operator><name>DWORD</name><op:operator>)</op:operator><name>lParam</name><op:operator>==</op:operator><op:operator>(</op:operator><name>DWORD</name><op:operator>)</op:operator><name>OBJID_CLIENT</name> <op:operator>&amp;&amp;</op:operator> <name>m_pAccessible</name></expr>)</condition><then> <block>{
        <return>return <expr><call><name>LresultFromObject</name><argument_list>(<argument><expr><name>IID_IAccessible</name></expr></argument>,<argument><expr><name>wParam</name></expr></argument>,<argument><expr><name>m_pAccessible</name></expr></argument>)</argument_list></call></expr>;</return>
    }</block></then> <else>else <block>{
        <expr_stmt><expr><name>bHandled</name><op:operator>=</op:operator><name>FALSE</name></expr>;</expr_stmt>
        <return>return <expr><lit:literal type=""number"">0</lit:literal></expr>;</return>
    }</block></else></if>
}</block></function>";
            var unit = fileUnitSetup.GetFileUnitForXmlSnippet(xml, "test.cpp");

            var func = unit.Descendants(SRC.Function).First();
            var mdn = new MethodDeclarationNode(SrcMLHelper.GetNameForMethod(func).Value, ContextBuilder.BuildMethodContext(func));
            builder.ApplyRules(mdn);

            Assert.AreEqual(typeof(LeadingPrepositionRule), mdn.SwumRuleUsed.GetType());
            var expected = @"handle(Verb) | On(NounModifier) Get(NounModifier) Acc(NounModifier) Object(NounIgnorable)
	 ++ [UINT(Noun) - u(Unknown) Msg(Unknown)] ++ [WPARAM(Noun) - w(Unknown) Param(Unknown)] ++ [LPARAM(Noun) - l(Unknown) Param(Unknown)] ++ [BOOL(Noun) - b(Unknown) Handled(Unknown)] ++ C(NounModifier) Menu(NounModifier) Container(NounIgnorable) ++ LRESULT(Noun)";
            Assert.AreEqual(expected, mdn.ToString());
        }

        [Test]
        public void TestMethod1() {
        }
    }
}
