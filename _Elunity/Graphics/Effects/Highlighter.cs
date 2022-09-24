using UnityEngine;
using System.Collections;

/*
 * 반짝임 색 변조 등 스프라이트 하이라이트 효과
 * 
 * Sprite GFX
 * May become obsolete with Shaders
 * 
 */

namespace Elang {

    public class Highlighter : MonoBehaviour {

        enum eStartType
        {
            None,
            Linear,
            Flash,
            Sine
        }

        [SerializeField]
        eStartType _type;
        [SerializeField]
        Color _color;
        [SerializeField]
        float _time = 0.5f;

        Renderer[] _rends;

        Coroutine _activeHighlight;
        void Awake() {
            _rends = GetComponentsInChildren<Renderer>();

            switch (_type) {
            case eStartType.Linear:
                Linear(_color, _time);
                break;
            case eStartType.Flash:
                Flash(_color, _time);
                break;
            case eStartType.Sine:
                Sine(_color, _time, Color.black);
                break;
            }

        }

        public void Basic(Color color) {
            Stop();
            ChangeColor(color);
        }

        public void SetTimer(float f) {
            StartCoroutine(WaitSeconds(f));
        }

        public void Linear(Color color, float interval) {
            Stop();
            _activeHighlight = StartCoroutine(LinearCoroutine(color, interval));
        }

        // 반짝임
        public void Flash(Color color, float interval) {
            Stop();
            _activeHighlight = StartCoroutine(FlashCoroutine(color, interval));
        }

        // Sine
        public void Sine(Color color, float speed, Color addColor, bool ignoreAlpha = true) {
            Stop();
            _activeHighlight = StartCoroutine(SineCoroutine(color, speed, addColor, ignoreAlpha));
        }

        public void Stop() {
            if (_activeHighlight != null)
                StopCoroutine(_activeHighlight);
            ChangeColor(Color.white);
        }

        void ChangeColor(Color color) {
            foreach (Renderer rend in _rends) {
                rend.material.color = color;
            }
        }

        IEnumerator WaitSeconds(float f) {
            yield return new WaitForSeconds(f);
            Stop();
        }

        IEnumerator FlashCoroutine(Color color, float interval) {
            while (true) {
                yield return new WaitForSeconds(interval);
                ChangeColor(color);
                yield return new WaitForSeconds(interval);
                ChangeColor(Color.white);
            }
        }

        IEnumerator LinearCoroutine(Color color, float interval) {
            float a, r, b, g;
            while (true) {
                foreach (Renderer rend in _rends) {
                    a = Mathf.MoveTowards(rend.material.color.a, color.a, interval * Time.deltaTime);
                    r = Mathf.MoveTowards(rend.material.color.r, color.r, interval * Time.deltaTime);
                    b = Mathf.MoveTowards(rend.material.color.b, color.b, interval * Time.deltaTime);
                    g = Mathf.MoveTowards(rend.material.color.g, color.g, interval * Time.deltaTime);
                    rend.material.color = new Color(r, g, b, a);
                }

                yield return null;
            }
        }

        IEnumerator SineCoroutine(Color color, float speed, Color addColor, bool ignoreAlpha = true) {
            color.r = 1 - color.r;
            color.b = 1 - color.b;
            color.g = 1 - color.g;
            color.a = ignoreAlpha ? 0 : color.a;

            while (true) {
                float wave = (Mathf.Sin(Time.time * speed) + 1.0f) / 4;
                Color c = Color.white - color * wave + addColor;
                ChangeColor(c);
                yield return null;
            }
        }
    }
}