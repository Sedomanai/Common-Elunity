using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elang
{
    public static class Math2D
    {
        public static Vector2 ArrowVelocity(this Transform transform, Vector3 target, float velocity, Vector2 marginOfError) {
            Vector2 delta = target - transform.position;
            delta += new Vector2(Random.Range(-marginOfError.x, marginOfError.x), Random.Range(-marginOfError.y, marginOfError.y));
            var time = (velocity == 0.0f) ? 0.001f : delta.magnitude / velocity;
            float ay = Physics2D.gravity.y;
            var xVelocity = KinematicEquation.U_SAT(delta.x, 0.0f, time);
            var yVelocity = KinematicEquation.U_SAT(delta.y, ay, time);
            return new Vector2(xVelocity, yVelocity);
        }

        public static float JumpVelocity(float jumpHeight) {
            return KinematicEquation.U_SVA(jumpHeight, 0.0f, Physics2D.gravity.y);
        }

        public static float HomingAngle(this Transform transform, Vector3 target) {
            Vector2 c = target - transform.position;
            return Mathf.Atan2(c.y, c.x);
        }

        public static float SineValue(this Transform transform, float amplitude, float frequency, float phaseShift, float time) {
            return amplitude * Mathf.Sin(frequency * time + phaseShift);
        }
    }
}