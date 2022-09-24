using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Elang {
    /// <summary>
    /// <br> For saving a binary file on rom to record lasting game data i.e. to save and load a game.</br>
    /// <br> At this current point it's not certain whether it's deprecated due to SerializedObject.</br>
    /// </summary>
    public class SaveMgr : Singleton<SaveMgr> {
        BinaryFormatter _bf;
        FileStream _file;
        MemoryStream _ms;
        bool _loaded;

        public string dataPath { private set; get; }

        protected override void Init() {
            name = "[Save Manager]";
            _bf = new BinaryFormatter();
            dataPath = Application.persistentDataPath;
        }

        // Update is called once per frame
        public void Save<T>(ref T container, string path) {
            _file = File.Create(path);
            _bf.Serialize(_file, container);
            _file.Close();
        }

        public void Load<T>(ref T container, string path) {
            if (File.Exists(path)) {
                _file = File.Open(path, FileMode.Open);
                container = (T)_bf.Deserialize(_file);
                _file.Close();
            }
        }

        // WWW deprecated
        // public void LoadToAndroid<T>(ref T container, string path) {
        //     WWW file = new WWW(path);
        //     while (!file.isDone) {; }
        //     _ms = new MemoryStream(file.bytes);
        //     container = (T)_bf.Deserialize(_ms);
        //     _ms.Close();
        // }

        // IEnumerator LoadToAndroidCoroutine(string path) {
        //     WWW file = new WWW(path);
        //     yield return file;
        //     _ms = new MemoryStream(file.bytes);
        //     _loaded = true;
        // }
    }
}