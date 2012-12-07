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

namespace ABB.Swum.Nodes {
    /// <summary>
    /// Represents a node that is passed as an argument to a method.
    /// </summary>
    public class ArgumentNode : Node {
        /// <summary>
        /// The node being passed as an argument;
        /// </summary>
        public Node Argument { get; set; }
        /// <summary>
        /// A prepostion describing the argument's relationship to its method.
        /// </summary>
        public WordNode Preposition { get; set; }

        /// <summary>
        /// Creates a new ArgumentNode.
        /// </summary>
        /// <param name="argument">The node being passed as an argument.</param>
        /// <param name="preposition">A prepostion describing the argument's relationship to its method.</param>
        public ArgumentNode(Node argument, WordNode preposition) {
            this.Argument = argument;
            this.Preposition = preposition;
        }

        /// <summary>
        /// The program location of the argument.
        /// </summary>
        public override Location Location {
            get { return Argument.Location; }
        }

        /// <summary>
        /// Converts the ArgumentNode to a string representation, containing the preposition and a string representation of the argument.
        /// </summary>
        /// <returns>A string representation of the ArgumentNode.</returns>
        public override string ToString() {
            return string.Format("{0} {1}", Preposition, Argument);
        }

        /// <summary>
        /// Returns a string representation of the node without any SWUM markup.
        /// </summary>
        /// <returns></returns>
        public override string ToPlainString() {
            return Argument.ToPlainString();
        }

        /// <summary>
        /// Returns a PhraseNode containing the node's parsed name.
        /// </summary>
        public override PhraseNode GetParse() {
            return Argument.GetParse();
        }
    }
}
