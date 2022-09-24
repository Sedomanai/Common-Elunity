using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Elang
{
    [System.Serializable]
    public class SpriteDictionary : SerializableDictionary<int, Sprite> { }
    [System.Serializable]
    public class BlockSpriteDictionary : SerializableDictionary<Vector2Int, Sprite> { }

    //Elang auto tiling base class
    //자동 타일 베이스 클래스
    [System.Serializable]
    public abstract class ElangTile : TileBase {
        [System.Serializable]
        public class TileQuirks
        {
            public int index, subIndex;
            public bool pipe, countSubIndex;
            public Tile.ColliderType colliderType;
        }

        [SerializeField]
        public TileQuirks quirks;

        public override void GetTileData(Vector3Int location, ITilemap map, ref TileData data) {
            GetTileDataBase(location, map, ref data);
            if (TileAlgorithm.NeighborIsSelf(location, Vector3Int.zero, map, quirks))
                SetTile(location, map, ref data);
        }

        public virtual void SetTile(Vector3Int location, ITilemap map, ref TileData data) {
            data.sprite = GetSprite(location, map, quirks);
            data.colliderType = data.sprite ? quirks.colliderType : Tile.ColliderType.None;
        }

        public virtual Sprite GetSprite(Vector3Int location, ITilemap map, TileQuirks quirks) { return null; }
        protected void GetTileDataBase(Vector3Int location, ITilemap map, ref TileData data) {
            data.color = Color.white;
            data.flags = (TileFlags.LockTransform | TileFlags.LockColor);
            data.transform = Matrix4x4.identity;
            data.colliderType = Tile.ColliderType.None;
            base.GetTileData(location, map, ref data);
        }

        public override void RefreshTile(Vector3Int location, ITilemap map) {
            for (int yd = -1; yd <= 1; yd++) {
                for (int xd = -1; xd <= 1; xd++) {
                    Vector3Int direction = new Vector3Int(xd, yd, 0);
                    if (TileAlgorithm.NeighborIsSelf(location, direction, map, quirks))
                        map.RefreshTile(location + direction);
                }
            }
        }
#if UNITY_EDITOR
        public virtual Sprite Thumbnail { get { return null; } }
        public virtual void ClearSubAssets() { }
        public virtual void Write(TileHub hub, TileMold mold, ref Vector2Int writer) { }

        protected void CreateSprite(TileHub hub, Vector2Int writer, BlockSpriteDictionary sprites, Vector2Int piece) {
            var cell = Sprite.Create(hub.texture, new Rect(writer, new Vector2(hub.ppu, hub.ppu)), new Vector2(0.5f, 0.5f), hub.ppu);
            if (sprites.ContainsKey(piece)) {
                EditorUtility.CopySerialized(cell, sprites[piece]);
            } else {
                sprites[piece] = cell;
                cell.hideFlags = HideFlags.HideInHierarchy;
                AssetDatabase.AddObjectToAsset(cell, AssetDatabase.GetAssetPath(this));
            }
        }
        protected void CreateSprite(TileHub hub, Vector2Int writer, SpriteDictionary sprites, int piece) {
            var cell = Sprite.Create(hub.texture, new Rect(writer, new Vector2(hub.ppu, hub.ppu)), new Vector2(0.5f, 0.5f), hub.ppu);
            if (sprites.ContainsKey(piece)) {
                EditorUtility.CopySerialized(cell, sprites[piece]);
            } else {
                sprites[piece] = cell;
                cell.hideFlags = HideFlags.HideInHierarchy;
                AssetDatabase.AddObjectToAsset(cell, AssetDatabase.GetAssetPath(this));
            }
        }
        protected void CreateSprite(TileHub hub, Vector2Int writer, ref Sprite sprite) {
            var cell = Sprite.Create(hub.texture, new Rect(writer, new Vector2(hub.ppu, hub.ppu)), new Vector2(0.5f, 0.5f), hub.ppu);
            if (sprite) {
                EditorUtility.CopySerialized(cell, sprite);
            } else {
                sprite = cell;
                cell.hideFlags = HideFlags.HideInHierarchy;
                AssetDatabase.AddObjectToAsset(cell, AssetDatabase.GetAssetPath(this));
            }
        }
        protected void ShiftWriter(TileHub hub, ref Vector2Int writer) {
            writer.x += hub.ppu;
            if (writer.x > (hub.texture.width - hub.ppu)) {
                writer.x = 0;
                writer.y -= hub.ppu;
            }
        }
#endif
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ElangTile), true)]
    public class ElangTileEditor : Editor
    {
        private ElangTile tile { get { return (target as ElangTile); } }

        public override Texture2D RenderStaticPreview(string assetPath, UnityEngine.Object[] subAssets, int width, int height) {
            Texture2D source = AssetPreview.GetAssetPreview(tile.Thumbnail);
            Texture2D cache = new Texture2D(width, height);
            if (source & cache) {
                EditorUtility.CopySerialized(source, cache);
                return cache;
            }
            return base.RenderStaticPreview(assetPath, subAssets, width, height);
        }
    };
#endif
}