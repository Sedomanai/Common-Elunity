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
    public class TransitionEvent
    {
        [SerializeField]
        TransitionEffect _effect;

        public Camera referenceCamera;

        [Header("Events")]
        [Space(10)]
        public UnityEvent beginAction;
        [SerializeField]
        public UnityEvent endAction;

        Image image;
        Material mat;

        void ReadyTransition() {
            beginAction.Invoke();
            _effect.ReadyTransition(mat);
        }

        public static void PreserveRatio(Image image) {
            var tr = image.GetComponent<RectTransform>();
            if (Screen.width > Screen.height) {
                tr.localScale = new Vector3(tr.localScale.x, (float)Screen.width / (float)Screen.height, tr.localScale.z);
            } else {
                tr.localScale = new Vector3((float)Screen.height / (float)Screen.width, tr.localScale.y, tr.localScale.z);
            }
        }

        public void SetupImage(Image image) {
            this.image = image;
            mat = image.material;
        }

        public IEnumerator FadeOutCO(bool preserveRatio = false) {
            ReadyTransition();
            image.enabled = true;
            yield return _effect.FadeOutEffect(mat, referenceCamera, preserveRatio);
            image.enabled = false;
            yield return new WaitForSeconds(0.02f);
            endAction.Invoke();
        }

        public IEnumerator FadeInCO(bool preserveRatio = false) {
            ReadyTransition();
            image.enabled = true;
            yield return _effect.FadeInEffect(mat, referenceCamera, preserveRatio);
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