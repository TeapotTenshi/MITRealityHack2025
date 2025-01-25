using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Haptikos;
using Haptikos.Exoskeleton;
using UnityEngine.Events;

public class IMUCalibrationManager : SingletonMonobehaviour<IMUCalibrationManager>
{
    [SerializeField] CalibrationController calibrationController;
    [SerializeField] RealtimeRecenter realtimeRecenter;

    CalibateHologram[] holograms = new CalibateHologram[2];
    FollowController rightMovingHologram, leftMovingHologram;
    SkinnedMeshRenderer leftMeshRenderer, rightMeshRenderer;

    public static UnityEvent<HaptikosExoskeleton> StartedCalibration = new();
    public static UnityEvent<HaptikosExoskeleton> ExitedCalibration = new();
    public static UnityEvent<HaptikosExoskeleton> FinishedCalibration = new();

    public bool calibrating = false;

    private void OnEnable()
    {
        ExitedCalibration.AddListener(FinishCalibration);
        FinishedCalibration.AddListener(FinishCalibration);
        StartedCalibration.AddListener(StartCalibration);
    }

    private void OnDisable()
    {
        ExitedCalibration.RemoveAllListeners();
        FinishedCalibration.RemoveAllListeners();
        StartedCalibration.RemoveAllListeners();
    }
    public void FinishCalibration(HaptikosExoskeleton hand)
    {
        HideHolograms();
    }

    public void StartCalibration(HaptikosExoskeleton hand)
    {
        ShowHolograms();
    }

    public void HideHolograms()
    {
        for (int i = 0; i < holograms.Length; i++)
        {
            holograms[i].gameObject.SetActive(false);
        }
        calibrating = false;

        rightMovingHologram.enabled = false;
        leftMovingHologram.enabled = false;

        if (ExoskeletonConnectionController.LeftGloveConnected)
        {
            leftMeshRenderer.enabled = true;
        }
        if (ExoskeletonConnectionController.RightGloveConnetected)
        {
            rightMeshRenderer.enabled = true;
        }
    }

    public void ShowHolograms()
    {
        for (int i = 0; i < holograms.Length; i++)
        {
            holograms[i].gameObject.SetActive(true);
        }
        if (ExoskeletonConnectionController.RightGloveConnetected)
        {
            rightMovingHologram.enabled = true;
        }
        if (ExoskeletonConnectionController.LeftGloveConnected)
        {
            leftMovingHologram.enabled = true;
        }
        rightMeshRenderer.enabled = false;
        leftMeshRenderer.enabled = false;
        calibrating = true;
    }

    protected override void Awake()
    {

        holograms = GetComponentsInChildren<CalibateHologram>(true);
        
        FollowController[] movingHolograms = GetComponentsInChildren<FollowController>(true);
        if (movingHolograms[0].isRight)
        {
            rightMovingHologram = movingHolograms[0];
            leftMovingHologram = movingHolograms[1];
        }
        else
        {
            rightMovingHologram = movingHolograms[1];
            leftMovingHologram = movingHolograms[0];
        }

        HaptikosPlayer glove = FindAnyObjectByType<HaptikosPlayer>();
        leftMeshRenderer = glove.transform.GetChild(3).GetComponentInChildren<SkinnedMeshRenderer>(true);
        rightMeshRenderer = glove.transform.GetChild(4).GetComponentInChildren<SkinnedMeshRenderer>(true);

        HideHolograms();

    }
}
