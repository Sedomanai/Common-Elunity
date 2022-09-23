using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Elang
{
    // Algorithms to deduce neighbor data, tile piece index, etc.
    public class TileAlgorithm
    {
        public static char NeighborBit(Vector3Int location, ITilemap map, ElangTile.TileQuirks quirks) {
            char neighbors = NeighborIsSelf(location, new Vector3Int(0, 1, 0), map, quirks) ? (char)2 : (char)0;
            neighbors += NeighborIsSelf(location, new Vector3Int(-1, 0, 0), map, quirks) ? (char)8 : (char)0;
            neighbors += NeighborIsSelf(location, new Vector3Int(1, 0, 0), map, quirks) ? (char)16 : (char)0;
            neighbors += NeighborIsSelf(location, new Vector3Int(0, -1, 0), map, quirks) ? (char)64 : (char)0;
            if (!quirks.pipe) {
                neighbors += NeighborIsSelf(location, new Vector3Int(-1, 1, 0), map, quirks) ? (char)1 : (char)0;
                neighbors += NeighborIsSelf(location, new Vector3Int(1, 1, 0), map, quirks) ? (char)4 : (char)0;
                neighbors += NeighborIsSelf(location, new Vector3Int(-1, -1, 0), map, quirks) ? (char)32 : (char)0;
                neighbors += NeighborIsSelf(location, new Vector3Int(1, -1, 0), map, quirks) ? (char)128 : (char)0;
            }
            return neighbors;
        }

        public static bool NeighborIsSelf(Vector3Int location, Vector3Int direction, ITilemap map, ElangTile.TileQuirks quirks) {
            var neighbor = map.GetTile<ElangTile>(location + direction);
            if (neighbor && neighbor.quirks.index == quirks.index) {
                return (quirks.countSubIndex) ? (neighbor.quirks.subIndex == quirks.subIndex) : true;
            } return false;
        }

        public static int CornerTypeOf(int piece, int cornerIndex) {
            return (piece >> (cornerIndex * 3)) % 8;
        }
        public static int GetPiece(char bit) {
            int piece = 0;
            piece += GetCornerPiece(2, 8, 1, 1, bit);
            piece += GetCornerPiece(2, 16, 4, 8, bit);
            piece += GetCornerPiece(64, 8, 32, 64, bit);
            piece += GetCornerPiece(64, 16, 128, 512, bit);
            return piece;
        }

        public static int GetCornerPiece(int h, int v, int d, int cp, char b) {
            bool hh = ((b & h) == h);
            bool vv = ((b & v) == v);
            bool dd = ((b & d) == d);

            if (hh) {
                if (vv) {
                    if (!dd)
                        return 4 * cp;
                    else return 5 * cp;
                } else
                    return 2 * cp;
            } else {
                if (vv) {
                    return 3 * cp;
                } else return cp;
            }
        }
    }
}
