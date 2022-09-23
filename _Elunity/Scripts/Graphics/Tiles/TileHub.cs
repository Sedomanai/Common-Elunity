using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Elang
{
    // Tile Hub
    // Contains a hub texture made from various molds
    // Create this first to make Elang Tiles

    // 타일 헙
    // 타일 틀(Tile Mold)에서 일랑 타일 제작
    // 텍스쳐 아틀라스 추출 후 저장
    [System.Serializable]
#if UNITY_EDITOR
    [CreateAssetMenu(fileName = "Tile Hub", menuName="Elang Tile/Tile Hub", order = 11)]
#endif

    public class TileHub : ScriptableObject
    {
        public Texture2D texture;
#if UNITY_EDITOR
        public int count = 0;
        public int ppu = 16;
        public bool debugOut = false;
        public Vector2Int texsize = new Vector2Int(512, 512);
        public List<TileMold> molds = new List<TileMold>();
#endif
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(TileHub))]
    public class TileHubEditor : Editor
    {
        private TileHub hub { get { return target as TileHub; } }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            hub.texsize = EditorGUILayout.Vector2IntField("Generated Texture Size", hub.texsize);
            hub.debugOut = EditorGUILayout.ToggleLeft("Export Texture?", hub.debugOut);
            hub.ppu = EditorGUILayout.IntField("Pixel Per Unit", hub.ppu);

            var count = EditorGUILayout.DelayedIntField("Mold Count", hub.molds.Count);            
            if (count != hub.molds.Count) {
                SyncMolds(count);
            }

            for (int i = 0; i < hub.molds.Count; i++) {
                EditorGUILayout.LabelField("Mold " + i);
                EditorGUI.indentLevel++;
                var mold = hub.molds[i];
                mold.assetName = EditorGUILayout.TextField("Name", mold.assetName);
                mold.tileType = (eElangTileType)EditorGUILayout.EnumPopup("Tile Type", mold.tileType);
                if (mold.tileType == eElangTileType.Base) {
                    EditorGUI.indentLevel++;
                    mold.tileFlags = (eElangTileFlags)EditorGUILayout.EnumFlagsField("Tile Flags", mold.tileFlags);
                    EditorGUI.indentLevel--;
                }
                mold.material = (Sprite)EditorGUILayout.ObjectField("Sprite", mold.material, typeof(Sprite), false, null);
                EditorGUI.indentLevel--;
            }

            if (GUILayout.Button("GENERATE!")) {
                Generate();
                EditorUtility.SetDirty(hub);
                serializedObject.ApplyModifiedProperties(); // just in case
            }

            if (EditorGUI.EndChangeCheck()) {
                EditorUtility.SetDirty(hub);
                serializedObject.ApplyModifiedProperties();
            }
        }
        
        void SyncMolds(int count) {
            var currCount = hub.molds.Count;
            if (count != currCount) {
                if (count < currCount) {
                    bool trim = EditorUtility.DisplayDialog("Hub Trim Warning Dialog", "WARNING: Target mold count is lower than current mold count. Proceeding will destroy some trailing molds which will affect all existing tiles dependent on those molds when regenerated. Continue?", "Yes", "No");
                    if (trim) {        
                        var buffer = hub.molds;
                        hub.molds = new List<TileMold>();
                        for (int i = 0; i < buffer.Count; i++) {
                            if (i < count) {
                                hub.molds.Add(buffer[i]);
                            } else {
                                AssetDatabase.RemoveObjectFromAsset(buffer[i]);
                            }
                        }
                    }
                } else {                    
                    for (int i = currCount; i < count; i++) {
                        var mold = CreateInstance<TileMold>();
                        mold.name = "mold_" + i;
                        mold.hideFlags = HideFlags.HideInHierarchy;
                        AssetDatabase.AddObjectToAsset(mold, AssetDatabase.GetAssetPath(target));
                        hub.molds.Add(mold);
                    }
                }
            }
        }

        void Generate() {
            if (hub.molds.Count > 0) {
                var buffer = new Texture2D(hub.texsize.x, hub.texsize.y, TextureFormat.RGBA32, false);
                if (hub.texture) {
                    EditorUtility.CopySerialized(buffer, hub.texture);
                } else {
                    buffer.name = "texture";
                    buffer.hideFlags = HideFlags.HideInHierarchy;
                    AssetDatabase.AddObjectToAsset(buffer, AssetDatabase.GetAssetPath(target));
                    hub.texture = buffer;
                }

                var writer = new Vector2Int(0, hub.texture.height - hub.ppu);
                for (int i = 0; i < hub.molds.Count; i++) {
                    var mold = hub.molds[i];

                    ElangTile tile = null;
                    switch (mold.tileType) {
                        case eElangTileType.Single:
                            tile = CreateTileFromMold<SingleTile>(ref mold);
                            break;
                        case eElangTileType.Base:
                            tile = CreateTileFromMold<BaseTile>(ref mold);
                            break;
                        //case ElangTileType.Surface:
                        //    tile = CreateTileFromMold<SurfaceTile>(ref mold);
                        //    break;
                        //case ElangTileType.Outer:
                        //    tile = CreateTileFromMold<OuterTile>(ref mold);
                        //    break;
                        //case ElangTileType.Cross:
                        //    tile = CreateTileFromMold<CrossTile>(ref mold);
                        //    break;
                        //case ElangTileType.Pipe:
                        //    tile = CreateTileFromMold<PipeTile>(ref mold);
                        //    break;
                        case eElangTileType.Slope:
                           // tile = CreateTileFromMold<SlopeTile>(ref mold);
                            break;
                        case eElangTileType.Pillar:
                            tile = CreateTileFromMold<PillarTile>(ref mold);
                            break;
                        case eElangTileType.Block:
                            tile = CreateTileFromMold<BlockTile>(ref mold);
                            break;
                    }

                    if (tile) {
                        tile.Write(hub, mold, ref writer);
                        EditorUtility.SetDirty(tile);
                    }
                }
                hub.texture.alphaIsTransparency = true;
                hub.texture.Compress(false);
                hub.texture.filterMode = FilterMode.Point;
                hub.texture.Apply();
            }
        }

        ElangTile CreateTileFromMold<T>(ref TileMold mold) where T : ElangTile {
            string hubPath = AssetDatabase.GetAssetPath(target);
            if (hubPath != "" && mold.assetName != "") {
                var path = hubPath.Substring(0, hubPath.LastIndexOf('/') + 1) +  mold.assetName + ".asset";

                ElangTile tile = AssetDatabase.LoadAssetAtPath(path, typeof(ElangTile)) as ElangTile;

                if (tile != null) {
                    if (!(tile is T)) {
                        AssetDatabase.DeleteAsset(path);
                        AssetDatabase.CreateAsset(CreateInstance<T>(), path);
                        tile = AssetDatabase.LoadAssetAtPath(path, typeof(T)) as T;
                    } else {
                        tile.ClearSubAssets();
                    }
                } else {
                    AssetDatabase.CreateAsset(CreateInstance<T>(), path);
                    tile = AssetDatabase.LoadAssetAtPath(path, typeof(T)) as T;
                }

                return tile;
            }
            return null;
        }
        public override Texture2D RenderStaticPreview(string assetPath, UnityEngine.Object[] subAssets, int width, int height) {
            if (hub && hub.texture) {
                Texture2D source = hub.texture;
                Texture2D cache = new Texture2D(width, height);
                if (source && cache) {
                    EditorUtility.CopySerialized(source, cache);
                }
                return cache;
            }

            return base.RenderStaticPreview(assetPath, subAssets, width, height);
        }
    }


    [CustomPreview(typeof(TileHub))]
    public class ElangTileHubPreview : ObjectPreview
    {
        private GUIContent previewTitle = new GUIContent("Texture");
        private TileHub hub { get { return (target as TileHub); } }
        public override bool HasPreviewGUI() {
            if (!target) return false;
            return hub.texture;
        }

        public override GUIContent GetPreviewTitle() {
            return previewTitle;
        }
        public override void OnPreviewGUI(Rect r, GUIStyle background) {
            if (hub.texture) {
                r.height = r.width;
                EditorGUI.DrawTextureTransparent(r, hub.texture);
            }
        }
    }
#endif
}