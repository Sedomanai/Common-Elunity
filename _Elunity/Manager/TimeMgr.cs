using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;

namespace Elang {
    /// <summary>
    /// <br> Singleton that manages everything related to Unity time. </br>
    /// <br> Also contains interface for global stop watches. </br>
    /// </summary>
    public class TimeMgr : Singleton<TimeMgr> {
        Stopwatch _watch;
        Dictionary<string, Stopwatch> _watches = new Dictionary<string, Stopwatch>();

        [SerializeField]
        float _timeScale = 0.0f;

        protected override void Init() {
            name = "[Timer Manager]";
        }

        public bool Paused {
            get { return _timeScale == 0.0f; }
        }
        public float TimeScale {
            get { return _timeScale; }
        }
        public void PauseGame() {
            _timeScale = 0.0f;
        }
        public void UnpauseGame() {
            _timeScale = 1.0f;
        }

        public void StartTimer(string name) {
            if (!_watches.ContainsKey(name)) {
                _watches[name] = new Stopwatch();
            }

            _watch = _watches[name];
            _watch.Reset();
            _watch.Start();
        }

        public long EndTimer(string name, bool debug) {
            if (!_watches.ContainsKey(name))
                return -1;

            _watch = _watches[name];
            _watch.Stop();

            if (debug) {
                UnityEngine.Debug.Log(name + ": "
                    + "\n\t Milliseconds: " + _watch.ElapsedMilliseconds
                    + "\t Ticks: " + _watch.ElapsedTicks);
            }

            return _watch.ElapsedTicks;
        }
    }
}