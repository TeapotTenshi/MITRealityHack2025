using Haptikos.Exoskeleton;
using Haptikos.Gloves;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Detects the hand that is closer to this gameobject, use this script in a object that you want to contiously check for nearby hands
/// </summary>
public class HandDetector : MonoBehaviour
{
    [SerializeField] HaptikosExoskeleton nearestGlove;

    [Space(5)]
    [Header("Number of Fingers to Return")]
    [SerializeField] int numberOfFingers = 2;
    [SerializeField] List<HandPart> nearestHapticFingers;
    [SerializeField] bool triggerNearestFingers;

    public HaptikosExoskeleton NearestGlove { get => nearestGlove; }
    public bool HandWithinAcceptableDistance { get => handWithinAcceptableDistance; }
    public List<HandPart> NearestHapticFingers { get => nearestHapticFingers; }

    public bool contiouslyUpdateFingers = true;

    HaptikosExoskeleton[] haptikGloves = new HaptikosExoskeleton[2];


    [Space(5)]
    [Header("Acceptable Distance")]
    [SerializeField] float handAcceptableDistance = 0.05f;
    [SerializeField] float fingerAcceptableDistance = 0.05f;

    [Space(2)]
    public UnityEvent<HaptikosExoskeleton> onHandWithinAcceptableDistance;

    [Space(2)]
    public UnityEvent<List<HandPart>> onFingerWithinAcceptableDistance;

    private bool handWithinAcceptableDistance = false;
    private bool fingersWithinAcceptableDistance = false;

    // Start is called before the first frame update
    void Start()
    {
        haptikGloves = FindObjectsOfType<HaptikosExoskeleton>(true);
        nearestHapticFingers = new List<HandPart>();

        StartCoroutine(CheckForGlove());
    }

    private IEnumerator CheckForGlove()
    {
        while (true)
        {
            var distance = 100000f;

            nearestGlove = null;

            foreach (var glove in haptikGloves)
            {
                var newDistance = Vector3.Distance(glove.positionReference.position, transform.position);
                if (newDistance < distance)
                {
                    nearestGlove = glove;
                    distance = newDistance;

                }
            }

            yield return new WaitForSeconds(0.5f);

            if (Vector3.Distance(nearestGlove.positionReference.position, transform.position) < handAcceptableDistance)
            {
                if(!handWithinAcceptableDistance)
                    onHandWithinAcceptableDistance?.Invoke(nearestGlove);

                handWithinAcceptableDistance = true;

            }
            else
            {
                handWithinAcceptableDistance = false;
            }

            nearestHapticFingers = nearestGlove.TouchingFingers
                .OrderBy((finger) => (finger.transform.position - transform.position).sqrMagnitude)
                .Where((nearestHapticFinger) => (Vector3.Distance(transform.position, nearestHapticFinger.transform.position) < fingerAcceptableDistance))
                .Take(numberOfFingers).ToList();

            if(nearestHapticFingers.Count > 0)
            {
                if(contiouslyUpdateFingers)
                {
                    onFingerWithinAcceptableDistance?.Invoke(nearestHapticFingers);
                }
                else
                {
                    if (!fingersWithinAcceptableDistance)
                    {
                        onFingerWithinAcceptableDistance?.Invoke(nearestHapticFingers);

                        fingersWithinAcceptableDistance = true;
                    }
                }
            }
            else
            {
                fingersWithinAcceptableDistance = false;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, 0.2f);
        Gizmos.DrawSphere(transform.position, handAcceptableDistance);


    }
}
