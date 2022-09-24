using UnityEngine;
/*
 * 
 * Not sure if I need this..
 * 
 */
namespace Elang {
    public class BasicParticle : MonoBehaviour {
        ParticleSystem _system;

        // Use this for initialization
        void Awake() {
            _system = GetComponent<ParticleSystem>();
        }

        // Update is called once per frame
        void Update() {
            if (!_system.main.loop &&
                !_system.IsAlive()) {
                gameObject.SetActive(false);
            }
        }
    }
}