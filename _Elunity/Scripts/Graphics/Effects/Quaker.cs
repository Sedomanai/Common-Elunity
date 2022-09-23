using UnityEngine;
using System.Collections;
using DG.Tweening;
using System;

namespace Elang {
    public class Quaker : MonoBehaviour {
        [SerializeField]
        float _magnitude = 0.5f, _duration = 1.0f;

        [SerializeField]

#if UNITY_EDITOR
        [Boolean(3)]
#endif
        byte constraints;

        public float Magnitude { get { return _magnitude; } set { _magnitude = value; } }
        public float Duration { get { return _duration; } set { _duration = value; } }

        public void StartQuate() {
            //DOTween.To()
            //Tween tween;
            //tween.SetEase(Ease.InOutElastic);
            //transform.DOShakePosition(_duration, _magnitude);
            StartCoroutine(BeginQuake());
        }
        public void StopQuake() {
            StopCoroutine(BeginQuake());
        }

        IEnumerator BeginQuake() {
            Vector3 origin = transform.localPosition;
            float t = 0;
            while (t < _duration) {
                Vector3 fixedMagnitude = UnityEngine.Random.insideUnitSphere * _magnitude;

                fixedMagnitude.x = (1 == (constraints & 1)) ? 0 : fixedMagnitude.x;
                fixedMagnitude.y = (2 == (constraints & 2)) ? 0 : fixedMagnitude.y;
                fixedMagnitude.z = (4 == (constraints & 4)) ? 0 : fixedMagnitude.z;

                transform.localPosition = origin + fixedMagnitude;
                t += Time.deltaTime;
                yield return null;
            }

            transform.localPosition = origin;
        }
    }
}