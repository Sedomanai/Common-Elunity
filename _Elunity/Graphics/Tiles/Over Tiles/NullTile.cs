using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Elang
{
    // Null tile. No sprites, just tile quirks.
    // 투명 타일. TileQuirks만 존재. 보이지 않는 물리 벽 등을 만들 거나 할 때 사용.
#if UNITY_EDITOR
    [CreateAssetMenu(fileName = "Sibling Tile", menuName = "Elang Tile/Sibling Tile", order = 12)]
#endif
    public class NullTile : ElangTile
    {
        public override void SetTile(Vector3Int location, ITilemap map, ref TileData data) {
            data.color = Color.cyan;
            data.colliderType = quirks.colliderType;
        }

#if UNITY_EDITOR
        public override Sprite Thumbnail { get { return null; } }
#endif
    }

}