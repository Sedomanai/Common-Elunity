using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;


namespace Elang
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
        float transitionDelay;
        [SerializeField]
        string nextScene;

        void Start() {
            Image curtain = GetComponentInChildren<Image>();
            fadeIn.SetupImage(curtain);
            fadeOut.SetupImage(curtain);
            if (preserveRatio)
                TransitionEvent.PreserveRatio(curtain);
            if (fadeInImmediately)
                FadeIn();

            if (!fadeIn.referenceCamera)
                fadeIn.referenceCamera = GetComponentInChildren<Camera>();
            if (!fadeOut.referenceCamera)
                fadeOut.referenceCamera = GetComponentInChildren<Camera>();

        }

        public void FadeIn() {
            StartCoroutine(fadeIn.FadeOutCO(preserveRatio));
        }

        public void FadeOut() {
            StartCoroutine(FadeOutCO());
        }

        IEnumerator FadeOutCO() {
            yield return StartCoroutine(fadeOut.FadeInCO(preserveRatio));
            yield return new WaitForSeconds(transitionDelay);
            SceneManager.LoadSceneAsync(nextScene);

        }
    }
}