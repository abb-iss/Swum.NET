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

namespace ABB.Swum
{
    /// <summary>
    /// Constructs the Software Word Use Model using a defined set of rules.
    /// </summary>
    public class SwumBuilder
    {
        /// <summary>
        /// The set of rules to use for building SWUMs.
        /// </summary>
        protected IEnumerable<SwumRule> Rules;

        /// <summary>
        /// Creates a new SwumBuilder with the default rule set.
        /// </summary>
        //The rule set is not actually initialized until ApplyRules is called.
        //This is so that child classes can define additional properties as necessary for their rules, and these
        //can be set prior to the rule set being initialized.
        public SwumBuilder() { }

        /// <summary>
        /// Creates a new SwumBuilder with the specified rule set.
        /// </summary>
        /// <param name="ruleSet">The set of rules to use for building SWUMs.</param>
        public SwumBuilder(IEnumerable<SwumRule> ruleSet)
        {
            this.Rules = ruleSet;
        }

        /// <summary>
        /// Initializes the list of rules to use for building SWUMs.
        /// </summary>
        protected virtual void DefineRuleSet()
        {
            this.Rules = new List<SwumRule>();
        }

        /// <summary>
        /// Applies the rules defined within this SwumBuilder to the specified node.
        /// The first matching rule found is used to construct the SWUM.
        /// </summary>
        /// <param name="node">The ProgramElementNode to construct a SWUM on.</param>
        /// <returns>The SwumRule that was used to construct the SWUM for the given node.</returns>
        public SwumRule ApplyRules(ProgramElementNode node)
        {
            if (Rules == null) { DefineRuleSet(); }

            foreach (SwumRule rule in this.Rules)
            {
                if (rule.InClass(node))
                {
                    rule.ConstructSwum(node);
                    return rule;
                }
            }
            return null;
        }
    }
}
