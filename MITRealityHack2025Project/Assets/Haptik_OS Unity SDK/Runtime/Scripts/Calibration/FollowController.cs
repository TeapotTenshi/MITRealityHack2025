using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Haptikos;

public class FollowController : MonoBehaviour
{
    public Transform controller;
    Material material;
    public bool isRight;
    MeshRenderer meshRenderer;

    void Awake()
    {
        int index = (isRight) ? 2 : 1;
        controller = FindAnyObjectByType<HaptikosPlayer>().transform.GetChild(index).GetChild(0);
        meshRenderer = GetComponent<MeshRenderer>();
        material = meshRenderer.material;
        material.color = new Color(0.5f, 0.5f, 0.5f, 0.588f);
    }

    void OnDisable()
    {
        meshRenderer.enabled = false;   
    }

    private void OnEnable()
    {
        meshRenderer.enabled = true;
    }

    void Update()
    {
        transform.position = controller.position;
    }
}
