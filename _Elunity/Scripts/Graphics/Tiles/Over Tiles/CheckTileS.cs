using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Elang
{
    // Checkered tile out of four tiles.
    // Ÿ�� �� ���� �̷���� üũ ��� Ÿ��. 
#if UNITY_EDITOR
    [CreateAssetMenu(fileName = "Check Tile S", menuName = "Elang Tile/Check Tile S", order = 12)]
#endif
    public class CheckTileS : ElangTile
    {
        [SerializeField]
        ElangTile leftTop;
        [SerializeField]
        ElangTile rightTop;
        [SerializeField]
        ElangTile leftBottom;
        [SerializeField]
        ElangTile rightBottom;

        [SerializeField]
        Vector2Int shift;
        [SerializeField]
        int scale = 1;

        public override void SetTile(Vector3Int location, ITilemap map, ref TileData data) {
            var h = Mathf.Abs((location.x + shift.x - 10000) % (2 * scale)) / scale;
            var v = Mathf.Abs((location.y + shift.y - 10000) % (2 * scale)) / scale;

            ElangTile tile = null;
            if (h == 0) {
                if (v == 0)
                    tile = leftTop;
                else tile = leftBottom;
            } else {
                if (v == 0)
                    tile = rightTop;
                else tile = rightBottom;
            }
            if (tile != null) {
                data.sprite = tile.GetSprite(location, map, quirks);
                data.colliderType = quirks.colliderType;
            }
        }
        public override Sprite GetSprite(Vector3Int location, ITilemap map, TileQuirks quirks) {
            var h = Mathf.Abs((location.x + shift.x - 10000) % (2 * scale)) / scale;
            var v = Mathf.Abs((location.y + shift.y - 10000) % (2 * scale)) / scale;

            ElangTile tile = null;
            if (h == 0) {
                if (v == 0)
                    tile = leftTop;
                else tile = leftBottom;
            } else {
                if (v == 0)
                    tile = rightTop;
                else tile = rightBottom;
            }
            return tile ? tile.GetSprite(location, map, quirks) : null;
        }

#if UNITY_EDITOR
        public override Sprite Thumbnail { get { return (leftTop != null) ? leftTop.Thumbnail : null; } }
#endif
    }
}
