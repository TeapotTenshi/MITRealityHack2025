using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HaptikosHandpose
{
    public string name;

    private Vector3 xAxisThumb;
    private Vector3 yAxisWrist;
    private Vector3 xAxisWrist;
    private Vector3[] tips;
    private Quaternion[] rotations;

    private float[] curl;
    private float[] flexion;
    private float[] abduction;
    private float[] opposition;
    private List<float> valuesList;
    public float[] values;

    public HaptikosHandpose(Vector3 _xAxisThumb , Vector3 _zAxisIndex, Vector3 _xAxisIndex , Vector3[] _tips, Quaternion[] _rotations, string _name = "Displayed Pose")
    {
        Update(_xAxisThumb, _zAxisIndex, _xAxisIndex, _tips, _rotations, _name);
    }

    public HaptikosHandpose(string _name = "Displayed Pose")
    {
        name = _name;
        values = new float[18];
    }

    public void Update(Vector3 _xAxisThumb, Vector3 _yAxisIndex, Vector3 _xAxisIndex, Vector3[] _tips, Quaternion[] _rotations, string _name)
    {
        curl = new float[5];
        flexion = new float[5];
        abduction = new float[5];
        opposition = new float[5];
        valuesList = new();

        tips = _tips;
        name = _name;
        xAxisWrist = _xAxisIndex;
        xAxisThumb = _xAxisThumb;
        yAxisWrist = _yAxisIndex;
        rotations = _rotations;

        curl[0] = -normalizeAngle(rotations[2].eulerAngles.z);
        flexion[0] = ThumbFlexion(xAxisThumb, xAxisWrist, yAxisWrist);
        opposition[0] = 0;
        abduction[0] = ThumbAbduction(xAxisThumb, xAxisWrist, yAxisWrist);

        for (int i = 1; i < 5; i++)
        {

            curl[i] = (-normalizeAngle(rotations[3 * i + 1].eulerAngles.z) - normalizeAngle(rotations[3 * i + 2].eulerAngles.z)) / 2;
            flexion[i] = -normalizeAngle(rotations[3 * i].eulerAngles.z);
            opposition[i] = (tips[0] - tips[i]).magnitude;
            if (i == 4)
            {
                break;
            }
            abduction[i] = -normalizeAngle(rotations[3 * i].eulerAngles.y) + normalizeAngle(rotations[3 * i + 3].eulerAngles.y);
        }

        for (int i = 0; i < 5; i++)
        {
            valuesList.Add(curl[i]);
            valuesList.Add(flexion[i]);
            valuesList.Add(abduction[i]);
            valuesList.Add(opposition[i]);
        }

        valuesList.RemoveAt(18);
        valuesList.RemoveAt(3);

        values = new float[18];
        for (int i = 0; i < 18; i++)
        {
            values[i] = valuesList[i];
        }
    }

    private float ThumbFlexion(Vector3 xThumb, Vector3 xWrist, Vector3 yWrist)
    {
        xThumb = Vector3.ProjectOnPlane(xThumb, yWrist);
        xWrist = Vector3.ProjectOnPlane(xWrist, yWrist);
        return Vector3.SignedAngle(xThumb, xWrist, yWrist);
    }

    private float ThumbAbduction(Vector3 xThumb, Vector3 xWrist, Vector3 yWrist)
    {
        Vector3 zWrist = Vector3.Cross(xWrist, yWrist);
        xThumb = Vector3.ProjectOnPlane(xThumb, zWrist);
        xWrist = Vector3.ProjectOnPlane(xWrist, zWrist);
        return Vector3.SignedAngle(xThumb, xWrist, zWrist);
    }

    private float normalizeAngle(float x)
    {
        while (x > 180)
        {
            x -= 360;
        }
        while (x < -180)
        {
            x += 360;
        }
        return x;
    }
}
