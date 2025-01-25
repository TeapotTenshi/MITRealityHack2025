using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class Hand

{
    [SerializeField]
    private HandType handType;

    [SerializeField]
    private Quaternion IMU_orientation;

    [SerializeField]
    private string connectionStatus;

    [SerializeField]
    private int batteryLevel;

    [SerializeField]
    public Quaternion[] joints = new Quaternion[17];

    [SerializeField]
    private Vector3 position = new Vector3();

    [SerializeField]
    public Vector3[] jointPositions = new Vector3[17];

    [SerializeField]
    private float handScale;

    public float HandScale
    {
        get { return handScale; }
        set { handScale = value; }
    }

    public HandType HandType
    {
        get { return handType; }
        set { handType = value; }
    }
    public Quaternion Orientation
    {
        get { return IMU_orientation; }
        set { IMU_orientation = value; }
    }
    public string ConnectionStatus
    {
        get { return connectionStatus; }
        set { connectionStatus = value; }
    }

    public int BatteryLevel
    {
        get { return batteryLevel; }
        set { batteryLevel = value; }
    }

    public Vector3 Position
    {
        get { return position; }
        set { position = value; }
    }
}

[Serializable]
public enum HandType
{
    LeftHand,
    RightHand
}