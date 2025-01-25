using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Haptikos.Gloves;

[RequireComponent(typeof(InteractionDetector))]
public class ColliderInfo : HapticItem
{
    InteractionDetector interactionDetector;
    HandType type => interactionDetector.LastTouchedFinger.ParentHand.hand.HandType;
    string fingerName => interactionDetector.LastTouchedFinger.Name;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Base Cube")
        {
            //Debug.Log("Active");

            //onHapticFeedbackStarted?.Invoke(true, "index3", HandType.LeftHand);

            //StartCoroutine(InvokeWithDelay(0.35f));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Base Cube")
        {
            //Debug.Log("Inactive");

            //onHapticFeedbackStarted?.Invoke(true, "index3", HandType.LeftHand);
            //StartCoroutine(InvokeWithDelay(0.35f));
        }
    }

    private IEnumerator InvokeWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        //onHapticFeedbackStarted?.Invoke(false, "index3", HandType.LeftHand);
    }

}
