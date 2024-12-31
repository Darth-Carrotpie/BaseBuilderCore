using System;
using UnityEngine;

namespace BaseBuilderCore {
#if (UNITY_EDITOR)
    public class GizmoSystemHandler : MonoBehaviour {
        public Action DrawGizmos;
        public Action DrawGizmosSelected;

        private void OnDrawGizmos() {
            if (Application.isPlaying) {
                DrawGizmos?.Invoke();
            }
        }

        private void OnDrawGizmosSelected() {
            if (Application.isPlaying) {
                DrawGizmosSelected?.Invoke();
            }
        }
    }
#endif
}
