using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
public static class GizmoManager
{
    public static void OnStopDrawGizmos(Action action)
    {
        Handler.DrawGizmos -= action;
    }

    public static void OnDrawGizmos(Action action)
    {
        Handler.DrawGizmos += action;
    }

    private static GizmoSystemHandler Handler => _handler != null ? _handler : (_handler = createHandler());
    private static GizmoSystemHandler _handler;

    private static GizmoSystemHandler createHandler()
    {
        var go = new GameObject("Gizmo Handler") { hideFlags = HideFlags.DontSave };

        return go.AddComponent<GizmoSystemHandler>();
    }

}
#endif