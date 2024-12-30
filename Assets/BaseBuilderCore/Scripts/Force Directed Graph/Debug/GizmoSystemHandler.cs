using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if (UNITY_EDITOR)
public class GizmoSystemHandler : MonoBehaviour
{
    public Action DrawGizmos;
    public Action DrawGizmosSelected;

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            DrawGizmos?.Invoke();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            DrawGizmosSelected?.Invoke();
        }
    }
}
#endif
