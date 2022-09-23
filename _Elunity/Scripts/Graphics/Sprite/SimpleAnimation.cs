using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Events;

namespace Elang
{

    [RequireComponent(typeof(Animator))]
    public class SimpleAnimation : MonoBehaviour
    {
        [SerializeField]
        string state;
        public eAutomation automationType;

        Animator _anim;

        void Awake() {
            _anim = GetComponent<Animator>();

            if (automationType == eAutomation.OnAwake)
                _anim.Play(state);
        }

        void OnEnable() {
            if (automationType == eAutomation.OnEnable)
                _anim.Play(state);
        }
    }
}
