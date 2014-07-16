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
using System.IO;
using ABB.Swum.WordData;
using System.Globalization;

namespace ABB.Swum.Utilities
{
    /// <summary>
    /// Contains various utility functions for loading library data files.
    /// </summary>
    public static class LibFileLoader
    {
        /// <summary>
        /// Writes a word count dictionary to a file. Each dictionary entry is written in the format "[count] [word]".
        /// </summary>
        /// <param name="wordCount">The word count dictionary to write.</param>
        /// <param name="path">The file to write to.</param>
        public static void WriteWordCount(Dictionary<string, int> wordCount, string path)
        {
            using (StreamWriter file = new StreamWriter(path, false, Encoding.UTF8))
            {
                foreach (var kvp in wordCount.OrderBy(v => v.Key))
                {
                    file.WriteLine("{0} {1}", kvp.Value, kvp.Key);
                }
            }
        }

        /// <summary>
        /// Reads a word count file and parses it into a dictionary. The file is assumed to contain 
        /// one entry per line, with each entry of the format "[count] [word]". Count must be a non-negative whole number.
        /// If the file contains duplicate entries for a given word, only the last entry will be in the dictionary.
        /// Words are converted to lower case before being added to the dictionary.
        /// </summary>
        /// <param name="path">The file to read.</param>
        /// <returns>A Dictionary mapping words to counts.</returns>
        public static Dictionary<string, int> ReadWordCount(string path)
        {
            return ReadWordCount(path, false);
        }

        /// <summary>
        /// Reads a word count file and parses it into a dictionary. The file is assumed to contain 
        /// one entry per line, with each entry of the format "[count] [word]". Count must be a non-negative whole number.
        /// If the file contains duplicate entries for a given word, only the last entry will be in the dictionary.
        /// Words are converted to lower case before being added to the dictionary.
        /// </summary>
        /// <param name="path">The file to read.</param>
        /// <param name="keepOriginalCasing">If True, words are added to the dictionary in their original case. If False, words are converted to lower case.</param>
        /// <returns>A Dictionary mapping words to counts.</returns>
        public static Dictionary<string, int> ReadWordCount(string path, bool keepOriginalCasing)
        {
            return ReadWordCount(path, keepOriginalCasing, (s, i) => true);
        }

        /// <summary>
        /// Reads a word count file and parses it into a dictionary. The file is assumed to contain 
        /// one entry per line, with each entry of the format "[count] [word]". Count must be a non-negative whole number.
        /// If the file contains duplicate entries for a given word, only the last entry will be in the dictionary.
        /// </summary>
        /// <param name="path">The file to read.</param>
        /// <param name="keepOriginalCase">If True, words are added to the dictionary in their original case. If False, words are converted to lower case.</param>
        /// <param name="includeFunction">A function specifying whether or not to include a given entry from the file. 
        /// This takes a string and uint as parameters and returns True if the entry should be included and False otherwise.</param>
        /// <returns>A Dictionary mapping words to counts.</returns>
        public static Dictionary<string, int> ReadWordCount(string path, bool keepOriginalCase, Func<string, int, bool> includeFunction)
        {
            Dictionary<string, int> obs = new Dictionary<string, int>();
            using (StreamReader file = new StreamReader(path))
            {
                char[] space = { ' ' }; //the delimeter between the count and word fields
                while (!file.EndOfStream)
                {
                    string entry = file.ReadLine().Trim();
                    if (!entry.StartsWith("#") && !entry.StartsWith("//")) //ignore commented lines
                    {
                        //the format of an entry is "<count> <word>"
                        string[] parts = entry.Split(space, 2);
                        if (parts.Length == 2)
                        {
                            int count = int.Parse(parts[0]);
                            if (includeFunction(parts[1], count))
                            {
                                //add entry to dictionary
                                if (keepOriginalCase)
                                {
                                    obs[parts[1]] = count;
                                }
                                else
                                {
                                    obs[parts[1].ToLower()] = count;
                                }
                            }
                        }
                        else
                        {
                            //not a valid entry
                            Console.Error.WriteLine("Invalid entry found in {0}: {1}", path, entry);
                        }
                    }
                }
            }

            return obs;
        }

        /// <summary>
        /// Reads a file containing a list of words, and returns it as a set. The file is assumed 
        /// to contain a single word on each line. Duplicate words will be ignored. Words are converted to lower case before being added.
        /// </summary>
        /// <param name="paths">One or more files to read.</param>
        /// <returns>A HashSet containing the words in the file(s).</returns>
        public static HashSet<string> ReadWordList(params string[] paths)
        {
            return ReadWordList(false, paths);
        }

        /// <summary>
        /// Reads a file containing a list of words, and returns it as a set. The file is assumed 
        /// to contain a single word on each line. Duplicate words will be ignored.
        /// </summary>
        /// <param name="keepOriginalCase">If True, words are added to the set in their original case. If False, words are converted to lower case.</param>
        /// <param name="paths">One or more files to read.</param>
        /// <returns>A HashSet containing the words in the file(s).</returns>
        public static HashSet<string> ReadWordList(bool keepOriginalCase, params string[] paths)
        {
            var culture = CultureInfo.CurrentCulture;
            if (paths == null) throw new ArgumentNullException("paths");

            HashSet<string> wordList = new HashSet<string>();
            foreach (string path in paths)
            {
                using (StreamReader file = new StreamReader(path))
                {
                    string word;
                    while (!file.EndOfStream)
                    {
                        word = file.ReadLine().Trim();
                        if (word != string.Empty && word[0]!='#' && !(word.Length>=2 && word[0]=='/' && word[1]=='/')) //ignore blank and commented lines
                        {
                            if (keepOriginalCase)
                            {
                                wordList.Add(word);
                            }
                            else
                            {
                                wordList.Add(word.ToLower(culture));
                            }
                        }
                    }
                }
            }

            return wordList;
        }

        /// <summary>
        /// Reads a word stem file and parses it into a dictionary. The file is assumed to contain 
        /// one entry per line, with each entry of the format "[word] [stem]".
        /// If the file contains duplicate entries for a given word, only the last entry will be in the dictionary.
        /// </summary>
        /// <param name="path">The file to read.</param>
        /// <returns>A Dictionary mapping words to stems.</returns>
        public static Dictionary<string, string> ReadStemFile(string path)
        {
            Dictionary<string, string> obs = new Dictionary<string, string>();
            using (StreamReader file = new StreamReader(path))
            {
                char[] space = { ' ' }; //the delimeter between the fields
                while (!file.EndOfStream)
                {
                    string entry = file.ReadLine().Trim();
                    if (!string.IsNullOrEmpty(entry) && !entry.StartsWith("#") && !entry.StartsWith("//")) //ignore blank and commented lines
                    {
                        //the format of an entry is "<word> <stem>"
                        string[] parts = entry.Split(space, 2);
                        if (parts.Length == 2)
                        {
                            //add entry to dictionary
                            obs[parts[0]] = parts[1];
                        }
                        else
                        {
                            //not a valid entry
                            Console.Error.WriteLine("Invalid entry found in {0}: {1}", path, entry);
                        }
                    }
                }
            }

            return obs;
        }

        /// <summary>
        /// Reads the verb particle file at the given path and reads it into a Dictionary.
        /// Each line of the file should be in the format: [verb] [particle].
        /// The returned Dictionary maps a particle to a set of verbs.
        /// </summary>
        /// <param name="path">The verb particle file to read.</param>
        /// <returns>A Dictionary mapping a particle to a set of verbs.</returns>
        public static Dictionary<string, HashSet<string>> ReadVerbParticleFile(string path)
        {
            //maps particle -> {set of verbs}
            Dictionary<string, HashSet<string>> particleMap = new Dictionary<string, HashSet<string>>();

            using (StreamReader file = new StreamReader(path))
            {
                string entry;
                while (!file.EndOfStream)
                {
                    entry = file.ReadLine().Trim();
                    if (!string.IsNullOrEmpty(entry) && !entry.StartsWith("#") && !entry.StartsWith("//")) //ignore commented lines
                    {
                        //the format of an entry is <verb> <particle>
                        string[] parts = entry.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length == 2)
                        {
                            //found a valid entry
                            string verb = parts[0];
                            string particle = parts[1];

                            //if we haven't seen this particle yet, initialize the set
                            if (!particleMap.ContainsKey(particle))
                                particleMap[particle] = new HashSet<string>();

                            //add new verb
                            particleMap[particle].Add(verb);
                        }
                        else
                        {
                            //not a valid entry
                            Console.Error.WriteLine("Invalid entry found in {0}: {1}", path, entry);
                        }
                    }
                }
            }

            return particleMap;
        }

    }
}
