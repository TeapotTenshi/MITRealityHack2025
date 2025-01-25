using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Haptikos.Gloves;
using Haptikos;
using Haptikos.Exoskeleton;

public class CalibrationController : MonoBehaviour
{
    HaptikosExoskeleton rightGlove;
    HaptikosExoskeleton leftGlove;
    Transform rightController;
    Transform leftController;
    bool calibrating = false;
    public Vector3 leftOffset;
    public Vector3 rightOffset;
    Transform haptikPlayer;

    private void OnEnable()
    {
        IMUCalibrationManager.FinishedCalibration.AddListener(IMUCalibration);
    }

    void OnDisable()
    {
        IMUCalibrationManager.FinishedCalibration.RemoveAllListeners();
    }

    // Start is called before the first frame update
    void Start()
    {
        haptikPlayer = FindFirstObjectByType<HaptikosPlayer>().transform;
        rightGlove = haptikPlayer.transform.GetChild(4).GetComponent<HaptikosExoskeleton>();
        leftGlove = haptikPlayer.transform.GetChild(3).GetComponent<HaptikosExoskeleton>();
        rightController = haptikPlayer.transform.GetChild(2);
        leftController = haptikPlayer.transform.GetChild(1);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && !calibrating)
        {
            IMUCalibrationManager.FinishedCalibration.Invoke(rightGlove);
        }
    }

    void IMUCalibration(HaptikosExoskeleton hand)
    {
        if (!calibrating)
        {
            StartCoroutine(CalibrateCoroutine());
        }
    }


    IEnumerator CalibrateCoroutine()
    {
        calibrating = true;
        if (ExoskeletonConnectionController.RightGloveConnetected)
        {
            rightGlove.uDPReciever.SendHapticData("Calibrate");
        }
        else
        {
            leftGlove.uDPReciever.SendHapticData("Calibrate");
        }
        yield return new WaitForSeconds(0.3f);
        CalibrateControllers();
        calibrating = false;
    }

    void CalibrateControllers()
    {
        Vector3 offset;

        Transform leftTarget = leftController.GetChild(0);
        Transform rightTarget = rightController.GetChild(0);

        offset = leftOffset.x * leftGlove.transform.right + leftOffset.y * leftGlove.transform.up + leftOffset.z * leftGlove.transform.forward;
        leftTarget.position = leftController.position - offset;

        offset = rightOffset.x * rightGlove.transform.right + rightOffset.y * rightGlove.transform.up + rightOffset.z * rightGlove.transform.forward;
        rightTarget.position = rightController.position - offset;
    }
}
