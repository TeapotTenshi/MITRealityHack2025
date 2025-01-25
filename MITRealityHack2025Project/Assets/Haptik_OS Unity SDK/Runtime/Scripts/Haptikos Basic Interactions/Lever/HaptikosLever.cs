using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Haptikos.UI
{
    [Serializable]
    public class OnLeverValueChanged : UnityEvent<float> { }

    public class HaptikosLever : MonoBehaviour
    {
        public HingeJoint leverJoint;

        public GameObject leverBase;

        [SerializeField]
        private float leverValue;

        public OnLeverValueChanged onLeverValue;

        public float LeverValue
        {
            get { return leverValue; }
            set
            {
                leverValue = value;

                onLeverValue?.Invoke(value);
            }
        }
        // Start is called before the first frame update
        private void Awake()
        {
            leverBase = transform.Find("Base").gameObject;
        }

        private void Start()
        {
            leverJoint = leverBase.GetComponent<HingeJoint>();
        }

        // Update is called once per frame
        void Update()
        {

            LeverValue = ((leverJoint.angle + leverJoint.limits.max) / 110) * 100;
        }
    }
}