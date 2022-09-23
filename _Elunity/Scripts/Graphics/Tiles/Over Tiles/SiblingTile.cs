using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Elang
{
    // Used to create a tile that looks the same but behaves differently. Wallless tiles for example. Refer to TileQuirks.
    // 똑같은 모양의 타일에 다른 TileQuirk를 적용하고 싶을 때 사용. 대표적으로 물리 벽이 없는 타일 등을 위해 사용.
    // 설명이 어려우니 동영상 참조. 또 TileQuirk 참조.
#if UNITY_EDITOR
    [CreateAssetMenu(fileName = "Sibling Tile", menuName= "Elang Tile/Sibling Tile", order = 12)]
#endif
    public class SiblingTile : ElangTile
    {
        [SerializeField]
        ElangTile tile;

        public override void SetTile(Vector3Int location, ITilemap map, ref TileData data) {
            if (tile != null) {
                data.sprite = tile.GetSprite(location, map, quirks);
                data.colliderType = quirks.colliderType;
            }
        }

#if UNITY_EDITOR
        public override Sprite Thumbnail { get { return (tile != null) ? tile.Thumbnail : null; } }
#endif
    }

}