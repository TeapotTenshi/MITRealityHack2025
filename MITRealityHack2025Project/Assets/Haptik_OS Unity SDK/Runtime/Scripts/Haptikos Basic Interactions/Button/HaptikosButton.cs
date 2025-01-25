using System;
using UnityEngine;
using UnityEngine.Events;
using Haptikos.Gloves;
using System.Collections.Generic;
using System.Collections;
using Haptikos.Exoskeleton;

namespace Haptikos.UI
{
    /// <summary>
    /// Button Press Event Class
    /// 
    /// A Unity Event for the MagosButton class to handle.
    /// </summary>
    [System.Serializable]
    public class OnButtonPressed : UnityEvent { }

    /// <summary>
    /// Vritrual Button Class.
    /// 
    /// This class is responsible for mimicing a real life button in a VR environment.
    /// It Also handles the according events to trigger any action setted as the event listener.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class HaptikosButton : HapticItem
    {
        /// <summary>
        /// The distance that the button needs to travel to trigger the OnButtonPressEvent.
        /// </summary>
        public float pressLength;

        /// <summary>
        /// The horizontal limits of the button (how much the pressable button can move on the X and Z axis.)
        /// </summary>
        public float horizontalAxisLimits = 0.1f;

        /// <summary>
        /// The Rigidbody of the Button.
        /// </summary>
        public Rigidbody rb;

        /// <summary>
        /// The starting position of the Button.
        /// </summary>
        public Vector3 startingPosition;

        /// <summary>
        /// Instance of OnButtonPressed event.
        /// </summary>
        [SerializeField]
        public OnButtonPressed buttonPressedEvent; //To use from Unity's UI

        /// <summary>
        /// A C# action to trigger along side the OnButtonPressEvent, to assign methods real time in code.
        /// </summary>
        public Action<HaptikosButton> pressAction; //To use in code

        /// <summary>
        /// Turns true when the button is pressed
        /// </summary>
        bool pressed = false;

        //private HapticFeedback curve;
        List<HandPart> parts = new List<HandPart>();
        public bool isButtonExit = false;
        private bool isCurrentlyTriggered = false;
        private float exitDelay = 0.2f; // Time to wait before considering it fully exited
        private Coroutine exitRoutine = null;
        HandPart lastTouchedHandPart;

        private void Awake()
        {
            startingPosition = transform.localPosition;
            rb = GetComponent<Rigidbody>();

        }

        void Update()
        {
            float distanceY = Mathf.Abs(transform.localPosition.y - startingPosition.y);
            float distanceX = Mathf.Abs(transform.localPosition.x - startingPosition.x);
            float distanceZ = Mathf.Abs(transform.localPosition.z - startingPosition.z);

            //button position correction
            if (distanceX > horizontalAxisLimits || distanceZ > horizontalAxisLimits)
            {
                transform.localPosition = transform.localPosition.With(x: startingPosition.x, z: startingPosition.z);
            }

            if (!pressed)
            {
                if (transform.localPosition.y > startingPosition.y) transform.localPosition = transform.localPosition.With(y: startingPosition.y);

                //press detection
                if (distanceY >= pressLength)
                {
                    transform.localPosition = transform.localPosition.With(y: startingPosition.y - pressLength);

                    if (!pressed)
                    {
                        buttonPressedEvent?.Invoke();

                        pressAction?.Invoke(this);

                        pressed = true;
                    }
                }
                else
                {
                    pressed = false;
                }
            }

            if (distanceY >= pressLength)
            {
                transform.localPosition = transform.localPosition.With(y: startingPosition.y - pressLength);

                if (lastTouchedHandPart != null)
                    onHapticFeedbackStartAndEnd?.Invoke(true, lastTouchedHandPart.Name, lastTouchedHandPart.ParentHand.hand.HandType, true);

            }
           
        }

        private void OnTriggerEnter(Collider other)
        {
            HandPart hp = other.gameObject.GetComponent<HandPart>();
            lastTouchedHandPart = hp;

            if (hp != null && !parts.Contains(hp))
            {
                parts.Add(hp);

                if (exitRoutine != null)
                {
                    StopCoroutine(exitRoutine); // Cancel any pending exit routine
                    exitRoutine = null;
                }

                if (!isCurrentlyTriggered)
                {
                    isCurrentlyTriggered = true; // Mark as triggered
                    TriggerHaptics(true, hp.Name, hp.ParentHand.hand.HandType);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            HandPart hp = other.gameObject.GetComponent<HandPart>();
            lastTouchedHandPart = hp;

            if (hp != null && parts.Contains(hp))
            {
                parts.Remove(hp);
                if (exitRoutine == null)
                {
                    exitRoutine = StartCoroutine(DelayedExitRoutine(hp.Name, hp.ParentHand.hand.HandType));
                }
            }

           
        }

        private IEnumerator DelayedExitRoutine(string other, HandType type)
        {
            yield return new WaitForSeconds(exitDelay);

            if (isCurrentlyTriggered)
            {
                isCurrentlyTriggered = false; // Mark as not triggered
                TriggerHaptics(false, other, type);

                if (parts.Count == 0)
                    onHapticFeedbackStartAndEnd?.Invoke(true, lastTouchedHandPart.Name, lastTouchedHandPart.ParentHand.hand.HandType, true);
            }
            exitRoutine = null; // Clear the coroutine
        }

        private void TriggerHaptics(bool activate, string other, HandType type)
        {
            if (activate)
            {
                onHapticFeedbackStartAndEnd?.Invoke(true, other, type, true);
                isButtonExit = false;
            }
            else
            {
                onHapticFeedbackStartAndEnd?.Invoke(false, other, type, true);
                isButtonExit = true;
            }
        }
    }
}