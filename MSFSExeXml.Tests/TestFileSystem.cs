using System;
using System.IO;

namespace MSFSExeXml.Tests
{
    internal class TestFileSystem
    {

        private static string GetTemporaryDirectory()
        {
            // Create a directory name with spaces in the path
            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + " " + Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);
            return tempDirectory;
        }

        internal static Paths BuildTestFileSystem()
        {
            var folder = GetTemporaryDirectory();

            FlightSimulatorPaths.UnitTestRoot = folder;

            var windowsStoreFolder = Path.Join(folder, "LOCALAPPDATA", "Packages", "Microsoft.FlightSimulator_8wekyb3d8bbwe", "LocalCache");
            var steamFolder = Path.Join(folder, "APPDATA", "Microsoft Flight Simulator");

            var fsDataFolder = Path.Join(folder, "FSDATA");
            Directory.CreateDirectory(fsDataFolder);
            FlightSimulatorPaths.UnitTestRoot = folder;

            return new Paths
            {
                TestRoot = folder,
                StorePath = windowsStoreFolder,
                SteamPath = steamFolder,
                FSDataPath = fsDataFolder,
                Disposer = () =>
                {
                    FlightSimulatorPaths.UnitTestRoot = null;

                    Directory.Delete(folder, true);
                }
            };
        }
    }

    internal delegate void Disposer();

    class Paths : IDisposable
    {
        public Disposer Disposer { get; set; }

        public string TestRoot { get; set; }
        public string StorePath { get; set; }
        public string SteamPath { get; set; }
        public string FSDataPath { get; set; }

        public void EstablishAsMSFSRoot(string folder)
        {
            Directory.CreateDirectory(folder);
            File.WriteAllText(Path.Join(folder, "UserCfg.opt"), $"InstalledPackagesPath \"{FSDataPath}\"");
        }

        void IDisposable.Dispose()
        {
            this.Disposer();
        }
    }
}
