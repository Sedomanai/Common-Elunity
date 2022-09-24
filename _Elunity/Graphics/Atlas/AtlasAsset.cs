using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;
using UnityEditor;
using UnityEngine;

namespace Elang
{
    public class AtlasAsset : ScriptableObject
    {
        public List<Texture2D> textures;
        public bool makeClipAnimations;
        public bool makeController;

        [SerializeField]
        List<AtlasCellMeta> cells;
        [SerializeField]
        List<AtlasClipMeta> clips;
        [SerializeField]
        Int32 width, height;

        public void Import(byte[] bytes_) {
            SerializeStream ss = StreamCreator.create(bytes_);

            Int32 userSize;
            ss.parse(out width).parse(out height).parse(out userSize);

            var users = new List<string>();
            for (int i = 0; i < userSize; i++) {
                string userName = string.Empty;
                ss.parse(out userName);
                users.Add(userName);
            }

            Int32 cellCount;
            ss.parse(out cellCount);
            cells = new List<AtlasCellMeta>();
            for (int i = 0; i < cellCount; i++) {
                cells.Add(new AtlasCellMeta(ss));
            }

            Int32 clipCount;
            ss.parse(out clipCount);
            clips = new List<AtlasClipMeta>();
            for (int i = 0; i < clipCount; i++) {
                clips.Add(new AtlasClipMeta(ss));
            }

            Generate();
        }

        public void Generate() {
            foreach (var texture in textures) {
                var path = AssetDatabase.GetAssetPath(texture);

                TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
                if (ti) {
                    // This removes any possible lingering SpriteMeta data
                    if (ti.spriteImportMode == SpriteImportMode.Multiple) {
                        ti.spritesheet = null;
                        ti.spriteImportMode = SpriteImportMode.Single;
                        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                    }

                    ti.isReadable = true;
                    ti.textureType = TextureImporterType.Sprite;
                    ti.spriteImportMode = SpriteImportMode.Multiple;
                    ti.mipmapEnabled = false;
                    ti.filterMode = FilterMode.Point;

                    FillAtlas(ti, path);
                    MakeClips(path, texture.name);
                }
            }
        }

        void FillAtlas(TextureImporter ti, string path) {
            List<SpriteMetaData> spritesheet = new List<SpriteMetaData>();

            for (int i = 0; i < cells.Count; i++) {
                spritesheet.Add(cells[i].mold(height));
            }

            ti.spritesheet = spritesheet.ToArray();
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }
        void MakeClips(string path, string textureName) {
            if (clips.Count > 0 && makeClipAnimations) {
                var dir = path.Substring(0, path.LastIndexOf('/'));
                string folderName = textureName + "Anim";
                var animDir = dir + "/" + folderName;
                if (makeClipAnimations && !AssetDatabase.IsValidFolder(animDir)) {
                    AssetDatabase.CreateFolder(dir, folderName);
                }

                if (!AssetDatabase.IsValidFolder(animDir))
                    AssetDatabase.CreateFolder(dir, folderName);

                AnimatorController controller = makeController ? CreateController(animDir, textureName) : null;
                Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().ToArray();

                for (int i = 0; i < clips.Count; i++) {
                    clips[i].createAnimationAsset(animDir, controller, sprites);
                }
            }
        }
        AnimatorController CreateController(string animDir, string texName) {
            var contFilename = animDir + "/" + texName + ".controller";
            AnimatorController controller = (File.Exists(contFilename))  ?
                AssetDatabase.LoadAssetAtPath(contFilename, typeof(AnimatorController)) as AnimatorController :
                AnimatorController.CreateAnimatorControllerAtPath(contFilename);
            return controller;
        }
    }
}