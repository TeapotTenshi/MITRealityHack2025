using System;
using System.Collections.Generic;
using UnityEngine;
using Haptikos.Gloves;
using System.Collections;
using Haptikos.Exoskeleton;

namespace Haptikos.UI
{

    /// <summary>
    /// Movig handle class of Haptikos Dimmer.
    /// This class contains the logic of how the Dimmer is grabbed.
    /// </summary>
    public class DimmerHandle : HapticItem
    {

        private HaptikosDimmer dimmer;
        private Vector3 initialPos;
        private Vector3 dimmerHandleGlobalPos;
        private Quaternion InitialRot;
        public DimmerIndicator dimmerIndicator;


        public HaptikosDimmer Dimmer
        {
            get { return dimmer; }
            set { dimmer = value; }
        }

        private void Start()
        {
            initialPos = dimmer.DimmerHandle.transform.localPosition;
            InitialRot = transform.localRotation;
        }

        private void Update()
        {
            dimmerHandleGlobalPos = dimmer.DimmerHandle.transform.position;

            if (dimmer.HandReference != null)
            {
                float distance = Vector3.Distance(dimmerHandleGlobalPos, dimmer.HandReference.transform.position);

                //addition validation

                ValidatePartsIn();

                if (distance >= 0.2 || !ValidGrabCondition(dimmer.PartsIn))
                {
                    ResetHandle();
                }
            }
        }

        private IEnumerator InvokeWithDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            onHapticFeedbackStarted?.Invoke(false, "index3", HandType.LeftHand);
        }

        private void OnTriggerEnter(Collider collider)
        {
            HandPart hp = collider.gameObject.GetComponent<HandPart>();
            if(hp != null)
            {
                if (!dimmer.PartsIn.Contains(hp))
                {
                    dimmer.PartsIn.Add(hp);
                }

                onHapticFeedbackStartAndEnd?.Invoke(true, hp.Name, hp.ParentHand.hand.HandType, true);
                
                //addition validation 

                if (ValidGrabCondition(dimmer.PartsIn) && AreFingersFromTheSameHand(dimmer.PartsIn))
                {
                    dimmer.HandReference = hp.ParentHand;
                    dimmer.InitialHandZ = dimmer.HandReference.transform.localEulerAngles.CorrectedEulers().z;
                    dimmer.CurrentRotation = dimmer.DimmerHandle.transform.localEulerAngles.CorrectedEulers().y;

                    transform.SetParent(dimmer.HandReference.transform);
                }
            }
        }

        private void OnTriggerExit(Collider collider)
        {
            HandPart hp = collider.gameObject.GetComponent<HandPart>();
            if (hp != null && dimmer.PartsIn.Contains(hp))
            {
                dimmer.PartsIn.Remove(hp);
                onHapticFeedbackStartAndEnd?.Invoke(false, hp.Name, hp.ParentHand.hand.HandType, true);

                //addition validation
                if (!ValidGrabCondition(dimmer.PartsIn))
                {
                    ResetHandle();
                }
            }
        }

        // addition validate that fingers are grabbing

        private bool ValidGrabCondition(List<HandPart> parts)
        {
            bool thumbTouching = false;
            bool otherFingerTouching = false;

            foreach (HandPart part in parts)
            {
                if (part.Name.Contains("thumb2") || part.Name.Contains("thumb3"))
                {
                    thumbTouching = true;
                }
                else
                {
                    otherFingerTouching = true;
                }

                if(thumbTouching && otherFingerTouching)
                {
                    return true;
                }
            }
            return false;
        }
        
        //addition validate that parts are still in the dimmer

        private void ValidatePartsIn()
        {
            dimmer.PartsIn.RemoveAll(part => part == null || !part.GetComponent<Collider>().bounds.Intersects(GetComponent<Collider>().bounds));
        }

        public void ResetHandle()
        {
            transform.SetParent(dimmer.transform);
            transform.localPosition = initialPos;
            transform.localRotation = InitialRot;
            if (dimmer.HandReference != null) dimmer.HandReference = null;
        }

        public bool ThumbTouching(List<HandPart> parts)
        {
            foreach (HandPart part in parts)
            {
                if (part.Name.Contains("thumb"))
                {
                    return true;
                }
            }

            return false;
        }

        public bool AreFingersFromTheSameHand(List<HandPart> parts)
        {
            HandPart thumb = Array.Find(parts.ToArray(), part => part.Name.Contains("thumb"));

            if (thumb)
            {
                foreach (HandPart part in parts)
                {
                    if (part.ParentHand != thumb.ParentHand)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}