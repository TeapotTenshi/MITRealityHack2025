using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaptikosPinchRaycastController : HaptikosRayCastController
{
    Transform indexBase;
    Transform thumbBase;
    Transform wrist;
    GameObject helper;

    public float offset = 0.15f;

    protected override void Start()
    {
        CheckComponets();

        wrist = hand.transform.GetChild(0).GetChild(0);
        indexBase = wrist.GetChild(3);
        thumbBase = wrist.GetChild(4);
        if(helper == null)
        {
            helper = new();
            helper.transform.parent = transform;
            helper.name = "helper";
        }
    }

    protected override void Update()
    {
        if (!raycast.Validated)
        {
            return;
        }

        Vector3 startingPoint = (indexBase.position + thumbBase.position) / 2;
        helper.transform.rotation = wrist.rotation;
        helper.transform.RotateAround(helper.transform.position, helper.transform.forward, -factor * 40);
        raycast.direction = factor * helper.transform.right;
        raycast.startingPoint = startingPoint + raycast.direction * offset;
        
    }


}
