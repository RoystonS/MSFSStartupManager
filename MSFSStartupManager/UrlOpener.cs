using System;
using System.Diagnostics;

namespace MSFSStartupManager
{
    internal class UrlOpener
    {
        public static void OpenUrl(Uri uri)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = uri.AbsoluteUri,
                UseShellExecute = true
            });
        }
    }
}
