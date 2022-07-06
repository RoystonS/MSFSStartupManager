using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MSFSExeXml
{
    public class ExeXmlModel
    {
        private readonly XDocument document;
        private readonly XElement rootElement;

        private ExeXmlModel(XDocument document)
        {
            this.document = document;
            rootElement = document.Element("SimBase.Document");
        }

        public IEnumerable<Addon> Addons
        {
            get { return rootElement.Elements("Launch.Addon").Select(el => new Addon(el)); }
        }

        public Addon AddAddon(string name, string path, bool disabled = false)
        {
            var el = new XElement("Launch.Addon");
            var addon = new Addon(el);
            addon.Name = name;
            addon.Path = path;
            addon.Disabled = disabled;
            rootElement.Add(el);

            return addon;
        }

        public static ExeXmlModel Load(string filename)
        {
            return new ExeXmlModel(XDocument.Load(filename));
        }
        public static ExeXmlModel Load(TextReader reader)
        {
            return new ExeXmlModel(XDocument.Load(reader));
        }

        public static ExeXmlModel CreateEmpty()
        {
            var doc = new XDocument(new XDeclaration("1.0", "utf-8", ""),
                new XElement("SimBase.Document",
                    new XElement("Descr", "Autolaunch"),
                    new XElement("Filename", "exe.xml"),
                    new XElement("Disabled", "False")
                )
            );

            return new ExeXmlModel(doc);
        }

        public void Save(string filename)
        {
            document.Save(filename);
        }

        static ExeXmlModel()
        {
            // Ensure that other code-page/encodings such as CP1252 are registered with
            // the text parsing framework.
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }
    }

    public class Addon
    {
        private readonly XElement element;

        public Addon(XElement element)
        {
            this.element = element;
        }

        public string Name
        {
            get { return element.Element("Name")?.Value; }
            set { element.SetElementValue("Name", value); }
        }

        public string Path
        {
            get { return element.Element("Path")?.Value; }
            set { element.SetElementValue("Path", value); }
        }

        public string CommandLine
        {
            get { return element.Element("CommandLine")?.Value; }
            set { element.SetElementValue("CommandLine", value); }
        }

        public bool NewConsole
        {
            get { return AsBoolean(element.Element("NewConsole")?.Value); }
            set { element.SetElementValue("NewConsole", value); }
        }

        public bool Disabled
        {
            get { return AsBoolean(element.Element("Disabled")?.Value); }
            set { element.SetElementValue("Disabled", value); }
        }

        private static bool AsBoolean(string value)
        {
            return String.Equals(value, "True", StringComparison.OrdinalIgnoreCase);
        }
    }
}
