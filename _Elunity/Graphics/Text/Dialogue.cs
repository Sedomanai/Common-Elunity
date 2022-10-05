using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

namespace Elang
{
    public class Dialogue : MonoBehaviour
    {
        [SerializeField]
        Image image;
        [SerializeField]
        TextMeshProUGUI header;
        [SerializeField]
        TextMeshProUGUI content;


        [SerializeField]
        InputActionMap map;

        //[SerializeField]
        //InputActionAsset input;
        InputAction flip, skip, turbo;

        [SerializeField]
        TransitionEvent fadeIn;
        [SerializeField]
        TransitionEvent fadeOut;

        [Header("Customization")]
        [SerializeField]
        float warmup = 0.05f;
        [SerializeField]
        float pageInterval = 0.15f;
        [SerializeField]
        bool useSkip;
        [SerializeField]
#if UNITY_EDITOR
        [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(useSkip))]
#endif
        int skipCharacterThreshold = 8;

        public DialogueScriptableObject dialogue;
        // Start is called before the first frame update
        void Start() {
            if (!image)
                image = GetComponentInChildren<Image>();
            fadeIn.SetupImage(image);
            fadeOut.SetupImage(image);

            image.gameObject.SetActive(false);
            SetTextActive(false);

            //var dui = map["Dialogue"];
            map.Disable();
            flip = map["Flip"];
            skip = map["Skip"];
            turbo = map["Turbo"];

            if (flip == null)
                Debug.Log("This shouldn't happen, I think.");

            flip.Disable();
            skip.Disable();
            turbo.Disable();


            //click = input.FindActionMap("UI").FindAction("Click");
            //var dui = input.FindActionMap("Dialogue");
            //flip = dui.FindAction("Flip");
            //if (useSkip)
            //    skip = dui.FindAction("Skip");
            //turbo = dui.FindAction("Turbo");


            //fadeIn.beginAction.AddListener(click.Disable);
            fadeIn.endAction.AddListener(Talk);
            fadeOut.endAction.AddListener(AllFinished);
        }

        void OnDestroy() {
            //fadeIn.beginAction.RemoveListener(click.Disable);
            fadeIn.endAction.RemoveListener(Talk);
            fadeOut.endAction.RemoveListener(AllFinished);
        }

        void AllFinished() {
            image.gameObject.SetActive(false);
        }

        void SetTextActive(bool value) {
            header.gameObject.SetActive(value);
            content.gameObject.SetActive(value);
            if (!value) {
                header.text = "";
                content.text = "";
            }
        }

        public void FadeIn() {
            image.gameObject.SetActive(true);
            StartCoroutine(fadeIn.FadeInCO());
        }
        public void FadeOut() {
            StartCoroutine(fadeOut.FadeOutCO());
        }

        public void Talk() {
            StartCoroutine(TalkCO());
        }

        IEnumerator TalkCO() {
            var dia = dialogue.dialogue;
            map.Enable();

            SetTextActive(true);
            yield return new WaitForSeconds(warmup);

            turbo.Enable();

            for (int i = 0; i < dia.Count; i++) {
                header.text = dia[i].speaker;
                header.color = dia[i].color;
                yield return StartCoroutine(MonoCO(i));
            }

            turbo.Disable();
            SetTextActive(false);
            FadeOut();
        }


        IEnumerator MonoCO(int reader) {
            var mono = dialogue.dialogue[reader];

            for (int i = 0; i < mono.content.Count; i++) {
                var strip = mono.content[i];
                int page = 0;

                if (useSkip)
                    skip.Enable();

                while (true) {
                    string sub = strip.Substring(0, page);
                    content.text = sub;
                    if (strip == sub)
                        break;
                    page++;

                    yield return new WaitForSeconds(0.00001f);

                    if (turbo.IsPressed() || (useSkip && skip.triggered && (sub.Length > skipCharacterThreshold) && skip.triggered)) {
                        content.text = strip;
                        break;
                    }
                }

                if (useSkip)
                    skip.Disable();

                flip.Enable();
                yield return new WaitForSeconds(pageInterval);

                while (!flip.triggered && !turbo.IsPressed()) {
                    yield return null;
                }

                flip.Disable();
            }
        }
    }
}

