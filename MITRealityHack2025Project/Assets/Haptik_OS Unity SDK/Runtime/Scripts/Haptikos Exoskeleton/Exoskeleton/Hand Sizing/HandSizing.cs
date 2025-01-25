using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Haptikos.Sizing
{
    /// <summary>
    /// This class is responsible for orchsestrating and modifying the lengths between finger joints
    /// Waits a second before starts executing. It actual wait for joints1 to be modified, so make sure that DistanceJoints component on these joints are enabled
    /// </summary>
    public class HandSizing : MonoBehaviour
    {
        public DistanceJoints index2, middle2, ring2, pinky2, thumb2;

        private void Start()
        {
            StartCoroutine(LateStart());
        }

        IEnumerator LateStart()
        {
            yield return new WaitForSeconds(1f);

            index2.enabled = true;
            middle2.enabled = true;
            ring2.enabled = true;
            pinky2.enabled = true;
            thumb2.enabled = true;

            yield return new WaitForSeconds(1f);

            index2.transform.GetChild(0).GetComponent<DistanceJoints>().enabled = true;
            middle2.transform.GetChild(0).GetComponent<DistanceJoints>().enabled = true;
            ring2.transform.GetChild(0).GetComponent<DistanceJoints>().enabled = true;
            pinky2.transform.GetChild(0).GetComponent<DistanceJoints>().enabled = true;
            thumb2.transform.GetChild(0).GetComponent<DistanceJoints>().enabled = true;
        }
    }
}

