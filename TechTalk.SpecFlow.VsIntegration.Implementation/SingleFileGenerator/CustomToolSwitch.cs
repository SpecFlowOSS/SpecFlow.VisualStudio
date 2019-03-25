using System.Collections.Generic;
using System.IO;
using System.Linq;
using EnvDTE;
using Microsoft.Win32;
using VSLangProj80;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.SingleFileGenerator
{
    public class CustomToolSwitch
    {
        private readonly DTE _dte;

        public CustomToolSwitch(DTE dte)
        {
            _dte = dte;
        }

        private string _fileEndingRegKey = @"Generators\{0}\.feature";
        private string _customToolRegKey = @"Generators\{0}\SpecFlowSingleFileGenerator";

        private List<string> _contextGuids = new List<string>()
        {
            "{9A19103F-16F7-4668-BE54-9A1E7A4F7556}",
            vsContextGuids.vsContextGuidVCSProject,
            vsContextGuids.vsContextGuidVBProject,
        };

        private IEnumerable<string> GetRegistryKeysForProbing()
        {
            foreach (var contextGuid in _contextGuids)
            {
                yield return string.Format(_fileEndingRegKey, contextGuid);
                yield return string.Format(_customToolRegKey, contextGuid);
            }
        }

        public bool IsEnabled()
        {
            foreach (var registryKey in GetRegistryKeysForProbing())
            {
                var finalRegKey = Path.Combine(_dte.RegistryRoot, registryKey);

                var subkey = Registry.LocalMachine.OpenSubKey(finalRegKey);

                if (subkey == null)
                {
                    continue;
                }

                return true;
            }

            return false;
        }

        public void Disable()
        {
            foreach (var contextGuid in _contextGuids)
            {
                var finalRegKey = Path.Combine(_dte.RegistryRoot, string.Format(@"Generators\{0}", contextGuid));

                using (var subkey = Registry.LocalMachine.OpenSubKey(finalRegKey, true))
                {
                    if (subkey == null)
                    {
                        continue;
                    }

                    var subKeyNames = subkey.GetSubKeyNames();
                    if (subKeyNames.Contains(".feature"))
                    {
                        subkey.DeleteSubKeyTree(".feature");
                    }

                    if (subKeyNames.Contains("SpecFlowSingleFileGenerator"))
                    {
                        subkey.DeleteSubKeyTree("SpecFlowSingleFileGenerator");
                    }
                }
            }
        }

        public void Enable()
        {
            foreach (var contextGuid in _contextGuids)
            {
                var fileEndingReyKey = Path.Combine(_dte.RegistryRoot, string.Format(_fileEndingRegKey, contextGuid));

                using (var fileEndingSubKey = Registry.LocalMachine.CreateSubKey(fileEndingReyKey))
                {
                    fileEndingSubKey.SetValue(null, "SpecFlowSingleFileGenerator");
                }

                var customToolReyKey = Path.Combine(_dte.RegistryRoot, string.Format(_customToolRegKey, contextGuid));

                using (var customToolSubKey = Registry.LocalMachine.CreateSubKey(customToolReyKey))
                {
                    customToolSubKey.SetValue(null, "SpecFlow CustomTool");
                    customToolSubKey.SetValue("CLSID", "{44F8C2E2-18A9-4B97-B830-6BCD0CAA161C}");
                    customToolSubKey.SetValue("GeneratesDesignTimeSource", 1, RegistryValueKind.DWord);
                }
            }
        }
    }
}