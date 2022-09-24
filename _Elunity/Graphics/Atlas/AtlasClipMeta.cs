using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEditor.Animations;

namespace Elang
{
    [Serializable]
    public class AtlasClipMeta
    {
        public AtlasClipMeta(SerializeStream stream) {
            stream
                .parse(out name)
                .parse(out speed)
                .parse(out repeat);

            int clipSize;
            stream.parse(out clipSize);

            frames = new List<int>();
            int prev = -1;
            int length = 1;
            for (int i = 0; i < clipSize; i++) {
                int curr;
                stream.parse(out curr);

                if (prev != curr) {
                    frames.Add(curr);
                    frames.Add(length);
                } else {
                    frames[frames.Count - 1]++;
                }

                prev = curr;
            }
        }

        public void createAnimationAsset(string animDir, AnimatorController controller, Sprite[] sprites) {
            AnimationClip clip;
            string clipFilename = animDir + "/" + name + ".anim";

            if (!File.Exists(clipFilename)) {
                clip = new AnimationClip();
                AssetDatabase.CreateAsset(clip, clipFilename);
            } else {
                clip = AssetDatabase.LoadAssetAtPath(clipFilename, typeof(AnimationClip)) as AnimationClip;
            }

            if (clip) {
                var settings = AnimationUtility.GetAnimationClipSettings(clip);
                settings.loopTime = repeat;
                AnimationUtility.SetAnimationClipSettings(clip, settings);

                List<ObjectReferenceKeyframe> keyframes = new List<ObjectReferenceKeyframe>();
                ObjectReferenceKeyframe keyframe;
                float time = 0.0f;
                float mult = 1.0f / 30.0f;

                for (int j = 0; j < frames.Count; j += 2) {
                    keyframe = new ObjectReferenceKeyframe();
                    keyframe.time = time;
                    keyframe.value = sprites[frames[j]];
                    keyframes.Add(keyframe);
                    time += frames[j + 1] * mult;
                }

                if (keyframes.Count > 0) {
                    var last = keyframes.Last();
                    keyframe = new ObjectReferenceKeyframe();
                    keyframe.time = time - 1.0f / 60.0f;
                    keyframe.value = last.value;
                    keyframes.Add(keyframe);
                }

                EditorCurveBinding binding = new EditorCurveBinding();
                binding.propertyName = "m_Sprite";
                binding.type = typeof(SpriteRenderer);
                AnimationUtility.SetObjectReferenceCurve(clip, binding, keyframes.ToArray());

                if (controller) {
                    bool exists = false;
                    var st = controller.layers[0].stateMachine;

                    // it's O(N)
                    foreach (var state in st.states) {
                        if (state.state.motion == clip) {
                            state.state.speed = speed;
                            exists = true;
                            break;
                        }
                    }

                    if (!exists)
                        controller.AddMotion(clip).speed = speed;
                }
            }
        }

        [SerializeField]
        string name;
        [SerializeField]
        bool repeat;
        [SerializeField]
        float speed;
        [SerializeField]
        List<int> frames;
    }
}
