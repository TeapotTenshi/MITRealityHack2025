using System;
using System.Collections.Generic;
using UnityEngine;

namespace Haptikos.Gloves.CommunicationLayer
{
    [Serializable]
    public class Finger
    {
        [SerializeField]
        private string fingerName;

        [SerializeField]
        private Joint[] joints = new Joint[3];

        public string FingerName
        {
            get { return fingerName; }
            set { fingerName = value; }
        }


        public Joint[] Joints
        {
            get { return joints; }
            set { joints = value; }
        }

        public Finger()
        {
            for (int i = 0; i < joints.Length; i++)
            {
                joints[i] = new Joint();
            }
        }
    }
}