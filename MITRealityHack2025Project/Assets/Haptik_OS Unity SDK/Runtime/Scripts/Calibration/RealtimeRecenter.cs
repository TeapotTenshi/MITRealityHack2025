using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Management;
using UnityEngine.XR;
using System;

public class RealtimeRecenter : MonoBehaviour
{
    public Transform vrCamera;

    [Header("Invoke this event to recenter all devices based on current HMD position and orientation.")]
    public UnityEvent reCenter;
    XRInputSubsystem xrInput;


    private void Awake()
    {
        vrCamera = Camera.main.transform;

        //copy only the y rotation - this is the steamvr orientation-angle when game starts
        transform.eulerAngles = new Vector3(this.transform.eulerAngles.x, vrCamera.transform.eulerAngles.y, this.transform.eulerAngles.z);
    }

    private void Start()
    {
        var xrSettings = XRGeneralSettings.Instance;
        if (xrSettings == null)
        {
            Debug.Log($"XRGeneralSettings is null.");
            return;
        }

        var xrManager = xrSettings.Manager;
        if (xrManager == null)
        {
            Debug.Log($"XRManagerSettings is null.");
            return;
        }

        var xrLoader = xrManager.activeLoader;
        if (xrLoader == null)
        {
            Debug.Log($"XRLoader is null.");
            return;
        }

        xrInput = xrLoader.GetLoadedSubsystem<XRInputSubsystem>();

        if (xrInput != null)
        {
            
            xrInput.TrySetTrackingOriginMode(TrackingOriginModeFlags.Floor);            /*should be the default */
        }

        reCenter.AddListener(ReCenter);
    }

 
    private void Update()
    {
        transform.position = new Vector3(vrCamera.position.x, 0f, vrCamera.position.z);
    }

    public void ReCenter()
    {
        //Centers the tracking features on all InputDevices to the current position and orientation of the HMD
        if (xrInput != null)
        {
            xrInput.TryRecenter();
            Debug.Log("Recenter() executed!");

        }
        else
            Debug.Log("Couldn't recenter. xrInput == null!");

    }
}
