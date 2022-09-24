using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Elang
{
    // Generic elang tiling system that is the base of many other systems
    // 일랑 타일의 꽃. 오토 타일. 자세한 건 동영상 참조
    [System.Serializable]
    public class BaseTile : ElangTile
    {
        [HideInInspector]
        public SpriteDictionary sprites = new SpriteDictionary();
        Sprite _thumbnail;

        public override Sprite GetSprite(Vector3Int location, ITilemap map, TileQuirks quirks) {
            int piece = TileAlgorithm.GetPiece(TileAlgorithm.NeighborBit(location, map, quirks));
            if (sprites.ContainsKey(piece))
                return sprites[piece];
            else return null;
        }


#if UNITY_EDITOR
        public override Sprite Thumbnail { get { return (sprites.Count > 0) ? sprites.First().Value : null; } }
        public override void Write(TileHub hub, TileMold mold, ref Vector2Int writer) {
            HashSet<int> hash = new HashSet<int>();
            int tileIndex = 0;
            for (char i = (char)0; i < 256; i++) {
                if (mold.tileFlags.HasFlag(eElangTileFlags.SuppressFull) && i == 255)
                    continue;
                if (mold.tileFlags.HasFlag(eElangTileFlags.SuppressSurface) && IsSurface(i))
                    continue;

                int piece = TileAlgorithm.GetPiece(i);

                if (!hash.Contains(piece)) {
                    WriteTexture(hub, mold, piece, ref writer);
                    tileIndex++;
                    hash.Add(piece);
                }
            }

            _thumbnail = sprites.ContainsKey(585) ? sprites[585] : null;

        }

        protected virtual bool WritePredicate(char neighbor) { return true; }

        void WriteTexture(TileHub hub, TileMold mold, int piece, ref Vector2Int writer) {
            int hppu = hub.ppu / 2;

            Rect rect = mold.material.rect;
            int rectX = (int)Mathf.Round(rect.xMin), rectY = (int)Mathf.Round(rect.yMin);
            int rectH = (int)Mathf.Round(rect.height);
            for (int ci = 0; ci < 4; ci++) {
                int ct = rectH - TileAlgorithm.CornerTypeOf(piece, ci) * hub.ppu;
                int ox = (ci % 2) * hppu;
                int oy = hppu - (ci / 2) * hppu;

                int rx = rectX + ox;
                int ry = rectY + oy + ct;

                Color[] colors = mold.material.texture.GetPixels(rx, ry, hppu, hppu);
                hub.texture.SetPixels(writer.x + ox, writer.y + oy, hppu, hppu, colors);
            }

            var cell = Sprite.Create(hub.texture, new Rect(writer, new Vector2(hub.ppu, hub.ppu)), new Vector2(0.5f, 0.5f), hub.ppu);
            if (sprites.ContainsKey(piece)) {
                EditorUtility.CopySerialized(cell, sprites[piece]);
            } else {
                sprites[piece] = cell;
                cell.hideFlags = HideFlags.HideInHierarchy;
                AssetDatabase.AddObjectToAsset(cell, AssetDatabase.GetAssetPath(this));
            }

            CreateSprite(hub, writer, sprites, piece);
            ShiftWriter(hub, ref writer);
        }

        bool IsSurface(char neighbor) {
            bool lt = ((neighbor & 1) == 1);
            bool t = ((neighbor & 2) == 2);
            bool rt = ((neighbor & 4) == 4);
            bool l = ((neighbor & 8) == 8);
            bool r = ((neighbor & 16) == 16);
            return (!t || (t && l && !lt) || (t && r && !rt));
        }

        public override void ClearSubAssets() {
            foreach (var spr in sprites) {
                AssetDatabase.RemoveObjectFromAsset(spr.Value);
            } sprites.Clear();
        }
#endif
    }
}