using UnityEngine;
using System.Collections.Generic;

/*
 * manager for multiple sprites 
 * sprites are assigned from the inspector
 * 
 * 스프라이트 매니저
 * 
 */
namespace Elang {
    [RequireComponent(typeof(SpriteRenderer))]
    public class MultiSprite : MonoBehaviour {
        [SerializeField]
        int currentSprite;

        [SerializeField]
        List<Sprite> _sprites;

        SpriteRenderer _rend;

        void Awake() {
            _rend = GetComponent<SpriteRenderer>();
            SetIndex(currentSprite);
        }

        public void SetRandom() {
            SetIndex(Random.Range(0, _sprites.Count));
        }

        public void SetIndex(int i) {
            _rend.sprite = _sprites[i];
            currentSprite = i;
        }

        public void AddSprite(string path) {
            Sprite s = Resources.Load<Sprite>("path");
            AddSprite(s);
        }

        public void AddSprite(Sprite sprite) {
            _sprites.Add(sprite);
        }

        /* 
         * Set sprite in list
         * May override existing sprites
         * 
         * 리스트 수정
         * 기존 원소에 덮어씌울 수 있으니 주의
         */
        public void SetSpriteAt(string path, int index) {
            Sprite s = Resources.Load<Sprite>("path");
            SetSpriteAt(s, index);
        }

        public void SetSpriteAt(Sprite sprite, int index) {
            _sprites[index] = sprite;
        }
    }
}