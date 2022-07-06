using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace MSFSExeXml
{
    public static class FlightSimulatorPaths
    {
        /// <summary>
        /// Gets the location of the main Flight Simulator installation.
        /// </summary>
        public static string FlightSimulatorPath
        {
            get
            {
                var localAppData = (UnitTestRoot == null) ? Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) : Path.Join(UnitTestRoot, "LOCALAPPDATA");
                var windowsStoreLocation = Path.Join(localAppData, "Packages", "Microsoft.FlightSimulator_8wekyb3d8bbwe", "LocalCache");

                var appData = (UnitTestRoot == null) ? Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) : Path.Join(UnitTestRoot, "APPDATA");
                var steamLocation = Path.Join(appData, "Microsoft Flight Simulator");

                var pathsToTry = new[]
                {
                    windowsStoreLocation,
                    steamLocation
                };

                foreach (var path in pathsToTry)
                {
                    if (File.Exists(Path.Join(path, "FlightSimulator.CFG")) || File.Exists(Path.Join(path, "UserCfg.opt")))
                    {
                        return path;
                    }
                }

                var pathsTried = String.Join(", ", pathsToTry);
                throw new Exception($"Could not locate main Flight Simulator path. Paths tried: {pathsTried}");
            }
        }

        /// <summary>
        /// Gets the location of the MSFS exe.xml file, which may not actually exist yet.
        /// </summary>
        public static string ExeXmlPath
        {
            get
            {
                return Path.Join(FlightSimulatorPath, "exe.xml");
            }
        }

        /// <summary>
        /// Gets the location of the UserCfg.opt file.
        /// </summary>
        public static string UserCfgOptPath
        {
            get
            {
                return Path.Join(FlightSimulatorPath, "UserCfg.opt");
            }
        }

        private static readonly Regex installedPackagesPathRegex = new("^InstalledPackagesPath \"(.*)\"");

        /// <summary>
        /// Gets the location of the Official and Community directories.
        /// </summary>
        public static string MSFSPackagesPath
        {
            get
            {
                var lines = File.ReadAllLines(UserCfgOptPath);
                foreach (var line in lines)
                {
                    var match = installedPackagesPathRegex.Match(line);
                    if (match.Success)
                    {
                        return match.Groups[1].Value;
                    }
                }

                throw new Exception("Cannot locate FS packages path");
            }
        }


        /// <summary>
        /// Gets the location of the Community directory.
        /// </summary>
        public static string CommunityPath
        {
            get { return Path.Join(MSFSPackagesPath, "Community"); }
        }

        internal static string UnitTestRoot = null;
    }
}
