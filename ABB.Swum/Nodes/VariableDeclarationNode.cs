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
    /// Represents a variable declaration in the program.
    /// </summary>
    public class VariableDeclarationNode : ProgramElementNode {
        /// <summary>
        /// The type of the variable being declared.
        /// </summary>
        public TypeNode Type { get; set; }
        /// <summary>
        /// The position of the variable within the declaration list. E.g. in "int x, y, z" variables x, y and z have positions 0, 1 and 2, respectively.
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Creates a new VariableDeclarationNode.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        public VariableDeclarationNode(string name) : base(name) { }
        /// <summary>
        /// Creates a new VariableDeclarationNode.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="splitter">An IdSplitter to split the name into words.</param>
        public VariableDeclarationNode(string name, IdSplitter splitter) : base(name, splitter) { }

        /// <summary>
        /// Creates a new VariableDeclarationNode.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="type">A string giving the type of the variable.</param>
        /// <param name="isPrimitive">Whether the type is a primitive data type.</param>
        /// <param name="splitter">An IdSplitter to split the name into words.</param>
        /// <param name="tagger">A part-of-speech tagger to tag the words in the name.</param>
        public VariableDeclarationNode(string name, string type, bool isPrimitive, IdSplitter splitter, Tagger tagger)
            : this(name, type, isPrimitive, splitter, tagger, Location.None) { }

        /// <summary>
        /// Creates a new VariableDeclarationNode.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="type">A string giving the type of the variable.</param>
        /// <param name="isPrimitive">Whether the type is a primitive data type.</param>
        /// <param name="splitter">An IdSplitter to split the name into words.</param>
        /// <param name="tagger">A part-of-speech tagger to tag the words in the name.</param>
        /// <param name="location">The program location of the variable declaration.</param>
        public VariableDeclarationNode(string name, string type, bool isPrimitive, IdSplitter splitter, Tagger tagger, Location location)
            : this(name, type, isPrimitive, splitter, tagger, location, 0) { }

        /// <summary>
        /// Creates a new VariableDeclarationNode.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="type">A string giving the type of the variable.</param>
        /// <param name="isPrimitive">Whether the type is a primitive data type.</param>
        /// <param name="splitter">An IdSplitter to split the name into words.</param>
        /// <param name="tagger">A part-of-speech tagger to tag the words in the name.</param>
        /// <param name="location">The program location of the variable declaration.</param>
        /// <param name="position">The position of this variable in the declaration list.</param>
        public VariableDeclarationNode(string name, string type, bool isPrimitive, IdSplitter splitter, Tagger tagger, Location location, int position)
            : this(name, splitter) {
            this.InitType(type, isPrimitive, splitter, tagger);
            //tagger.TagVariableName(this.ParsedName);
            this.SetLocation(location);
            this.Position = position;
        }

        /// <summary>
        /// Creates a TypeNode from a string version of the type, and set it to the Type property.
        /// </summary>
        /// <param name="typeName">A string version of the variable's type.</param>
        /// <param name="isPrimitive">Whether the type is a primitive data type.</param>
        /// <param name="splitter">An IdSplitter to split the type into words.</param>
        /// <param name="tagger">A Tagger to tag the parts-of-speech of the type words.</param>
        protected virtual void InitType(string typeName, bool isPrimitive, IdSplitter splitter, Tagger tagger) {
            this.Type = new TypeNode(typeName, isPrimitive, splitter, tagger);
        }

        /// <summary>
        /// Creates a string representation, consisting of the type and the variable name.
        /// </summary>
        /// <returns>A string representation of the node.</returns>
        public override string ToString() {
            return string.Format("[{0} - {1}]", Type.ToString(), base.ToString());
        }
    }
}
