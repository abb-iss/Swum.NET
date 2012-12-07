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
using ABB.Swum;

namespace ABB.Swum.Nodes {
    /// <summary>
    /// Represents an abstract program element.
    /// </summary>
    public abstract class ProgramElementNode : Node {
        /// <summary>
        /// The name of the program element. For example, method name, variable name, etc.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public PhraseNode Preamble { get; set; }
        /// <summary>
        /// A PhraseNode representing the Name split into words and tagged with parts-of-speech.
        /// </summary>
        public PhraseNode ParsedName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double Score { get; set; }
        /// <summary>
        /// The SwumRule that was used to construct the SWUM on this node.
        /// </summary>
        public SwumRule SwumRuleUsed { get; set; }

        /// <summary>
        /// Creates a new ProgramElementNode in its default state.
        /// </summary>
        public ProgramElementNode() { }

        /// <summary>
        /// Creates a new ProgramElementNode and sets Name to the supplied value.
        /// </summary>
        /// <param name="name">The name of the program element.</param>
        public ProgramElementNode(string name) : this(name, (PhraseNode)null) { }

        /// <summary>
        /// Creates a new ProgramElementNode and sets Name and ParsedName to the supplied values.
        /// </summary>
        /// <param name="name">The name of the program element.</param>
        /// <param name="parsedName">A PhraseNode constructed from the program element's name.</param>
        public ProgramElementNode(string name, PhraseNode parsedName) {
            this.Name = name;
            this.ParsedName = parsedName;
        }

        /// <summary>
        /// Creates a new ProgramElementNode and sets Name to the supplied value. Name is then parsed using the supplied IdSplitter.
        /// </summary>
        /// <param name="name">The name of the program element.</param>
        /// <param name="splitter">An IdSplitter to use to parse the name.</param>
        public ProgramElementNode(string name, IdSplitter splitter)
            : this(name) {
            Parse(splitter);
        }

        /// <summary>
        /// Parses the node's Name into words.
        /// </summary>
        /// <param name="splitter"></param>
        public virtual void Parse(IdSplitter splitter) {
            if(ParsedName == null)
                ParsedName = new PhraseNode(Name, splitter);
        }

        /// <summary>
        /// Sets the program location of the node.
        /// </summary>
        /// <param name="location">The program location of the node.</param>
        public void SetLocation(Location location) {
            this.Location = location;
        }

        /// <summary>
        /// Returns a string representation of the node. This is simply the string representation of the ParsedName.
        /// </summary>
        /// <returns>A string representation of the node.</returns>
        public override string ToString() {
            return ParsedName.ToString();
        }

        /// <summary>
        /// Returns a string representation of the node without any SWUM markup.
        /// </summary>
        /// <returns></returns>
        public override string ToPlainString() {
            if(ParsedName != null) {
                return ParsedName.ToPlainString();
            } else {
                return Name;
            }
        }

        /// <summary>
        /// Returns a PhraseNode containing the node's parsed name.
        /// </summary>
        public override PhraseNode GetParse() {
            if(ParsedName != null) {
                return ParsedName;
            } else {
                return new PhraseNode();
            }
        }
    }
}
