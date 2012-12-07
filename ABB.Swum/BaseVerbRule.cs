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
using ABB.Swum.Nodes;
using ABB.Swum.WordData;

namespace ABB.Swum
{
    /// <summary>
    /// The rule to construct SWUM for methods whose names follow the base verb pattern.
    /// Base Verb -> V (VP|VM)? (DT|NM)* N?
    /// </summary>
    public class BaseVerbRule : UnigramMethodRule
    {
        /// <summary>
        /// Creates a new BaseVerbRule using default values for data sets.
        /// </summary>
        public BaseVerbRule() : base() { }

        /// <summary>
        /// Creates a new BaseVerbRule.
        /// </summary>
        /// <param name="posData">The part-of-speech data to use.</param>
        /// <param name="tagger">The part-of-speech tagger to use.</param>
        /// <param name="splitter">The identifier splitter to use.</param>
        public BaseVerbRule(PartOfSpeechData posData, Tagger tagger, IdSplitter splitter) : base(posData, tagger, splitter) { }

        /// <summary>
        /// Creates a new BaseVerbRule.
        /// </summary>
        /// <param name="specialWords">A list of words that indicate the method name needs special handling.</param>
        /// <param name="booleanArgumentVerbs">A list of verbs that indicate that the boolean arguments to a method should be included in the UnknownArguments list.</param>
        /// <param name="nounPhraseIndicators">A list of word that indicate that beginning of a noun phrase.</param>
        /// <param name="positionalFrequencies">Positional frequency data.</param>
        public BaseVerbRule(HashSet<string> specialWords, HashSet<string> booleanArgumentVerbs, HashSet<string> nounPhraseIndicators, PositionalFrequencies positionalFrequencies)
            : base(specialWords, booleanArgumentVerbs, nounPhraseIndicators, positionalFrequencies) { }

        /// <summary>
        /// Creates a new BaseVerbRule.
        /// </summary>
        /// <param name="posData">The part-of-speech data to use.</param>
        /// <param name="tagger">The part-of-speech tagger to use.</param>
        /// <param name="splitter">The identifier splitter to use.</param>
        /// <param name="specialWords">A list of words that indicate the method name needs special handling.</param>
        /// <param name="booleanArgumentVerbs">A list of verbs that indicate that the boolean arguments to a method should be included in the UnknownArguments list.</param>
        /// <param name="nounPhraseIndicators">A list of word that indicate that beginning of a noun phrase.</param>
        /// <param name="positionalFrequencies">Positional frequency data.</param>
        public BaseVerbRule(PartOfSpeechData posData, Tagger tagger, IdSplitter splitter, HashSet<string> specialWords, HashSet<string> booleanArgumentVerbs, HashSet<string> nounPhraseIndicators, PositionalFrequencies positionalFrequencies)
            : base(posData, tagger, splitter, specialWords, booleanArgumentVerbs, nounPhraseIndicators, positionalFrequencies) { }

        /// <summary>
        /// Determines whether the given MethodDeclarationNode meets the conditions for this rule.
        /// </summary>
        /// <param name="node">The MethodDeclarationNode to test.</param>
        /// <returns>True if the node meets the conditions for this rule, False otherwise.</returns>
        protected override bool MakeClassification(MethodDeclarationNode node)
        {
            string firstWord = node.ParsedName[0].Text;
            if (IsChecker(firstWord)
                || IsSpecialCase(firstWord)
                || IsEventHandler(node.ParsedName)
                //|| IsEventHandler(node.FormalParameters) //pretty sure the parameters haven't been set yet
                || StartsNounPhrase(firstWord)
                || IsPrepositionalPhrase(node.ParsedName)
                || IsNonBaseVerb(firstWord))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Constructs the SWUM for the given node, using this rule.
        /// </summary>
        /// <param name="node">The node to construct SWUM for.</param>
        public override void ConstructSwum(ProgramElementNode node)
        {
            if (node is MethodDeclarationNode)
            {
                MethodDeclarationNode mdn = (MethodDeclarationNode)node;
                ParseBaseVerbName(mdn);
                mdn.AssignStructuralInformation(this.Splitter, this.PosTagger);
                DetermineMethodRole(mdn);
                SetDefaultActionAndTheme(mdn);
                mdn.SwumRuleUsed = this;
            }
        }


        private bool IsPrepositionalPhrase(PhraseNode parsedName)
        {
            foreach (WordNode word in parsedName.GetPhrase())
            {
                if (word.Tag == PartOfSpeechTag.Preposition) { return true; }
            }
            return false;
        }

        /// <summary>
        /// Sets the Role on the given MethodDeclarationNode based on its name.
        /// </summary>
        /// <param name="mdn">The node to set the role on.</param>
        private void DetermineMethodRole(MethodDeclarationNode mdn)
        {
            if (mdn.ReturnType == null)
            {
                //Constructors and destructors don't have a return type.
                //However, they should be processed by a different rule, and therefore shouldn't reach this point.
                mdn.Role = MethodRole.Unknown;
            }
            else if (mdn.ReturnType.Name.ToLower() == "void")
            {
                if (mdn.ParsedName[0].Text.ToLower() == "set")
                {
                    mdn.Role = MethodRole.Setter;
                }
                else
                {
                    mdn.Role = MethodRole.Action;
                }
            }
            else
            {
                if (mdn.ParsedName[0].Text.ToLower() == "get")
                {
                    mdn.Role = MethodRole.Getter;
                }
                else
                {
                    mdn.Role = MethodRole.Function;
                }
            }
        }
    }
}
