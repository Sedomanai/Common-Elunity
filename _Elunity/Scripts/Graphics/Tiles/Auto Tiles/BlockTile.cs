using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Elang
{
    // Simple block auto tiling that requires only 9 tiles. Simpler version of BaseTile.
    // Ÿ�� 9���� �̷���� ���� ���� Ÿ��. BaseTile ��������. �ڼ��� �� ������ ����
    [System.Serializable]
    public class BlockTile : ElangTile
    {
        [HideInInspector]
        public BlockSpriteDictionary sprites = new BlockSpriteDictionary();

        public override Sprite GetSprite(Vector3Int location, ITilemap map, TileQuirks quirks) {
            Vector2Int pos = new Vector2Int();

            var neighbors = TileAlgorithm.NeighborBit(location, map, quirks);
            if ((neighbors & 8) == 8) {
                if ((neighbors & 16) == 16) {
                    pos.x = 1;
                } else {
                    pos.x = 2;
                }
            } else {
                if ((neighbors & 16) == 16) {
                    pos.x = 0;
                } else {
                    pos.x = 1;
                }
            }

            if ((neighbors & 2) == 2) {
                if ((neighbors & 64) == 64) {
                    pos.y = 1;
                } else {
                    pos.y = 2;
                }
            } else {
                if ((neighbors & 64) == 64) {
                    pos.y = 0;
                } else {
                    pos.y = 1;
                }
            }

            if (sprites.ContainsKey(pos))
                return sprites[pos];
            else return null;
        }

#if UNITY_EDITOR
        public override Sprite Thumbnail { get { return sprites.ContainsKey(new Vector2Int(1, 1)) ? sprites[new Vector2Int(1, 1)] : null; } }
        public override void Write(TileHub hub, TileMold mold, ref Vector2Int writer) {
            for (int j = 0; j < 3; j++) {
                for (int i = 0; i < 3; i++) {
                    WriteTexture(hub, mold, ref writer, new Vector2Int(i, j));
                }
            }
        }

        void WriteTexture(TileHub hub, TileMold mold, ref Vector2Int writer, Vector2Int pos) {
            var ppu = hub.ppu;
            Rect rect = mold.material.rect;
            int rx = (int)Mathf.Round(rect.xMin) + pos.x * ppu;
            int ry = (int)Mathf.Round(rect.yMin + rect.height - ppu) - ppu * pos.y;
            Color[] colors = mold.material.texture.GetPixels(rx, ry, ppu, ppu);
            hub.texture.SetPixels(writer.x, writer.y, ppu, ppu, colors);

            CreateSprite(hub, writer, sprites, pos);
            ShiftWriter(hub, ref writer);
        }
#endif

    }
}