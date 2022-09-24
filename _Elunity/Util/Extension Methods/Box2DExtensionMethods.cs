using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elang
{
    public static class Box2DExtensionMethods
    {

        public static void BoxCastMove(this BoxCollider2D box, Vector2 dir, LayerMask mask) {
            var transform = box.transform;
            ContactFilter2D contact = new ContactFilter2D();
            contact.SetLayerMask(mask);
            List<RaycastHit2D> hits = new List<RaycastHit2D>();
            box.Cast(new Vector2(dir.x, 0), contact, hits, 1.0f, true);

            if (hits.Count == 0) {
                transform.position += new Vector3(dir.x, 0, 0);
            } else {
                Vector2 normal = new Vector2(float.NaN, float.NaN);
                bool right = (dir.x > 0.0f);
                float maxX = right ? -Mathf.Infinity : Mathf.Infinity;
                for (int i = 0; i < hits.Count; i++) {
                    var hit = hits[i];
                    maxX = right ? Mathf.Max(maxX, hit.point.x) : Mathf.Min(maxX, hit.point.x);
                    if (maxX == hit.point.x) {
                        normal = hit.normal;
                    }
                }

                var dx = (right) ?
                    maxX - (transform.position.x + box.bounds.size.x / 2.0f) - 0.2f :
                    maxX - (transform.position.x - box.bounds.size.x / 2.0f) + 0.2f;
                transform.position += new Vector3(dx, 0, 0);

                var leftover = dir.x - dx;

                Debug.Log("Normal " + normal);

            }

            box.Cast(new Vector2(0, dir.y), contact, hits, 1.0f, true);

            if (hits.Count == 0) {
                transform.position += new Vector3(0, dir.y, 0);
            } else {
                bool up = (dir.y > 0.0f);
                float maxY = up ? -Mathf.Infinity : Mathf.Infinity;
                for (int i = 0; i < hits.Count; i++) {
                    maxY = up ? Mathf.Max(maxY, hits[i].point.y) : Mathf.Min(maxY, hits[i].point.y);
                }

                var dy = (up) ?
                    maxY - (transform.position.y + box.bounds.size.y / 2.0f) - 0.2f :
                    maxY - (transform.position.y - box.bounds.size.y / 2.0f) + 0.2f;
                transform.position += new Vector3(0, dy, 0);
            }
        }
    }

}
