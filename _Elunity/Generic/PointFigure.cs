using UnityEngine;

namespace Elang {
    public class PointFigure {
        public Vector2 range { get; private set; }
        public float value { get; private set; }

        public PointFigure(Vector2 range_, float value_ = 0) {
            Init(range_, value_);
        }

        public void Init(Vector2 range_, float value_ = 0) {
            range = range_;
            value = value_;
        }

        public void SetValue(float value_) {
            Mathf.Clamp(value, range.x, range.y);
            value = value_;
        }

        // heal with negative value 
        public bool DamageValue(float value_) {
            value -= value_;
            bool dead = (this.value < range.x) ? true : false;
            value = Mathf.Clamp(this.value, range.x, range.y);
            return dead;
        }

        public void Fill() {
            value = range.y;
        }

        public void Empty() {
            value = range.x;
        }

        public void Extend(Vector2 range) {
            range.x += range.x;
            range.y += range.y;
        }

        public float GetRatio() {
            return value / range.y;
        }
    }
}