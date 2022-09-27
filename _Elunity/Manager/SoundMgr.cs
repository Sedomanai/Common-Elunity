using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
 
namespace Elang {

    /// <summary>
    /// Manages all global sounds like BGM or global SFX (as in most simple 2d games).
    /// </summary>
    public class SoundMgr : Singleton<SoundMgr> {
        /// <summary>
        /// Parsed by a time marker.
        /// </summary>
        public struct TimeMarker
        {
            public string intro;
            public float introStart, introEnd;
            public float loopStart, loopEnd;
        }
        /// <summary>
        /// Wrapper used specifically for looping BGM.
        /// </summary>
        public struct BGMPort {
            AudioSource source1, source2;
            AudioClip clip;
            TimeMarker timeMarker;
            bool isFirstSource;
            bool isIntro;

            public AudioClip Clip {
                get { return clip; }
            }
            public float Volume {
                get { return source1.volume;  }
                set {
                    source1.volume = source2.volume = value;
                }
            }
            public void Init(GameObject parent, int index) {
                GameObject obj = new GameObject("Port_" + index);
                source1 = obj.AddComponent<AudioSource>();
                source2 = obj.AddComponent<AudioSource>();
                obj.transform.SetParent(parent.transform);
                isIntro = false;
            }

            public void SetVolume(float value) {
                source1.volume = source2.volume = value;
            }

            public void Stop() {
                source1.Stop();
                source2.Stop();
                isFirstSource = true;
            }

            public void Play(AudioClip clip_, AudioClip intro, TimeMarker timeMarker_) {
                Stop();
                timeMarker = timeMarker_;
                clip = clip_;
                Volume = 0.3f;

                isIntro = intro ? true : false;
                if (isIntro) {
                    source1.clip = intro;
                    source1.time = timeMarker.introStart;
                } else {
                    source1.clip = clip;
                    source1.time = 0.0f;
                }

                source1.Play();
            }

            public void CheckLoop() {
                if (isFirstSource) {
                    BufferAudio(source1, source2);
                } else {
                    BufferAudio(source2, source1);
                }
            }

            void BufferAudio(AudioSource giver, AudioSource receiver) {
                Vector2 loopPoint = isIntro ? 
                    new Vector2(timeMarker.introStart, timeMarker.introEnd) :
                    new Vector2(timeMarker.loopStart, timeMarker.loopEnd);

                // if timeline reached the end of the audio (without pausing/stopping) then loop
                if (!giver.isPlaying || giver.time >= loopPoint.y) {
                    receiver.volume = giver.volume;
                    receiver.clip = clip;
                    receiver.time = loopPoint.x;
                    receiver.Play();
                    isFirstSource = !isFirstSource;
                    isIntro = false;
                }
            }
        }

        // From root Resources folder (not Asset folder)
        [SerializeField]
        string _SFXDirectory = "SFX/";
        [SerializeField]
        string _BGMDirectory = "BGM/";
        [SerializeField]
        TextAsset _timeMarkerFile;

        float _bgmMax, _sfxMax;
        public float bgmMax { set { _bgmMax = value; _port.SetVolume(0.7f); } get { return _bgmMax; } }
        public float sfxMax { set { _sfxMax = value; _sfxSource.volume = _sfxMax / 500.0f; } get { return _sfxMax; } }

        AudioSource _sfxSource;
        BGMPort _port;
        bool _playing = false;

        Dictionary<string, AudioClip> _sfx = new Dictionary<string, AudioClip>();
        Dictionary<string, TimeMarker> _timeMarkers = new Dictionary<string, TimeMarker>();

        void surefirePath(ref string dir) {
            var back = dir[dir.Length - 1];
            if (back != '/') {
                if (back == '\\') {
                    dir = dir.Substring(0, dir.Length - 1) + '/';
                } else {
                    dir += '/';
                }
            }
        }

        override protected void Init() {
            gameObject.name = "[Sound Manager]";
            transform.position = Camera.main.transform.position;
            surefirePath(ref _BGMDirectory);
            surefirePath(ref _SFXDirectory);
            _sfxSource = gameObject.AddComponent<AudioSource>();
            _port.Init(gameObject, 0);

            var sfx = Resources.LoadAll<AudioClip>(_SFXDirectory);
            for (uint i = 0; i < sfx.Length; i++) {
                _sfx.Add(sfx[i].name, sfx[i]);
            }

            ParseMarker();
            SceneManager.sceneLoaded += AdjustToCamera;
        }

        void ParseMarker() {
            if (!_timeMarkerFile)
                return;

            var lines = _timeMarkerFile.text.Split('\n');
            TimeMarker marker = new TimeMarker();

            for (uint i = 0; i < lines.Length; i++) {
                if (lines[i].Length > 0) {
                    var tokens = lines[i].Split(' ');
                    for (uint j = 0; j < tokens.Length; j++) {
                        var pair = tokens[j].Split('=');
                        switch (pair[0]) {
                            case "LoopStart":
                                marker.loopStart = float.Parse(pair[1]);
                                break;
                            case "LoopEnd":
                                marker.loopEnd = float.Parse(pair[1]);
                                break;
                            case "Intro":
                                marker.intro = pair[1];
                                break;
                            case "IntroStart":
                                marker.introStart = float.Parse(pair[1]);
                                break;
                            case "IntroEnd":
                                marker.introEnd = float.Parse(pair[1]);
                                break;
                        }
                    }

                    _timeMarkers.Add(tokens[0], marker);
                }
            }
        }

        void AdjustToCamera(Scene scene, LoadSceneMode mode) {
            transform.position = Camera.main.transform.position;
        }

        public void PlayBGM(AudioClip clip) {
            if (clip && _port.Clip != clip) {
                var key = clip.name;

                if (_timeMarkers.ContainsKey(key)) {
                    var marker = _timeMarkers[key];
                    AudioClip intro = (marker.intro != "") ?
                        Resources.Load<AudioClip>(_BGMDirectory + marker.intro)  :  null;
                    _port.Play(clip, intro, _timeMarkers[key]);
                } else {
                    var timeMarker = new TimeMarker();
                    timeMarker.loopStart = 0.0f;
                    timeMarker.loopEnd = clip.length;
                    timeMarker.intro = "";
                    _port.Play(clip, null, timeMarker);
                }
                _playing = true;
            } 
        }

        public void PlayBGM(string clipname) {
            var clip = Resources.Load<AudioClip>(_BGMDirectory + clipname);
            if (clip) {
                PlayBGM(clip);
            } else {
                Debug.Log("BGM doesn't exist; check if you got the name right");
            }
        }

        void Update() {
            if (_playing) {
                _port.CheckLoop();
            }
        }
        public void PlaySFX(AudioClip clip) {
            _sfxSource.PlayOneShot(clip, _sfxMax / 500.0f);
        }
        public void PlaySFX(string name) {
            _sfxSource.PlayOneShot(_sfx[name], _sfxMax / 500.0f);
        }


        float _fadeSeconds;
        public void FadeOut(float seconds) {
            _fadeSeconds = seconds;
            StartCoroutine("FadeOutBGM");
        }

        IEnumerator FadeOutBGM() {
            float initVolume = _port.Volume;
            float t = 0f;

            while (t < 1) {
                t += Time.deltaTime / _fadeSeconds;
                _port.Volume = Mathf.MoveTowards(initVolume, 0, t);

                yield return null;
            }
            _port.Stop();
            StopAllCoroutines();
        }

        //    public void SetPlayerPrefs() {
        //        if (PlayerPrefs.HasKey("SFX"))
        //            sfxMax = PlayerPrefs.GetFloat("SFX");
        //        else sfxMax = 300;

        //        if (PlayerPrefs.HasKey("BGM"))
        //            sfxMax = PlayerPrefs.GetFloat("BGM");
        //        else bgmMax = 250;
        //    }
    }
}