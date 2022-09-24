using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Elang
{


    /// <summary>
    /// <br> 1. s = t * (u + v) / 2 </br>
    /// <br> 2. v = u + at </br>
    /// <br> 3. s = ut + at^2 / 2 </br>
    /// <br> 4. s = vt - at^2 / 2 </br>
    /// <br> 5. v^2 = u^2 + 2as </br>
    /// <br></br>
    /// <br> Where S is distance, U is initial velocity, V is final velocity, A is acceleration, and T is time taken. </br>
    /// <br> Method name: [Output Value]_[Input Values (Listed in the above order)] </br>
    /// </summary>
    public class KinematicEquation : MonoBehaviour
    {
        /// <summary>  Get distance from initial velocity, acceleration, and time taken. </summary>
        public static float S_UAT(float u, float a, float t) {
            return u * t + a * Mathf.Pow(t, 2) / 2.0f;
        }

        /// <summary>  Get initial velocity from distance, final velocity, and acceleration. </summary>
        public static float U_SVA(float s, float v, float a) {
            return Mathf.Pow(Mathf.Pow(v, 2) - 2 * a * s, 0.5f);
        }

        /// <summary> Get initial velocity from distance, final velocity, and time taken. </summary>
        public static float U_SVT(float s, float v, float t) {
            return ((2.0f / s) * t - v);
        }

        /// <summary> Get initial velocity from distance, acceleration, and time taken. </summary>
        public static float U_SAT(float s, float a, float t) {
            return (s / t - a * t / 2.0f);
        }

        /// <summary> Get final velocity from distance, initial velocity, and acceleration. </summary>
        public static float V_SUA(float s, float u, float a) {
            return Mathf.Pow(Mathf.Pow(u, 2) + 2 * a * s, 0.5f);
        }

        /// <summary> Get final velocity from distance, initial velocity, and time taken. </summary>
        public static float V_SUT(float s, float u, float t) {
            return (2.0f * s / t - u);
        }

        /// <summary> Get final velocity from distance, acceleration, and time taken. </summary>
        public static float V_SAT(float s, float a, float t) {
            return (s / t + a * t / 2.0f);
        }
        
    }
}

