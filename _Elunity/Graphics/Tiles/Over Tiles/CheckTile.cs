using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Elang
{
    // Checkered tile out of two tiles.
    // Ÿ�� �ΰ��� �̷���� üũ ��� Ÿ��. 
#if UNITY_EDITOR
    [CreateAssetMenu(fileName = "Check Tile", menuName = "Elang Tile/Check Tile", order = 12)]
#endif
    public class CheckTile : ElangTile
    {
        [SerializeField]
        ElangTile left;
        [SerializeField]
        ElangTile right;
        [SerializeField]
        Vector2Int shift;
        [SerializeField]
        int scale = 1;

        public override void SetTile(Vector3Int location, ITilemap map, ref TileData data) {
            var h = Mathf.Abs((shift.x + location.x - 10000) % (2 * scale)) / scale;
            var v = Mathf.Abs((shift.y + location.y - 10000) % (2 * scale)) / scale;
            bool l = (h ^ v) == 1;
            ElangTile tile = l ? left : right;
            if (tile != null) {
                data.sprite = tile.GetSprite(location, map, quirks);
                data.colliderType = quirks.colliderType;
            }
        }
#if UNITY_EDITOR
        public override Sprite Thumbnail { get { return (left != null) ? left.Thumbnail : null; } }
#endif
    }
}
