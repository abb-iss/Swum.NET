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
using System.Text.RegularExpressions;
using System.Configuration;
using ABB.Swum.Nodes;
using ABB.Swum.WordData;
using ABB.Swum.Utilities;

namespace ABB.Swum
{
    /// <summary>
    /// Assigns part-of-speech tags to the word nodes in an identifier. This is done based on unigram part-of-speech data.
    /// </summary>
    public class UnigramTagger : Tagger
    {
        private PartOfSpeechData pos;
        private PositionalFrequencies positionalFrequencies;

        /// <summary>
        /// Creates a new UnigramTagger object, using the supplied data sets.
        /// </summary>
        /// <param name="posData">The part-of-speech data to use.</param>
        /// <param name="frequencies">The positional frequency data to use.</param>
        public UnigramTagger(PartOfSpeechData posData, PositionalFrequencies frequencies)
        {
            this.pos = posData;
            this.positionalFrequencies = frequencies;
        }

        /// <summary>
        /// Creates a new UnigramTagger object, using the specified part-of-speech data.
        /// </summary>
        /// <param name="posData">The part-of-speech data to use.</param>
        public UnigramTagger(PartOfSpeechData posData) : this(posData, new PositionalFrequencies()) { }

        /// <summary>
        /// Creates a new UnigramTagger object. The default values are used for necessary data sets.
        /// </summary>
        public UnigramTagger() : this(new PCKimmoPartOfSpeechData()) { }
        
        /// <summary>
        /// Performs various actions that should occur prior to further tagging. 
        /// This method tags any digits in the name, identifies and removes any preamble, and tags any prepositions remaining in the name.
        /// </summary>
        /// <param name="node">The node to be tagged.</param>
        public override void PreTag(ProgramElementNode node)
        {
            if (node == null) { return; }

            TagDigits(node.ParsedName);

            if (node.Preamble == null)
            {
                int wordIndex = 0;
                bool checkForMorePreamble = true;
                //identify and tag any preamble words
                while (node.ParsedName.Size() > 1 && checkForMorePreamble)
                {
                    checkForMorePreamble = false;

                    //skip any digits
                    while (wordIndex < node.ParsedName.Size() && node.ParsedName[wordIndex].Tag == PartOfSpeechTag.Digit) { wordIndex++; }

                    //check if word is preamble
                    if (wordIndex < node.ParsedName.Size() - 1)
                    {
                        string word = node.ParsedName[wordIndex].Text;
                        if (word.Length == 1 ||
                            (word.Length == 2 && !pos.IsTwoLetterDictionaryWord(word)) ||
                            (word.Length < 5 && !Regex.IsMatch(word, ".*[gs]et.*") && !pos.IsPotentialVerb(word) &&
                             positionalFrequencies.GetOnlyFrequency(word) == 0 && positionalFrequencies.GetFirstFrequency(word) > 0))
                        {
                            node.ParsedName[wordIndex].Tag = PartOfSpeechTag.Preamble;
                            wordIndex++;
                            checkForMorePreamble = true;
                        }
                    }
                }

                //move preamble words from ParsedName to Preamble
                node.Preamble = node.ParsedName.GetNewEmpty();
                for (int j = 0; j < wordIndex; j++)
                {
                    node.Preamble.Add(node.ParsedName[0]);
                    node.ParsedName.RemoveWord(0);
                }
            }

            TagPrepostions(node.ParsedName);
        }

        /// <summary>
        /// Assigns part-of-speech tags to the word nodes in the given variable name.
        /// </summary>
        /// <param name="parsedName">The parsed variable name to tag.</param>
        public override void TagVariableName(PhraseNode parsedName)
        {
            TagNounPhrase(parsedName);
        }

        /// <summary>
        /// Assigns part-of-speech tags to the word nodes in the given type name.
        /// </summary>
        /// <param name="parsedName">The parsed type name to tag.</param>
        public override void TagType(PhraseNode parsedName)
        {
            TagNounPhrase(parsedName);
        }

        /// <summary>
        /// Assigns part-of-speech tags to the word nodes in the given phrase, assuming it is a noun phrase.
        /// </summary>
        /// <param name="phrase">The noun phrase to tag.</param>
        public override void TagNounPhrase(PhraseNode phrase)
        {
            TagNounPhrase(phrase, 0, phrase.Size() - 1);
        }

        /// <summary>
        /// Assigns part-of-speech tags to the word nodes in the given phrase, assuming it is a noun phrase.
        /// Only the words between startIndex and stopIndex, inclusive, are tagged.
        /// </summary>
        /// <param name="phrase">The noun phrase to tag.</param>
        /// <param name="startIndex">The index of the first word to tag.</param>
        /// <param name="stopIndex">The index of the last word to tag.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">startIndex or stopIndex are not valid indices, or stopIndex is less than startIndex</exception>
        public override void TagNounPhrase(PhraseNode phrase, int startIndex, int stopIndex)
        {
            if (phrase == null || phrase.Size() <= 0) { return; }
            if (startIndex < 0 || startIndex >= phrase.Size())
            {
                throw new ArgumentOutOfRangeException("startIndex", startIndex, string.Format("The given value is not a valid index of the PhraseNode. It must be between 0 and {0}.", phrase.Size() - 1));
            }
            if (stopIndex < startIndex || stopIndex >= phrase.Size())
            {
                throw new ArgumentOutOfRangeException("stopIndex", stopIndex, string.Format("The given value must be a valid index of the PhraseNode, and must be larger than the given startIndex value of {0}", startIndex));
            }

            //start from the end of the phrase
            int currentWord = stopIndex;
            //skip any digits at the end
            while (phrase[currentWord].Tag == PartOfSpeechTag.Digit) { currentWord--; }

            //tag the last word
            if (currentWord >= startIndex)
            {
                if (pos.IsDeterminer(phrase[currentWord].Text))
                {
                    phrase[currentWord].Tag = PartOfSpeechTag.Determiner;
                }
                else if (pos.IsPronoun(phrase[currentWord].Text))
                {
                    phrase[currentWord].Tag = PartOfSpeechTag.Pronoun;
                }
                else if (pos.IsIgnorableHeadWord(phrase[currentWord].Text))
                {
                    phrase[currentWord].Tag = PartOfSpeechTag.NounIgnorable;
                }
                else
                {
                    phrase[currentWord].Tag = PartOfSpeechTag.Noun;
                }
                currentWord--;
            }

            //tag the rest of the words
            while (currentWord >= startIndex)
            {
                if (pos.IsDeterminer(phrase[currentWord].Text))
                {
                    phrase[currentWord].Tag = PartOfSpeechTag.Determiner;
                }
                else if (pos.IsPronoun(phrase[currentWord].Text))
                {
                    phrase[currentWord].Tag = PartOfSpeechTag.Pronoun;
                }
                else if (phrase[currentWord].Tag != PartOfSpeechTag.Digit)
                {
                    phrase[currentWord].Tag = PartOfSpeechTag.NounModifier;
                }

                currentWord--;
            }
        }

        /// <summary>
        /// Tags any word nodes in the given phrase that contain digits.
        /// </summary>
        /// <param name="phrase">The phrase to tag.</param>
        public void TagDigits(PhraseNode phrase)
        {
            if (phrase == null) { return; }

            foreach (var word in phrase.GetPhrase())
            {
                if(Regex.IsMatch(word.Text, @"\d+"))
                {
                    word.Tag = PartOfSpeechTag.Digit;
                }
            }
        }

        /// <summary>
        /// Tags any word nodes in the given phrase that are prepositions.
        /// </summary>
        /// <param name="phrase">The phrase to tag.</param>
        public void TagPrepostions(PhraseNode phrase)
        {
            if (phrase == null) { return; }

            foreach (var word in phrase.GetPhrase())
            {
                if (pos.IsPreposition(word.Text))
                {
                    word.Tag = PartOfSpeechTag.Preposition;
                }
            }
        }

    }
}
