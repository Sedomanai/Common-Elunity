using UnityEngine;

namespace Elang {
    /// <summary>
    /// A simple singleton GameObject that profiles all objects.
    /// </summary>
    public class ProfileMgr : Singleton<ProfileMgr> {
        float deltaTime = 0.0f;

        [SerializeField]
        bool _showLeak = true;
        public bool showLeak { set { _showLeak = value; } }
            
        [SerializeField]
        bool _showFPS = true;
        public bool showFPS { set { _showFPS = value; } }
        
        protected override void Init() {
            gameObject.name = "[Profile Manager]";
        }

        void Update() {
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        }

        void OnGUI() {
            if (_showLeak)
                ShowLeaks();
            if (_showFPS)
                ShowFPS();
        }

        void ShowLeaks() {
            GUILayout.Label("");
            GUILayout.Label("All " + FindObjectsOfType(typeof(UnityEngine.Object)).Length);
            GUILayout.Label("Textures " + FindObjectsOfType(typeof(Texture)).Length);
            GUILayout.Label("AudioClips " + FindObjectsOfType(typeof(AudioClip)).Length);
            GUILayout.Label("Meshes " + FindObjectsOfType(typeof(Mesh)).Length);
            GUILayout.Label("Materials " + FindObjectsOfType(typeof(Material)).Length);
            GUILayout.Label("GameObjects " + FindObjectsOfType(typeof(GameObject)).Length);
            GUILayout.Label("Components " + FindObjectsOfType(typeof(Component)).Length);
        }

        void ShowFPS() {
            int w = Screen.width, h = Screen.height;
            GUIStyle style = new GUIStyle();
            Rect rect = new Rect(0, 0, w, h * 2 / 100);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = h * 2 / 100;
            style.normal.textColor = new Color(255.0f, 255.0f, 254.5f, 255.0f);
            float msec = deltaTime * 1000.0f;
            float fps = 1.0f / deltaTime;
            string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
            GUI.Label(rect, text, style);
        }
    }
}