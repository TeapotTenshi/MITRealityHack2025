using Haptikos;
using Haptikos.Exoskeleton;
using Haptikos.Gloves;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CalibrationLoader : MonoBehaviour
{
    public Sprite icon;
    Slider slider;
    HaptikosGestureRecognizer leftRecognizer, rightRecognizer;
    GameObject visualization;
    Transform leftMiddle, rightMiddle, mainCamera;
    public int state = 0;
    float timer;
    public static UnityEvent<HaptikosExoskeleton> RecognizedThumbsUp = new();
    
    HaptikosExoskeleton leftHand, rightHand;

    IMUCalibrationManager IMUCalibrationManager;

    DataStreamingEvents dataStreamingEvents;
    void Awake()
    {
        leftHand = transform.parent.GetChild(3).GetComponent<HaptikosExoskeleton>();
        rightHand = transform.parent.GetChild(4).GetComponent<HaptikosExoskeleton>();
        visualization = Instantiate(HaptikosResources.Instance.slider);
        visualization.SetActive(false);
        slider = visualization.GetComponentInChildren<Slider>();
        Image iconImage = visualization.transform.GetChild(0).GetChild(2).GetComponent<Image>();
        if (icon != null)
        {
            iconImage.sprite = icon;
        }
        else
        {
            iconImage.sprite = HaptikosResources.Instance.circle;
        }
        Image fillImage = visualization.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Image>();
        fillImage.color = Color.green;
        visualization.transform.parent = transform;

        mainCamera = GameObject.FindWithTag("MainCamera").transform;

        leftRecognizer = transform.GetChild(0).GetComponent<HaptikosGestureRecognizer>();
        rightRecognizer = transform.GetChild(1).GetComponent<HaptikosGestureRecognizer>();
        GameObject calibrationObject = Instantiate(HaptikosResources.Instance.calibrationManager);
        calibrationObject.transform.parent = transform;
        IMUCalibrationManager = calibrationObject.GetComponent<IMUCalibrationManager>();

        dataStreamingEvents = FindAnyObjectByType<DataStreamingEvents>();
    }

    private void Start()
    {
        rightMiddle = rightRecognizer.Hand.transform.GetChild(0).GetChild(0).GetChild(2);
        leftMiddle = leftRecognizer.Hand.transform.GetChild(0).GetChild(0).GetChild(2);
    }

    private void OnEnable()
    {
        dataStreamingEvents.OnDataReceived += ResetState;
        dataStreamingEvents.OnDataStoppedReceiving += ResetState;
    }

    private void OnDisable()
    {
        dataStreamingEvents.OnDataReceived -= ResetState;
        dataStreamingEvents.OnDataStoppedReceiving += ResetState;
    }

    void ResetState(HaptikosExoskeleton hand)
    {
        if (IMUCalibrationManager.calibrating)
        {
            IMUCalibrationManager.ExitedCalibration.Invoke(hand);
        }
        else
        {
            visualization.SetActive(false);
        }

        state = 0; 
        
    }

    void Update()
    {
        switch (state)
        {
            case 0:
                if(ExoskeletonConnectionController.RightGloveConnetected && ExoskeletonConnectionController.LeftGloveConnected)
                {
                    if(rightRecognizer.Activated && leftRecognizer.Activated)
                    {
                        state = 5;
                        timer = 1f;
                        RecognizedThumbsUp?.Invoke(rightHand);
                        RecognizedThumbsUp?.Invoke(leftHand);
                    }
                }
                else if (rightRecognizer.Activated && !HaptikosPlayer.calibrated)
                {
                    state = 1;
                    timer = 1f;
                    RecognizedThumbsUp?.Invoke(rightHand);
                }
                else if (leftRecognizer.Activated && !HaptikosPlayer.calibrated)
                {
                    state = 2;
                    timer = 1f;
                    RecognizedThumbsUp?.Invoke(leftHand);
                }
                break;
            case 1:
                if (!rightRecognizer.Activated || HaptikosPlayer.calibrated)
                {
                    state = 0;
                }
                else
                {
                    timer -= Time.deltaTime;
                    if (timer <= 0)
                    {
                        visualization.SetActive(true);
                        visualization.transform.SetPositionAndRotation(rightMiddle.position + Vector3.up * 0.15f, mainCamera.rotation);
                        slider.value = 0f;
                        state = 3;
                        timer = 2f;
                    }
                }
                break;
            case 2:
                if (!leftRecognizer.Activated || HaptikosPlayer.calibrated)
                {
                    state = 0;
                }
                else
                {
                    timer -= Time.deltaTime;
                    if (timer <= 0)
                    {
                        visualization.SetActive(true);
                        visualization.transform.SetPositionAndRotation(leftMiddle.position + Vector3.up * 0.15f, mainCamera.rotation);
                        slider.value = 0f;
                        state = 4;
                        timer = 2f;
                    }
                }
                break;
            case 3:
                if (!rightRecognizer.Activated || HaptikosPlayer.calibrated)
                {
                    visualization.SetActive(false);
                    state = 0;
                    return;
                }
                else
                {
                    timer -= Time.deltaTime;
                    if (timer <= 0)
                    {
                        state = 0;
                        visualization.SetActive(false);
                        HandleCalibration(true);
                    }
                    else
                    {
                        visualization.transform.SetPositionAndRotation(rightMiddle.position + Vector3.up * 0.15f, mainCamera.rotation);
                        slider.value = (2 - timer) / 2;
                    }
                }
                break;
            case 4:
                if (!leftRecognizer.Activated || HaptikosPlayer.calibrated)
                {
                    visualization.SetActive(false);
                    state = 0;
                    return;
                }
                else
                {
                    timer -= Time.deltaTime;
                    if (timer <= 0)
                    {
                        state = 0;
                        visualization.SetActive(false);
                        HandleCalibration(false);
                    }
                    else
                    {
                        visualization.transform.SetPositionAndRotation(leftMiddle.position + Vector3.up * 0.15f, mainCamera.rotation);
                        slider.value = (2 - timer) / 2;
                    }
                }
                break;
            case 5:
                if (!leftRecognizer.Activated || !rightRecognizer.Activated)
                {
                    visualization.SetActive(false);
                    state = 0;
                    return;
                }
                else
                {
                    timer -= Time.deltaTime;
                    if (timer < 0)
                    {
                        visualization.SetActive(true);
                        visualization.transform.SetPositionAndRotation(mainCamera.position + mainCamera.forward * 0.4f, mainCamera.rotation);
                        slider.value = 0f;
                        timer = 2f;
                        state = 6;
                    }
                }
                break;
            case 6:
                if(!leftRecognizer.Activated || !rightRecognizer.Activated)
                {
                    visualization.SetActive(false);
                    state = 0;
                    return;
                }
                else
                {
                    timer -= Time.deltaTime;
                    if (timer <= 0)
                    {
                        state = 0;
                        visualization.SetActive(false);
                        HandleCalibration(false);
                    }
                    else
                    {
                        visualization.transform.SetPositionAndRotation(mainCamera.position + mainCamera.forward * 0.4f, mainCamera.rotation);
                        slider.value = (2 - timer) / 2;
                    }
                }

                break;
            default:
                break;
        }
    }

    void HandleCalibration(bool isRight)
    {
        HaptikosExoskeleton currentGlove = (isRight) ? rightHand : leftHand;
        if (IMUCalibrationManager.calibrating)
        {
            IMUCalibrationManager.ExitedCalibration.Invoke(currentGlove);
        }
        else
        {
            IMUCalibrationManager.StartedCalibration?.Invoke(currentGlove);
        }
    }
}


