using System.Collections;
using UnityEngine;

namespace Elang
{
    [CreateAssetMenu(fileName = "Transition Effect", menuName = "Elang UI/Transition", order = 12)]
    public class TransitionEffect : ScriptableObject {
        public float duration = 1.0f;

        [Header("Cutoff")]
        public bool useCutoff = false;
        public bool cutoffInversion = false;
        public float cutoffSmoothFactor = 1.0f;
        public Texture2D cutoffTexture;

        [Header("Stencil Mask")]
        public bool useMask = false;
        public float maskMaxScale = 3.0f;
        public Texture2D maskTexture;
        public Transform maskPivot;

        [Header("Particles")]
        [SerializeField]
        public bool useParticles = false;
        public ParticleSystem particles;

        [Header("Animation")]
        [SerializeField]
        public bool useAnimation = false;
        public Animator animator;
        public AnimationClip animation;


        float _cutoff = 0.0f;
        //public float cutoff { get; }
        public void ReadyTransition(Material mat) {
            if (mat) {
                mat.SetFloat("_Slider", 1.0f);
                if (useCutoff) {
                    mat.SetTexture("_AlphaTex", cutoffTexture);
                    mat.SetFloat("_Inversion", cutoffInversion ? 1.0f : 0.0f);
                    mat.SetFloat("_Smooth", cutoffSmoothFactor);
                }

                if (useMask) {
                    mat.SetTexture("_MaskTex", maskTexture);
                    mat.SetFloat("_MaskMaxScale", maskMaxScale);
                    mat.SetVector("_MaskOffset", new Vector2(0, 0));
                } else {
                    mat.SetTexture("_MaskTex", null);
                }
            }

            if (useParticles) {
                particles.gameObject.SetActive(true);
                var main = particles.main;
                main.simulationSpeed = 1.0f / duration;
                particles.Play();
            }

            if (useAnimation) {
                animator.speed = 1.0f / duration;
                animator.Play(animation.name, 0);
            }
        }


        public IEnumerator FadeOutEffect(Material mat, Camera cam, bool preserveRatio) {
            if (useCutoff || useMask) {
                _cutoff = 1.0f;
                if (mat)
                    mat.SetFloat("_Slider", _cutoff);
                yield return new WaitForSeconds(0.01f);

                while (true) {
                    if (mat) {
                        mat.SetFloat("_Slider", _cutoff);
                        if (maskPivot) {
                            mat.SetVector("_MaskOffset", OffsetPoint(maskPivot, cam, preserveRatio));
                        }
                    }

                    if (_cutoff < 0.0f) {
                        _cutoff = 0.0f;
                        break;
                    }
                    yield return null;
                    _cutoff -= (Time.deltaTime / duration);
                }
            } else
                yield return new WaitForSeconds(duration);

            StopParticles();
        }


        public IEnumerator FadeInEffect(Material mat, Camera cam, bool preserveRatio) {
            if (useCutoff || useMask) {
                _cutoff = 0.0f;
                if (mat)
                    mat.SetFloat("_Slider", _cutoff);
                yield return new WaitForSeconds(0.01f);

                while (true) {
                    if (mat) {
                        mat.SetFloat("_Slider", _cutoff);
                        if (maskPivot) {
                            mat.SetVector("_MaskOffset", OffsetPoint(maskPivot, cam, preserveRatio));
                        }
                    }

                    if (_cutoff > 1.0f) {
                        _cutoff = 1.0f;
                        break;
                    }
                    yield return null;
                    _cutoff += (Time.deltaTime / duration);
                }
            } else
                yield return new WaitForSeconds(duration);

            StopParticles();
        }

        void StopParticles() {
            if (useParticles) {
                particles.Stop();
                particles.Clear();
                particles.gameObject.SetActive(false);
            }
        }

        public IEnumerator TransitionWait() {
            yield return new WaitForSeconds(duration);
            StopParticles();
        }

        Vector2 OffsetPoint(Transform tr, Camera cam, bool preserveRatio = false) {
            Vector3 screenPos = cam.WorldToScreenPoint(tr.position);

            float yFactor = (preserveRatio && (Screen.width > Screen.height)) ?
                          (float)Screen.height / (float)Screen.width : 1.0f;

            screenPos.x = (screenPos.x - Screen.width * 0.5f) / Screen.width;
            screenPos.y = (screenPos.y - Screen.height * 0.5f) * yFactor / Screen.height;

            return screenPos;
        }
    }

}