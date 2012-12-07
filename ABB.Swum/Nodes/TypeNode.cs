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
    /// Represents a type within a program.
    /// </summary>
    public class TypeNode : ProgramElementNode {
        /// <summary>
        /// Indicates whether this TypeNode represents a primitive type, e.g. int, bool, char, etc.
        /// </summary>
        public bool IsPrimitive { get; set; }

        /// <summary>
        /// Creates a new TypeNode with default values.
        /// </summary>
        public TypeNode() : base() { }
        /// <summary>
        /// Creates a new TypeNode.
        /// </summary>
        /// <param name="name">The name of the type.</param>
        /// <param name="isPrimitive">Whether the type is a primitive data type.</param>
        public TypeNode(string name, bool isPrimitive)
            : base(name) {
            this.IsPrimitive = isPrimitive;
        }
        /// <summary>
        /// Creates a new TypeNode.
        /// </summary>
        /// <param name="name">The name of the type.</param>
        /// <param name="isPrimitive">Whether the type is a primitive data type.</param>
        /// <param name="splitter">An IdSplitter to split the type name into words.</param>
        public TypeNode(string name, bool isPrimitive, IdSplitter splitter)
            : base(name, splitter) {
            this.IsPrimitive = isPrimitive;
        }
        /// <summary>
        /// Creates a new TypeNode.
        /// </summary>
        /// <param name="name">The name of the type.</param>
        /// <param name="isPrimitive">Whether the type is a primitive data type.</param>
        /// <param name="parsedName">A PhraseNode representing the parsed type name.</param>
        public TypeNode(string name, bool isPrimitive, PhraseNode parsedName)
            : base(name, parsedName) {
            this.IsPrimitive = isPrimitive;
        }

        /// <summary>
        /// Creates a new TypeNode.
        /// </summary>
        /// <param name="name">The name of the type.</param>
        /// <param name="isPrimitive">Whether the type is a primitive data type.</param>
        /// <param name="splitter">An IdSplitter to split the type name into words.</param>
        /// <param name="tagger">A Tagger to tag the parts-of-speech of each word of the name.</param>
        public TypeNode(string name, bool isPrimitive, IdSplitter splitter, Tagger tagger)
            : this(name, isPrimitive, splitter, tagger, Location.None) { }

        /// <summary>
        /// Creates a new TypeNode.
        /// </summary>
        /// <param name="name">The name of the type.</param>
        /// <param name="isPrimitive">Whether the type is a primitive data type.</param>
        /// <param name="splitter">An IdSplitter to split the type name into words.</param>
        /// <param name="tagger">A Tagger to tag the parts-of-speech of each word of the name.</param>
        /// <param name="location">The location of the type.</param>
        public TypeNode(string name, bool isPrimitive, IdSplitter splitter, Tagger tagger, Location location)
            : this(name, isPrimitive, splitter) {
            if(tagger != null) {
                tagger.TagType(this.ParsedName);
            }
            this.SetLocation(location);
        }

    }
}
