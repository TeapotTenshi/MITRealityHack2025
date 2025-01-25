using System;
using System.Collections.Generic;
using UnityEngine;

namespace Haptikos.Gloves.CommunicationLayer
{

    [Serializable]
    public class JointVector
    {
        [SerializeField]
        private float[] rotations = new float[2];

        public float[] Rotations
        {
            get { return rotations; }
            set { rotations = value; }
        }
    }
}