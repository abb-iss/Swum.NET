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

namespace ABB.Swum.WordData
{
    /// <summary>
    /// Encapsulates a collection of word part-of-speech data.
    /// </summary>
    public abstract class PartOfSpeechData
    {
        /// <summary>
        /// Indicates whether the given word is a preposition.
        /// </summary>
        /// <param name="word">The word to test.</param>
        /// <returns>True if the word is a preposition, False otherwise.</returns>
        public abstract bool IsPreposition(string word);
        /// <summary>
        /// Indicates whether the given word is a noun.
        /// </summary>
        /// <param name="word">The word to test.</param>
        /// <returns>True if the word is a noun, False otherwise.</returns>
        public abstract bool IsNoun(string word);
        /// <summary>
        /// Indicates whether the given word is an adjective.
        /// </summary>
        /// <param name="word">The word to test.</param>
        /// <returns>True if the word is a adjective, False otherwise.</returns>
        public abstract bool IsAdjective(string word);
        /// <summary>
        /// Indicates whether the given word is an adverb.
        /// </summary>
        /// <param name="word">The word to test.</param>
        /// <returns>True if the word is a adverb, False otherwise.</returns>
        public abstract bool IsAdverb(string word);
        /// <summary>
        /// Indicates whether the given word is a determiner, e.g. the, this, that.
        /// </summary>
        /// <param name="word">The word to test.</param>
        /// <returns>True if the word is a determiner, False otherwise.</returns>
        public abstract bool IsDeterminer(string word);
        /// <summary>
        /// Indicates whether the given word is a pronoun.
        /// </summary>
        /// <param name="word">The word to test.</param>
        /// <returns>True if the word is a pronoun, False otherwise.</returns>
        public abstract bool IsPronoun(string word);
        /// <summary>
        /// Indicates whether the given word is in past tense.
        /// </summary>
        /// <param name="word">The word to test.</param>
        /// <returns>True if the word is in past tense, False otherwise.</returns>
        public abstract bool IsPastTense(string word);
        /// <summary>
        /// Indicates whether the given word is in past participle form, e.g. -ed
        /// </summary>
        /// <param name="word">The word to test.</param>
        /// <returns>True if the word is a past participle, False otherwise.</returns>
        public abstract bool IsPastParticiple(string word);
        /// <summary>
        /// Indicates whether the given word is in present participle form, e.g. -ing
        /// </summary>
        /// <param name="word">The word to test.</param>
        /// <returns>True if the word is a present participle, False otherwise.</returns>
        public abstract bool IsPresentParticiple(string word);
        /// <summary>
        /// Indicates whether the given word is a verb particle. For example, in "pick up", "pick" is a verb and "up" is a verb particle.
        /// </summary>
        /// <param name="verb">The verb to test the particle against.</param>
        /// <param name="word">The potential particle.</param>
        /// <returns>True if the word is a verb particle, False otherwise.</returns>
        public abstract bool IsVerbParticle(string verb, string word);
        /// <summary>
        /// Indicates whether the given word is a verb in third person singular tense.
        /// </summary>
        /// <param name="word">The word to test.</param>
        /// <returns>True if the word is a third person singular verb, False otherwise.</returns>
        public abstract bool IsThirdPersonSingularVerb(string word);
        /// <summary>
        /// Indicates whether the given word is an irregular third person singular verb.
        /// </summary>
        /// <param name="word">The word to test.</param>
        /// <returns>True if the word is an irregular third person verb, False otherwise.</returns>
        public abstract bool IsThirdPersonIrregularVerb(string word);
        /// <summary>
        /// Indicates whether the given word is a modal verb, e.g. can, should, will.
        /// </summary>
        /// <param name="word">The word to test.</param>
        /// <returns>True if the word is a modal verb, False otherwise.</returns>
        public abstract bool IsModalVerb(string word);
        /// <summary>
        /// Indicates whether the given word is an ignorable verb.
        /// </summary>
        /// <param name="word">The word to test.</param>
        /// <returns>True if the word is an ignorable verb, False otherwise.</returns>
        public abstract bool IsIgnorableVerb(string word);
        /// <summary>
        /// Indicates whether the given word is an ignorable head word.
        /// </summary>
        /// <param name="word">The word to test.</param>
        /// <returns>True if the word is an ignorable head word, False otherwise.</returns>
        public abstract bool IsIgnorableHeadWord(string word);
        /// <summary>
        /// Indicates whether the given word is a two letter word that appears in the dictionary.
        /// </summary>
        /// <param name="word">The word to test.</param>
        /// <returns>True if the word is a two letter dictionary word, False otherwise.</returns>
        public abstract bool IsTwoLetterDictionaryWord(string word);
        /// <summary>
        /// Indicates whether the given word is potentially a verb.
        /// </summary>
        /// <param name="word">The word to test.</param>
        /// <returns>True if the word is potentially a verb, False otherwise.</returns>
        public abstract bool IsPotentialVerb(string word);
        /// <summary>
        /// Indicates whether the given word is a general verb.
        /// </summary>
        /// <param name="word">The word to test.</param>
        /// <returns>True if the word is a general verb, False otherwise.</returns>
        public abstract bool IsGeneralVerb(string word);
        /// <summary>
        /// Indicates whether the given word relates generally to the execution of an event, e.g. start, begin, finish, int.
        /// </summary>
        /// <param name="word">The word to test.</param>
        /// <returns>True if the word is an event word, False otherwise.</returns>
        public abstract bool IsEventWord(string word);
        /// <summary>
        /// Indicates whether the given word indicates that a method might have some side effect, e.g. log, notify, throw.
        /// </summary>
        /// <param name="word">The word to test.</param>
        /// <returns>True if the word is a side effect word, False otherwise.</returns>
        public abstract bool IsSideEffectWord(string word);
    }
}
