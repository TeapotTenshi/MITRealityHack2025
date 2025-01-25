using Haptikos.Exoskeleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Video;
using UnityEngine.UI;
using Haptikos;

public class DemoController : MonoBehaviour
{
    public HaptikosSelectableButton nextButton;
    public HaptikosSelectableButton previousButton;
    RawImage rawImage;
    TMP_Text text;
    public VideoPlayer videoPlayer;
    public VideoClip[] videoClips;
    int step = 0;
    HaptikosCanvas portalPanel;
    Transform canvas;
    PortalSceneLoader[] portals;

    private void Awake()
    {
        nextButton = transform.GetChild(0).GetChild(0).GetComponent<HaptikosSelectableButton>();
        previousButton = transform.GetChild(0).GetChild(1).GetComponent<HaptikosSelectableButton>();
        text = transform.GetChild(0).GetChild(2).GetComponent<TMP_Text>();
        rawImage = transform.GetChild(0).GetChild(3).GetComponent<RawImage>();
        videoPlayer = GetComponentInChildren<VideoPlayer>();
        canvas = transform.GetChild(0);
        portals = FindObjectsOfType<PortalSceneLoader>(true);
        portalPanel = transform.GetChild(1).GetComponent<HaptikosCanvas>();
        foreach(PortalSceneLoader portal in portals)
        {
            portalPanel.gameObject.SetActive(false);
            portal.gameObject.SetActive(false);
        }
    }
    private void OnEnable()
    {
        nextButton.OnClick.AddListener(Next);
        previousButton.OnClick.AddListener(Previous);
        IMUCalibrationManager.FinishedCalibration.AddListener((HaptikosExoskeleton hand) =>
        { 
            if(step==0)
                Next(null); 
        });
        IMUCalibrationManager.FinishedCalibration.AddListener((HaptikosExoskeleton hand) => canvas.gameObject.SetActive(true));
        IMUCalibrationManager.ExitedCalibration.AddListener((HaptikosExoskeleton hand) => canvas.gameObject.SetActive(true));
        previousButton.gameObject.SetActive(false);
        IMUCalibrationManager.StartedCalibration.AddListener((HaptikosExoskeleton hand) => canvas.gameObject.SetActive(false));
        step = 0;
        videoPlayer.clip = videoClips[0];
        videoPlayer.Play();
    }

    private void OnDisable()
    {
        nextButton.OnClick.RemoveListener(Next);
        previousButton.OnClick.RemoveListener(Previous);
        IMUCalibrationManager.FinishedCalibration.RemoveListener((HaptikosExoskeleton hand) => canvas.gameObject.SetActive(true));
        IMUCalibrationManager.StartedCalibration.RemoveListener((HaptikosExoskeleton hand) => canvas.gameObject.SetActive(false));
        IMUCalibrationManager.ExitedCalibration.RemoveListener((HaptikosExoskeleton hand) => canvas.gameObject.SetActive(true));
    }

    void Next(HaptikosExoskeleton exoskeleton)
    {
        switch (step)
        {
            case 0:
                if (!HaptikosPlayer.calibrated)
                {
                    //POPUP DO THE CALIBRATION
                    Debug.Log("NOT CALIBRATED");
                    break;
                }
                step++;
                text.text = "HOW TO ACCESS MENU";
                previousButton.gameObject.SetActive(true);
                videoPlayer.clip = videoClips[1];
                videoPlayer.Play();
                //ADD VIDEO
                break;
            case 1:
                step++;
                text.text = "HOW TO USE THE CURSOR";
                videoPlayer.clip = videoClips[2];
                videoPlayer.Play();
                //ADD VIDEO
                break;
            case 2:
                step++;
                text.text = "HOW TO USE THE TELEPORT";
                videoPlayer.clip = videoClips[3];
                videoPlayer.Play();
                //ADD VIDEO
                break;
            case 3:
                step++;
                text.text = "SELECT ONE PORTAL";
                rawImage.gameObject.SetActive(false);
                nextButton.gameObject.SetActive(false);
                StartCoroutine(HapticFeedback.StopAllHaptics(0.3f));
                foreach (PortalSceneLoader portal in portals)
                {
                    portal.gameObject.SetActive(true);
                    portalPanel.gameObject.SetActive(true);
                }
                break;
        }

    }

    void Previous(HaptikosExoskeleton exoskeleton)
    {
        switch (step)
        {
            case 1:
                step--;
                text.text = "HOW TO CALIBRATE";
                previousButton.gameObject.SetActive(false);
                StartCoroutine(HapticFeedback.StopAllHaptics(0.3f));
                videoPlayer.clip = videoClips[0];
                videoPlayer.Play();
                //ADD VIDEO
                break;
            case 2:
                step--;
                text.text = "HOW TO ACCESS MENU";
                videoPlayer.clip = videoClips[1];
                videoPlayer.Play();
                //ADD VIDEO
                break;
            case 3:
                step--;
                text.text = "HOW TO USE THE CURSOR";
                videoPlayer.clip = videoClips[2];
                videoPlayer.Play();
                //ADD VIDEO
                break;
            case 4:
                step--;
                text.text = "HOW TO USE THE TELEPORT";
                videoPlayer.clip = videoClips[3];
                videoPlayer.Play();
                nextButton.gameObject.SetActive(true);
                rawImage.gameObject.SetActive(true);
                foreach (PortalSceneLoader portal in portals)
                {
                    portalPanel.gameObject.SetActive(false);
                    portal.gameObject.SetActive(false);
                }
                break;
        }
    }
}
