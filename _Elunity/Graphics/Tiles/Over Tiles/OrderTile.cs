using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace Elang
{

    // Ordered tile. Iterates through list, checks if appropriate sprite exists for the given environment, if it doesn't go to the next one.
    // ���� ���� Ÿ��. ����Ʈ ù° Ÿ�Ϻ��� ���� ��ҿ� �˸´� ��������Ʈ�� �������� ������ ���� Ÿ�Ϸ� �Ѿ�� ��������Ʈ ã�⸦ �ݺ�.
    // ������ ������ ������ ����.
#if UNITY_EDITOR
    [CreateAssetMenu(fileName = "Order Tile", menuName = "Elang Tile/Order Tile", order = 13)]
#endif
    public class OrderTile : ElangTile
    {
        public List<ElangTile> list = new List<ElangTile>();

        public override Sprite GetSprite(Vector3Int location, ITilemap map, TileQuirks quirks) {
            for (int i = 0; i < list.Count; i++) {
                var tile = list[i];
                if (tile) {
                    var spr = tile.GetSprite(location, map, quirks);
                    if (spr) return spr;
                }
            } return null;
        }

#if UNITY_EDITOR
        public override Sprite Thumbnail { get { return (list.Count > 0 && list[0] != null) ? list[0].Thumbnail : null; } }

#endif
    }
}
