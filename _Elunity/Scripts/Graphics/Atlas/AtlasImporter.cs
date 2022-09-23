using System.IO;
using UnityEngine;
using UnityEditor.AssetImporters;
using UnityEditor;
using System.Collections.Generic;

namespace Elang
{
    [ScriptedImporter(1, "atls")]
    public class AtlasImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx) {
            var assetPath = ctx.assetPath + ".asset";
            if (!File.Exists(assetPath)) {
                AtlasAsset atlas = ScriptableObject.CreateInstance<AtlasAsset>();
                AssetDatabase.CreateAsset(atlas, assetPath);
            }
        }
    }

    public class AtlasPreprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload) {
            foreach (var str in importedAssets) {
                if (Path.GetExtension(str) == ".atls") {
                    var atlas = AssetDatabase.LoadAssetAtPath(str + ".asset", typeof(AtlasAsset)) as AtlasAsset;
                    if (atlas) {
                        atlas.Import(File.ReadAllBytes(str));
                    }
                }
            }
        }
    }
}