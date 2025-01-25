using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandMeshController : MonoBehaviour
{
    [SerializeField] MeshRenderer leftHandMeshRenderer;
    [SerializeField] MeshRenderer rightHandMeshRenderer;

    public void ShowHand(HandType hand)
    {
        if (hand == HandType.LeftHand)
        {
            leftHandMeshRenderer.enabled = true;
        }
        else if (hand == HandType.RightHand)
        {
            rightHandMeshRenderer.enabled = true;
        }
    }

    public void HideHand(HandType hand)
    {
        if (hand == HandType.LeftHand)
        {
            leftHandMeshRenderer.enabled = false;
        }
        else if (hand == HandType.RightHand)
        {
            rightHandMeshRenderer.enabled = false;
        }
    }

    private void Start()
    {
        var meshRenderers = GameObject.FindGameObjectsWithTag("HandMesh");
    }
}
