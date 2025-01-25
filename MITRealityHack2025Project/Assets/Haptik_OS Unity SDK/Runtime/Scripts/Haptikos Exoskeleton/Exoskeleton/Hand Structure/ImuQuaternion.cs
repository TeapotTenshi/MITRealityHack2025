using System;
using System.Collections.Generic;
using UnityEngine;

namespace Haptikos.Gloves.CommunicationLayer
{

    [Serializable]
    public class ImuQuaternion
    {
        [SerializeField]
        private float[] values = new float[4];

        public float[] Values
        {
            get { return values; }
            set { values = value; }
        }

        public ImuQuaternion()
        {
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = new float();
            }

        }

    }
}