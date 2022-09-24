using UnityEngine;
using System.Collections;

/*
 * 
 * Creates particles that ignore time
 * 
 */
namespace Elang {
    public class TimelessParticle : MonoBehaviour {
        ParticleSystem _ps;

        void Awake() {
            _ps = GetComponent<ParticleSystem>();
        }

        void Update() {
            if (Time.timeScale < 0.01f) {
                _ps.Simulate(Time.unscaledDeltaTime, true, false);
            }
        }
    }
}