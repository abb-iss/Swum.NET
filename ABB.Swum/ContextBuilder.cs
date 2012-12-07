/******************************************************************************
 * Copyright (c) 2012 ABB Group
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the Eclipse Public License v1.0
 * which accompanies this distribution, and is available at
 * http://www.eclipse.org/legal/epl-v10.html
 *
 * Contributors:
 *    Patrick Francis (ABB Group) - initial implementation and documentation
 *****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Diagnostics;
using ABB.SrcML;
using ABB.SrcML.Data;

namespace ABB.Swum
{
    /// <summary>
    /// Contains various helper functions to build context objects from SrcML.
    /// </summary>
    public static class ContextBuilder
    {
        private static string[] primitiveTypes = { "_Bool",
                                                     "bool",
                                                     "char",
                                                     "int",
                                                     "signed",
                                                     "unsigned",
                                                     "short",
                                                     "long",
                                                     "float",
                                                     "double",
                                                     "_Complex",
                                                     "complex",
                                                     "wchar_t",
                                                     "char16_t",
                                                     "char32_t"};


        /// <summary>
        /// Builds a FieldContext object based on the given field element.
        /// </summary>
        /// <param name="declElement">An XElement representing the decl element of the field.</param>
        /// <returns>A FieldContext object based on the given field.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.ArgumentException">Thrown if the passed XElement does not represent a decl element.</exception>
        public static FieldContext BuildFieldContext(XElement declElement)
        {
            if (declElement == null)
            {
                throw new ArgumentNullException("declElement");
            }
            if (declElement.Name != SRC.Declaration)
            {
                throw new ArgumentException(string.Format("The passed XElement must represent a <decl> element. Received a <{0}> element.", declElement.Name.ToString()), "declElement");
            }

            FieldContext fc = new FieldContext();
            //Type of the field
            var typeElement = declElement.Element(SRC.Type);
            if (typeElement != null)
            {
                bool isPrimitive;
                fc.IdType = ConstructTypeName(typeElement, out isPrimitive);
                fc.IdTypeIsPrimitive = isPrimitive;
            }

            //Determine declaring class
            XElement classElement = FindEnclosingClassElement(declElement);
            if (classElement != null && classElement.Element(SRC.Name) != null)
            {
                fc.DeclaringClass = GetNameFromNameElement(classElement.Element(SRC.Name));
            }

            return fc;
        }

        /// <summary>
        /// Builds a MethodContext object based on the given method element.
        /// </summary>
        /// <param name="methodTag">An XElement representing the method to build the context for. This can be either a function, constructor or destructor element.</param>
        /// <returns>A MethodContext object based on the given method.</returns>
        /// <exception cref="System.ArgumentException">The passed XElement does not represent a function, constructor, or destructor element.</exception>
        /// <exception cref="System.ArgumentNullException">methodTag is null.</exception>
        public static MethodContext BuildMethodContext(XElement methodTag)
        {
            if (methodTag == null)
            {
                throw new ArgumentNullException("methodTag");
            }
            else if (!(methodTag.Name == SRC.Function || methodTag.Name == SRC.Constructor || methodTag.Name == SRC.Destructor))
            {
                throw new ArgumentException(string.Format("The passed XElement must represent a <function>, <constructor> or <destructor> element. Received a <{0}> element.", methodTag.Name.ToString()), "methodTag");
            }

            MethodContext mc = new MethodContext();

            //set return type
            if (methodTag.Name == SRC.Function)
            {
                bool isPrimitive;
                mc.IdType = ConstructTypeName(methodTag.Element(SRC.Type), out isPrimitive);
                mc.IdTypeIsPrimitive = isPrimitive;
            }

            //record if constructor
            if (methodTag.Name == SRC.Constructor) { mc.IsConstructor = true; }
            //record if destructor
            if (methodTag.Name == SRC.Destructor) { mc.IsDestructor = true; }

            //record if static
            //look for the static keyword (at the function definition)
            //This is not entirely sufficient because it's possible for the static keyword to be present at only the function declaration and not the definition
            //Also, some other word may be #defined to static and used instead
            var typeElement = methodTag.Element(SRC.Type);
            if (typeElement != null)
            {
                foreach (var typeNameTag in typeElement.Elements(SRC.Name))
                {
                    if (typeNameTag.Value.Equals("static", StringComparison.InvariantCultureIgnoreCase))
                    {
                        mc.IsStatic = true;
                        break;
                    }
                }
            }

            //formal parameters
            mc.FormalParameters = new List<FormalParameterRecord>();
            XElement paramList = methodTag.Element(SRC.ParameterList);
            if (paramList != null)
            {
                foreach (var param in paramList.Elements(SRC.Parameter))
                {
                    //a param (usually) looks like: <param><decl><type><name>...</name></type> <name>...</name></decl></param>
                    string type = string.Empty;
                    string name = string.Empty;
                    bool isPrimitive = false;
                    var declElement = param.Element(SRC.Declaration);
                    if (declElement != null && declElement.Element(SRC.Type) != null)
                    {
                        type = ConstructTypeName(declElement.Element(SRC.Type), out isPrimitive);
                    }
                    if (declElement != null && declElement.Element(SRC.Name) != null)
                    {
                        name = param.Element(SRC.Declaration).Element(SRC.Name).Value;
                    }
                    //add parameter, if it's valid
                    if(!(string.IsNullOrEmpty(type) && string.IsNullOrEmpty(name)) && type != "void")
                    {
                        mc.FormalParameters.Add(new FormalParameterRecord(type, isPrimitive, name));
                    }
                }
            }

            //Determine declaring class
            XElement classNameTag = SrcMLHelper.GetClassNameForMethod(methodTag);
            if (classNameTag != null)
            {
                //class name listed with method name: <ClassName>::<MethodName>
                mc.DeclaringClass = classNameTag.Value;
            }
            else if (classNameTag == null && mc.IsConstructor)
            {
                //class name not listed, but the method is a constructor
                //I'm not sure if this is actually possible
                mc.DeclaringClass = SrcMLHelper.GetNameForMethod(methodTag).Value;
            }
            else if (classNameTag == null)
            {
                //no class name listed, but this might be an inline method in the class declaration
                //search for the enclosing <class> or <struct> tag
                XElement classElement = FindEnclosingClassElement(methodTag);
                if (classElement != null && classElement.Element(SRC.Name) != null)
                {
                    mc.DeclaringClass = GetNameFromNameElement(classElement.Element(SRC.Name));
                }
            }

            return mc;
        }

        /// <summary>
        /// Creates a MethodContext object based on the given srcML call element.
        /// </summary>
        /// <param name="callElement">The srcML Call element to create the context from.</param>
        /// <param name="srcDb">A SrcMLDataContext database built on the srcML file that <paramref name="callElement"/> is part of.</param>
        /// <returns>A MethodContext object based on <paramref name="callElement"/>.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="callElement"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="callElement"/> is not a Call element.</exception>
        /// <exception cref="System.ArgumentNullException"><paramref name="srcDb"/> is null.</exception>
        public static MethodContext BuildMethodContextFromCall(XElement callElement, SrcMLDataContext srcDb) {
            if(callElement == null) { throw new ArgumentNullException("callElement"); }
            if(callElement.Name != SRC.Call) { throw new ArgumentException("Passed element not a Call element.", "callElement"); }
            if(srcDb == null) { throw new ArgumentNullException("srcDb"); }
            
            //Process parameters
            List<FormalParameterRecord> args = new List<FormalParameterRecord>();
            foreach(XElement argument in callElement.Element(SRC.ArgumentList).Elements(SRC.Argument)) {
                string argType = "int"; //default assumption for unknown types
                bool primitiveArg = true;
                var expr = argument.Element(SRC.Expression);
                if(expr != null) {
                    var exprElements = expr.Elements().ToList();
                    if(exprElements.Count() == 1 && exprElements[0].Name == SRC.Name) {
                        //variable
                        var varDecl = srcDb.GetDeclarationForVariableName(exprElements[0]);
                        if(varDecl != null) {
                            argType = varDecl.VariableTypeName;
                            primitiveArg = primitiveTypes.Contains(argType);
                        }
                    } else if(exprElements.Count() == 1 && exprElements[0].Name == LIT.Literal) {
                        var typeAttr = exprElements[0].Attribute("type");
                        if(typeAttr != null && typeAttr.Value == "string") {
                            //string literal
                            argType = "char*";
                            primitiveArg = true;
                        } else if(typeAttr != null && typeAttr.Value == "number") {
                            //number literal, assume it's an integer
                            argType = "int";
                            primitiveArg = true;
                        }
                    } else if(exprElements.Count() == 1 && exprElements[0].Name == SRC.Call) {
                        //method call
                        var md = srcDb.GetDefinitionForMethodCall(exprElements[0]);
                        if(md != null) {
                            argType = md.MethodReturnTypeName;
                            primitiveArg = primitiveTypes.Contains(argType);
                        } else {
                            //undefined method, assume to return int
                            argType = "int";
                            primitiveArg = true;
                        }
                    } else {
                        //found an actual expression
                        //don't bother trying to evaluate it, just ignore
                        argType = "int";
                        primitiveArg = true;
                    }
                } else {
                    Console.WriteLine("Found <argument> without child <expr>: {0}", callElement.ToString());
                    //add empty arg to list
                    argType = "int";
                    primitiveArg = true;
                }
                args.Add(new FormalParameterRecord(argType, primitiveArg, string.Empty));
            }

            //Get declaring class name, if present
            //If this is present, it will only be a class if calling a static method. Otherwise it's probably a namespace.
            XElement classNameElement = SrcMLHelper.GetClassNameForMethod(callElement);
            string className = classNameElement != null ? classNameElement.Value : string.Empty;
            //TODO: determine other aspects of MethodContext from call site

            return new MethodContext("int", true, className, args, false, false, false);
        }
        /// <summary>
        /// Extracts the type name from a type srcML element. A type element may contain several name and type modifier elements.
        /// This function returns only the actual type name, ignoring any access modifiers, such as static, private or public.
        /// Type modifiers such as '*' or '&amp;' are treated as part of the type name and concatenated with the name text.
        /// </summary>
        /// <param name="typeElement">A type srcML element.</param>
        /// <returns>The type name.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.ArgumentException">typeElement does not represent a type element.</exception>
        public static string ConstructTypeName(XElement typeElement)
        {
            bool dummy;
            return ConstructTypeName(typeElement, out dummy);
        }
        
        /// <summary>
        /// Extracts the type name from a type srcML element. A type element may contain several name and type modifier elements.
        /// This function returns only the actual type name, ignoring any access modifiers, such as static, private or public.
        /// Type modifiers such as '*' or '&amp;' are treated as part of the type name and concatenated with the name text.
        /// </summary>
        /// <param name="typeElement">A type srcML element.</param>
        /// <param name="isPrimitive">An output parameter indicating whether the type is a primitive data type.</param>
        /// <returns>The type name.</returns>
        /// <exception cref="System.ArgumentNullException">typeElement is null.</exception>
        /// <exception cref="System.ArgumentException">typeElement does not represent a type element.</exception>
        public static string ConstructTypeName(XElement typeElement, out bool isPrimitive)
        {
            if (typeElement == null)
            {
                throw new ArgumentNullException("typeElement");
            }
            else if (typeElement.Name != SRC.Type)
            {
                throw new ArgumentException(string.Format("The passed XElement must represent a <type> element. Received a <{0}> element.", typeElement.Name.ToString()), "typeElement");
            }

            //Find the name of the type
            StringBuilder typeName = new StringBuilder();
            isPrimitive = false;
            XElement typeNameElement;
            if (typeElement.Elements(SRC.Name).Count() > 0)
            {
                //the Type element may actually contain several Name elements because access modifiers (e.g. static, private, etc.) get tagged as Names
                //This line assumes that the last Name element is the actual name
                typeNameElement = typeElement.Elements(SRC.Name).Last();

                if (primitiveTypes.Contains(typeNameElement.Value))
                {
                    //last name element is a primitive type, look for more primitive names preceding it
                    //e.g. "unsigned short int"
                    isPrimitive = true;
                    typeName.Append(typeNameElement.Value);
                    foreach (var currElem in typeNameElement.ElementsBeforeSelf().Reverse())
                    {
                        if (currElem.Name == SRC.Name && primitiveTypes.Contains(currElem.Value))
                        {
                            typeName.Insert(0, currElem.Value + " ");
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    //A name element might contain several child name elements, in the case where the name is qualified
                    //E.g. <Namespace>::<typeName>
                    //This grabs the last child name element, assuming that it is the actual type name
                    var childNames = typeNameElement.Elements(SRC.Name);
                    if (childNames != null && childNames.Count() > 0)
                    {
                        typeNameElement = childNames.Last();
                    }

                    typeName.Append(typeNameElement.Value);
                }
            }
            else
            {
                //It's possible to have zero name elements, in the case of "..." for variable length argument lists.
                typeNameElement = null;
            }

            //append any type modifiers that are present, e.g. '*' or '&'
            IEnumerable<XElement> typeModifierCollection;
            if (typeNameElement != null)
            {
                typeModifierCollection = typeNameElement.ElementsAfterSelf(TYPE.Modifier);
            }
            else
            {
                //no type name, probably because type is "...", so append all modifiers
                typeModifierCollection = typeElement.Elements(TYPE.Modifier);
            }
            foreach (var typeModifier in typeModifierCollection)
            {
                typeName.Append(typeModifier.Value);
            }

            return typeName.ToString();
        }

        /// <summary>
        /// Finds the nearest class or struct element that encloses the given element.
        /// </summary>
        /// <param name="startElement">The element from which to start looking for the enclosing class or struct.</param>
        /// <returns>The nearest class or struct XElement that encloses the given element. 
        /// If there is no enclosing class or struct element, this function returns null.</returns>
        private static XElement FindEnclosingClassElement(XElement startElement)
        {
            if (startElement == null) { return null; }

            var ancestors = from a in startElement.Ancestors()
                            where a.Name == SRC.Class || a.Name == SRC.Struct
                            select a;
            if (ancestors.Count() > 0)
            {
                return ancestors.First();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Given a name element, this function extracts the identifier name from it.
        /// A name element may potentially contain several child name elements. For example, when a name has the form [ClassName]::[Identifier].
        /// This function assumes that the last child name element is the desired name, and returns that.
        /// If there are no child name elements, the text of the name element is returned.
        /// </summary>
        /// <param name="nameElement">The name element from which to extract the identifier name.</param>
        /// <returns>The text of the identifier name contained within the given name element.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.ArgumentException">Thrown if the passed XElement does not represent a name element.</exception>
        private static string GetNameFromNameElement(XElement nameElement)
        {
            if (nameElement == null)
            {
                throw new ArgumentNullException("nameElement");
            }
            else if (nameElement.Name != SRC.Name)
            {
                throw new ArgumentException(string.Format("The passed XElement must represent a <name> element. Received a <{0}> element.", nameElement.Name.ToString()), "nameElement");
            }

            string nameText;
            var childNameElements = nameElement.Elements(SRC.Name);
            if (childNameElements != null && childNameElements.Count() > 0)
            {
                nameText = childNameElements.Last().Value;
            }
            else
            {
                nameText = nameElement.Value;
            }

            return nameText;
        }
    }
}
