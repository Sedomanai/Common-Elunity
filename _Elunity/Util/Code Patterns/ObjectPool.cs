using UnityEngine;
using System.Collections.Generic;

namespace Elang
{
    /// <summary>
    /// <br> // Generic Object Pool for GameObject prefabs.</br>
    /// <br></br>
    /// <br> At the time being (Unity Engine 2021), there is a built in feature for Object Pools in Unity. </br>
    /// <br> However, its storage is not dynamic and apparently there is a need to write a new script for every use case. </br>
    /// <br> This is because the object has to know of the pool's existence in order to release itself. </br>
    /// <br> It may be possible to prevent this by calling an event instead but we still need to modify the script of the object. </br>
    /// <br></br>
    /// <br> On the other hand, this pool only cares about whether a GameObject has been disabled or not. </br>
    /// <br> It does not care in the slightest about what composes that pool object; only that it's a GameObject. </br>
    /// <br> There is no need for the prefab and any of its components to know of the pool's existence. </br>
    /// <br></br>
    /// <br> Normally a built-in feature is prefered over a custom one, but in this case I'd like to make an exception. </br>
    /// </summary>
    public class ObjectPool : MonoBehaviour {
        
        [SerializeField]
        GameObject _object;
        [SerializeField]
        Transform _container;
        [SerializeField]
        bool _resetTransform = true;

        List<GameObject> _pool = new List<GameObject>();

        void Awake() {
            if (!_container)
                _container = transform;
        }


        /// <summary>
        /// <br> Pull an existing disabled GameObject to recycle.</br>
        /// <br> Instantiate a new GameObject if there are no more existing GameObjects.</br>
        /// <br> The transform of the newly gotten game object follows that of this component's GameObject. </br>
        /// </summary>
        /// <returns> Pool Object </returns>
        public GameObject Get() {
            for (int i = 0; i < _pool.Count; i++) {
                var obj = _pool[i];
                if (!obj.activeSelf) {
                    ResetTransform(obj.transform);
                    obj.SetActive(true);
                    return obj;
                }
            } return CreateObject();
        }

        GameObject CreateObject() {
            GameObject obj = Instantiate(_object);
            _pool.Add(obj);
            ResetTransform(obj.transform);
            obj.SetActive(true);
            return obj;
        }

        void ResetTransform(Transform tr) {
            if (_resetTransform) {
                tr.position = transform.position;
                tr.eulerAngles = transform.eulerAngles;
            }
            tr.SetParent(_container);
        }

        public ObjectPool SetObject(GameObject obj) {
            _object = obj;
            _pool.Clear();
            return this;
        }

        public void ClearPool() {
            _pool.Clear();
        }
    }
}