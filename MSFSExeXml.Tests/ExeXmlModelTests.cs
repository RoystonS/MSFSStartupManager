using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit;
namespace MSFSExeXml.Tests
{
    public class ExeXmlModelTests
    {
        [Fact]
        public void ReadsNamesAndDisabledState()
        {
            using (var paths = TestFileSystem.BuildTestFileSystem())
            {
                var steamFolder = paths.SteamPath;

                paths.EstablishAsMSFSRoot(steamFolder);

                File.WriteAllText(Path.Join(steamFolder, "exe.xml"),
                    @"<?xml version=""1.0"" encoding=""Windows-1252""?>
<SimBase.Document Type=""SimConnect"" version=""1,0"">
  <Launch.Addon>
    <Name>AFCBridge</Name>
    <Disabled>False</Disabled>
    <Path>C:\Users\owner\AppData\Roaming\Microsoft Flight Simulator\Packages\community\AFC_Bridge\bin\AFC_Bridge.exe</Path>
  </Launch.Addon>
  <Launch.Addon>
    <Name>BetterBravoLights</Name>
    <Disabled>False</Disabled>
    <Path>Something</Path>
  </Launch.Addon>
  <Launch.Addon>
    <Name>Some Other Addon</Name>
    <Disabled>True</Disabled>
    <Path>Something</Path>
  </Launch.Addon>
</SimBase.Document>
");

                var model = ExeXmlModel.Load(FlightSimulatorPaths.ExeXmlPath);

                var names = (from addon in model.Addons where !addon.Disabled select addon.Name).ToArray();
                var expectedNames = new[] { "AFCBridge", "BetterBravoLights" };
                Assert.Equal(expectedNames, names);
            }
        }
    }
}