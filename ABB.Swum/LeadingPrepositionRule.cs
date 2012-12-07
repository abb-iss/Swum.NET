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
    /// The rule to construct the SWUM for methods whose names begin with a preposition.
    /// </summary>
    public class LeadingPrepositionRule : UnigramMethodRule
    {
        /// <summary>
        /// Creates a new LeadingPrepositionRule using default values for data sets.
        /// </summary>
        public LeadingPrepositionRule() : base() { }

        /// <summary>
        /// Creates a new LeadingPrepositionRule.
        /// </summary>
        /// <param name="posData">The part-of-speech data to use.</param>
        /// <param name="tagger">The part-of-speech tagger to use.</param>
        /// <param name="splitter">The identifier splitter to use.</param>
        public LeadingPrepositionRule(PartOfSpeechData posData, Tagger tagger, IdSplitter splitter) : base(posData, tagger, splitter) { }

        /// <summary>
        /// Creates a new LeadingPrepositionRule.
        /// </summary>
        /// <param name="specialWords">A list of words that indicate the method name needs special handling.</param>
        /// <param name="booleanArgumentVerbs">A list of verbs that indicate that the boolean arguments to a method should be included in the UnknownArguments list.</param>
        /// <param name="nounPhraseIndicators">A list of word that indicate that beginning of a noun phrase.</param>
        /// <param name="positionalFrequencies">Positional frequency data.</param>
        public LeadingPrepositionRule(HashSet<string> specialWords, HashSet<string> booleanArgumentVerbs, HashSet<string> nounPhraseIndicators, PositionalFrequencies positionalFrequencies)
            : base(specialWords, booleanArgumentVerbs, nounPhraseIndicators, positionalFrequencies) { }

        /// <summary>
        /// Creates a new LeadingPrepositionRule.
        /// </summary>
        /// <param name="posData">The part-of-speech data to use.</param>
        /// <param name="tagger">The part-of-speech tagger to use.</param>
        /// <param name="splitter">The identifier splitter to use.</param>
        /// <param name="specialWords">A list of words that indicate the method name needs special handling.</param>
        /// <param name="booleanArgumentVerbs">A list of verbs that indicate that the boolean arguments to a method should be included in the UnknownArguments list.</param>
        /// <param name="nounPhraseIndicators">A list of word that indicate that beginning of a noun phrase.</param>
        /// <param name="positionalFrequencies">Positional frequency data.</param>
        public LeadingPrepositionRule(PartOfSpeechData posData, Tagger tagger, IdSplitter splitter, HashSet<string> specialWords, HashSet<string> booleanArgumentVerbs, HashSet<string> nounPhraseIndicators, PositionalFrequencies positionalFrequencies)
            : base(posData, tagger, splitter, specialWords, booleanArgumentVerbs, nounPhraseIndicators, positionalFrequencies) { }

        /// <summary>
        /// Determines whether the given MethodDeclarationNode meets the conditions for this rule.
        /// This method assumes that the name is parsed, preamble is stripped, and digits and prepositions are tagged.
        /// </summary>
        /// <param name="node">The MethodDeclarationNode to test.</param>
        /// <returns>True if the node meets the conditions for this rule, False otherwise.</returns>
        protected override bool MakeClassification(MethodDeclarationNode node)
        {
            if (node.ParsedName != null && node.ParsedName.Size() > 0)
            {
                return PosData.IsPreposition(node.ParsedName[0].Text);
            }
            else
            {
                return false;
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
                var mdn = (MethodDeclarationNode)node;

                mdn.AssignStructuralInformation(this.Splitter, this.PosTagger);
                if (mdn.ParsedName[0].Tag != PartOfSpeechTag.Preposition)
                {
                    //this shouldn't be necessary, because prepositions should have been tagged in tagger.PreTag, which should have been called prior to this method
                    mdn.ParsedName[0].Tag = PartOfSpeechTag.Preposition;
                    Console.Error.WriteLine("LeadingPrepositionRule.ConstructSwum(): found node with untagged preposition: {0}", mdn);
                }
                this.PosTagger.TagNounPhrase(mdn.ParsedName, 1, mdn.ParsedName.Size() - 1);
                mdn.CreateThemeFromPhrases(mdn.Preamble, mdn.ParsedName);
                //TODO: setting the Theme might need to account for an empty preamble

                //TODO: from Emily, make name proper SecondaryArg

                string prep = mdn.ParsedName[0].Text.ToLower();
                if (prep == "to" || prep == "from")
                {
                    mdn.Action = mdn.ParsedName[0].GetNewWord("convert", PartOfSpeechTag.Verb);
                    SetDefaultUnknownArguments(mdn);
                }
                else if (prep == "on" || prep == "before" || prep == "after") //EventHandlerRule should be run first
                {
                    ParseReactiveName(mdn);
                }
                else
                {
                    SetDefaultUnknownArguments(mdn);
                }

                mdn.SwumRuleUsed = this;
            }
        }
    }
}
