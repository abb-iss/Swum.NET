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
using ABB.Swum.Utilities;

namespace ABB.Swum.WordData
{
    /// <summary>
    /// Encapsulates part-of-speech data obtained from the PCKimmo/KTagger part-of-speech tagger.
    /// </summary>
    public class PCKimmoPartOfSpeechData : PartOfSpeechData
    {
        private HashSet<string> Prepositions;
        private HashSet<string> VerbsThirdPersonSingular;
        private HashSet<string> VerbsThirdPersonIrregular;
        private HashSet<string> ModalVerbs;
        private HashSet<string> IngVerbs;
        private HashSet<string> PastTenseVerbs;
        private HashSet<string> PastParticipleVerbs;
        private HashSet<string> PotentialVerbs;
        private HashSet<string> OnlyNouns;
        private HashSet<string> Adjectives;
        private HashSet<string> Adverbs;
        private HashSet<string> Determiners;
        private HashSet<string> Pronouns;
        private HashSet<string> IgnorableVerbs;
        private HashSet<string> IgnorableHeadWords;
        private HashSet<string> GeneralVerbs;
        private HashSet<string> EventWords;
        private HashSet<string> SideEffectWords;
        private HashSet<string> TwoDict;
        private Dictionary<string, HashSet<string>> VerbParticles;

        /// <summary>
        /// Creates a new PCKimmoPartOfSpeechData object using the default file locations for the part-of-speech data.
        /// </summary>
        public PCKimmoPartOfSpeechData()
        {
            TwoDict = LibFileLoader.ReadWordList(SwumConfiguration.GetFileSetting("PCKimmoPartOfSpeechData.TwoDictFile"));
            Prepositions = LibFileLoader.ReadWordList(SwumConfiguration.GetFileSetting("PCKimmoPartOfSpeechData.PrepositionsFile"));
            VerbsThirdPersonSingular = LibFileLoader.ReadWordList(SwumConfiguration.GetFileSetting("PCKimmoPartOfSpeechData.VerbsThirdPersonSingularFile"));
            VerbsThirdPersonIrregular = LibFileLoader.ReadWordList(SwumConfiguration.GetFileSetting("PCKimmoPartOfSpeechData.VerbsThirdPersonIrregularFile"));
            ModalVerbs = LibFileLoader.ReadWordList(SwumConfiguration.GetFileSetting("PCKimmoPartOfSpeechData.ModalVerbsFile"));
            IngVerbs = LibFileLoader.ReadWordList(SwumConfiguration.GetFileSetting("PCKimmoPartOfSpeechData.IngVerbsFile"));
            PastTenseVerbs = LibFileLoader.ReadWordList(SwumConfiguration.GetFileSetting("PCKimmoPartOfSpeechData.PastTenseVerbsRegularFile"), SwumConfiguration.GetFileSetting("PCKimmoPartOfSpeechData.PastTenseVerbsIrregularFile"));
            PastParticipleVerbs = LibFileLoader.ReadWordList(SwumConfiguration.GetFileSetting("PCKimmoPartOfSpeechData.PastParticipleVerbsRegularFile"), SwumConfiguration.GetFileSetting("PCKimmoPartOfSpeechData.PastParticipleVerbsIrregularFile"));
            PotentialVerbs = LibFileLoader.ReadWordList(SwumConfiguration.GetFileSetting("PCKimmoPartOfSpeechData.PotentialVerbsFile"));
            OnlyNouns = LibFileLoader.ReadWordList(SwumConfiguration.GetFileSetting("PCKimmoPartOfSpeechData.OnlyNounsFile"));
            Adjectives = LibFileLoader.ReadWordList(SwumConfiguration.GetFileSetting("PCKimmoPartOfSpeechData.AdjectivesFile"));
            Adverbs = LibFileLoader.ReadWordList(SwumConfiguration.GetFileSetting("PCKimmoPartOfSpeechData.AdverbsFile"));
            Determiners = LibFileLoader.ReadWordList(SwumConfiguration.GetFileSetting("PCKimmoPartOfSpeechData.DeterminersFile"));
            Pronouns = LibFileLoader.ReadWordList(SwumConfiguration.GetFileSetting("PCKimmoPartOfSpeechData.PronounsFile"));
            IgnorableVerbs = new HashSet<string>(LibFileLoader.ReadWordCount(SwumConfiguration.GetFileSetting("PCKimmoPartOfSpeechData.IgnorableVerbsFile")).Keys);
            IgnorableHeadWords = new HashSet<string>(LibFileLoader.ReadWordCount(SwumConfiguration.GetFileSetting("PCKimmoPartOfSpeechData.IgnorableHeadWordsFile")).Keys);
            GeneralVerbs = LibFileLoader.ReadWordList(SwumConfiguration.GetFileSetting("PCKimmoPartOfSpeechData.GeneralVerbsFile"));
            EventWords = LibFileLoader.ReadWordList(SwumConfiguration.GetFileSetting("PCKimmoPartOfSpeechData.EventWordsFile"));
            SideEffectWords = LibFileLoader.ReadWordList(SwumConfiguration.GetFileSetting("PCKimmoPartOfSpeechData.SideEffectWordsFile"));
            VerbParticles = LibFileLoader.ReadVerbParticleFile(SwumConfiguration.GetFileSetting("PCKimmoPartOfSpeechData.VerbParticlesFile"));
        }

        /// <summary>
        /// Indicates whether the given word is a preposition.
        /// </summary>
        /// <param name="word">The word to test.</param>
        /// <returns>True if the word is a preposition, False otherwise.</returns>
        public override bool IsPreposition(string word)
        {
            return Prepositions.Contains(word.ToLower());
        }

        /// <summary>
        /// Indicates whether the given word is a noun.
        /// </summary>
        /// <param name="word">The word to test.</param>
        /// <returns>True if the word is a noun, False otherwise.</returns>
        public override bool IsNoun(string word)
        {
            string lowerWord = word.ToLower();
            return !PotentialVerbs.Contains(lowerWord) && OnlyNouns.Contains(lowerWord) && !Regex.IsMatch(lowerWord, "^.*(up|down|out)$");
        }

        /// <summary>
        /// Indicates whether the given word is a verb particle. For example, in "pick up", "pick" is a verb and "up" is a verb particle.
        /// </summary>
        /// <param name="verb">The verb to test the particle against.</param>
        /// <param name="word">The potential particle.</param>
        /// <returns>True if the word is a verb particle, False otherwise.</returns>
        public override bool IsVerbParticle(string verb, string word)
        {
            string lowerWord = word.ToLower();
            return VerbParticles.ContainsKey(lowerWord) && VerbParticles[lowerWord].Contains(verb.ToLower());
        }

        /// <summary>
        /// Indicates whether the given word is in past participle form, e.g. -ed
        /// </summary>
        /// <param name="word">The word to test.</param>
        /// <returns>True if the word is a past participle, False otherwise.</returns>
        public override bool IsPastParticiple(string word)
        {
            return PastParticipleVerbs.Contains(word.ToLower());
        }

        /// <summary>
        /// Indicates whether the given word is in past tense.
        /// </summary>
        /// <param name="word">The word to test.</param>
        /// <returns>True if the word is in past tense, False otherwise.</returns>
        public override bool IsPastTense(string word)
        {
            return PastTenseVerbs.Contains(word.ToLower());
        }

        /// <summary>
        /// Indicates whether the given word is in present participle form, e.g. -ing
        /// </summary>
        /// <param name="word">The word to test.</param>
        /// <returns>True if the word is a present participle, False otherwise.</returns>
        public override bool IsPresentParticiple(string word)
        {
            return IngVerbs.Contains(word.ToLower());
        }

        /// <summary>
        /// Indicates whether the given word is an adjective.
        /// </summary>
        /// <param name="word">The word to test.</param>
        /// <returns>True if the word is a adjective, False otherwise.</returns>
        public override bool IsAdjective(string word)
        {
            return !PotentialVerbs.Contains(word.ToLower()) && Adjectives.Contains(word.ToLower());
        }

        /// <summary>
        /// Indicates whether the given word is an adverb.
        /// </summary>
        /// <param name="word">The word to test.</param>
        /// <returns>True if the word is a adverb, False otherwise.</returns>
        public override bool IsAdverb(string word)
        {
            return Adverbs.Contains(word.ToLower());
        }

        /// <summary>
        /// Indicates whether the given word is a determiner, e.g. the, this, that.
        /// </summary>
        /// <param name="word">The word to test.</param>
        /// <returns>True if the word is a determiner, False otherwise.</returns>
        public override bool IsDeterminer(string word)
        {
            return Determiners.Contains(word.ToLower());
        }

        /// <summary>
        /// Indicates whether the given word is a pronoun.
        /// </summary>
        /// <param name="word">The word to test.</param>
        /// <returns>True if the word is a pronoun, False otherwise.</returns>
        public override bool IsPronoun(string word)
        {
            return Pronouns.Contains(word.ToLower());
        }

        /// <summary>
        /// Indicates whether the given word is a verb in third person singular tense.
        /// </summary>
        /// <param name="word">The word to test.</param>
        /// <returns>True if the word is a third person singular verb, False otherwise.</returns>
        public override bool IsThirdPersonSingularVerb(string word)
        {
            return VerbsThirdPersonSingular.Contains(word.ToLower());
        }

        /// <summary>
        /// Indicates whether the given word is an irregular third person singular verb.
        /// </summary>
        /// <param name="word">The word to test.</param>
        /// <returns>True if the word is an irregular third person verb, False otherwise.</returns>
        public override bool IsThirdPersonIrregularVerb(string word)
        {
            return VerbsThirdPersonIrregular.Contains(word.ToLower());
        }

        /// <summary>
        /// Indicates whether the given word is a modal verb, e.g. can, should, will.
        /// </summary>
        /// <param name="word">The word to test.</param>
        /// <returns>True if the word is a modal verb, False otherwise.</returns>
        public override bool IsModalVerb(string word)
        {
            return ModalVerbs.Contains(word.ToLower());
        }

        /// <summary>
        /// Indicates whether the given word is an ignorable verb.
        /// </summary>
        /// <param name="word">The word to test.</param>
        /// <returns>True if the word is an ignorable verb, False otherwise.</returns>
        public override bool IsIgnorableVerb(string word)
        {
            return IgnorableVerbs.Contains(word.ToLower());
        }

        /// <summary>
        /// Indicates whether the given word is a general verb.
        /// </summary>
        /// <param name="word">The word to test.</param>
        /// <returns>True if the word is a general verb, False otherwise.</returns>
        public override bool IsGeneralVerb(string word)
        {
            return GeneralVerbs.Contains(word.ToLower());
        }

        /// <summary>
        /// Indicates whether the given word relates generally to the execution of an event, e.g. start, begin, finish, int.
        /// </summary>
        /// <param name="word">The word to test.</param>
        /// <returns>True if the word is an event word, False otherwise.</returns>
        public override bool IsEventWord(string word)
        {
            return EventWords.Contains(word.ToLower());
        }

        /// <summary>
        /// Indicates whether the given word indicates that a method might have some side effect, e.g. log, notify, throw.
        /// </summary>
        /// <param name="word">The word to test.</param>
        /// <returns>True if the word is a side effect word, False otherwise.</returns>
        public override bool IsSideEffectWord(string word)
        {
            return SideEffectWords.Contains(word.ToLower());
        }

        /// <summary>
        /// Indicates whether the given word is a two letter word that appears in the dictionary.
        /// </summary>
        /// <param name="word">The word to test.</param>
        /// <returns>True if the word is a two letter dictionary word, False otherwise.</returns>
        public override bool IsTwoLetterDictionaryWord(string word)
        {
            return TwoDict.Contains(word.ToLower());
        }

        /// <summary>
        /// Indicates whether the given word is potentially a verb.
        /// </summary>
        /// <param name="word">The word to test.</param>
        /// <returns>True if the word is potentially a verb, False otherwise.</returns>
        public override bool IsPotentialVerb(string word)
        {
            return PotentialVerbs.Contains(word.ToLower());
        }

        /// <summary>
        /// Indicates whether the given word is an ignorable head word.
        /// </summary>
        /// <param name="word">The word to test.</param>
        /// <returns>True if the word is an ignorable head word, False otherwise.</returns>
        public override bool IsIgnorableHeadWord(string word)
        {
            return IgnorableHeadWords.Contains(word.ToLower());
        }
    }
}
