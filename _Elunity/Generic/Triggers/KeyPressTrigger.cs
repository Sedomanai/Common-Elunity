using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace Elang
{
    /// <summary>
    /// 
    /// </summary>
    public class KeyPressTrigger : MonoBehaviour
    {
        public KeyCode key;
        public float intervalSeconds;
        public bool holdKey;
        public UnityEvent OnKeyPress;

        float _seconds = 0.0f;

        void Update() {
            _seconds += Time.deltaTime;
            if (_seconds > intervalSeconds) {
                _seconds = intervalSeconds;
            }

            if (_seconds == intervalSeconds && !TimeMgr.Instance.Paused) {
                if (holdKey) {
                    if (Input.GetKey(key)) { 
                        OnKeyPress.Invoke();
                        _seconds = 0.0f;
                    }
                } else if (Input.GetKeyDown(key)) {
                    OnKeyPress.Invoke();
                    _seconds = 0.0f;
                }
            }
        }

    }
}