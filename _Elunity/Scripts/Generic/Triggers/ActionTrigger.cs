using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace Elang {

    /// <summary>
    /// <br> Triggers a UnityEvent after a given time in seconds.</br>
    /// <br> Can also be set to trigger automatically upon awake, enable, or disable. </br>
    /// </summary>
    public class ActionTrigger : MonoBehaviour
    {
        public float waitSeconds;
        public UnityEvent onWaitEnded;
        public eAutomation automateOption;

        void Awake() {
            if (automateOption == eAutomation.OnAwake) {
                Trigger();
            }
        }

        void OnEnable() {
            if (automateOption == eAutomation.OnEnable) {
                Trigger();
            }
        }

        void OnDisable() {
            if (automateOption == eAutomation.OnDisable) {
                Trigger();
            }
        }

        public void Trigger() {
            StartCoroutine(CoToggle());
        }

        IEnumerator CoToggle() {
            yield return new WaitForSeconds(waitSeconds);
            onWaitEnded.Invoke();
        }
    }
}