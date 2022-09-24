using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Elang
{
    public class TextProChange : MonoBehaviour
    {
        //TextMeshPro is being weird
        [SerializeField]
        string _originalText;

        TextMeshProUGUI _text;
        void Awake() {
            _text = GetComponent<TextMeshProUGUI>();
            _text.text = _originalText;
        }

        public void ChangeText(string newText) {
            _text.text = newText;
        }
    }
}