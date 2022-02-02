//
// Copyright (c) arfinity GmbH and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for details.
//
using System.Linq;
using Arfinity.Libraries.Unity.Importers.SassImporter.Editor.ScriptedImporters;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Arfinity.Libraries.Unity.Importers.SassImporter.Editor.Validator
{
    /// <summary>
    /// Class that validates whether we would need sass during the build process and fails if we do but don't have it available
    /// </summary>
    public class SassBuildValidator : IPreprocessBuildWithReport
    {

        public int callbackOrder { get; }

        public void OnPreprocessBuild(BuildReport report)
        {
            // Check if sass is available, if so we are done
            if (SassDependency.SassFound)
            {
                return;
            }
            
            // Else check if we even have scss files in the build
            var paths = AssetDatabase.GetAllAssetPaths();

            if (paths.Any(path => path.EndsWith(".scss")))
            {
                throw new BuildFailedException("Build with scss assets but no sass executable found");
            }
        }
    }
}