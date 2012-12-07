/******************************************************************************
 * Copyright (c) 2012 ABB Group
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the Eclipse Public License v1.0
 * which accompanies this distribution, and is available at
 * http://www.eclipse.org/legal/epl-v10.html
 *
 * Contributors:
 *    Patrick Francis (ABB Group) - C# implementation and documentation
 *    Emily Hill (Univ. of Delaware) - Original design and implementation
 *****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ABB.Swum.Nodes;
using ABB.Swum.WordData;

namespace ABB.Swum
{
    /// <summary>
    /// The SWUM creation rule for fields in classes.
    /// </summary>
    public class FieldRule : UnigramRule
    {
        /// <summary>
        /// Creates a new FieldRule using the defaults.
        /// </summary>
        public FieldRule() : base() { }
        /// <summary>
        /// Creates a new FieldRule.
        /// </summary>
        /// <param name="posData">The part-of-speech data to use.</param>
        /// <param name="tagger">The part-of-speech tagger to use.</param>
        /// <param name="splitter">The identifier splitter to use.</param>
        public FieldRule(PartOfSpeechData posData, Tagger tagger, IdSplitter splitter) : base(posData, tagger, splitter) { }

        /// <summary>
        /// Determines whether the supplied ProgramElementNode matches the conditions of this rule.
        /// </summary>
        /// <param name="node">The ProgramElementNode to test.</param>
        /// <returns>True if the node matches this rule, False otherwise.</returns>
        public override bool InClass(ProgramElementNode node)
        {
            return (node is FieldDeclarationNode);
        }

        /// <summary>
        /// Constructs the Software Word Use Model on the given node, using this Rule.
        /// </summary>
        /// <param name="node">The node to construct the SWUM on.</param>
        public override void ConstructSwum(ProgramElementNode node)
        {
            if (node is FieldDeclarationNode)
            {
                FieldDeclarationNode fdn = node as FieldDeclarationNode;
                fdn.Parse(this.Splitter);
                this.PosTagger.TagNounPhrase(fdn.ParsedName);
                fdn.AssignStructuralInformation(this.Splitter, this.PosTagger);
                //TODO: set fdn.Type.IsPrimitive
                fdn.SwumRuleUsed = this;
            }
            else
            {
                //TODO: return some sort of error indicator?
                Console.Error.WriteLine("FieldRule.ConstructSwum expected a FieldDeclarationNode, received a {0}", node.GetType());
            }
        }
    }
}
