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
    // One tile one sprite
    [System.Serializable]
    public class SingleTile : ElangTile
    {
        [HideInInspector]
        [SerializeField]
        Sprite _sprite;

        public override Sprite GetSprite(Vector3Int location, ITilemap map, TileQuirks quirks) {
            return _sprite;
        }

#if UNITY_EDITOR
        public override Sprite Thumbnail { get { return _sprite; } }
        public override void Write(TileHub hub, TileMold mold, ref Vector2Int writer) {
            var ppu = hub.ppu;
            Rect rect = mold.material.rect;
            int rx = (int)Mathf.Round(rect.xMin); 
            int ry = (int)Mathf.Round(rect.yMin + rect.height - ppu);
            Color[] colors = mold.material.texture.GetPixels(rx, ry, ppu, ppu);
            hub.texture.SetPixels(writer.x, writer.y, ppu, ppu, colors);

            CreateSprite(hub, writer, ref _sprite);
            ShiftWriter(hub, ref writer);
        }

        public override void ClearSubAssets() {
            _sprite = null;
        }
#endif
    }
}