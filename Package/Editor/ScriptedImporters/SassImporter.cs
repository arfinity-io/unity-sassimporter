//
// Copyright (c) arfinity GmbH and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for details.
//
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using UnityEditor.AssetImporters;
using UnityEngine;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;

namespace Arfinity.Libraries.Unity.Importers.SassImporter.Editor.ScriptedImporters
{
    /// <summary>
    /// The main scripted importer. Imports files of type scss by pushing them to an external sass executable that is
    /// optionally provided by the io.arfinity.unity.packages.sassexcutables package
    /// </summary>
    [ScriptedImporter(version: 13, exts: new []{"scss"}, importQueueOffset: 10)]
    public class SassImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            // Declare dependency on the detected sass version
            ctx.DependsOnCustomDependency("io.arfinity.unity.packages.sassimporter/sass-version");
            
            // If the dependency on sass was not found don't try to import as it will fail anyway
            if (!SassDependency.SassFound)
            {
                ctx.LogImportError($"No Sass available on path. Not importing: {ctx.assetPath}");
                return;
            }
            
            // If we don't have a path ignore
            if(ctx.assetPath == null) return;
            
            // Only transpile scss files not starting with an underscore which is by convention a partial scss file
            var fileName = Path.GetFileNameWithoutExtension(ctx.assetPath);
            if (fileName.StartsWith("_")) return;


            var fullPath = Path.GetFullPath(ctx.assetPath);

            // Prepare to start the external process
            var processStartInfo = new ProcessStartInfo() {
                FileName = Utilities.Utilities.GetSassPath(),
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                Arguments = $"\"{fullPath}\""
            };

            try
            {
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

                // This is some magic with reflection to access the internals of the UIToolkit package that imports uss
                Type T = typeof(UnityEditor.UIElements.Toolbar).Assembly
                    .GetType("UnityEditor.UIElements.StyleSheets.StyleSheetImporterImpl");

                MethodInfo method = T.GetMethod("Import");

                var styleSheet = ScriptableObject.CreateInstance<StyleSheet>();
                if (method == null)
                {
                    Debug.LogError("Unable to get Unity-internal USS Import() method.");
                    return;
                }

                // Import the transpiled output as a uss file
                method.Invoke(Activator.CreateInstance(T, new object[] { ctx }), new object[] { styleSheet, stdout });

                var stderr = process?.StandardError.ReadToEnd();
                if (!string.IsNullOrEmpty(stderr))
                {
                    Debug.LogError(stderr);
                }

                process?.Close();

                // Add the imported style as object and set as main
                ctx.AddObjectToAsset("stylesheet", styleSheet);
                ctx.SetMainObject(styleSheet);
            }
            catch (Win32Exception exception)
            {
                // On error log
                ctx.LogImportError(exception.Message);
            } 
        }
    }
}