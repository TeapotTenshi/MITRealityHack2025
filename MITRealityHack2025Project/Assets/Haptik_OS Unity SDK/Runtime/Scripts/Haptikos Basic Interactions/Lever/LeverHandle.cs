using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Haptikos.Gloves;
using Haptikos.Exoskeleton;

namespace Haptikos.UI
{

    public class LeverHandle : HapticItem
    {
        private GameObject lever;

        private GameObject wrist;

        public GameObject rodHandle;

        private float distance;

        [SerializeField] float distanceForLever = 0.06f;

        private void Start()
        {
            lever = transform.parent.gameObject;
        }

        private void Update()
        {
            if (wrist)
            {
                distance = Vector3.Distance(transform.position, rodHandle.transform.position);
            }

            if (distance > distanceForLever)
            {
                transform.SetParent(lever.transform);

                transform.position = rodHandle.transform.position;

                wrist = null;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<HandPart>() && other.GetComponent<HandPart>().Type == Hand_Part_Type.Palm)
            {
                wrist = other.gameObject;

                transform.SetParent(other.GetComponent<HandPart>().ParentHand.positionReference);

                onHapticFeedbackStartAndEnd?.Invoke(true, "wrist", other.GetComponent<HandPart>().ParentHand.hand.HandType, true);
            }
        }
    }
}