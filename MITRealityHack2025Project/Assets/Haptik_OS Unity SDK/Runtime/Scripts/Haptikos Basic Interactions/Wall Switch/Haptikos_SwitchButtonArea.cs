using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Haptikos.Gloves;
using Haptikos.Exoskeleton;
using UnityEngine.Events;


public class Haptikos_SwitchButtonArea : MonoBehaviour
{
    public UnityEvent<HaptikosExoskeleton, HandPart> OnButtonAreaTouched = new();

    private void OnTriggerEnter(Collider other)
    {
        // on hand touch change state
        if (other.gameObject.GetComponent<HandPart>() != null)
        {
            var fingerTouching = other.gameObject.GetComponent<HandPart>();

            float y_angle = (-1) * this.transform.parent.localEulerAngles.y;
            this.transform.parent.localEulerAngles = new Vector3(this.transform.localEulerAngles.x, y_angle, this.transform.localEulerAngles.z);

            OnButtonAreaTouched?.Invoke(fingerTouching.ParentHand, fingerTouching);
        }
    }
}
