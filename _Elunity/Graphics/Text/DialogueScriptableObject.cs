using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Elang
{

    [System.Serializable]
#if UNITY_EDITOR
    [CreateAssetMenu(fileName = "DialogueScriptableObject", menuName = "Elang UI/Dialogue", order = 11)]
#endif
    public class DialogueScriptableObject : ScriptableObject
    {
        [System.Serializable]
        public class Monologue
        {
            public string speaker;
            public Color color;
            public List<string> content;
        }

        public List<Monologue> dialogue;
    }
}