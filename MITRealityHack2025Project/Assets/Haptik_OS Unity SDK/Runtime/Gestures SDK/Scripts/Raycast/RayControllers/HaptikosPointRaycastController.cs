using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HaptikosPointRaycastController : HaptikosRayCastController
{
    Transform indexBase;
    Transform wrist;
    
    public float offset = 0.12f;

     protected override void Start()
    {
        CheckComponets();

        wrist = hand.transform.GetChild(0).GetChild(0);
        indexBase = wrist.GetChild(3);
    }

    protected override void Update()
    {
        if (!raycast.Validated)
        {
            return;
        }
        raycast.direction = indexBase.right * factor;//On the left hand the x axis of the wrist transform points in the opposite direction
        raycast.startingPoint = indexBase.position + raycast.direction * offset;
    }
}
