using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Haptikos.Exoskeleton.CommunicationLayer
{
    /// <summary>
    /// Hand Model Manipulator class
    /// 
    /// This class is responsible for translating the deserialized data from the glove into rotations that a 3D model of a hand can use,
    /// to be able to move accordingly.
    /// </summary>
    [Serializable]
    public class HandModelManipulator
    {
        [SerializeField]
        private HaptikosExoskeleton exoskeleton;

        public JointObject[] fingerJoints = new JointObject[17];

        public void IntializeTransforms(Transform parent)
        {
            Transform wristTr = parent.GetChild(0).GetChild(0);
            int count = 0;

            var fingerJointsTr = wristTr.GetComponentsInChildren<Transform>(true);
            for (int i = fingerJointsTr.Length - 1; i > 0; i--)
            {
                var joint = fingerJointsTr[i];
              
                if (joint.GetComponent<Rigidbody>())
                {
                    fingerJoints[count].JointTransform = joint;
                    count++;
                }
            }
        }


        public void ApplyTransforms()
        {
            if (exoskeleton.hand == null)
                return;

            Hand magosHand = exoskeleton.hand;
            Quaternion orientation = magosHand.Orientation;

            exoskeleton.transform.localRotation = orientation;

            for (int i = 0; i < fingerJoints.Length; i++)
            {
                fingerJoints[i].JointTransform.localRotation = magosHand.joints[i];
                fingerJoints[i].JointTransform.localPosition = magosHand.jointPositions[i];
            }
            
        }

        [Serializable]
        public class JointObject
        {
            [SerializeField]
            private Transform jointTransform;

            public Transform JointTransform
            {
                get { return jointTransform; }
                set { jointTransform = value; }
            }
        }

        public HandModelManipulator(HaptikosExoskeleton magosGlove)
        {
            this.exoskeleton = magosGlove;

            for (int i = 0; i < 17; i++)
            {
                fingerJoints[i] = new JointObject();
            }
        }
    }
}