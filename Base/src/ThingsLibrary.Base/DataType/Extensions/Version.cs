// ================================================================================
// <copyright file="Version.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.DataType.Extensions
{
    public static partial class VersionExtensions
    {
        /// <summary>
        /// Outputs the version as a Long 
        /// </summary>
        /// <param name="version">Version</param>
        /// <returns>Version as Long data type</returns>
        public static long ToLong(this Version version)
        {
            return version.Major * 1000000000L +
                version.Minor * 1000000L +
                version.Build * 1000L
                + (version.Revision > 0 ? version.Revision : 0);
        }

        /// <summary>
        /// Convert the version object to a dot notated string Major.Minor.Build.Revision
        /// </summary>
        /// <param name="version">Version</param>
        /// <returns>{Major}.{Minor}.{Build}.{Revision}</returns>
        public static string ToDotString(this Version version)
        {
            return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }

        /// <summary>
        /// Backward Compatible Check
        /// </summary>
        /// <param name="version">Version</param>
        /// <param name="compareVersion">Compare Version</param>
        /// <remarks>
        /// Follows Semantic Versioning Backward Compatibility Standards (https://semver.org/)
        /// - MAJOR version when you make incompatible API changes,
        /// - MINOR version when you add functionality in a backwards compatible manner, and
        /// - PATCH version when you make backwards compatible bug fixes.
        /// </remarks>
        /// <param name="compareVersion"></param>
        /// <returns>Is Backward Compatible</returns>
        /// <exception cref="ArgumentNullException">CompareVersion is null</exception>
        public static bool IsBackCompatible(this Version version, Version compareVersion)
        {
            ArgumentNullException.ThrowIfNull(compareVersion, nameof(compareVersion));
            
            // anything under major = 1.0 should just come back as compatible?

            // if major versions don't match we aren't compatible
            if (version.Major != compareVersion.Major) { return false; }
                         
            // make sure that we aren't going BACK but forward.. so if we are less then our compare then we are backward compatible
            return (version.Minor <= compareVersion.Minor);            
        }
    }
}
