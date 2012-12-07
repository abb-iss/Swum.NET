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
    /// A node representing a set of equivalent nodes.
    /// </summary>
    public class EquivalenceNode : Node {
        /// <summary>
        /// A list of the equivalent nodes.
        /// </summary>
        public List<Node> EquivalentNodes { get; set; }

        /// <summary>
        /// Creates a new EquivalenceNode.
        /// </summary>
        /// <param name="n">The initial node to add to the list of equivalent nodes.</param>
        public EquivalenceNode(Node n) {
            AddEquivalentNode(n);
        }

        /// <summary>
        /// Adds a node to the list of equivalent nodes.
        /// </summary>
        /// <param name="n">The node to add.</param>
        public void AddEquivalentNode(Node n) {
            if(EquivalentNodes == null) {
                EquivalentNodes = new List<Node>();
            }
            EquivalentNodes.Add(n);
        }

        /// <summary>
        /// The program location of the first equivalent node.
        /// </summary>
        public override Location Location {
            get {
                if(EquivalentNodes == null || EquivalentNodes.Count == 0) {
                    return Location.None;
                } else {
                    return EquivalentNodes[0].Location;
                }
            }
        }

        /// <summary>
        /// Converts the EquivalenceNode to a string representation, comprising "EQUIV" and a string representation of the equivalent nodes.
        /// </summary>
        /// <returns>A string representation of the EquivalenceNode.</returns>
        public override string ToString() {
            //return "EQUIV" + EquivalentNodes.ToString();
            return string.Format("EQUIV[{0}]", string.Join<Node>(" ; ", EquivalentNodes));
        }

        /// <summary>
        /// Returns a string representation of the node without any SWUM markup.
        /// </summary>
        public override string ToPlainString() {
            StringBuilder sb = new StringBuilder();
            if(EquivalentNodes != null) {
                for(int i = 0; i < EquivalentNodes.Count - 1; i++) {
                    sb.AppendFormat("{0}|", EquivalentNodes[i].ToPlainString());
                }
                sb.Append(EquivalentNodes.Last().ToPlainString());
            }
            return sb.ToString();
        }

        /// <summary>
        /// Returns a PhraseNode containing the parsed name of the first node in EquivalentNodes. If this does not exist, the method returns an empty PhraseNode.
        /// </summary>
        public override PhraseNode GetParse() {
            if(EquivalentNodes != null && EquivalentNodes.Count > 0) {
                return EquivalentNodes[0].GetParse();
            } else {
                return new PhraseNode();
            }
        }
    }
}
