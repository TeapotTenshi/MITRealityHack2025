using Haptikos;
using Haptikos.Exoskeleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CalibateHologram : MonoBehaviour
{
    RealtimeRecenter realtimeRecenter;

    Transform vrCamera => realtimeRecenter.vrCamera;

    private Material material;
    public Collider handPlacementCollider;

    private Coroutine coroutine;

    public bool rightHand;

    bool isRunning = false;

    Color initialColor;

    HaptikosExoskeleton hand;

    private void Awake()
    {
        material = GetComponent<MeshRenderer>().material;
        initialColor = material.color;
        hand = (rightHand) ? HaptikosPlayer.GetExoskeleton(HandType.RightHand) : HaptikosPlayer.GetExoskeleton(HandType.LeftHand);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Calibration Hand"))
            if (!isRunning)
                coroutine = StartCoroutine(Countdown());
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Calibration Hand"))
        {
            isRunning = false;
            StopCoroutine(coroutine);
        }
    }

    private void OnEnable()
    {
        material.color = initialColor;
    }

    private void OnDisable()
    {
        material.color = initialColor;
    }

    IEnumerator Countdown()
    {
        isRunning = true;

        float timer = 0f;
        while (timer < 3f)
        {
            float f = (timer / 3f);
            material.color = new Color(1f - f, f, 0f, 0.588f);

            yield return new WaitForSeconds(0.1f);
            timer += 0.1f;
        }

        material.color = material.color = new Color(0, 1f, 0f, 0.588f);

        isRunning = false;
        IMUCalibrationManager.FinishedCalibration.Invoke(hand);
    }

    private void Start()
    {
        realtimeRecenter = FindObjectOfType<RealtimeRecenter>();
    }

    private void Update()
    {
        if (rightHand)
        {
            transform.position = vrCamera.position + realtimeRecenter.transform.up * 0.35f + realtimeRecenter.transform.right * 0.15f - Vector3.up * 0.3f;
            transform.right = realtimeRecenter.transform.up;
        }
        else
        {
            transform.position = vrCamera.position + realtimeRecenter.transform.up * 0.35f - realtimeRecenter.transform.right * 0.15f - Vector3.up * 0.3f;
            transform.right = -realtimeRecenter.transform.up;
        }
    }
}
