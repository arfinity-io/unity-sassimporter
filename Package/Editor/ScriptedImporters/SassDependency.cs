//
// Copyright (c) arfinity GmbH and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for details.
//
using System.ComponentModel;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Arfinity.Libraries.Unity.Importers.SassImporter.Editor.ScriptedImporters
{
    /// <summary>
    /// Class that initializes the dependency on a sass executable and concrete version
    /// </summary>
    public static class SassDependency 
    {
        /// <summary>
        /// Variable to indicate whether we were even able to find sass at all
        /// </summary>
        internal static bool SassFound = false;
        
        /// <summary>
        /// Reads out the sass version from the sass executable (if any).
        /// </summary>
        [InitializeOnLoadMethod]
        static void GetSassDependency()
        {
            try
            {
                var processStartInfo = new ProcessStartInfo()
                {
                    FileName = Utilities.Utilities.GetSassPath(),
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    Arguments = $"--version"
                };

                var process = Process.Start(processStartInfo);

                // Read (and empty) output buffer. Otherwise the process gets stuck and waits for its output to be read for
                // large outputs
                // https://stackoverflow.com/questions/439617/hanging-process-when-run-with-net-process-start-whats-wrong
                string stdout = "";
                while (process != null && !process.StandardOutput.EndOfStream)
                {
                    stdout += process.StandardOutput.ReadLine();
                }

                process?.WaitForExit();
                stdout += process?.StandardOutput.ReadToEnd();
                
                // Set the version to the result of the executable. Change in version triggers reimport of scss files.
                AssetDatabase.RegisterCustomDependency("io.arfinity.unity.packages.sassimporter/sass-version", Hash128.Compute(stdout));
                SassFound = true;
            }
            catch (Win32Exception exception)
            {
                Debug.LogError("Executable named sass not found on path");
                SassFound = false;
                // If the execution fails dependency is still set so that a reimport is triggered when it is available again.
                AssetDatabase.RegisterCustomDependency("io.arfinity.unity.packages.sassimporter/sass-version", Hash128.Compute("failed"));
            }
        }
    }
}