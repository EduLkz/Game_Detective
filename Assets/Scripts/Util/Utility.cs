using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util {
    public class Utility : MonoBehaviour {

        public static void SetTransform(Transform transform, Vector3 position, Quaternion rotation) {
            transform.position = position;
            transform.rotation = rotation;
        }

        public static void SetTransform(Transform transform, Vector3 position, Quaternion rotation, bool isLocal) {
            transform.localPosition = position;
            transform.localRotation = rotation;
        }
    }
}