using System;
using Unity;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FieldOfView))]
public class FieldOfViewEditor : Editor
{
    void OnSceneGUI()
    {
        FieldOfView fieldOfView = (FieldOfView)target;

        fieldOfView.viewAngle = fieldOfView.viewAngle;

        Handles.color = Color.white;

        Handles.DrawWireArc(fieldOfView.transform.position, 
            Vector3.up, Vector3.forward, 360, fieldOfView.viewRadius);

        Vector3 viewAngleA = fieldOfView.DirFromAngle(-fieldOfView.viewAngle / 2, false);
        Vector3 viewAngleB = fieldOfView.DirFromAngle(fieldOfView.viewAngle / 2, false);

        Handles.color = Color.red;
        Handles.DrawLine(fieldOfView.transform.position, 
            fieldOfView.transform.position + viewAngleA * fieldOfView.viewRadius);

        Handles.DrawLine(fieldOfView.transform.position,
            fieldOfView.transform.position + viewAngleB * fieldOfView.viewRadius);

        Handles.color = Color.red;
        Handles.DrawWireArc(fieldOfView.transform.position,
           Vector3.up, viewAngleA, fieldOfView.viewAngle, fieldOfView.viewRadius);

        if (fieldOfView.targetIsDetected)
        {
            Handles.DrawLine(fieldOfView.transform.position, fieldOfView.targetPosition);
            Handles.DrawSphere(1, fieldOfView.targetPosition, Quaternion.identity, 1);
        }
    }
}