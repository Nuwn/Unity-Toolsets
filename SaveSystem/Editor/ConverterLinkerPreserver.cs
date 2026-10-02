using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace LazySaveSystem
{
    /// <summary>
    /// Writes a link.xml covering every discovered converter before a build.
    /// <para>
    /// Converters are found by reflection, so the linker holds no static reference to them and would
    /// strip them out of an IL2CPP player. A stripped converter does not throw: the registry falls back to
    /// JsonUtility and the save quietly loses readonly fields again. link.xml cannot ship inside this
    /// package, so it has to be generated into the project.
    /// </para>
    /// </summary>
    public sealed class ConverterLinkerPreserver : IPreprocessBuildWithReport
    {
        private const string LinkAssetPath = "Assets/SaveSystem.Converters.link.xml";

        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            var converters = ConverterDiscovery.FindConverterTypes();

            if (converters.Count == 0)
            {
                Debug.Log($"[SaveSystem] No converters discovered, not writing {LinkAssetPath}.");
                return;
            }

            var link = new StringBuilder();
            link.AppendLine("<linker>");

            var groups = converters
                .GroupBy(type => type.Assembly.GetName().Name)
                .OrderBy(group => group.Key, StringComparer.Ordinal);

            foreach (var group in groups)
            {
                // ignoreIfMissing keeps a converter that only exists in the editor from aborting a player
                // build, which the linker does by default when a listed assembly is absent.
                link.AppendLine($"  <assembly fullname=\"{group.Key}\" ignoreIfMissing=\"true\">");

                foreach (var type in group.OrderBy(type => type.FullName, StringComparer.Ordinal))
                    link.AppendLine($"    <type fullname=\"{type.FullName}\" preserve=\"all\" />");

                link.AppendLine("  </assembly>");
            }

            link.AppendLine("</linker>");

            var assetPath = Path.Combine(Application.dataPath, "SaveSystem.Converters.link.xml");
            File.WriteAllText(assetPath, link.ToString());
            AssetDatabase.ImportAsset(LinkAssetPath, ImportAssetOptions.ForceUpdate);

            Debug.Log($"[SaveSystem] Preserved {converters.Count} converter(s) in {LinkAssetPath}.");
        }
    }
}
