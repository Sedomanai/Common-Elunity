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
        Transform _maskPivot;
        [SerializeField]
        Camera _maskReferenceCamera;

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
            Animator anim = GetComponentInChildren<Animator>();
            ParticleSystem particles = GetComponentInChildren<ParticleSystem>();

            fadeIn.Setup(curtain, particles, anim);
            fadeOut.Setup(curtain, particles, anim);
            if (preserveRatio)
                TransitionEvent.PreserveRatio(curtain);
            if (fadeInImmediately)
                FadeIn();
        }

        public void FadeIn() {
            StartCoroutine(fadeIn.FadeOutCO(preserveRatio, _maskReferenceCamera, _maskPivot));
        }

        public void FadeOut() {
            StartCoroutine(FadeOutCO());
        }

        IEnumerator FadeOutCO() {
            yield return StartCoroutine(fadeOut.FadeInCO(preserveRatio, _maskReferenceCamera, _maskPivot));
            yield return new WaitForSeconds(transitionDelay);
            SceneManager.LoadSceneAsync(nextScene);

        }
    }
}