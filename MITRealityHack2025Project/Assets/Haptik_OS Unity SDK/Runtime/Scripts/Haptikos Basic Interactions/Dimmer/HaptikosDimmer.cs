using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Haptikos.Gloves;
using Haptikos.Exoskeleton;

namespace Haptikos.UI
{
    [Serializable]
    public class OnDimmerValueChanged : UnityEvent<float> { }

    public class HaptikosDimmer : HapticItem
    {
        private GameObject dimmerHandle;
        private GameObject dimmerHandleMoving;
        private GameObject dimmerIndicator;
        private HaptikosExoskeleton handReference;

        private float currentRotation = 0;
        private Action<float> rotateEvent;
        public List<HandPart> partsIn = new List<HandPart>();
        private float initialHandZ;

        [SerializeField]
        private int dimmerValue = 0;
        public OnDimmerValueChanged onDimmerValueChangedEvent;
        private int lastIndicatorValue = 0;
        private int previousIndicatorValue = -1;
        private int previousDimmerValue = -1;

        public GameObject DimmerHandle => dimmerHandle;
        public HaptikosExoskeleton HandReference
        {
            get => handReference;
            set => handReference = value;
        }

        public List<HandPart> PartsIn
        {
            get => partsIn;
            set => partsIn = value;
        }

        public float InitialHandZ
        {
            get => initialHandZ;
            set => initialHandZ = value;
        }

        public int DimmerValue
        {
            get => dimmerValue;
            set
            {
                dimmerValue = value;
                onDimmerValueChangedEvent?.Invoke(value);
            }
        }

        public float CurrentRotation
        {
            get => currentRotation;
            set => currentRotation = value;
        }

        private void Awake()
        {
            dimmerHandle = transform.Find("DimmerHandle").gameObject;
            dimmerHandleMoving = transform.Find("DimmerHandleMoving").gameObject;
            dimmerIndicator = dimmerHandle.transform.Find("Indicator").gameObject;

            dimmerHandleMoving.GetComponent<DimmerHandle>().Dimmer = GetComponent<HaptikosDimmer>();
            dimmerIndicator.GetComponent<DimmerIndicator>().Dimmer = GetComponent<HaptikosDimmer>();

            rotateEvent += OnRotate;
        }


        public void OnRotate(float amount)
        {
            dimmerHandle.transform.localEulerAngles = new Vector3(0, amount, 0);
        }

        private void Update()
        {
            if (handReference)
            {
                float rotateAmount = initialHandZ - handReference.transform.localEulerAngles.CorrectedEulers().z;
                rotateEvent.Invoke(rotateAmount + currentRotation);
            }

            //addition snap to indicator
            else
            {
                SnapToIndicatorValue(); // Always snap back to the last valid indicator on release
            }

            CheckForValueChange();

        }

        void CheckForValueChange()
        {
            if (dimmerValue != previousDimmerValue)
            {
                TriggerHapticFeedback(dimmerValue);
                previousDimmerValue = dimmerValue;
            }
        }

        //addition for snapping

        public void SetLastIndicatorValue(int value)
        {
            lastIndicatorValue = value;
        }

        //addition for snapping
        private void SnapToIndicatorValue()
        {
            float targetRotation = MapValueToRotation(lastIndicatorValue);
            dimmerHandle.transform.localEulerAngles = new Vector3(dimmerHandle.transform.localEulerAngles.x, targetRotation, dimmerHandle.transform.localEulerAngles.z);
            TriggerHapticFeedback(lastIndicatorValue);
        }

        private void TriggerHapticFeedback(int indicatorValue)
        {
            if (handReference != null)
            {
                foreach (HandPart part in partsIn)
                {
                    if(part.Type == Hand_Part_Type.Finger_Tip)
                    {
                        onHapticFeedbackStartAndEnd?.Invoke(true, part.Name, part.ParentHand.hand.HandType, true);
                    }
                }
            }
        }

        //addition for snapping
        private float MapValueToRotation(int value)
        {
            return Mathf.Lerp(0f, 360f, value / 8f); // Adjust the division factor (8f) based on your indicator count
        }
    }
}