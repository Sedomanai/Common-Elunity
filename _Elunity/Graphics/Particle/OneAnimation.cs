using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elang
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Animator))]
    public class OneAnimation : MonoBehaviour
    {
        [SerializeField]
        string animationState;
       
        void OnEnable() {
            if (animationState != "")
                GetComponent<Animator>().Play(animationState);
        }

        public void Disable() {
            gameObject.SetActive(false);
        }
    }
}