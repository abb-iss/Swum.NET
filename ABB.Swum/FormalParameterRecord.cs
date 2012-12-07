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
    /// Encapsulates the data about a formal parameter to a function.
    /// </summary>
    public class FormalParameterRecord : IEquatable<FormalParameterRecord>
    {
        /// <summary>
        /// The type of the parameter, e.g. int, bool, etc.
        /// </summary>
        public string ParameterType { get; private set; }
        /// <summary>
        /// Whether the type of the parameter is a primitive data type.
        /// </summary>
        public bool IsPrimitiveType { get; private set; }
        /// <summary>
        /// The name of the parameter.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Creates a new FormalParameterRecord.
        /// </summary>
        /// <param name="parameterType">The type of the parameter, e.g. int, bool, etc.</param>
        /// <param name="isPrimitiveType">Whether the type of the parameter is a primitive data type.</param>
        /// <param name="name">The name of the parameter.</param>
        public FormalParameterRecord(string parameterType, bool isPrimitiveType, string name)
        {
            this.ParameterType = parameterType;
            this.IsPrimitiveType = isPrimitiveType;
            this.Name = name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (!(obj is FormalParameterRecord))
            {
                return false;
            }
            else
            {
                return Equals((FormalParameterRecord)obj);
            }
        }

        /// <summary>
        /// Returns the hash code for this object.
        /// </summary>
        public override int GetHashCode()
        {
            return ParameterType.GetHashCode() ^ IsPrimitiveType.GetHashCode() ^ Name.GetHashCode();
        }

        #region IEquatable<FormalParameterRecord> Members
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(FormalParameterRecord other)
        {
            return this.ParameterType == other.ParameterType
                && this.IsPrimitiveType == other.IsPrimitiveType
                && this.Name == other.Name;
        }

        #endregion
    }
}
