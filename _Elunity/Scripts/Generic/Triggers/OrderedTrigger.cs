using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Collections;
using UnityEditor;


namespace Elang
{
    public class OrderedTrigger : MonoBehaviour
    {
        [System.Serializable]
        public class Unit
        {
            public float preSeconds, postSeconds;
            public UnityEvent onWaitEnded;
        }

        public List<Unit> units;
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
            StartCoroutine(TriggerCO());
        }

        IEnumerator TriggerCO() {
            for (int i = 0; i < units.Count; i++) {
                var unit = units[i];
                yield return new WaitForSeconds(unit.preSeconds);
                unit.onWaitEnded.Invoke();
                yield return new WaitForSeconds(unit.postSeconds);
            }
        }
    }
}