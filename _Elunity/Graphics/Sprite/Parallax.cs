using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/*
 * 배경 시차적용(Parallax) 
 * 
 * 기존 스크립트 명칭만 바꾸고 그대로 사용
 * _rate 변수를 추가해서 속도 배수를 쉽게 바꿀 수 있게 만들었다
 * 
 */

namespace Elang {
    public class Parallax : MonoBehaviour {
        [SerializeField]
        Vector2 _camSyncRate = new Vector2(1.0f, 1.0f);
        [SerializeField]
        Vector2 _autoScrollRate = new Vector2(0.0f, 0.0f);

        [SerializeField]
        Material _defaultMaterial;

        Mesh _mesh;
        Renderer _rend;
        Material _mat;
        Texture2D _tex;

        Rect _sprr;
        Vector2 _uvMult;

        Vector2 _offset, _multi;
        float hw, hh;

        void Awake() {
            var sprite = GetComponent<SpriteRenderer>().sprite;
            DestroyImmediate(GetComponent<SpriteRenderer>());

           //makePlane(sprite);
           makeMesh(sprite);


            _mat = new Material(_defaultMaterial);
            _mat.SetTexture("_MainTex", _tex);
            _rend.material = _mat;

            _multi = new Vector2(1.0f / sprite.bounds.size.x, 1.0f / sprite.bounds.size.y);
        }

        void makePlane(Sprite sprite) {
            var plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            plane.transform.SetParent(transform);
            _rend = plane.GetComponent<Renderer>();

            var targetSize = sprite.bounds.size;
            plane.transform.eulerAngles = new Vector3(90, 180, 0);
            plane.transform.localScale = new Vector3(targetSize.x / _rend.bounds.size.x, 1, targetSize.y / _rend.bounds.size.y);


            var rect = sprite.rect;
            _tex = new Texture2D((int)Mathf.Round(rect.width), (int)Mathf.Round(rect.height));
            Color[] colors = sprite.texture.GetPixels(
                (int)Mathf.Round(rect.x),
                (int)Mathf.Round(rect.y),
                (int)Mathf.Round(rect.width),
                (int)Mathf.Round(rect.height)
            );
            _tex.SetPixels(colors);
            _tex.filterMode = FilterMode.Point;
            _tex.wrapMode = TextureWrapMode.Repeat;
            _tex.anisoLevel = 0;
            _tex.Apply();
        }

        void makeMesh(Sprite sprite) {
            hw = sprite.rect.width / 2.0f / sprite.pixelsPerUnit;
            hh = sprite.rect.height / 2.0f / sprite.pixelsPerUnit;

            _mesh = new Mesh();
            _mesh.name = "Parallax Background";
            _mesh.vertices = new Vector3[]{
                new Vector3(-hw, -hh, 0.01f), new Vector3(hw, -hh, 0.01f), new Vector3(hw, hh, 0.01f), new Vector3(-hw, hh, 0.01f), // lb
                new Vector3(0, 0, 0.01f), new Vector3(0, 0, 0.01f), new Vector3(0, 0, 0.01f), new Vector3(0, 0, 0.01f),   // rb
                new Vector3(0, 0, 0.01f), new Vector3(0, 0, 0.01f), new Vector3(0, 0, 0.01f), new Vector3(0, 0, 0.01f),   // lt
                new Vector3(0, 0, 0.01f), new Vector3(0, 0, 0.01f), new Vector3(0, 0, 0.01f), new Vector3(0, 0, 0.01f),     // rt
            };

            _uvMult.x = sprite.rect.width / sprite.texture.width;
            _uvMult.y = sprite.rect.height / sprite.texture.height;
            var left = sprite.rect.x / sprite.texture.width;
            var right = left + _uvMult.x;
            var bottom = sprite.rect.y / sprite.texture.height;
            var top = bottom + _uvMult.y;
            _sprr = new Rect();
            _sprr.xMin = left;
            _sprr.xMax = right;
            _sprr.yMin = bottom;
            _sprr.yMax = top;

            _mesh.uv = new Vector2[]{
                 new Vector2(left, bottom), new Vector2(right, bottom), new Vector2(right, top), new Vector2(left, top),
                 Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero,
                 Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero,
                 Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero
            };
            _mesh.triangles = new int[] { 0, 1, 2, 0, 2, 3, 4, 5, 6, 4, 6, 7, 8, 9, 10, 8, 10, 11, 12, 13, 14, 12, 14, 15 };
            _mesh.RecalculateNormals();

            _rend = gameObject.AddComponent<MeshRenderer>();
            gameObject.AddComponent<MeshFilter>().mesh = _mesh;

            _tex = sprite.texture;
        }

        void LateUpdate() {
            var cpos = Camera.main.transform.position;
            _offset = _multi * new Vector2(cpos.x, cpos.y) * _camSyncRate;
            _offset += _autoScrollRate * Time.deltaTime;
            setOffset();
        }

        // I still don't know how I did it...
        void setOffset() {
            float fracX = _offset.x % 1;
            fracX = (fracX < 0) ? 1.0f + fracX : fracX;
            float fracY = _offset.y % 1;
            fracY = (fracY < 0) ? 1.0f + fracY : fracY;

            float mx = hw - _rend.bounds.size.x * fracX;
            float my = hh - _rend.bounds.size.y * fracY;

            _mesh.vertices = new Vector3[]{
                new Vector3(-hw, -hh, 0.01f), new Vector3(mx, -hh, 0.01f), new Vector3(mx, my, 0.01f), new Vector3(-hw, my, 0.01f), // lb
                new Vector3(mx, -hh, 0.01f), new Vector3(hw, -hh, 0.01f), new Vector3(hw, my, 0.01f), new Vector3(mx, my, 0.01f),   // rb
                new Vector3(-hw, my, 0.01f), new Vector3(mx, my, 0.01f), new Vector3(mx, hh, 0.01f), new Vector3(-hw, hh, 0.01f),   // lt
                new Vector3(mx, my, 0.01f), new Vector3(hw, my, 0.01f), new Vector3(hw, hh, 0.01f), new Vector3(mx, hh, 0.01f),     // rt
            };

            float left = _sprr.xMin;
            float right = _sprr.xMax;
            float bottom = _sprr.yMin;
            float top = _sprr.yMax;
            float midx = left + _uvMult.x * fracX;
            float midy = bottom + _uvMult.y * fracY;

            _mesh.uv = new Vector2[]{
                new Vector2(midx, midy), new Vector2(right, midy), new Vector2(right, top), new Vector2(midx, top),
                new Vector2(left, midy), new Vector2(midx, midy), new Vector2(midx, top), new Vector2(left, top),
                new Vector2(midx, bottom), new Vector2(right, bottom), new Vector2(right, midy), new Vector2(midx, midy),
                new Vector2(left, bottom), new Vector2(midx, bottom), new Vector2(midx, midy), new Vector2(left, midy),
            };
        }

        void OnDestroy() {
            DestroyImmediate(_mat);
        }
    }
}