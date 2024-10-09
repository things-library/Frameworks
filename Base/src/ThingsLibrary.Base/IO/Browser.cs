// ================================================================================
// <copyright file="Browser.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using System.Runtime.InteropServices;

namespace ThingsLibrary.IO
{
    public static class Browser
    {
        /// <summary>
        /// Opens a new default browser window going to the specified url
        /// </summary>
        /// <param name="url">Url to open</param>
        /// <exception cref="ArgumentException">If OS does not support this</exception>
        private static void OpenUrl(string url)
        {
            // hack because of this: https://github.com/dotnet/corefx/issues/10361
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {                
                url = url.Replace("&", "^&");
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
            }
            else
            {
                throw new ArgumentException("Unknown operating system support for Browser Popup.");
            }
        }
    }
}
