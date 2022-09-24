using System;
using UnityEditor;
using UnityEngine;

namespace Elang
{
    [Serializable]
    public class AtlasCellMeta
    {
        public AtlasCellMeta(SerializeStream stream) {
            stream
                .parse(out name)
                .parse(out x).parse(out y)
                .parse(out w).parse(out h)
                .parse(out oX).parse(out oY);
        }

        public SpriteMetaData mold(int atlasHeight) {
            var data = new SpriteMetaData();
            data.alignment = (int)SpriteAlignment.Custom;
            data.name = name;
            data.rect = new Rect(x, atlasHeight - h - y, w, h);
            data.pivot = new Vector2(
              (float)((oX) / (double)w),
              (float)((h - oY) / (double)h)
            );
            return data;
        }

        [SerializeField]
        string name;
        [SerializeField]
        Int16 x, y, w, h, oX, oY;
    }
}
