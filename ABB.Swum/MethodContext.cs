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

namespace ABB.Swum
{
    /// <summary>
    /// Encapsulates the program context relevant to a method declaration.
    /// </summary>
    public class MethodContext : IdContext
    {
        /// <summary>
        /// Indicates whether this method is a constructor.
        /// </summary>
        public bool IsConstructor { get; set; }
        /// <summary>
        /// Indicates whether this method is a destructor.
        /// </summary>
        public bool IsDestructor { get; set; }
        /// <summary>
        /// Indicates whether this method is static, i.e. the signature includes the static keyword.
        /// </summary>
        public bool IsStatic { get; set; }
        /// <summary>
        /// The text of the formal method parameters included in the method declaration. 
        /// Each parameter is a separate entry in the List, and each entry includes both the parameter type and parameter name.
        /// </summary>
        public IList<FormalParameterRecord> FormalParameters { get; set; }

        /// <summary>
        /// Creates a new MethodContext with default values.
        /// </summary>
        public MethodContext() : this("") { }
        /// <summary>
        /// Creates a new MethodContext, with the given return type.
        /// </summary>
        /// <param name="idType">The return type of the method.</param>
        public MethodContext(string idType) : this(idType, false) { }
        /// <summary>
        /// Creates a new MethodContext.
        /// </summary>
        /// <param name="idType">The return type of the method.</param>
        /// <param name="idTypeIsPrimitive">Whether the return type is a primitive data type.</param>
        public MethodContext(string idType, bool idTypeIsPrimitive) : this(idType, idTypeIsPrimitive, string.Empty, null, false, false, false) { }

        /// <summary>
        /// Creates a new MethodContext.
        /// </summary>
        /// <param name="idType">The return type of the method.</param>
        /// <param name="idTypeIsPrimitive">Whether the return type is a primitive data type.</param>
        /// <param name="declaringClass">The class that this method is a part of, String.Empty if not part of a class.</param>
        /// <param name="formalParameters">A list of the formal parameters, including both type and name.</param>
        /// <param name="isStatic">Whether this method is static or not.</param>
        /// <param name="isConstructor">Whether this method is a constructor or not.</param>
        /// <param name="isDestructor">Whether this method is a destructor or not.</param>
        public MethodContext(string idType, bool idTypeIsPrimitive, string declaringClass, IEnumerable<FormalParameterRecord> formalParameters, bool isStatic, bool isConstructor, bool isDestructor)
            : base(idType, idTypeIsPrimitive, declaringClass)
        {
            if (formalParameters != null)
            {
                this.FormalParameters = new List<FormalParameterRecord>(formalParameters);
            }
            else
            {
                this.FormalParameters = new List<FormalParameterRecord>();
            }
            this.IsStatic = isStatic;
            this.IsConstructor = isConstructor;
            this.IsDestructor = isDestructor;
        }
    }
}
