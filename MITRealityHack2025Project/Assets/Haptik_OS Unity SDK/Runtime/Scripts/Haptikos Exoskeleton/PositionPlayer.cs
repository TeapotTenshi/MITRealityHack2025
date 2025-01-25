using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PositionPlayer : MonoBehaviour
{
    private Transform cameraTransform;
    private Vector3 initial_cameraPosition = new Vector3();

    private void Awake()
    {
        cameraTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
        initial_cameraPosition = cameraTransform.position;
    }

    private void Start()
    {
        StartCoroutine(Flicks());

    }

    IEnumerator Flicks()
    {
        yield return new WaitForSeconds(0.2f);

        //Wait for HMD to be put on
        //while (OpenVR.System != null && OpenVR.System.GetTrackedDeviceActivityLevel(OpenVR.k_unTrackedDeviceIndex_Hmd) != EDeviceActivityLevel.k_EDeviceActivityLevel_UserInteraction)
        while(!IsHardwarePresent())
        {
            yield return new WaitForSeconds(Time.deltaTime);

        }

        Debug.Log("Headset was put on");

        //Apply the opposite offset into player (except from vertical axis)
        Vector3 offset = cameraTransform.position - initial_cameraPosition;
        this.transform.position -= new Vector3(offset.x, 0f, offset.z);


        //Checking for some seconds if more than one flicks occur
        Vector3 previous_pos = cameraTransform.position;
        float seconds = 0f;
        while (seconds < 10f)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            seconds += Time.deltaTime;

            float dist = Vector3.Distance(previous_pos, cameraTransform.position);
            if (dist > 0.5f)
            {
                Debug.Log("Player flick corrected.");

                //Go back to the initial position
                offset = cameraTransform.position - initial_cameraPosition;
                this.transform.position -= new Vector3(offset.x, 0f, offset.z);
            }

            previous_pos = cameraTransform.position;
        }

        Debug.Log("PositionPlayer stopped checking.");
    }

    public static bool IsHardwarePresent()
    {
        var xrDisplaySubsystems = new List<XRDisplaySubsystem>();
        SubsystemManager.GetInstances<XRDisplaySubsystem>(xrDisplaySubsystems);
        foreach (var xrDisplay in xrDisplaySubsystems)
        {
            if (xrDisplay.running)
            {
                return true;
            }
        }
        return false;
    }
}
