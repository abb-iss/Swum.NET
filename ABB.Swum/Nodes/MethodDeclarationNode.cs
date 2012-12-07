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
    /// Represents a method declaration in the program.
    /// </summary>
    public class MethodDeclarationNode : ProgramElementNode {
        /// <summary>
        /// Contains various information relevent to the context of the method declaration.
        /// </summary>
        public MethodContext Context { get; set; }
        /// <summary>
        /// This method's role in the program. For example, is it a getter or setter or event handler or action, etc.
        /// </summary>
        public MethodRole Role { get; set; }

        //Structural information
        /// <summary>
        /// The formal parameters in the method declaration.
        /// </summary>
        public List<VariableDeclarationNode> FormalParameters { get; private set; }
        /// <summary>
        /// The class that this method is part of.
        /// </summary>
        public TypeNode DeclaringClass { get; set; }

        //Emily had this. I've removed it because it didn't seem to be used for anything.
        //public List<Node> Returns { get; set; }
        /// <summary>
        /// The return type of the method.
        /// </summary>
        public TypeNode ReturnType { get; set; }

        //Linguistic information
        /// <summary>
        /// Represents the action that this method is performing.
        /// </summary>
        public Node Action { get; set; }
        /// <summary>
        /// Represents the object that this method is acting upon.
        /// </summary>
        public Node Theme { get; set; }
        /// <summary>
        /// Arguments that have been identified as Indirect Objects for the action in the method name.
        /// </summary>
        public List<ArgumentNode> SecondaryArguments { get; private set; }
        /// <summary>
        /// Any remaining arguments that have unknown roles.
        /// </summary>
        //public List<ArgumentNode> UnknownArguments { get; set; }
        // This is a list of VariableDeclarationNodes rather than ArgumentNodes because they're copied directly from the FormalParameters.
        public List<Node> UnknownArguments { get; private set; }

        //Emily had these but they were never used
        //Additional linguistic info for checkers
        //public Node Agent { get; set; }
        //public Node Condition { get; set; }

        //Emily had this. I've removed it in favor of Theme.GetLocation().
        //private Location ThemeLocation = Location.None;

        //Not yet implemented, left here as a reminder
        //public double Confidence { get; set; }
        /// <summary>
        /// Indicates whether this method is designed to react to some event, e.g. an event handler, callback, etc.
        /// </summary>
        public bool IsReactive { get; set; }
        /// <summary>
        /// Indicates whether this is a declaration for a constructor.
        /// </summary>
        public bool IsConstructor { get; set; }
        /// <summary>
        /// Indicates whether this is a declaration for a destructor.
        /// </summary>
        public bool IsDestructor { get; set; }

        /// <summary>
        /// Creates a new MethodDeclarationNode with the given method name.
        /// </summary>
        /// <param name="name">The name of the method.</param>
        public MethodDeclarationNode(string name) : this(name, null) { }

        /// <summary>
        /// Creates a new MethodDeclarationNode with the given method name and context.
        /// </summary>
        /// <param name="name">The name of the method.</param>
        /// <param name="context">The method's context.</param>
        public MethodDeclarationNode(string name, MethodContext context)
            : base(name) {
            this.Context = context;
        }


        //TODO: should this be a separate function, or integrated into the constructor? This occurs in other nodes as well
        /// <summary>
        /// Initializes the fields containing structural information about the method. This includes the location, return type and parameters.
        /// </summary>
        /// <param name="splitter">An IdSplitter to split the method name into words.</param>
        /// <param name="tagger">A Tagger to tag the parts-of-speech of the name.</param>
        /// <exception cref="System.ArgumentNullException">splitter is null.</exception>
        /// <exception cref="System.ArgumentNullException">tagger is null.</exception>
        /// <exception cref="System.InvalidOperationException">Context property is not set.</exception>
        public void AssignStructuralInformation(IdSplitter splitter, Tagger tagger) {
            if(splitter == null) { throw new ArgumentNullException("splitter"); }
            if(tagger == null) { throw new ArgumentNullException("tagger"); }
            if(this.Context == null) { throw new InvalidOperationException("Context property must be set prior to calling AssignStructuralInformation"); }

            this.ParsedName.SetLocation(Location.Name);
            if(!string.IsNullOrEmpty(Context.IdType)) {
                this.ReturnType = new TypeNode(Context.IdType, Context.IdTypeIsPrimitive, splitter, tagger, Location.Return);
            }
            if(!string.IsNullOrEmpty(Context.DeclaringClass)) {
                this.DeclaringClass = new TypeNode(Context.DeclaringClass, false, splitter, tagger, Location.OnClass);
            }

            this.FormalParameters = new List<VariableDeclarationNode>();
            if(Context.FormalParameters.Count > 0) {
                int pos = 0;
                foreach(FormalParameterRecord paramRecord in Context.FormalParameters) {
                    this.FormalParameters.Add(new VariableDeclarationNode(paramRecord.Name, paramRecord.ParameterType, paramRecord.IsPrimitiveType, splitter, tagger, Location.Formal, pos));
                    pos++;
                }
            }
        }

        /// <summary>
        /// Creates a string representation of the node, which contains the action, theme and arguments.
        /// </summary>
        /// <returns>A string representation of the node.</returns>
        public override string ToString() {
            //Action
            string a = (Action != null) ? Action.ToString() : "";

            //Theme
            string t = (Theme != null) ? Theme.ToString() : "";

            //Arguments
            StringBuilder s = new StringBuilder();
            if(SecondaryArguments != null) {
                foreach(ArgumentNode arg in SecondaryArguments) {
                    s.AppendFormat(" -- {0}", arg.ToString());
                }
            }
            if(UnknownArguments != null && UnknownArguments.Count > 0) {
                s.Append(Environment.NewLine + "\t");
                foreach(Node arg in UnknownArguments) {
                    s.AppendFormat(" ++ {0}", arg);
                }
            }

            return string.Format("{0} | {1}{2}", a, t, s);
        }

        /// <summary>
        /// Adds the given argument to the UnknownArguments list.
        /// </summary>
        /// <param name="argument">A Node corresponding to the argument to add.</param>
        public void AddUnknownArgument(Node argument) {
            if(this.UnknownArguments == null) {
                this.UnknownArguments = new List<Node>();
            }
            this.UnknownArguments.Add(argument);
        }

        /// <summary>
        /// Adds the given arguments to the UnknownArguments list.
        /// </summary>
        /// <param name="arguments">A collection of the arguments to add.</param>
        public void AddUnknownArguments(IEnumerable<Node> arguments) {
            if(this.UnknownArguments == null) {
                this.UnknownArguments = new List<Node>();
            }
            this.UnknownArguments.AddRange(arguments);
        }

        /// <summary>
        /// Clears the list of UnknownArguments.
        /// </summary>
        public void ClearUnknownArguments() {
            if(this.UnknownArguments == null) {
                this.UnknownArguments = new List<Node>();
            }
            this.UnknownArguments.Clear();
        }

        /// <summary>
        /// Creates a new ArgumentNode from the given parameters, and adds it to the SecondaryArguments list.
        /// </summary>
        /// <param name="argument">The node that is serving as the argument.</param>
        /// <param name="preposition">The preposition describing the argument's relation to the method.</param>
        public void AddSecondaryArgument(Node argument, WordNode preposition) {
            if(this.SecondaryArguments == null) {
                this.SecondaryArguments = new List<ArgumentNode>();
            }
            ArgumentNode an = new ArgumentNode(argument, preposition);
            this.SecondaryArguments.Add(an);
        }

        /// <summary>
        /// Clears the list of SecondaryArguments.
        /// </summary>
        public void ClearSecondaryArguments() {
            if(this.SecondaryArguments == null) {
                this.SecondaryArguments = new List<ArgumentNode>();
            }
            this.SecondaryArguments.Clear();
        }

        /// <summary>
        /// Creates a new EquivalenceNode containing the passed node and the specified nodes from the UnknownArguments list.
        /// The arguments are removed from the UnknownArguments list after being added to the EquivalenceNode.
        /// </summary>
        /// <param name="equivalentNode">The first node to add to the EquivalenceNode, i.e. the node that the unknown arguments are equivalent with.</param>
        /// <param name="equivalentUnknownArgs">An array of booleans indicating which unknown arguments to include in the equivalence. 
        /// This array must be the same length as the UnknownArguments list.
        /// For each element in the array, a value of True means to include the corresponding unknown argument, False means to not include it.</param>
        /// <returns>An EquivalenceNode containing the passed node and the specified nodes from the UnknownArguments list.</returns>
        /// <exception cref="System.ArgumentNullException">equivalentNode is null.</exception>
        /// <exception cref="System.ArgumentException">The length of equivalentUnknownArgs is not the same as the UnknownArguments property of the MethodDeclarationNode.</exception>
        public virtual EquivalenceNode CreateEquivalenceFromUnknownArguments(Node equivalentNode, bool[] equivalentUnknownArgs) {
            if(equivalentNode == null) { throw new ArgumentNullException("equivalentNode"); }
            if(equivalentUnknownArgs.Length != UnknownArguments.Count) {
                throw new ArgumentException("Length of equivalentUnknownArgs array not equal to length of UnknownArguments", "equivalentUnknownArgs");
            }

            //grab the specified UnknownArguments and put in a list
            List<Node> equivArgs = new List<Node>();
            for(int argIndex = 0; argIndex < equivalentUnknownArgs.Length; argIndex++) {
                if(equivalentUnknownArgs[argIndex]) {
                    equivArgs.Add(this.UnknownArguments[argIndex]);
                }
            }
            return CreateEquivalenceFromUnknownArguments(equivalentNode, equivArgs);
        }

        /// <summary>
        /// Creates a new EquivalenceNode containing the passed node and the specified nodes from the UnknownArguments list.
        /// The arguments are removed from the UnknownArguments list after being added to the EquivalenceNode.
        /// </summary>
        /// <param name="equivalentNode">The first node to add to the EquivalenceNode, i.e. the node that the unknown arguments are equivalent with.</param>
        /// <param name="equivalentUnknownArgs">The UnknownArguments to include in the equivalence.</param>
        /// <returns>An EquivalenceNode containing the passed node and the specified nodes from the UnknownArguments list.</returns>
        /// <exception cref="System.ArgumentNullException">equivalentNode is null.</exception>
        /// <exception cref="System.ArgumentNullException">equivalentUnknownArgs is null.</exception>
        public virtual EquivalenceNode CreateEquivalenceFromUnknownArguments(Node equivalentNode, IEnumerable<Node> equivalentUnknownArgs) {
            if(equivalentNode == null) { throw new ArgumentNullException("equivalentNode"); }
            if(equivalentUnknownArgs == null) { throw new ArgumentNullException("equivalentUnknownArgs"); }

            EquivalenceNode equivNode = new EquivalenceNode(equivalentNode);
            //add specified UnknownArguments to the equivalence
            foreach(var equivArg in equivalentUnknownArgs) {
                if(!this.UnknownArguments.Contains(equivArg)) {
                    throw new InvalidOperationException(string.Format("Node is not present in the UnknownArguments collection: {0}", equivArg));
                }
                equivNode.AddEquivalentNode(equivArg);
            }
            //remove equivalent arguments from the UnknownArguments list
            this.UnknownArguments.RemoveAll(n => equivalentUnknownArgs.Contains(n));

            return equivNode;
        }

        /// <summary>
        /// Concatenates the given phrases and assigns the result to Theme.
        /// ** Note that this will potentially modify phrase1. **
        /// </summary>
        /// <param name="phrase1">The first phrase.</param>
        /// <param name="phrase2">The second phrase.</param>
        public void CreateThemeFromPhrases(PhraseNode phrase1, PhraseNode phrase2) {
            if(phrase1 != null) {
                phrase1.Add(phrase2);
                this.Theme = phrase1;
            } else {
                this.Theme = phrase2;
            }
        }
    }
}
