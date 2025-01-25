using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DeserializedHandData
{
    public DeserializedHandData(int handType, List<float> iMU_orientation, string connectionStatus, int batteryLevel, List<List<float>> joints, List<float> position, List<List<float>> jointPositions, float handScale)
    {
        this.handType = handType;
        IMU_orientation = iMU_orientation;
        this.connectionStatus = connectionStatus;
        this.batteryLevel = batteryLevel;
        this.joints = joints;
        this.position = position;
        this.jointPositions = jointPositions;
        this.handScale = handScale;
    }

    public DeserializedHandData()
    {
    }

    public int handType { get; set; }
    public List<float> IMU_orientation { get; set; }
    public string connectionStatus { get; set; }
    public int batteryLevel { get; set; }
    public List<List<float>> joints { get; set; }
    public List<float> position { get; set; }
    public List<List<float>> jointPositions { get; set; }
    public float handScale { get; set; }


 
}
