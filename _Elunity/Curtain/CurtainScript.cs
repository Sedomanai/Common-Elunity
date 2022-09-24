using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;


namespace Ilang
{
    public class CurtainScript : MonoBehaviour
    {

        [SerializeField]
        bool fadeInImmediately = true;
        [SerializeField]
        bool preserveRatio = false;

        [SerializeField]
        TransitionEvent fadeIn;
        [SerializeField]
        TransitionEvent fadeOut;

        [SerializeField]
        float transitionDelaySeconds;
        [SerializeField]
        string nextScene;

        void Start() {
            Image curtain = GetComponentInChildren<Image>();

            var tr = curtain.GetComponent<RectTransform>();
            if (Screen.width > Screen.height) {
                tr.localScale = new Vector3(tr.localScale.x, (float)Screen.width / (float)Screen.height, tr.localScale.z);
            } else {
                tr.localScale = new Vector3((float)Screen.height / (float)Screen.width, tr.localScale.y, tr.localScale.z);
            }

            fadeIn.SetupImage(curtain);
            fadeOut.SetupImage(curtain);
            if (preserveRatio)
                TransitionEvent.PreserveRatio(curtain);

            if (fadeInImmediately)
                FadeIn();
        }

        public void FadeIn() {
            StartCoroutine(fadeIn.FadeOutCO(preserveRatio));
        }

        public void FadeOut() {
            StartCoroutine(FadeOutCO());
        }

        IEnumerator FadeOutCO() {
            yield return StartCoroutine(fadeOut.FadeInCO(preserveRatio));
            //yield return new WaitForSeconds(transitionDelaySeconds);
            SceneManager.LoadSceneAsync(nextScene);

        }
    }
}