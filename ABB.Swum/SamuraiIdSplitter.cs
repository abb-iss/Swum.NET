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
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Configuration;
using ABB.Swum.Utilities;
using ABB.SrcML;

namespace ABB.Swum
{
    /// <summary>
    /// Used to split the identifiers in a program into their constituent words.
    /// </summary>
    public class SamuraiIdSplitter : IdSplitter
    {
        private ConservativeIdSplitter CamelSplitter;
        private Dictionary<string, int> ProgramWordCount;
        private double LogProgramTotalWordCount;
        private Dictionary<string, double> GlobalWordCount;
        private HashSet<string> Prefixes;
        private HashSet<string> Suffixes;

        private bool IncludeIdentifier(string word, int count)
        {
            return (word.Length > 1) && (count > 2) && !Regex.IsMatch(word, @"^\d+$");
        }

        ///// <summary>
        ///// Creates a new IdentifierSplitter using the default location for program word count data.
        ///// </summary>
        //public SamuraiIdSplitter()
        //{
        //    //use default location for programWordCount
        //    //call Initialize()
        //}

        /// <summary>
        /// Creates a new IdentifierSplitter using the specified program word count file.
        /// </summary>
        /// <param name="programWordCountPath">The path to the file containing the local program word counts.</param>
        public SamuraiIdSplitter(string programWordCountPath)
        {
            Initialize(LibFileLoader.ReadWordCount(programWordCountPath, false, IncludeIdentifier));
        }

        /// <summary>
        /// Creates a new IdentifierSplitter using the specified word count dictionary.
        /// </summary>
        /// <param name="programWordCount">A dictionary containing the local program word counts.</param>
        public SamuraiIdSplitter(Dictionary<string, int> programWordCount)
        {
            Initialize(programWordCount);
        }

        /// <summary>
        /// Reads the necessary data files and initializes the member variables.
        /// </summary>
        /// <param name="programWordCount">A dictionary containing the word counts from the local program.</param>
        private void Initialize(Dictionary<string, int> programWordCount)
        {
            this.CamelSplitter = new ConservativeIdSplitter();

            //set ProgramWordCount and calculate log of total
            this.ProgramWordCount = programWordCount;
            ulong ProgramTotalWordCount = 0;
            foreach (int value in this.ProgramWordCount.Values)
            {
                ProgramTotalWordCount = ProgramTotalWordCount + (ulong)value;
            }
            this.LogProgramTotalWordCount = Math.Log10(ProgramTotalWordCount);
            
            //load globalWordCount from default location
            var rawGlobalWordCount = LibFileLoader.ReadWordCount(SwumConfiguration.GetFileSetting("SamuraiIdSplitter.GlobalWordCountFile"), false, IncludeIdentifier);
            this.GlobalWordCount = new Dictionary<string, double>();
            //add weighting to word counts
            foreach (var kvp in rawGlobalWordCount)
            {
                this.GlobalWordCount[kvp.Key] = kvp.Value * Math.Pow((double)kvp.Key.Length - 1, 1.5);
            }

            //read prefix and suffix lists from default locations
            //TODO: the words must be in lowercase. Should we lowercase them on loading, or just assume/require that they're in lowercase in the file?
            this.Prefixes = LibFileLoader.ReadWordList(SwumConfiguration.GetFileSetting("SamuraiIdSplitter.Prefixesfile"));
            this.Suffixes = LibFileLoader.ReadWordList(SwumConfiguration.GetFileSetting("SamuraiIdSplitter.Suffixesfile"));
        }


        /// <summary>
        /// Splits a word where an uppercase letter is followed by a lowercase letter. The word is split only once, at the first matching location.
        /// This method assumes the input consists of zero-or-more uppercase letters followed by zero-or-more lowercase letters.
        /// </summary>
        /// <param name="word">The word to be split.</param>
        /// <returns>An array of the subwords resulting from the split.</returns>
        private string[] SplitOnUppercaseToLowercase(string word)
        {
            List<string> splitWords = new List<string>();
            for (int i = 0; i < word.Length-1; i++)
            {
                if (char.IsUpper(word, i) && char.IsLower(word, i + 1))
                {
                    //found split point
                    double camelScore = Score(word.Substring(i)); //score for camelcase split, e.g. AST Visitor
                    double altScore = Score(word.Substring(i + 1)); //score for alternate split, e.g. ASTV isitor

//TODO: should this be >=? That way, if both alternatives have the same score, then camelcase is selected
//      This occurs most often when neither word is in the dictionaries, i.e. score = 0
                    if (camelScore >= Math.Sqrt(altScore))
                    {
                        //use camel split
                        if (i > 0) 
                        {
                            //don't add if only one uppercase at beginning of word, e.g. "Hourly"
                            splitWords.Add(word.Substring(0, i)); 
                        }
                        splitWords.Add(word.Substring(i));
                    }
                    else
                    {
                        //use alternate split
                        splitWords.Add(word.Substring(0, i + 1));
                        splitWords.Add(word.Substring(i + 1));
                    }
                    break; //there is only one uppercase-to-lowercase split per word (as per precondition), so don't look any further
                }
            }

            //if no split was found, add the original word
            if(splitWords.Count == 0)
                splitWords.Add(word);

            return splitWords.ToArray();
        }

        private double Score(string word)
        {
            int programCount;
            double globalCount;
            try
            {
                programCount = ProgramWordCount[word.ToLower()];
            } 
            catch(KeyNotFoundException) 
            {
                programCount = 0;
            }
            try
            {
                globalCount = GlobalWordCount[word.ToLower()];
            }
            catch (KeyNotFoundException)
            {
                globalCount = 0;
            }

            return programCount + globalCount / LogProgramTotalWordCount;
            //return programCount;
        }

        /// <summary>
        /// Checks whether the supplied word is a known prefix.
        /// </summary>
        /// <param name="word">The word to check.</param>
        /// <returns>True if the word is a prefix, False otherwise.</returns>
        private bool IsPrefix(string word)
        {
            return Prefixes.Contains(word.ToLower());
        }

        /// <summary>
        /// Checks whether the supplied word is a known suffix.
        /// </summary>
        /// <param name="word">The word to check.</param>
        /// <returns>True if the word is a suffix, False otherwise.</returns>
        private bool IsSuffix(string word)
        {
            return Suffixes.Contains(word.ToLower());
        }

        /// <summary>
        /// Splits a word into subwords. 
        /// The word should be either (1) all lowercase, (2) all uppercase, or (3) a single uppercase letter followed by lowercase letters
        /// </summary>
        /// <param name="word">The word to split.</param>
        /// <returns>An array of the subwords resulting from the split.</returns>
        private string[] SplitSameCase(string word)
        {
            return SplitSameCase(word, Score(word));
        }

        /// <summary>
        /// Splits a word into subwords.
        /// The word should be either (1) all lowercase, (2) all uppercase, or (3) a single uppercase letter followed by lowercase letters
        /// </summary>
        /// <param name="word">The word to split.</param>
        /// <param name="noSplitScore">The word's score before any splitting.</param>
        /// <returns>An array of the subwords resulting from the split.</returns>
        private string[] SplitSameCase(string word, double noSplitScore)
        {
            List<string> bestSplit = new List<string>();
            bestSplit.Add(word);
            double maxScore = -1;

            for (int i = 0; i < word.Length; i++)
            {
                string left = word.Substring(0, i + 1);
                string right = word.Substring(i + 1);
                double scoreLeft = Score(left);
                double scoreRight = Score(right);
                bool prefix = IsPrefix(left) || IsSuffix(right);

                bool splitLeft = (Math.Sqrt(scoreLeft) > Math.Max(Score(word), noSplitScore));
                bool splitRight = (Math.Sqrt(scoreRight) > Math.Max(Score(word), noSplitScore));

                if (!prefix && splitLeft && splitRight)
                {
                    if (scoreLeft + scoreRight > maxScore)
                    {
                        //found new best split
                        maxScore = scoreLeft + scoreRight;
                        bestSplit.Clear();
                        bestSplit.Add(left);
                        bestSplit.Add(right);
                    }
                }
                else if (!prefix && splitLeft)
                {
                    string[] furtherSplit = SplitSameCase(right, noSplitScore);
                    if (furtherSplit.Length > 1)
                    {
                        bestSplit.Clear();
                        bestSplit.Add(left);
//                        maxScore = scoreLeft; //Emily doesn't set maxScore here, but shouldn't it be?
                        foreach (string w in furtherSplit)
                        {
                            bestSplit.Add(w);
//                            maxScore += Score(w); //Emily doesn't set maxScore here, but shouldn't it be?
                        }
                    }
                }
            }

            return bestSplit.ToArray();
        }

        /// <summary>
        /// Splits a program identifier into its constituent words.
        /// </summary>
        /// <param name="identifier">The identifier to split.</param>
        /// <returns>An array of the words resulting from the split.</returns>
        public override string[] Split(string identifier)
        {
            return Split(identifier, false);
        }

        /// <summary>
        /// Splits a program identifier into its constituent words.
        /// </summary>
        /// <param name="identifier">The identifier to split.</param>
        /// <param name="printSplitTrace">Whether or not to print a trace of the splitting process.</param>
        /// <returns>An array of the words resulting from the split.</returns>
        public string[] Split(string identifier, bool printSplitTrace)
        {
            if (printSplitTrace) { Console.WriteLine(identifier); }

            List<string> splitWords = new List<string>();

            foreach (string word in CamelSplitter.Split(identifier))
            {
                if (printSplitTrace) { Console.WriteLine("\t{0}", word); }
                foreach (string word2 in SplitOnUppercaseToLowercase(word))
                {
                    if (printSplitTrace) { Console.WriteLine("\t\t{0}", word2); }
                    foreach (string word3 in SplitSameCase(word2))
                    {
                        if (printSplitTrace) { Console.WriteLine("\t\t\t{0}", word3); }
                        splitWords.Add(word3);
                    }
                }
            }

            return splitWords.ToArray();
        }

        /// <summary>
        /// Counts the number of occurrences of words within the identifiers in the given srcml files.
        /// </summary>
        /// <param name="archive">An archive containing the srcml files to analyze.</param>
        /// <returns>A dictionary mapping words to the number of occurrences within identifiers.</returns>
        public static Dictionary<string,int> CountProgramWords(SrcMLArchive archive)
        {
            if(archive == null) {
                throw new ArgumentNullException("archive");
            }
            
            var splitter = new ConservativeIdSplitter();
            var observations = new Dictionary<string, int>();
            foreach (var fileUnit in archive.FileUnits)
            {
                //query for all the identifiers
                var identifiers = from id in fileUnit.Descendants(SRC.Name)
                                  where !id.Elements().Any()
                                  select id.Value;

                foreach (var id in identifiers)
                {
                    string[] words = splitter.Split(id);
                    foreach (string word in words)
                    {
                        int obs;
                        string lowWord = word.ToLower();
                        observations.TryGetValue(lowWord, out obs); //gets the number of observations for the word. If it is new, obs is set to 0
                        observations[lowWord] = obs + 1;
                    }
                }
            }

            return observations;
        }

    }
}
