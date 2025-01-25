using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Haptikos.Sizing
{
    /// <summary>
    /// This component can modify the length of current finger joint and its child joint
    /// </summary>
    public class DistanceJoints : MonoBehaviour
    {
        public float desiredLength = 0f;
        private Transform nextJoint;

        public float distance = 0f;

        public bool scaleMode = false;      /*this mode exists only in fingertips (index3, middle3, ...) */


        private void Start()
        {
            nextJoint = this.transform.GetChild(0);

            if (scaleMode && desiredLength > 0f)
            {
                distance = Vector3.Distance(this.transform.position, nextJoint.position);
                float scale = desiredLength / distance;
                this.transform.localScale = new Vector3(scale, 1f, 1f);
            }

            if (!scaleMode && desiredLength > 0f)
            {
                Vector3 v = nextJoint.position - this.transform.position;
                nextJoint.position = this.transform.position + (v.normalized * desiredLength);
            }
            //else keep default distance and print it in inspector

            distance = Vector3.Distance(this.transform.position, nextJoint.position);
        }

        /*private void Update()
        {
            //For debugging
            distance = Vector3.Distance(this.transform.position, nextJoint.position);
        }*/
    }

}