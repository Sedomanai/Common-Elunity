using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Elang
{
    [RequireComponent(typeof(TextMeshPro))]
    public class TextProInteger : MonoBehaviour
    {
        [SerializeField]
        int _value;

        public int Value { get { return _value; }}

        TextMeshProUGUI _text;
        void Awake() {
            _text = GetComponent<TextMeshProUGUI>();
            _text.text = (_value).ToString();
        }

        public void SetValue(int value) {
            _value = value;
            _text.text = (_value).ToString();
        }
        public void AddValue(int addValue) {
            _value += addValue;
            _text.text = (_value).ToString();
        }
    }
}