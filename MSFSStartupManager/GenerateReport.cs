using MSFSExeXml;
using System;
using System.IO;
using System.Security.AccessControl;
using System.Text;

namespace MSFSStartupManager
{
    internal class GenerateReport
    {
        public static string BuildReport(MainWindowViewModel viewModel)
        {
            var builder = new StringBuilder();
            builder.AppendFormat("Simulator location: {0}", FlightSimulatorPaths.FlightSimulatorPath);
            builder.AppendLine();
            builder.AppendFormat("Community folder path: {0}", FlightSimulatorPaths.CommunityPath);
            builder.AppendLine();

            builder.AppendLine();
            builder.AppendLine("exe.xml file");
            builder.AppendFormat("Location: {0}", FlightSimulatorPaths.ExeXmlPath);
            builder.AppendLine();

            builder.AppendLine("===");

            string exeXmlContents;
            try
            {
                exeXmlContents = File.ReadAllText(FlightSimulatorPaths.ExeXmlPath);
                builder.AppendLine("Contents of exe.xml:");
                builder.AppendLine(exeXmlContents);
                builder.AppendLine("---");

                try
                {
                    ExeXmlModel model;
                    try
                    {
                        model = ExeXmlModel.Load(new StringReader(exeXmlContents));
                        // Exe.xml contents are fine.
                    } catch (Exception)
                    {
                        // Exe.xml contents are broken.
                        exeXmlContents = ExeXmlFixer.TryFix(exeXmlContents);
                        if (exeXmlContents == null)
                        {
                            // Not fixable.
                            throw;
                        }
                        else
                        {
                            model = ExeXmlModel.Load(new StringReader(exeXmlContents));
                        }
                    }

                    foreach (var addon in viewModel.AddonStartupStatusViewModels)
                    {
                        builder.AppendFormat("Tested addon: {0}. Confirmed start: {1}. Comments: {2}", addon.Name, addon.Started, addon.Comments);
                    }

                    foreach (var addon in model.Addons)
                    {
                        builder.AppendFormat("Configured addon: {0}", addon.Name);
                        builder.AppendLine();
                        builder.AppendFormat("Path: {0}", addon.Path);
                        builder.AppendLine();

                        try
                        {
                            var attributes = File.GetAttributes(addon.Path);
                            builder.AppendFormat("Attributes: {0}", attributes);
                            builder.AppendLine();
                        } catch (Exception ex)
                        {
                            builder.AppendFormat("Could not retrieve attributes for file: {0}", ex.Message);
                            builder.AppendLine();
                        }

                        try
                        {
                            var file = new FileInfo(addon.Path);
                            var acl = file.GetAccessControl(AccessControlSections.Access);

                            var rules = acl.GetAccessRules(true, true, typeof(System.Security.Principal.NTAccount));
                            foreach (FileSystemAccessRule fsar in rules)
                            {
                                string userName = fsar.IdentityReference.Value;
                                string ruleSource = fsar.IsInherited ? "Inherited" : "Explicit";
                                builder.AppendFormat("ACL: {0}, {1}, {2}, {3}, {4}, {5}", fsar.IdentityReference.Value, fsar.AccessControlType, fsar.FileSystemRights, ruleSource, fsar.PropagationFlags, fsar.InheritanceFlags);
                                builder.AppendLine();
                            }
                        } catch (Exception ex)
                        {
                            builder.AppendFormat("Could not retrieve ACL information for file: {0}", ex.Message);
                            builder.AppendLine();
                        }

                        try
                        {
                            using (var file = File.OpenRead(addon.Path))
                            {
                                var bytes = new byte[128];
                                file.Read(bytes, 0, bytes.Length);
                            }
                            builder.AppendLine("Can successfully read from file");
                        } catch (Exception ex)
                        {
                            builder.AppendFormat("Could not read data from file: {0}", ex.Message);
                            builder.AppendLine();
                        }

                        builder.AppendLine("---");
                    }
                }
                catch (Exception)
                {
                    builder.AppendLine("exe.xml file consists of corrupted XML, so can't check details of addons");
                }

            }
            catch (Exception ex)
            {
                builder.AppendFormat("Could not read exe.xml file: {0}", ex.Message);
                builder.AppendLine();
            }

            builder.AppendFormat("Attempted replacement exe.xml: {0}", viewModel.TriedReplacingExeXml);
            builder.AppendLine();
            if (viewModel.TriedReplacingExeXml.HasValue && viewModel.TriedReplacingExeXml.Value)
            {
                builder.AppendFormat("Replacement exe.xml worked: {0}", viewModel.ReplacementExeXmlWorked);
                builder.AppendLine();
            }
            builder.AppendLine("===");

            builder.AppendLine("UserCfg.opt file");
            builder.AppendFormat("Location: {0}", FlightSimulatorPaths.UserCfgOptPath);
            builder.AppendLine();

            try
            {
                var userCfgOptContents = File.ReadAllText(FlightSimulatorPaths.UserCfgOptPath);
                builder.AppendLine("Contents of UserCfg.opt:");
                builder.AppendLine(userCfgOptContents);
            }
            catch (Exception ex)
            {
                builder.AppendFormat("UserCfg.opt file could not be read: {0}", ex.Message);
                builder.AppendLine();
            }


            builder.AppendLine("Other comments");
            builder.AppendLine(viewModel.OtherComments);
            if (!string.IsNullOrEmpty(viewModel.Email))
            {
                builder.AppendFormat("Email: {0}", viewModel.Email);
                builder.AppendLine();
                builder.AppendFormat("Email permissions: technical: {0}, updates: {1}", viewModel.EmailAboutTechnical, viewModel.EmailSendUpdates);
                builder.AppendLine();
            }
            return builder.ToString();
        }
    }
}
