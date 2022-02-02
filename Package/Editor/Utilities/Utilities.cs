//
// Copyright (c) arfinity GmbH and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for details.
//
using UnityEngine;

namespace Arfinity.Libraries.Unity.Importers.SassImporter.Editor.Utilities
{
    /// <summary>
    ///  Convenience class
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Store sass path on first use so that log message isn't repeated
        /// </summary>
        private static string _sassPath = null;
        
        /// <summary>
        /// Get the path to the sass executable depending on whether the optional package is available or not
        /// </summary>
        /// <returns>Path to sass executable</returns>
        public static string GetSassPath()
        {
            if (_sassPath != null)
            {
                return _sassPath;
            }
            
#if SASS_EXECUTABLES_PACKAGE_PRESENT
            Debug.Log("Using sass executable from io.arfinity.unity.packages.sassexecutables");
            _sassPath = Arfinity.Libraries.Unity.SassExecutables.Editor.SassExecutable.GetEmbeddedSassPath();
#else
            Debug.Log("Trying to use system sass executable if it exists on path.");
            _sassPath = "sass";
#endif

            return _sassPath;
        }         
    }
}