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

        [SerializeField]
        Transform _maskPivot;

        void Start() {
            Image curtain = GetComponentInChildren<Image>();
            Animator anim = GetComponentInChildren<Animator>();
            ParticleSystem particles = GetComponentInChildren<ParticleSystem>();

            fadeIn.Setup(curtain, _maskPivot, particles, anim);
            fadeOut.Setup(curtain, _maskPivot, particles, anim);
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