using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Elang
{
    // Slope tiling
    // 경사길 타일. 자세한 건 동영상 참조
    [System.Serializable]
    public class SlopeTile : BaseTile
    {
        public struct SlopeFlags
        {
            public bool l, t, r, b;
            public bool h, v, d; // hori verti diagonal
            public int type;
            char neighbor;

            public void setFlags(char neighbor_) {
                t = ((neighbor_ & 2) == 2);
                l = ((neighbor_ & 8) == 8);
                r = ((neighbor_ & 16) == 16);
                b = ((neighbor_ & 64) == 64);
                neighbor = neighbor_;
            }

            public bool HasSlope() {
                return !((l && r) || (t && b));
            }

            public void FindNeighbors() {
                type = 0;
                if (l) type++;
                if (t) type += 2;

                h = (l || r) ? true : false;
                v = (b || t) ? true : false;

                int bit = 1;
                if (r) bit *= 4;
                if (b) bit *= 32;
                d = ((neighbor & bit) == bit);
            }
        }


#if UNITY_EDITOR
        public override Sprite Thumbnail { get { return sprites.ContainsKey(585) ? sprites[585] : null; } }
        public override void Write(TileHub hub, TileMold mold, ref Vector2Int writer) {
            HashSet<int> hash = new HashSet<int>();
            int tileIndex = 0;

            SlopeFlags reader = new SlopeFlags();

            for (char i = (char)0; i < 256; i++) {
                reader.setFlags(i);

                if (!reader.HasSlope())
                    continue;

                int piece = TileAlgorithm.GetPiece(i);

                if (!hash.Contains(piece)) {
                    WriteTexture(hub, mold, piece, ref writer, reader);
                    tileIndex++;
                    hash.Add(piece);
                }
            }
        }

        void WriteTexture(TileHub hub, TileMold mold, int piece, ref Vector2Int writer, SlopeFlags flags) {
            int ppu = hub.ppu;
            int hppu = ppu / 2;
            Texture2D buffer = new Texture2D(ppu, ppu);

            Rect rect = mold.material.rect;
            int rx = (int)Mathf.Round(rect.xMin), rectY = (int)Mathf.Round(rect.yMin);
            int ry = (int)Mathf.Round(rect.yMin) + (int)Mathf.Round(rect.height) - ppu;

            flags.FindNeighbors();
            int column = flags.type % 2;
            int row = flags.type / 2;

            Vector2Int slope = new Vector2Int(rx + column * ppu, ry - row * ppu);
            buffer.SetPixels(mold.material.texture.GetPixels(slope.x, slope.y, ppu, ppu));

            Vector2Int corner = new Vector2Int(rx + column * hppu, ry - ppu - hppu - row * hppu );
            bool rewriteCorner = true;
            if (flags.h) {
                corner.x += ppu;
            }
            if (flags.v) {
                corner.y -= ppu;
                if (flags.d)
                    rewriteCorner = false;
            }

            if (rewriteCorner) {
                Vector2Int offset = new Vector2Int(hppu - column * hppu, row * hppu);
                buffer.SetPixels(offset.x, offset.y, hppu, hppu, mold.material.texture.GetPixels(corner.x, corner.y, hppu, hppu));
            }

            corner = new Vector2Int(rx + column * hppu, ry - ppu - hppu - row * hppu);
            corner.y -= 2 * ppu;
            if (!flags.h) {
                Vector2Int offset = new Vector2Int(hppu - column * hppu, hppu - row * hppu);
                buffer.SetPixels(offset.x, offset.y, hppu, hppu, mold.material.texture.GetPixels(corner.x, corner.y, hppu, hppu));
            }

            corner.x += ppu;
            if (!flags.v) {
                Vector2Int offset = new Vector2Int(column * hppu, row * hppu);
                buffer.SetPixels(offset.x, offset.y, hppu, hppu, mold.material.texture.GetPixels(corner.x, corner.y, hppu, hppu));
            }

            hub.texture.SetPixels(writer.x , writer.y, ppu, ppu, buffer.GetPixels());

            var cell = Sprite.Create(hub.texture, new Rect(writer, new Vector2(ppu, ppu)), new Vector2(0.5f, 0.5f), ppu);
            if (sprites.ContainsKey(piece)) {
                EditorUtility.CopySerialized(cell, sprites[piece]);
            } else {
                sprites[piece] = cell;
                cell.hideFlags = HideFlags.HideInHierarchy;
                AssetDatabase.AddObjectToAsset(cell, AssetDatabase.GetAssetPath(this));
            }
            ShiftWriter(hub, ref writer);
        }
#endif
    }
}