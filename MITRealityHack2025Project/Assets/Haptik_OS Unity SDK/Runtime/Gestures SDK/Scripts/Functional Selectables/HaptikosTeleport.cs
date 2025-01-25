using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Haptikos;
using Haptikos.Gloves;
using Haptikos.Exoskeleton;

public class HaptikosTeleport : HaptikosRaycastAwareSelectable
{
    Transform haptikPlayer;
    Transform mainCamera;

    void Start()
    {
        haptikPlayer = FindAnyObjectByType<HaptikosPlayer>().transform;
        mainCamera = haptikPlayer.GetChild(0);
        if (!mainCamera.CompareTag("MainCamera"))
        {
            Debug.LogWarning("Game object is not a Haptikos Player");
        }
    }

    new protected void OnEnable()
    {
        base.OnEnable();
        OnClick.AddListener(Teleport);
    }


    void Teleport(HaptikosExoskeleton hand)
    {
        Vector3 cameraOffset = mainCamera.position - haptikPlayer.transform.position;
        Vector3 target = raycast.HitPoint - cameraOffset;
        target.y = haptikPlayer.transform.position.y;
        haptikPlayer.transform.position = target;
    }
}
