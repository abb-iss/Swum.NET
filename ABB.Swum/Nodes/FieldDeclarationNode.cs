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
    /// Represents a field declaration in the program.
    /// </summary>
    public class FieldDeclarationNode : ProgramElementNode {
        private FieldContext Context;
        /// <summary>
        /// The type of the declared field.
        /// </summary>
        public TypeNode Type { get; set; }
        /// <summary>
        /// The class that this field is part of.
        /// </summary>
        public TypeNode DeclaringClass { get; set; }

        /// <summary>
        /// Creates a new FieldDeclarationNode with the given name, and no context.
        /// </summary>
        /// <param name="name">The name of the declared field.</param>
        public FieldDeclarationNode(string name) : this(name, null) { }

        /// <summary>
        /// Creates a new FieldDeclarationNode.
        /// </summary>
        /// <param name="name">The name of the declared field.</param>
        /// <param name="context">The surrounding context of the field.</param>
        public FieldDeclarationNode(string name, FieldContext context)
            : base(name) {
            this.Context = context;
        }

        /// <summary>
        /// Creates a string representation, including the field's type and name.
        /// </summary>
        /// <returns>A string representation of this FieldDeclarationNode.</returns>
        public override string ToString() {
            return string.Format("[ {0} - {1} ]", Type.ToString(), base.ToString());
        }

        /// <summary>
        /// Assigns the attributes of this field related to its structure within the program.
        /// </summary>
        /// <param name="splitter">An IdSplitter to split the words of identifiers.</param>
        /// <param name="tagger">A part-of-speech tagger</param>
        public void AssignStructuralInformation(IdSplitter splitter, Tagger tagger) {
            this.Type = new TypeNode(Context.IdType, Context.IdTypeIsPrimitive, splitter, tagger);
            this.DeclaringClass = new TypeNode(Context.DeclaringClass, false, splitter, tagger);
        }
    }
}
