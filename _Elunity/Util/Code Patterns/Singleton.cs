using UnityEngine;

namespace Elang {
    /// <summary>
    /// <br> Is not destroyed on load (scene change) by default. </br>
    /// <br> Inherited by a MonoBehavior T, itself as generic type parameter. </br>
    /// </summary>
    /// <typeparam name="T"> Monobehavior that inherits this class. </typeparam>
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T> {
        static GameObject _container;
        static T _instance;

        /// <summary>
        /// <br> Gets the GameObject instance that contains the T component. </br>
        /// <br> If it doesn't exist then instantiate a new GameObject with the component. </br>
        /// <br> This can complicate things if the GameObject is expected to have other components and/or children. </br>
        /// <br> Be wary when debugging from different scenes, make sure that this global Instance exhibits expected behavior from different check points.</br>
        /// <br></br>
        /// <br> Consider overriding the Init method in order to add additional features to the Singleton GameObject</br>
        /// </summary>
        public static T Instance {
            get {
                if (!_instance) {
                    _instance = FindObjectOfType<T>();
                    if (!_instance) {
                        _container = new GameObject();
                        _instance = _container.AddComponent<T>();
                    }

                    _instance.Init();
                }

                return _instance;
            }
        }

        void Awake() {
            // This is a for cases where there is more than one GameObject with this monobehavior.
            // Keeps one and destroys the rest.
            if (this != Instance) 
                Destroy(this);
            else DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// <br> Override this method when the Singleton needs additional features and/or requires behavior to exhibit on first instantation. </br>
        /// <br> This is not the same as the constructor or Awake method. The Awake method is called immediately. </br>
        /// <br> Init method on the other hand is called when the Instance getter creates a new _instance. </br>
        /// </summary>
        virtual protected void Init() {}

        public static void ResetManager() {
            if (_instance) {
                Destroy(_instance.gameObject);
                _instance = null;
            }
        }

    }
}