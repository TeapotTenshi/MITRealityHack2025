using System;
using System.Collections.Generic;
using UnityEngine;

namespace Haptikos.Gloves.CommunicationLayer
{

    [Serializable]
    public class Joint
    {
        [SerializeField]
        private string jointName;

        [SerializeField]
        private JointVector jointRotation = new JointVector();

        public string JointName
        {
            get { return jointName; }
            set { jointName = value; }
        }

        public JointVector JointRotation
        {
            get { return jointRotation; }
            set { jointRotation = value; }
        }

    }
}