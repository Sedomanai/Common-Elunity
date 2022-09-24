using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Elang
{
    // Line tile of 3 tiles. Horizontal supported.
    // Ÿ�� 3���� �̷���� �� �� ���� Ÿ��. ���� �� (_isHorizontal) ����. �ڼ��� �� ������ ����
    [System.Serializable]
    public class PillarTile : ElangTile
    {
        [SerializeField]
        bool _isHorizontal;

        [HideInInspector]
        [SerializeField]
        Sprite _A, _C, _B, _SOLO;

        public override void SetTile(Vector3Int location, ITilemap map, ref TileData data) {
            data.sprite = GetSprite(location, map, quirks);
            data.colliderType = quirks.colliderType;
        }
        public override Sprite GetSprite(Vector3Int location, ITilemap map, TileQuirks quirks) {
            char neighbors = TileAlgorithm.NeighborBit(location, map, quirks);
            bool a, b;
            if (_isHorizontal) {
                a = ((neighbors & 16) == 16);
                b = ((neighbors & 8) == 8);
            } else {
                a = ((neighbors & 64) == 64);
                b = ((neighbors & 2) == 2);
            }

            if (a) {
                if (b) {
                    return _C;
                } else {
                    return _A;
                }
            } else if (b) {
                return _B;
            }
            return _SOLO;
        }

#if UNITY_EDITOR
        public override Sprite Thumbnail { get { return _SOLO ? _SOLO : null; } }
        public override void Write(TileHub hub, TileMold mold, ref Vector2Int writer) {
            WriteTexture(hub, mold, ref writer, ref _SOLO, 0);
            WriteTexture(hub, mold, ref writer, ref _A, 1);
            WriteTexture(hub, mold, ref writer, ref _C, 2);
            WriteTexture(hub, mold, ref writer, ref _B, 3);
        }

        void WriteTexture(TileHub hub, TileMold mold, ref Vector2Int writer, ref Sprite spr, int index) {
            var ppu = hub.ppu;
            Rect rect = mold.material.rect;
            int rx = (int)Mathf.Round(rect.xMin);
            int ry = (int)Mathf.Round(rect.yMin + rect.height - ppu) - ppu * index;

            Color[] colors = mold.material.texture.GetPixels(rx, ry, ppu, ppu);
            hub.texture.SetPixels(writer.x, writer.y, ppu, ppu, colors);

            var cell = Sprite.Create(hub.texture, new Rect(writer, new Vector2(ppu, ppu)), new Vector2(0.5f, 0.5f), ppu);
            CreateSprite(hub, writer, ref spr);
            ShiftWriter(hub, ref writer);
        }

        public override void ClearSubAssets() {
            if (_A)
                AssetDatabase.RemoveObjectFromAsset(_A);
            if (_C)
                AssetDatabase.RemoveObjectFromAsset(_C);
            if (_B)
                AssetDatabase.RemoveObjectFromAsset(_B);
            if (_SOLO)
                AssetDatabase.RemoveObjectFromAsset(_SOLO);
        }
#endif
    }
}