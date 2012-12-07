/******************************************************************************
 * Copyright (c) 2012 ABB Group
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the Eclipse Public License v1.0
 * which accompanies this distribution, and is available at
 * http://www.eclipse.org/legal/epl-v10.html
 *
 * Contributors:
 *    Patrick Francis (ABB Group) - implementation & documentation
 *    Emily Hill (Univ. of Delaware) - Original algorithms and design
 *****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ABB.SrcML;

namespace ABB.Swum
{
    /// <summary>
    /// Expands abbreviations found within programs to their long forms.
    /// </summary>
    public class AbbreviationExpander
    {
        private enum SingleWordPattern
        {
            Prefix = 0,
            DroppedLetter
        }

        private enum MultiWordPattern
        {
            Acronym = 0,
            CombinationWord
        }

        private string[] ExpandToSingleWord(string shortForm, SingleWordPattern pattern, XElement methodXml, string methodComment, string classComment)
        {
            //construct the regular expression for the pattern
            string patternRegex;
            if (pattern == SingleWordPattern.Prefix)
            {
                if (shortForm[0] == 'x')
                {
                    patternRegex = string.Format(@"\be?{0}\w+\b", shortForm);
                }
                else
                {
                    patternRegex = string.Format(@"\b{0}\w+\b", shortForm);
                }
            }
            else if (pattern == SingleWordPattern.DroppedLetter)
            {
                StringBuilder sb = new StringBuilder(@"\b");
                for (int i = 0; i < shortForm.Length; i++)
                {
                    sb.AppendFormat(@"{0}\w*", shortForm[i]);
                }
                sb.Append(@"\b");
                patternRegex = sb.ToString();
            }
            else
            {
                throw new ArgumentException("Pattern must be a valid member of SingleWordPattern.", "pattern");
            }

            //look for potential long forms
            if ((pattern == SingleWordPattern.Prefix
                 || Regex.IsMatch(shortForm, @"^\w[^aeiou]+$", RegexOptions.IgnoreCase) //first letter may be a vowel, the rest are consonents
                 || shortForm.Length > 3)
                && !Regex.IsMatch(shortForm, @"^\w[aeiou][aeiou]+$", RegexOptions.IgnoreCase)) //don't want words that are mostly consecutive vowels, e.g. GUI
            {
                List<string> matches = new List<string>();
                bool foundMatch = false;

                //1. Search JavaDoc comments. We don't have these, so skip this step.
                //2. Search Type Names and variable names for "pattern sf"
                var decls = from decl in methodXml.Descendants(SRC.Declaration)
                            select decl;
                foreach (var decl in decls)
                {
                    //Console.WriteLine("Decl found: {0}", decl.Value);

                    Match m = Regex.Match(decl.Value, string.Format(@"({0})\s+{1}", patternRegex, shortForm), RegexOptions.IgnoreCase);
                    if(m.Success)
                    {
                        matches.Add(m.Groups[1].Value);
                        foundMatch = true;
                    }
                }

                //3. Search Method Name for pattern
                if (!foundMatch)
                {
                    Match m = Regex.Match(methodXml.Element(SRC.Name).Value, string.Format("({0})", patternRegex), RegexOptions.IgnoreCase);
                    if (m.Success)
                    {
                        matches.Add(m.Groups[1].Value);
                        foundMatch = true;
                    }
                }

                //4. Search method text for "pattern sf" and "sf pattern"
                //Emily says to search within each statement. However, it is equivalent to search within the whole method text at once because if pattern and sf 
                //are separated only by whitespace then they must be within the same statement.
                if (!foundMatch)
                {
                    string methodText = methodXml.Value;
                    Match m = Regex.Match(methodText, string.Format(@"({0})\s+{1}", patternRegex, shortForm), RegexOptions.IgnoreCase);
                    if (m.Success)
                    {
                        matches.Add(m.Groups[1].Value);
                        foundMatch = true;
                    }

                    m = Regex.Match(methodText, string.Format(@"{0}\s+({1})", shortForm, patternRegex), RegexOptions.IgnoreCase);
                    if (m.Success)
                    {
                        matches.Add(m.Groups[1].Value);
                        foundMatch = true;
                    }
                }

                //5. Search method words for "pattern"
                if (!foundMatch && shortForm.Length != 2)
                {
                    Match m = Regex.Match(methodXml.Value, string.Format("({0})", patternRegex), RegexOptions.IgnoreCase);
                    if (m.Success)
                    {
                        matches.Add(m.Groups[1].Value);
                        foundMatch = true;
                    }
                }

                //6. Search method comment words
                if (!foundMatch && shortForm.Length != 2)
                {
                    Match m = Regex.Match(methodComment, string.Format("({0})", patternRegex), RegexOptions.IgnoreCase);
                    if (m.Success)
                    {
                        matches.Add(m.Groups[1].Value);
                        foundMatch = true;
                    }
                }

                //7. Search class comment words
                if (!foundMatch && pattern == SingleWordPattern.Prefix && shortForm.Length > 1)
                {
                    Match m = Regex.Match(classComment, string.Format("({0})", patternRegex), RegexOptions.IgnoreCase);
                    if (m.Success)
                    {
                        matches.Add(m.Groups[1].Value);
                        foundMatch = true;
                    }
                }

                return matches.ToArray();
            }
            else
            {
                return new string[0];
            }
        }

        private string[] ExpandToMultipleWords(string shortForm, MultiWordPattern pattern, XElement methodXml, string methodComment, string classComment)
        {
            //construct the regular expression for the pattern
            string patternRegex;
            if (pattern == MultiWordPattern.Acronym)
            {
                StringBuilder sb = new StringBuilder(@"\b");
                for (int i = 0; i < shortForm.Length - 1; i++)
                {
                    sb.AppendFormat(@"{0}\w+\s+", shortForm[i]);
                }
                sb.AppendFormat(@"{0}\w+\b", shortForm[shortForm.Length - 1]);
                patternRegex = sb.ToString();
            }
            else if (pattern == MultiWordPattern.CombinationWord)
            {
                StringBuilder sb = new StringBuilder(@"\b");
                for (int i = 0; i < shortForm.Length - 1; i++)
                {
                    sb.AppendFormat(@"{0}\w*?\s*?", shortForm[i]);
                }
                sb.AppendFormat(@"{0}\w*?\b", shortForm[shortForm.Length - 1]);
                patternRegex = sb.ToString();
            }
            else
            {
                throw new ArgumentException("Pattern must be a valid member of MultiWordPattern.", "pattern");
            }

            //search for potential long forms
            if (pattern == MultiWordPattern.Acronym || shortForm.Length > 3)
            {
                List<string> matches = new List<string>();
                bool foundMatch = false;

                //1. Search JavaDoc comments. We don't have these, so skip this step.
                //2. Search Type Names and variable names for "pattern sf"
                var decls = from decl in methodXml.Descendants(SRC.Declaration)
                            select decl;
                foreach (var decl in decls)
                {
                    //Console.WriteLine("Decl found: {0}", decl.Value);

                    Match m = Regex.Match(decl.Value, string.Format(@"({0})\s+{1}", patternRegex, shortForm), RegexOptions.IgnoreCase);
                    if (m.Success)
                    {
                        matches.Add(m.Groups[1].Value);
                        foundMatch = true;
                    }
                }

                //3. Search Method Name for pattern
                if (!foundMatch)
                {
                    Match m = Regex.Match(methodXml.Element(SRC.Name).Value, string.Format("({0})", patternRegex), RegexOptions.IgnoreCase);
                    if (m.Success)
                    {
                        matches.Add(m.Groups[1].Value);
                        foundMatch = true;
                    }
                }

                //4. Search all identifiers in the method for "pattern" (including type names)
                if (!foundMatch)
                {
                    var nameQuery = from name in methodXml.Descendants(SRC.Name)
                                    select name;
                    foreach (XElement name in nameQuery)
                    {
                        Match m = Regex.Match(name.Value, string.Format("({0})", patternRegex), RegexOptions.IgnoreCase);
                        if (m.Success)
                        {
                            matches.Add(m.Groups[1].Value);
                            foundMatch = true;
                        }
                    }
                }

                //5. Search string literals for "pattern"
                if (!foundMatch)
                {
                    var stringQuery = from lit in methodXml.Descendants(LIT.Literal)
                                      where lit.Attribute("type").Value == "string"
                                      select lit;
                    foreach (XElement str in stringQuery)
                    {
                        Match m = Regex.Match(str.Value, string.Format("({0})", patternRegex), RegexOptions.IgnoreCase);
                        if (m.Success)
                        {
                            matches.Add(m.Groups[1].Value);
                            foundMatch = true;
                        }
                    }
                }

                //6. Search method comment words for "pattern"
                if (!foundMatch)
                {
                    Match m = Regex.Match(methodComment, string.Format("({0})", patternRegex), RegexOptions.IgnoreCase);
                    if (m.Success)
                    {
                        matches.Add(m.Groups[1].Value);
                        foundMatch = true;
                    }
                }

                //7. Search class comment words for "pattern"
                if (!foundMatch && pattern == MultiWordPattern.Acronym)
                {
                    Match m = Regex.Match(classComment, string.Format("({0})", patternRegex), RegexOptions.IgnoreCase);
                    if (m.Success)
                    {
                        matches.Add(m.Groups[1].Value);
                        foundMatch = true;
                    }
                }

                return matches.ToArray();
            }
            else
            {
                return new string[0];
            }
        }

        /// <summary>
        /// Expands the given abbreviation (short form) to its long form. Potential long forms are found by searching the program texts relevent to the short form.
        /// </summary>
        /// <param name="shortForm">The abbreviation to expand.</param>
        /// <param name="methodXml">An XElement corresponding to the srcML of the method where the shortForm is located.</param>
        /// <param name="methodComment">The summary comments for the method where the short form is located.</param>
        /// <param name="classComment">The summary comments for the class that the short form is located within.</param>
        /// <returns>An array of the words making up the long form of the abbreviation.</returns>
        public string[] Expand(string shortForm, XElement methodXml, string methodComment, string classComment)
        {
            //according to Emily, patterns should be applied in order: acronym, prefix, dropped letter, combination word
            string[] longForms;
            longForms = ExpandToMultipleWords(shortForm, MultiWordPattern.Acronym, methodXml, methodComment, classComment);
            Console.WriteLine("{1} matches from Acronym pattern: {0}", string.Join(", ", longForms), longForms.Length);

            longForms = ExpandToSingleWord(shortForm, SingleWordPattern.Prefix, methodXml, methodComment, classComment);
            Console.WriteLine("{1} matches from Prefix pattern: {0}", string.Join(", ", longForms), longForms.Length);

            longForms = ExpandToSingleWord(shortForm, SingleWordPattern.DroppedLetter, methodXml, methodComment, classComment);
            Console.WriteLine("{1} matches from Dropped Letter pattern: {0}", string.Join(", ", longForms), longForms.Length);

            longForms = ExpandToMultipleWords(shortForm, MultiWordPattern.CombinationWord, methodXml, methodComment, classComment);
            Console.WriteLine("{1} matches from Combination Word pattern: {0}", string.Join(", ", longForms), longForms.Length);

            return new string[1];
        }
    }
}
