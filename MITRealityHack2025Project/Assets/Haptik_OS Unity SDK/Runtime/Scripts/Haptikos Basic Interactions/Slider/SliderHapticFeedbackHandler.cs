using UnityEngine;
using Haptikos.Gloves;
using System;
using Haptikos.Exoskeleton;

namespace Haptikos.UI
{
    /// <summary>
    /// Movig handle class of MagosSlider.
    /// This class contains the logic of how the Slider handle is grabbed.
    /// </summary>
    public class SliderHapticFeedbackHandler : HapticItem
    {
        /// <summary>
        /// The MagosSlider that this handle is on.
        /// </summary>
        private HaptikosSlider slider;
        /// <summary>
        /// The MagosSlider that this handle is on getter / setter.
        /// </summary>
        HandDetector handDetector;

        private void Awake()
        {
            handDetector = GetComponent<HandDetector>();
            slider = GetComponentInParent<HaptikosSlider>();
        }

        private void OnEnable()
        {
            slider.onSliderSnap.AddListener(TriggerHapticFeedback);
        }

        private void OnDisable()
        {
            slider.onSliderSnap.RemoveListener(TriggerHapticFeedback);
        }

        private void TriggerHapticFeedback()
        {
            foreach (var finger in handDetector.NearestHapticFingers)
            {
                onHapticFeedbackStartAndEnd?.Invoke(true, finger.Name, finger.ParentHand.hand.HandType, true);
            }
        }

        public HaptikosSlider Slider
        {
            get { return slider; }
            set { slider = value; }
        }

        private void OnTriggerEnter(Collider other)
        {
            HandPart hp = other.gameObject.GetComponent<HandPart>();

            if (other.GetComponent<HandPart>() != null && !slider.PartsIn.Contains(other.GetComponent<HandPart>()))
            {
                slider.PartsIn.Add(other.GetComponent<HandPart>());

                onHapticFeedbackStartAndEnd?.Invoke(true, hp.Name, hp.ParentHand.hand.HandType, true);
            }

        }

        private void OnTriggerExit(Collider other)
        {
            HandPart hp = other.gameObject.GetComponent<HandPart>();

            if (other.GetComponent<HandPart>() != null && slider.PartsIn.Contains(other.GetComponent<HandPart>()))
            {
                slider.PartsIn.Remove(other.GetComponent<HandPart>());
            }
        }
    }
}