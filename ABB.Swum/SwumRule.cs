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
    /// Defines the method for constructing the Software Word Use Model on a program element, provided it meets defined constraints.
    /// </summary>
    public abstract class SwumRule
    {
        /// <summary>
        /// Determines whether the supplied ProgramElementNode matches the conditions of this rule.
        /// </summary>
        /// <param name="node">The ProgramElementNode to test.</param>
        /// <returns>True if the node matches this rule, False otherwise.</returns>
        public abstract bool InClass(ProgramElementNode node);
        /// <summary>
        /// Constructs the Software Word Use Model on the given node, using this Rule.
        /// </summary>
        /// <param name="node">The node to construct the SWUM on.</param>
        public abstract void ConstructSwum(ProgramElementNode node);

        /// <summary>
        /// Returns a string representing the type of the rule.
        /// </summary>
        /// <returns>A string representing the type of the rule.</returns>
        public override string ToString()
        {
            return this.GetType().ToString();
        }
    }
}
