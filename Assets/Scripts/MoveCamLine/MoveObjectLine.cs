using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;

[Serializable]
public class PointInfo
{
    public Vector3 Pos;
    public Vector3 Start_Inclin;
    public Vector3 End_Inclin;
}

public class MoveObjectLine : MonoBehaviour
{
    public bool ShowGUI;

    public List<PointInfo> Points;

    [Header("오브젝트 이동")]
    public GameObject TestCube;

    public int TargetInt;
    [Range(0f,1f)] public float Value;

    private void Update()
    {
        TestCube.transform.position = MoveRoundPos(Points[TargetInt].Pos, Points[TargetInt + 1].Pos, Points[TargetInt].Start_Inclin, Points[TargetInt + 1].End_Inclin, Value);
    }

    public Vector3 MoveRoundPos(Vector3 Pos_1, Vector3 Pos_2, Vector3 Inclin_1, Vector3 Inclin_2,float Value)
    {
        Vector3 A = Vector3.Lerp(Pos_1, Inclin_1,Value);
        Vector3 B = Vector3.Lerp(Inclin_1, Inclin_2, Value);
        Vector3 C = Vector3.Lerp(Inclin_2, Pos_2, Value);

        Vector3 D = Vector3.Lerp(A, B, Value);
        Vector3 E = Vector3.Lerp(B, C, Value);

        Vector3 F = Vector3.Lerp(D, E, Value);
        return F;
    }
}

[CanEditMultipleObjects]
[CustomEditor(typeof(MoveObjectLine))]
public class EditorMove : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }

    protected virtual void OnSceneGUI()
    {
        MoveObjectLine MoveLine = (MoveObjectLine)target;

        if (!MoveLine.ShowGUI)
            return;

        if (MoveLine.Points.Count < 1)
            return;

        var style = new GUIStyle();
        style.normal.textColor = Color.yellow;

        for (int i = 0; i < MoveLine.Points.Count; i++)
        {
            EditorGUI.BeginChangeCheck();
            Vector3 newTargetPosition = Handles.PositionHandle(MoveLine.Points[i].Pos, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                MoveLine.Points[i].Pos = newTargetPosition;  //새로운 Pos 저장
            }

            Handles.Label(MoveLine.Points[i].Pos, i.ToString() + "P");

            Vector3 snap = Vector3.one * 0.5f;
            if (i < MoveLine.Points.Count - 1)
            {
                Vector3 newTargetStartTangent = Handles.FreeMoveHandle(MoveLine.Points[i].Start_Inclin, Quaternion.identity, 0.2f, snap, Handles.RectangleHandleCap);
                if (EditorGUI.EndChangeCheck())
                {
                    MoveLine.Points[i].Start_Inclin = newTargetStartTangent;  //새로운 StartTangent 저장
                }

                Handles.Label(MoveLine.Points[i].Start_Inclin, i.ToString() + " Start", style);
            }

            if (i > 0)
            {
                Vector3 newTargetEndTangent = Handles.FreeMoveHandle(MoveLine.Points[i].End_Inclin, Quaternion.identity, 0.2f, snap, Handles.RectangleHandleCap);
                if (EditorGUI.EndChangeCheck())
                {
                    MoveLine.Points[i].End_Inclin = newTargetEndTangent; //새로운 EndTangent 저장
                }

                Handles.Label(MoveLine.Points[i].End_Inclin, i.ToString() + " End" , style);
            }
        }

        for (int i = 0; i < MoveLine.Points.Count; i++)
        {
            if (i + 1 >= MoveLine.Points.Count)
                break;

            var points = Handles.MakeBezierPoints(MoveLine.Points[i].Pos,
                                                       MoveLine.Points[i + 1].Pos,
                                                       MoveLine.Points[i].Start_Inclin,
                                                       MoveLine.Points[i + 1].End_Inclin,
                                                       50);

            Handles.DrawAAPolyLine(points);
        }
    }
}
