using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Haptikos.Gloves;
using Haptikos.Exoskeleton;

namespace Haptikos.UI
{
    /// <summary>
    /// Button Press Event Class
    /// 
    /// A Unity Event for the HaptikosSlider class to handle.
    /// </summary>
    [Serializable]
    public class OnSliderValueChanged : UnityEvent<float> { }

    /// <summary>
    /// Vritrual Slider Class.
    /// 
    /// This class is responsible for mimicing a real life Slider in a VR environment.
    /// It Also handles the according events to trigger any action setted as the event listener.
    /// </summary>
    public class HaptikosSlider : MonoBehaviour
    {

        /// <summary>
        /// The not moving handle of the Slider.
        /// </summary>
        private GameObject handle;


        /// <summary>
        /// The moving handle of the Slider.
        /// </summary>
        private GameObject movingHandle;

        /// <summary>
        /// The parent object of all the indications.
        /// </summary>
        private GameObject indicationsParent;

        /// <summary>
        /// The closest indication from the handle.
        /// </summary>
        private Transform closestIndication;

        /// <summary>
        /// A list containing all the indications of the Slider.
        /// </summary>
        private List<Transform> indications;

        /// <summary>
        /// All the HandParts that are touching the Slider.
        /// </summary>
        private List<HandPart> partsIn;

        /// <summary>
        /// Turns true when the slider is currently being held.
        /// </summary>
        private bool parentSet = false;

        /// <summary>
        /// The current value of the slider.
        /// </summary>
        [SerializeField]
        private float sliderValue;

        /// <summary>
        /// Instance of OnSliderValueChanged event.
        /// </summary>
        [SerializeField]
        public OnSliderValueChanged sliderValueChangedEvent;

        [SerializeField]
        public UnityEvent onSliderSnap = new();

        /// <summary>
        /// Slider value getter / setter.
        /// </summary>
        public float SliderValue
        {
            get { return sliderValue; }
            set
            {
                sliderValueChangedEvent?.Invoke(value);

                foreach (HandPart handPart in partsIn)
                {
                    if (handPart.Type == Hand_Part_Type.Finger_Tip)
                    {
                        HaptikosExoskeleton glove = Held(partsIn).ParentHand;

                        if (glove != null)
                        {
                            glove.uDPReciever.SendHapticData(handPart.Name);
                        }
                    }
                }

                sliderValue = value;

            }
        }

        /// <summary>
        /// List of HandParts touching the Slider getter / setter.
        /// </summary>
        public List<HandPart> PartsIn
        {
            get { return partsIn; }
            set { partsIn = value; }
        }

        private void Awake()
        {
            handle = transform.Find("Handle").gameObject;
            movingHandle = transform.Find("Moving_Handle").gameObject;
            indicationsParent = transform.Find("IndicationsParent").gameObject;

            indications = new List<Transform>();
            partsIn = new List<HandPart>();
        }

        private void Start()
        {
            movingHandle.GetComponent<SliderHapticFeedbackHandler>().Slider = this;

            foreach (Transform tr in indicationsParent.transform)
            {
                indications.Add(tr);
            }

            SliderValue = SetSliderValue();
        }

        private void Update()
        {
            if (Held(partsIn) && !parentSet)
            {
                movingHandle.transform.SetParent(Held(partsIn).ParentHand.transform);
                parentSet = true;

            }

            float distanceBetwenHandles = Vector3.Distance(handle.transform.position, movingHandle.transform.position);

            if (distanceBetwenHandles > 0.03)
            {
                closestIndication = GetClosestIndication(indications, movingHandle.transform);

                onSliderSnap?.Invoke();

            }

            if (closestIndication)
            {
                float distanceBetweenIdicationAndHandle = Vector3.Distance(handle.transform.position, closestIndication.transform.position);

                if (distanceBetweenIdicationAndHandle > 0.02)
                {
                    handle.transform.position = closestIndication.transform.position;

                    SliderValue = SetSliderValue();

                }
            }

            if (distanceBetwenHandles >= 0.05) ResetHandle();
        }

        /// <summary>
        /// Sets the position of the Slider handle value by finding the closest indication.
        /// </summary>
        /// <returns> The value that the slider should take.</returns>
        public int SetSliderValue()
        {
            closestIndication = GetClosestIndication(indications, movingHandle.transform);
            string[] sub = closestIndication.transform.name.Split(',');

            return int.Parse(sub[1]);
        }

        /// <summary>
        /// Sets the position of the moving handle to be the same as the not moving handle when the user ungrabs it.
        /// </summary>
        public void ResetHandle()
        {
            movingHandle.transform.SetParent(transform);
            movingHandle.transform.position = handle.transform.position;
            parentSet = false;
        }

        /// <summary>
        /// Determines which HandPart should be the parent of the moving handle once grabbed.
        /// </summary>
        /// <param name="parts"> A list of all the HandParts that are touching the moving handle.</param>
        /// <returns> the HandPart to be set as the parent.</returns>
        public HandPart Held(List<HandPart> parts)
        {
            HandPart thumb = Array.Find(parts.ToArray(), prt => prt.Name.Contains("thumb"));

            if (thumb != null && parts.Count >= 2 && AllFingersFromTheSameHand(thumb))
            {
                return thumb;
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// Determines if all the fingers on the moving handle are from the same Hand.
        /// </summary>
        /// <param name="thumbIn"> A handPart reference to check ll the others from.</param>
        /// <returns> true if all the other fingers are from the same Hand.</returns>
        public bool AllFingersFromTheSameHand(HandPart thumbIn)
        {
            foreach (HandPart part in partsIn)
            {
                if (part.ParentHand != thumbIn.ParentHand)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Determines which is the closest indication for the moving handle to go to.
        /// </summary>
        /// <param name="indications"> All the available points to move to.</param>
        /// <param name="fromThis"> The Transform that the distance is calculated from.</param>
        /// <returns></returns>
        public Transform GetClosestIndication(List<Transform> indications, Transform fromThis)
        {
            Transform bestTarget = null;
            float closestDistanceSqr = Mathf.Infinity;
            Vector3 currentPosition = fromThis.position;
            foreach (Transform potentialTarget in indications)
            {
                Vector3 directionToTarget = potentialTarget.position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if (dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    bestTarget = potentialTarget;
                }
            }
            return bestTarget;
        }
    }
}