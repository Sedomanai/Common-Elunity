using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Elang
{
    [System.Serializable]
    public class RandomTileData
    {
        public ElangTile tile;
        public float weight;
    }


    // Random tile. Weighted.
    // 랜덤 타일. 가중치 적용 가능. 동영상 참조.
#if UNITY_EDITOR
    [CreateAssetMenu(fileName = "Random Tile", menuName = "Elang Tile/Random Tile", order = 13)]
#endif
    public class RandomTile : ElangTile
    {
        public List<RandomTileData> list = new List<RandomTileData>();

        public override Sprite GetSprite(Vector3Int location, ITilemap map, TileQuirks quirks) {
            float fullWeight = 0;
            for (int i = 0; i < list.Count; i++) {
                fullWeight += list[i].weight;
            }

            var point = Random.Range(0, fullWeight);
            float left = 0.0f, right = 0.0f;
            for (int i = 0; i < list.Count; i++) {
                var dat = list[i];
                right += dat.weight;
                if (left < point && point < right) {
                    return dat.tile.GetSprite(location, map, quirks);
                } left += dat.weight;
            } return null;
        }
#if UNITY_EDITOR
        public override Sprite Thumbnail { get { return (list.Count > 0 && list[0] != null) ? list[0].tile.Thumbnail : null; } }
      
#endif
    }
}
