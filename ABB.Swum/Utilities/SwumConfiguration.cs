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
using System.Configuration;
using System.Reflection;
using System.IO;

namespace ABB.Swum.Utilities
{
    /// <summary>
    /// Provides access to the settings specified in the SWUM configuration file.
    /// </summary>
    public static class SwumConfiguration
    {
        private static Configuration config;

        /// <summary>
        /// Reads the configuration file associated with the current assembly.
        /// </summary>
        static SwumConfiguration()
        {
            string assemblyPath;
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            if(!string.IsNullOrWhiteSpace(currentAssembly.CodeBase)) {
                assemblyPath = new Uri(currentAssembly.CodeBase).LocalPath;
            } else {
                assemblyPath = currentAssembly.Location;
            }
            
            config = ConfigurationManager.OpenExeConfiguration(assemblyPath);
        }

        /// <summary>
        /// Gets the value of the specified configuration setting.
        /// </summary>
        /// <param name="key">The name of the setting to retrieve.</param>
        /// <returns>The value associated with the specified key, or null if the key is not found in the configuration.</returns>
        public static string GetSetting(string key)
        {
            var setting = config.AppSettings.Settings[key];
            if (setting != null)
            {
                return setting.Value;
            }
            else
            {
                Console.Error.WriteLine("Setting {0} not found in configuration", key);
                if (config != null)
                {
                    Console.Error.WriteLine("Using configuration file: {0}", config.FilePath);
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the value of the specified configuration setting, which is assumed to be a file path. 
        /// If the path is relative, it is expanded to a full path, using the directory of the configuration file.
        /// </summary>
        /// <param name="key">The name of the setting to retrieve.</param>
        /// <returns>The value associated with the specified key, expanded to a full path. If the key is not found, this method returns null.</returns>
        public static string GetFileSetting(string key)
        {
            string filePath = GetSetting(key);
            if (filePath != null && !Path.IsPathRooted(filePath))
            {
                //relative path, so prepend with location of the config file
                filePath = Path.Combine(Path.GetDirectoryName(config.FilePath), filePath);
            }
            return filePath;
        }
    }
}
