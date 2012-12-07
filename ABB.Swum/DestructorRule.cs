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
    /// The rule to construct the SWUM for destructor methods.
    /// </summary>
    public class DestructorRule : UnigramRule
    {
        /// <summary>
        /// Creates a new DestructorRule using default values.
        /// </summary>
        public DestructorRule() : base() { }

        /// <summary>
        /// Creates a new DestructorRule.
        /// </summary>
        /// <param name="posData">The part-of-speech data to use.</param>
        /// <param name="tagger">The part-of-speech tagger to use.</param>
        /// <param name="splitter">The identifier splitter to use.</param>
        public DestructorRule(PartOfSpeechData posData, Tagger tagger, IdSplitter splitter) : base(posData, tagger, splitter) { }

        /// <summary>
        /// Determines whether the supplied node meets the criteria for this rule.
        /// </summary>
        /// <param name="node">The node to test.</param>
        /// <returns>True if the node meets this rule, False otherwise.</returns>
        public override bool InClass(ProgramElementNode node)
        {
            return (node is MethodDeclarationNode) && ((MethodDeclarationNode)node).Context != null && ((MethodDeclarationNode)node).Context.IsDestructor;
        }

        /// <summary>
        /// Constructs the Software Word Use Model for the given node. 
        /// This method assumes that the node has already been tested to satisfy this Rule, using the InClass method.
        /// </summary>
        /// <param name="node">The node to construct SWUM for.</param>
        public override void ConstructSwum(ProgramElementNode node)
        {
            if (node is MethodDeclarationNode)
            {
                MethodDeclarationNode mdn = (MethodDeclarationNode)node;
                mdn.Parse(this.Splitter);
                this.PosTagger.TagNounPhrase(mdn.ParsedName);

                mdn.AssignStructuralInformation(this.Splitter, this.PosTagger);
                mdn.Theme = mdn.ParsedName;
                mdn.AddUnknownArguments(mdn.FormalParameters);

                //TODO: from Emily, how to fill in Action?
                mdn.IsDestructor = true;
                mdn.SwumRuleUsed = this;
            }
            else
            {
                //TODO: return error?
            }
        }
    }
}
