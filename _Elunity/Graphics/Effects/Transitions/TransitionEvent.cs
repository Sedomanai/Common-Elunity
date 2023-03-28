using Elang;
using System;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Elang
{
    [Serializable]
    public class TransitionEvent {
        [SerializeField]
        TransitionEffect _effect;
        [SerializeField]
        AnimationClip _animClip;

        public Camera referenceCamera;

        [Header("Events")]
        [Space(10)]
        public UnityEvent beginAction;
        [SerializeField]
        public UnityEvent endAction;


        Image _image;
        ParticleSystem _particles;
        Animator _anim;
        Material _mat;
        Transform _maskPivot;

        public void Setup(Image image, Transform maskPivot = null, ParticleSystem particles = null, Animator anim = null) {
            this._image = image;
            this._particles = particles;
            this._maskPivot = maskPivot;
            this._anim = anim;
            _mat = image.material;
        }

        public static void PreserveRatio(Image image) {
            var tr = image.GetComponent<RectTransform>();
            if (Screen.width > Screen.height) {
                tr.localScale = new Vector3(tr.localScale.x, (float)Screen.width / (float)Screen.height, tr.localScale.z);
            } else {
                tr.localScale = new Vector3((float)Screen.height / (float)Screen.width, tr.localScale.y, tr.localScale.z);
            }
        }
        void ReadyTransition() {
            beginAction.Invoke();
            _effect.ReadyTransition(_mat, _particles, _anim, _animClip);
        }

        public IEnumerator FadeOutCO(bool preserveRatio = false) {
            ReadyTransition();
            _image.enabled = true;
            yield return _effect.FadeOutEffect(_mat, referenceCamera, preserveRatio, _maskPivot);
            _image.enabled = false;
            yield return new WaitForSeconds(0.02f);
            endAction.Invoke();
        }

        public IEnumerator FadeInCO(bool preserveRatio = false) {
            ReadyTransition();
            _image.enabled = true;
            yield return _effect.FadeInEffect(_mat, referenceCamera, preserveRatio, _maskPivot);
            yield return new WaitForSeconds(0.02f);
            endAction.Invoke();
        }

        public IEnumerator TransitionCO() {
            ReadyTransition();
            yield return _effect.TransitionWait();
            endAction.Invoke();
        }
    }
}