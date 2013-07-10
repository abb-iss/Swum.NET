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
using ABB.Swum.Nodes;
using ABB.Swum.WordData;

namespace ABB.Swum
{
    /// <summary>
    /// Base class for unigram-style SWUM rules for methods.
    /// </summary>
    public abstract class UnigramMethodRule : UnigramRule
    {
        /// <summary>
        /// A list of words that indicate the method name needs special handling.
        /// </summary>
        public HashSet<string> SpecialWords { get; set; }
        /// <summary>
        /// A list of verbs which indicate that the boolean arguments to a method should be included in the UnknownArguments list.
        /// </summary>
        public HashSet<string> BooleanArgumentVerbs { get; set; }
        /// <summary>
        /// A list of word that indicate that beginning of a noun phrase.
        /// </summary>
        public HashSet<string> NounPhraseIndicators { get; set; }
        /// <summary>
        /// Positional frequency data.
        /// </summary>
        public PositionalFrequencies PositionalFrequencies { get; set; }

        /// <summary>
        /// Creates a new UnigramMethodRule using default values for data sets.
        /// </summary>
        public UnigramMethodRule()
            : base()
        {
            InitializeMembers();
        }

        /// <summary>
        /// Creates a new UnigramMethodRule.
        /// </summary>
        /// <param name="posData">The part-of-speech data to use.</param>
        /// <param name="tagger">The part-of-speech tagger to use.</param>
        /// <param name="splitter">The identifier splitter to use.</param>
        public UnigramMethodRule(PartOfSpeechData posData, Tagger tagger, IdSplitter splitter)
        {
            this.PosData = posData;
            this.PosTagger = tagger;
            this.Splitter = splitter;
            InitializeMembers();
        }

        /// <summary>
        /// Creates a new UnigramMethodRule.
        /// </summary>
        /// <param name="specialWords">A list of words that indicate the method name needs special handling.</param>
        /// <param name="booleanArgumentVerbs">A list of verbs that indicate that the boolean arguments to a method should be included in the UnknownArguments list.</param>
        /// <param name="nounPhraseIndicators">A list of word that indicate that beginning of a noun phrase.</param>
        /// <param name="positionalFrequencies">Positional frequency data.</param>
        public UnigramMethodRule(HashSet<string> specialWords, HashSet<string> booleanArgumentVerbs, HashSet<string> nounPhraseIndicators, PositionalFrequencies positionalFrequencies)
            : base()
        {
            this.SpecialWords = specialWords;
            this.BooleanArgumentVerbs = booleanArgumentVerbs;
            this.NounPhraseIndicators = nounPhraseIndicators;
            this.PositionalFrequencies = positionalFrequencies;
        }

        /// <summary>
        /// Creates a new UnigramMethodRule.
        /// </summary>
        /// <param name="posData">The part-of-speech data to use.</param>
        /// <param name="tagger">The part-of-speech tagger to use.</param>
        /// <param name="splitter">The identifier splitter to use.</param>
        /// <param name="specialWords">A list of words that indicate the method name needs special handling.</param>
        /// <param name="booleanArgumentVerbs">A list of verbs that indicate that the boolean arguments to a method should be included in the UnknownArguments list.</param>
        /// <param name="nounPhraseIndicators">A list of word that indicate that beginning of a noun phrase.</param>
        /// <param name="positionalFrequencies">Positional frequency data.</param>
        public UnigramMethodRule(PartOfSpeechData posData, Tagger tagger, IdSplitter splitter, HashSet<string> specialWords, HashSet<string> booleanArgumentVerbs, HashSet<string> nounPhraseIndicators, PositionalFrequencies positionalFrequencies)
        {
            this.PosData = posData;
            this.PosTagger = tagger;
            this.Splitter = splitter;
            this.SpecialWords = specialWords;
            this.BooleanArgumentVerbs = booleanArgumentVerbs;
            this.NounPhraseIndicators = nounPhraseIndicators;
            this.PositionalFrequencies = positionalFrequencies;
        }

        /// <summary>
        /// Determines whether the given node satisfies this rule.
        /// 
        /// ** Note that calling this method has the effect of stripping any preamble from the given node, and tagging any digits and prepositions. **
        /// </summary>
        /// <param name="node">The node to test.</param>
        /// <returns>True if the node matches this rule, False otherwise.</returns>
        public override bool InClass(ProgramElementNode node)
        {
            if (node is MethodDeclarationNode)
            {
                MethodDeclarationNode mdn = (MethodDeclarationNode)node;
                mdn.Parse(this.Splitter);
                this.PosTagger.PreTag(mdn);
                return MakeClassification(mdn);
            }
            return false;
        }

        /// <summary>
        /// Performs further rule testing beyond the InClass method. 
        /// InClass tests whether the node is a MethodDeclarationNode, parses the node name, strips the preamble, tags digits and prepositions, then calls this method.
        /// </summary>
        /// <param name="node">The MethodDeclarationNode to test.</param>
        /// <returns>True if the node matches this rule, False otherwise.</returns>
        protected abstract bool MakeClassification(MethodDeclarationNode node);

        /// <summary>
        /// Sets the member data sets to their default states.
        /// </summary>
        private void InitializeMembers() {
            string specialWordsFile = SwumConfiguration.GetFileSetting("UnigramMethodRule.SpecialWordsFile");
            if(specialWordsFile != null) {
                this.SpecialWords = new HashSet<string>(LibFileLoader.ReadWordList(specialWordsFile), StringComparer.InvariantCultureIgnoreCase);
            } else {
                this.SpecialWords = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
                Console.Error.WriteLine("UnigramMethodRule.SpecialWordsFile not specified in config file.");
            }

            string booleanArgumentVerbsFile = SwumConfiguration.GetFileSetting("UnigramMethodRule.BooleanArgumentVerbsFile");
            if(booleanArgumentVerbsFile != null) {
                this.BooleanArgumentVerbs = new HashSet<string>(LibFileLoader.ReadWordList(booleanArgumentVerbsFile), StringComparer.InvariantCultureIgnoreCase);
            } else {
                this.BooleanArgumentVerbs = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
                Console.Error.WriteLine("UnigramMethodRule.BooleanArgumentVerbsFile not specified in config file.");
            }

            string nounPhraseIndicatorsFile = SwumConfiguration.GetFileSetting("UnigramMethodRule.NounPhraseIndicatorsFile");
            if(nounPhraseIndicatorsFile != null) {
                this.NounPhraseIndicators = new HashSet<string>(LibFileLoader.ReadWordList(nounPhraseIndicatorsFile), StringComparer.InvariantCultureIgnoreCase);
            } else {
                this.NounPhraseIndicators = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
                Console.Error.WriteLine("UnigramMethodRule.NounPhraseIndicatorsFile not specified in config file.");
            }

            this.PositionalFrequencies = new PositionalFrequencies();
        }

        /// <summary>
        /// Assigns part-of-speech tags to the words in the given MethodDeclarationNode's name, assuming that it follows a base verb pattern.
        /// This assumes that the node has already had its name split and preamble stripped.
        /// </summary>
        /// <param name="node">The MethodDeclarationNode to tag.</param>
        protected void ParseBaseVerbName(MethodDeclarationNode node)
        {
            //this assumes that the node has already had its name split and preamble stripped

            PhraseNode parsedName = node.ParsedName;

            //TODO: from Emily, what if it starts with an adverb??

            //if 1 word, assume verb
            if (parsedName.Size() == 1)
            {
                if (PosData.IsIgnorableVerb(parsedName[0].Text))
                {
                    parsedName[0].Tag = PartOfSpeechTag.VerbIgnorable;
                }
                else
                {
                    parsedName[0].Tag = PartOfSpeechTag.Verb;
                }
                return;
            }

            int currentWord = 0;
            currentWord = CheckForIgnorableVerb(parsedName, currentWord);

            //check for a verb modifier
            if (currentWord < parsedName.Size())
            {
                if (PosData.IsAdverb(parsedName[currentWord].Text)
                    && parsedName[currentWord].Text.EndsWith("ly")
                    && !PosData.IsDeterminer(parsedName[currentWord].Text)
                    && !PosData.IsPronoun(parsedName[currentWord].Text)
                    && !PosData.IsPreposition(parsedName[currentWord].Text))
                {
                    if (currentWord + 1 < parsedName.Size() && PosData.IsPotentialVerb(parsedName[currentWord + 1].Text))
                    {
                        parsedName[currentWord].Tag = PartOfSpeechTag.VerbModifier;
                    }
                }
            }

            if (PosData.IsIgnorableVerb(parsedName[currentWord].Text))
            {
                parsedName[currentWord].Tag = PartOfSpeechTag.VerbIgnorable;
            }
            else
            {
                parsedName[currentWord].Tag = PartOfSpeechTag.Verb;
            }

            currentWord++;

            //check for verb particle
            if (currentWord < parsedName.Size())
            {
                if (PosData.IsVerbParticle(parsedName[currentWord - 1].Text, parsedName[currentWord].Text))
                {
                    parsedName[currentWord].Tag = PartOfSpeechTag.VerbParticle;
                }
            }

            //rest of words should be objects or prepositions
            if (currentWord < parsedName.Size())
            {
                int prep = FindFirstPreposition(parsedName, currentWord);
                if (prep == -1)
                {
                    PosTagger.TagNounPhrase(parsedName, currentWord, parsedName.Size() - 1);
                }
                else
                {
                    //found a preposition, could be VX?PY?(f?)
                    bool noX = false;
                    bool noY = false;
                    if (currentWord == prep)
                    {
                        noX = true;
                    }
                    else
                    {
                        PosTagger.TagNounPhrase(parsedName, currentWord, prep - 1);
                    }
                    currentWord = prep + 1;
                    if (currentWord >= parsedName.Size())
                    {
                        noY = true;
                    }
                    else
                    {
                        PosTagger.TagNounPhrase(parsedName, currentWord, parsedName.Size() - 1);
                    }
                }
            }
        }

        /// <summary>
        /// Sets the Action and Theme properties of the given MethodDeclarationNode using the default algorithms.
        /// This also sets the node's SecondaryArguments and UnknownArguments properties.
        /// </summary>
        /// <param name="node">The MethodDeclarationNode to set the Action and Theme on.</param>
        protected void SetDefaultActionAndTheme(MethodDeclarationNode node)
        {
            node.Action = GetVerbPhrase(node.ParsedName, node.Preamble);
            SetPrepositionThemeAndArguments(node);
        }

        /// <summary>
        /// Determines whether the specified word in the given phrase is an ignorable verb. If so, it tags it appropriately.
        /// </summary>
        /// <param name="parsedName">The PhraseNode containing the word to check.</param>
        /// <param name="wordIndex">The index of the desired word within the PhraseNode.</param>
        /// <returns>wordIndex+1 if the word was an ignorable verb; wordIndex if it was not.</returns>
        private int CheckForIgnorableVerb(PhraseNode parsedName, int wordIndex)
        {
            if (wordIndex < parsedName.Size() - 1 //make sure last word in name is verb
                && (PosData.IsIgnorableVerb(parsedName[wordIndex].Text)
                    && (PositionalFrequencies.GetVerbProbability(parsedName[wordIndex + 1].Text) > PositionalFrequencies.GetNounProbability(parsedName[wordIndex + 1].Text))
                    || PosData.IsModalVerb(parsedName[wordIndex].Text))
                )
            {
                parsedName[wordIndex].Tag = PartOfSpeechTag.VerbIgnorable;
                wordIndex++;
            }
            return wordIndex;
        }        
        
        /// <summary>
        /// Finds the index of the first preposition within the given PhraseNode, starting from the word indicated by startIndex.
        /// </summary>
        /// <param name="parsedName">The PhraseNode to search.</param>
        /// <param name="startIndex">The index of the word to start searching for prepositions from.</param>
        /// <returns>The index of the first preposition in the PhraseNode after startIndex, inclusively.</returns>
        private int FindFirstPreposition(PhraseNode parsedName, int startIndex)
        {
            for (int i = startIndex; i < parsedName.Size(); i++)
            {
                if (parsedName[i].Tag == PartOfSpeechTag.Preposition) { return i; }
            }
            return -1;
        }

        /// <summary>
        /// Finds all the verbs in the given name and adds them to the given preamble.
        /// </summary>
        /// <param name="parsedName">The PhraseNode to gets the verbs from.</param>
        /// <param name="preamble">The preamble PhraseNode to add the verbs to.</param>
        /// <returns>The preamble PhraseNode with the verbs added.</returns>
        private PhraseNode GetVerbPhrase(PhraseNode parsedName, PhraseNode preamble)
        {
            //TODO: should this make a copy of the preamble first?
            PhraseNode phrase = preamble;
            foreach (WordNode word in parsedName.GetPhrase())
            {
                if (word.Tag == PartOfSpeechTag.Verb
                    || word.Tag == PartOfSpeechTag.VerbModifier
                    || word.Tag == PartOfSpeechTag.VerbParticle
                    || word.Tag == PartOfSpeechTag.NonVerb
                    || word.Tag == PartOfSpeechTag.VerbIgnorable)
                {
                    phrase.Add(word);
                }
            }
            return phrase;
        }

        /// <summary>
        /// Returns a PhraseNode containing the noun phrase words from the given name.
        /// All noun phrase words prior to the first preposition are included.
        /// </summary>
        /// <param name="parsedName">The PhraseNode to get the noun phrase from.</param>
        private PhraseNode GetNounPhrase(PhraseNode parsedName)
        {
            return GetNounPhrase(parsedName, 0);
        }

        /// <summary>
        /// Returns a PhraseNode containing the noun phrase words from the given name, starting from startIndex.
        /// All noun phrase words prior to the first encountered preposition are included.
        /// </summary>
        /// <param name="parsedName">The PhraseNode to get the noun phrase from.</param>
        /// <param name="startIndex">The index of the word to start from.</param>
        private PhraseNode GetNounPhrase(PhraseNode parsedName, int startIndex)
        {
            PhraseNode phrase = parsedName.GetNewEmpty();
            for (int i = startIndex; i < parsedName.Size(); i++)
            {
                PartOfSpeechTag tag = parsedName[i].Tag;
                if (tag == PartOfSpeechTag.Noun
                    || tag == PartOfSpeechTag.NounModifier
                    || tag == PartOfSpeechTag.Determiner
                    || tag == PartOfSpeechTag.Pronoun
                    || tag == PartOfSpeechTag.NounIgnorable
                    || tag == PartOfSpeechTag.Digit
                    || tag == PartOfSpeechTag.Preamble)
                {
                    phrase.Add(parsedName[i]);
                }
                else if (tag == PartOfSpeechTag.Preposition)
                {
                    break;
                }
            }
            return phrase;
        }

        /// <summary>
        /// Sets the Theme, SecondaryArguments and UnknownArguments properties of the given MethodDeclarationNode.
        /// </summary>
        /// <param name="node">The node to set the properties for.</param>
        private void SetPrepositionThemeAndArguments(MethodDeclarationNode node)
        {
            List<VariableDeclarationNode> unusedArgs = new List<VariableDeclarationNode>();
            //populate UnknownArgs
            if (node.ParsedName.Size() > 0 && BooleanArgumentVerbs.Contains(node.ParsedName[0].Text))
            {
                node.AddUnknownArguments(node.FormalParameters);
            }
            else
            {
                //only add non-boolean arguments
                foreach (VariableDeclarationNode arg in node.FormalParameters)
                {
                    if (arg.Type.Name.ToLower().Contains("bool"))
                    {
                        unusedArgs.Add(arg);
                    }
                    else
                    {
                        node.AddUnknownArgument(arg);
                    }
                }
            }

            int prepIndex = FindFirstPreposition(node.ParsedName, 0);
            PhraseNode nameTheme = GetNounPhrase(node.ParsedName);
            bool checkDO = false; //check Direct Object in name for overlap
            bool checkIO = false; //check Indirect Object in name for overlap

            //Assign args
            if (prepIndex > -1)
            {
                //There's a preposition in the name
                WordNode prep = node.ParsedName[prepIndex];
                PhraseNode indirectObject = GetNounPhrase(node.ParsedName, prepIndex + 1);

                //set IO or P->NM
                if (!indirectObject.IsEmpty()) //IO in name
                {
                    node.AddSecondaryArgument(indirectObject, prep);
                    checkIO = true;
                }
                else if (node.UnknownArguments != null && node.UnknownArguments.Count() > 0) //or IO = f
                {
                    node.AddSecondaryArgument(node.UnknownArguments[0], prep);
                }
                else
                {
                    //The preposition doesn't seem to have an object, so change it to a NounModifier
                    prep.Tag = PartOfSpeechTag.NounModifier;
                    nameTheme = GetNounPhrase(node.ParsedName); //reset name theme after changing prep POS tag
                }

                //set Theme
                if (!nameTheme.IsEmpty())
                {
                    //theme is in the name
                    nameTheme.SetLocation(Location.Name);
                    node.Theme = nameTheme;
                    checkDO = true;
                }
                else //use class as theme
                {
                    node.Theme = node.DeclaringClass;
                }
            }
            else
            {
                //no prep in name, so set Theme only
                if (!nameTheme.IsEmpty())
                {
                    //theme is in the name
                    nameTheme.SetLocation(Location.Name);
                    node.Theme = nameTheme;
                    checkDO = true;
                }
                else
                {
                    //theme is first UnknownArg, or class name
                    //also, potentially leaves class on list of unknown args, which is intentional
                    if (node.DeclaringClass != null)
                    {
                        node.AddUnknownArgument(node.DeclaringClass);
                    }
                    if (node.UnknownArguments != null && node.UnknownArguments.Count > 0)
                    {
                        node.Theme = node.UnknownArguments[0];
                        node.UnknownArguments.RemoveAt(0);
                    }
                }
            }

            //find equivalences
            if ((checkDO || checkIO) && node.UnknownArguments != null && node.UnknownArguments.Count > 0)
            {
                CheckOverlap(node, checkDO, checkIO);
            }

            //do cleanup
            node.AddUnknownArguments(unusedArgs);
            if(node.ReturnType != null && node.ReturnType.Name.ToLower() != "void") {
                //TODO: should this be done for primitive return types? SetDefaultUnknownArguments() excludes them
                node.AddUnknownArgument(node.ReturnType);
            }

            // Note: adding class as unknown arg indep of checkIO
            // if checkIO = true, and not checkDO, then DO will be class
            // if check IO = true and checkDO, then this will be executed
            // if no prep & DO not in name, class will already be on unused args
            // list for finding theme.
            if (checkDO && node.DeclaringClass != null)
            {
                node.AddUnknownArgument(node.DeclaringClass);
            }
        }

        /// <summary>
        /// Checks for semantic overlaps between parts of the given method's name and its UnknownArguments.
        /// If overlaps are found, appropriate EquivalenceNodes are created.
        /// </summary>
        /// <param name="mdn">The MethodDeclarationNode to check for overlaps.</param>
        /// <param name="checkDO">Indicates whether the Direct Object was taken from the method name, and therefore the Theme must be checked for overlap with UnknownArguments.</param>
        /// <param name="checkIO">Indicates whether the Indirect Object was taken from the method name, and therefore the SecondaryArguments must be checked for overlap with UnknownArguments.</param>
        private void CheckOverlap(MethodDeclarationNode mdn, bool checkDO, bool checkIO)
        {
            if (mdn.ParsedName[0].Text.ToLower() == "set") 
            { 
                return; //special case
            }

            PhraseNode theme = null;
            ArgumentNode arg = null;

            //get DO word from name
            string wordDO = "";
            if (checkDO) //theme is in the method name
            {
                theme = (PhraseNode)mdn.Theme;
                wordDO = theme.LastWord().Text;
            }

            //get IO word from name
            string wordIO = "";
            if (checkIO) //IO is in the method name
            {
                arg = mdn.SecondaryArguments[0];
                PhraseNode argn = (PhraseNode)arg.Argument;
                wordIO = argn.LastWord().Text;
                if (wordDO == wordIO)
                {
                    return; //no equivalence if multiple overlap
                }
            }

            //find overlap
            List<Node> unknownArgs = mdn.UnknownArguments;
            List<Node> DOOverlappingArgs = new List<Node>();
            List<Node> IOOverlappingArgs = new List<Node>();
            for (int i = 0; i < unknownArgs.Count; i++)
            {
                if (unknownArgs[i] is VariableDeclarationNode)
                {
                    VariableDeclarationNode var = (VariableDeclarationNode)unknownArgs[i];
                    PhraseNode name = var.ParsedName;
                    PhraseNode type = var.Type.ParsedName;
                    bool DOOverlaps = false;
                    bool IOOverlaps = false;

                    if (checkDO)
                    {
                        DOOverlaps = HasOverlap(name, wordDO) || HasOverlap(type, wordDO);
                        if (DOOverlaps) { DOOverlappingArgs.Add(unknownArgs[i]); }
                    }
                    if (checkIO)
                    {
                        IOOverlaps = HasOverlap(name, wordIO) || HasOverlap(type, wordIO);
                        if (IOOverlaps) { IOOverlappingArgs.Add(unknownArgs[i]); }
                    }

                    if (DOOverlaps && IOOverlaps)
                    {
                        return; //no equivalence if multiple overlap
                    }
                }
            }

            //Create overlap in SWUM
            if (DOOverlappingArgs.Count > 0)
            {
                EquivalenceNode en = mdn.CreateEquivalenceFromUnknownArguments(theme, DOOverlappingArgs);
                mdn.Theme = en; //reset theme in MethodDeclarationNode to new equivalence node
            }
            if (IOOverlappingArgs.Count > 0)
            {
                EquivalenceNode en = mdn.CreateEquivalenceFromUnknownArguments(arg.Argument, IOOverlappingArgs);
                arg.Argument = en; //reset mdn.SecondaryArguments to point to new equivalence node
            }
        }

        /// <summary>
        /// Determines whether the given PhraseNode overlaps with the given word.
        /// The two overlap if the last word of the phrase is the same as the given word, 
        /// or if the second-to-last word of the phrase is the same as the given word and the last word of the phrase is ignorable.
        /// </summary>
        /// <param name="name">The phrase to check for overlap.</param>
        /// <param name="word">The word to check for overlap with.</param>
        /// <returns>True if the phrase and word overlap, False otherwise.</returns>
        private bool HasOverlap(PhraseNode name, string word)
        {
            if (name == null || name.Size() == 0 || string.IsNullOrEmpty(word)) { return false; }

            bool hasOverlap = false;
            if (string.Equals(name.LastWord().Text, word, StringComparison.InvariantCultureIgnoreCase))
            {
                //last word of name is same as given word
                hasOverlap = true;
            }
            else if (name.Size() > 1)
            {
                if (string.Equals(name[name.Size() - 2].Text, word, StringComparison.InvariantCultureIgnoreCase) 
                    && PosData.IsIgnorableHeadWord(name.LastWord().Text))
                {
                    //second-to-last word of name is same as given word, and the last word of name is ignorable
                    hasOverlap = true;
                }
            }

            return hasOverlap;
        }

        /// <summary>
        /// Assigns part-of-speech tags, and sets Action, Theme and Arguments for methods that have a reactive name.
        /// </summary>
        /// <param name="mdn">The MethodDeclarationNode to parse.</param>
        protected void ParseReactiveName(MethodDeclarationNode mdn)
        {
            //this assumes that the name has already been split and the preamble stripped

            mdn.Action = mdn.ParsedName[0].GetNewWord("handle", PartOfSpeechTag.Verb);
            this.PosTagger.TagNounPhrase(mdn.ParsedName);
            mdn.CreateThemeFromPhrases(mdn.Preamble, mdn.ParsedName);
            SetDefaultUnknownArguments(mdn);
            mdn.IsReactive = true;
        }

        /// <summary>
        /// Sets the UnknownArguments list to its default state. This includes all the formal parameters, the declaring class, and the return type (if not primitive).
        /// </summary>
        /// <param name="mdn">The MethodDeclarationNode to set UnknownArguments on.</param>
        protected void SetDefaultUnknownArguments(MethodDeclarationNode mdn)
        {
            mdn.ClearUnknownArguments();
            mdn.AddUnknownArguments(mdn.FormalParameters);
            if (mdn.DeclaringClass != null)
            {
                mdn.AddUnknownArgument(mdn.DeclaringClass);
            }
            if (mdn.ReturnType != null && !mdn.ReturnType.IsPrimitive)
            {
                mdn.AddUnknownArgument(mdn.ReturnType);
            }
        }

        /// <summary>
        /// Determines whether the given word indicates a checker method.
        /// </summary>
        /// <param name="word">The word to test.</param>
        /// <returns>True if the word indicats a checker method, False otherwise.</returns>
        protected bool IsChecker(string word)
        {
            if (word == null) { return false; }

            return PosData.IsThirdPersonSingularVerb(word) || PosData.IsThirdPersonIrregularVerb(word) || PosData.IsModalVerb(word);
        }

        /// <summary>
        /// Determines whether the given word indicates a method that needs special handling.
        /// </summary>
        /// <param name="word">The word to test.</param>
        /// <returns>True if the word indicats a method that need special handling, False otherwise.</returns>
        protected bool IsSpecialCase(string word)
        {
            //TODO: Is the second clause even possible in non-Java languages?
            return SpecialWords.Contains(word) || Regex.IsMatch(word, "^[0-9].*");
        }

        /// <summary>
        /// Determines whether the given phrase indicates an event handler method.
        /// </summary>
        /// <param name="parsedName">The PhraseNode to test.</param>
        /// <returns>True if the phrase indicates an event handler method, False otherwise.</returns>
        protected bool IsEventHandler(PhraseNode parsedName)
        {
            if (parsedName == null || parsedName.Size() == 0)
            {
                return false;
            }
            else
            {
                return IsNonBaseVerb(parsedName.LastWord().Text)
                    && parsedName[0].Text.ToLower() != "get"
                    && parsedName[0].Text.ToLower() != "set";
            }
        }

        /// <summary>
        /// Determines whether a method is an event handler based upon its formal parameters.
        /// </summary>
        /// <param name="formalParameters">The formal parameters to test.</param>
        /// <returns>True if the parameters indicate an event handler method, False otherwise.</returns>
        protected bool IsEventHandler(IEnumerable<VariableDeclarationNode> formalParameters)
        {
            if (formalParameters == null) { return false; }

            foreach (VariableDeclarationNode vdn in formalParameters) {
                WordNode last = vdn.Type.ParsedName.LastWord();
                if (last != null && last.Text != null && last.Text.ToLower() == "event")
                {
//TODO: is this sufficient for non-Java languages?
//for C#, should match against EventArgs
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Determines whether the given word is a non-base verb, i.e. present/past participle or past tense.
        /// </summary>
        /// <param name="word">The word to test.</param>
        /// <returns>True if the word is a non-base verb, False otherwise.</returns>
        protected bool IsNonBaseVerb(string word)
        {
            if (word == null) { return false; }

            return PosData.IsPresentParticiple(word) //-ing
                || PosData.IsPastTense(word) //-ed
                || PosData.IsPastParticiple(word); //-en
        }

        /// <summary>
        /// Determines whether the given word indicates the beginning of a noun phrase.
        /// </summary>
        /// <param name="word">The word to test.</param>
        /// <returns>True if the word indicates the beginning of a noun phrase, False otherwise.</returns>
        protected bool StartsNounPhrase(string word)
        {
            if (word == null) { return false; }

            return NounPhraseIndicators.Contains(word)
                || PosData.IsNoun(word)
                || PosData.IsDeterminer(word)
                || PosData.IsPronoun(word)
                || PosData.IsAdjective(word);
        }
    }
}
