using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Haptikos.Exoskeleton
{
    public enum Hand_Part_Type
    {
        Finger_Tip,
        Finger_Middle,
        Finger_Base,
        Palm, 
        All
    }

    /// <summary>
    /// A Hand Part is a main tool to interact with the environment.
    /// Every joiunt of each finger contains one to detect collisions
    /// with other hand parts, allowing interactions to occur.
    /// </summary>
    [System.Serializable]
    public class HandPart : MonoBehaviour
    {
        /// <summary>
        /// The name of the HandPart
        /// </summary>
        [SerializeField]
        private new string name;

        /// <summary>
        /// The Collider of the HandPart.
        /// </summary>
        [SerializeField]
        private Collider partCollider;

        /// <summary>
        /// The GameObhject of the HandPart in the scene.
        /// </summary>
        [SerializeField]
        private GameObject target;

        /// <summary>
        /// The Hand that this HandPart belongs to.
        /// </summary>
        [SerializeField]
        private HaptikosExoskeleton parentHand;

        /// <summary>
        /// The Type of the handPart, it can be the base of the finger,
        /// the middle section of the finger,
        /// the fingertip or the Palm of the Hand.
        /// </summary>
        [SerializeField]
        private Hand_Part_Type type;

        /// <summary>
        /// A list of every other HandPart that is touching this one.
        /// </summary>
        [SerializeField]
        private List<HandPart> handPartsInside = new List<HandPart>();

        /// <summary>
        /// HandPart name's getter / setter.
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// The GameObject of the Handparts getter / setter.
        /// </summary>
        public GameObject TargetObject
        {
            get { return target; }
            set { target = value; }
        }

        /// <summary>
        /// Parent Hand getter / setter.
        /// </summary>
        public HaptikosExoskeleton ParentHand
        {
            get { return parentHand; }
            set { parentHand = value; }
        }


        /// <summary>
        /// HandPart Collider getter / setter.
        /// </summary>
        public Collider Collider
        {
            get { return partCollider; }
            set { partCollider = value; }
        }

        /// <summary>
        /// HandParts touching this one getter / setter.
        /// </summary>
        public List<HandPart> HandPartsInside
        {
            get { return handPartsInside; }
            set { handPartsInside = value; }
        }

        /// <summary>
        /// HandPart type getter / setter.
        /// </summary>
        public Hand_Part_Type Type
        {
            get { return type; }
            set { type = value; }
        }

        private void Start()
        {
            partCollider = this.gameObject.GetComponent<Collider>();
            partCollider.isTrigger = false;
        }

        public void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.GetComponent<HandPart>() != null && !handPartsInside.Contains(collision.gameObject.GetComponent<HandPart>()))
            {
                handPartsInside.Add(collision.gameObject.GetComponent<HandPart>());
            }

            if (collision.gameObject.tag == "Haptikos_Object" && type == Hand_Part_Type.Finger_Tip)// && !hapticSent)
            {
                //Debug.Log($"<color=magenta> Haptics GO (Collision Enter) {name}! </color>");
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.tag == "Haptikos_Object" && type == Hand_Part_Type.Finger_Tip)// && !hapticSent)
            {
                //Debug.Log($"<color=magenta> Haptics GO (Collision Exit) {name}! </color>");
            }
        }


        //addition below is used for the haptikos button flickering 

        private bool isCurrentlyTriggered = false;
        private float exitDelay = 0.2f; // Time to wait before considering it fully exited
        private Coroutine exitRoutine = null;

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Haptikos_Object" && type == Hand_Part_Type.Finger_Tip)
            {
                if (exitRoutine != null)
                {
                    StopCoroutine(exitRoutine); // Cancel any pending exit routine
                    exitRoutine = null;
                }

                if (!isCurrentlyTriggered)
                {
                    isCurrentlyTriggered = true; // Mark as triggered
                    TriggerHaptics(true, other);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Haptikos_Object" && type == Hand_Part_Type.Finger_Tip)
            {
                // Start a delayed exit to ensure no flickering
                if (exitRoutine == null)
                {
                    exitRoutine = StartCoroutine(DelayedExitRoutine(other));
                }
            }
        }

        private IEnumerator DelayedExitRoutine(Collider other)
        {
            yield return new WaitForSeconds(exitDelay);

            if (isCurrentlyTriggered)
            {
                isCurrentlyTriggered = false; // Mark as not triggered
                TriggerHaptics(false, other);
            }

            exitRoutine = null; // Clear the coroutine
        }

        private void TriggerHaptics(bool activate, Collider other)
        {
            if (activate)
            {
                int intensity = 20;
                HapticElement intensity_component = other.GetComponent<HapticElement>();
                if (intensity_component != null)
                    intensity = intensity_component.hapticsIntensity;
                //Debug.Log($"<color=green> Haptics GO (Trigger Enter)! {name}</color>");
            }
            else
            {
                //Debug.Log($"<color=green> Haptics OFF (Trigger Exit)! {name}</color>");
            }
        }
    }
}