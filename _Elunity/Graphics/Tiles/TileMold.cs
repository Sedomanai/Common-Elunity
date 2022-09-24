using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Elang
{

    // 일랑 타일 제작 타입
    public enum eElangTileType
    {
        Single,
        Base,
        Slope,
        Pillar,
        Block,
        Random,
    };

    // 일랑 타일 맞춤 플래그
    public enum eElangTileFlags
    {
        Nothing,
        SuppressFull,
        SuppressSurface
    };


    // Mold to create an ElangTile based on given (Sprite) material and various data
    // 일랑 타일 제작 틀. material 스프라이트에서 맞춤 제작 해준다
    [System.Serializable]
    public class TileMold : ScriptableObject
    {
        public ElangTile tile;
        public string assetName;
        public Sprite material;
        public eElangTileType tileType = eElangTileType.Base;
        public eElangTileFlags tileFlags = eElangTileFlags.Nothing;
    }
}