using Haptikos.Exoskeleton;
using Haptikos.Gloves;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaptikosFollowRay : HaptikosRaycastAwareSelectable
{
    Vector3 originalDirection;
    Vector3 originalDistanceVector;
    Rigidbody rb;
    [Range(0f,20f)]
    public float followSpeed;
    [Range(0f,2f)]
    public float throwSpeed = 1f;
    HaptikosRaycast curRaycast;
    bool locked = false;
    float rayMaxLength;
    LineRenderer line;
    Queue<Vector3> lastVelocities = new();
    Vector3 velocity;
    LayerMask rayLayerMask;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            enabled = false;
            Debug.LogError("Rigidbody Required for Haptikos Follow Ray objects");
            return;
        }
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    new protected void OnEnable()
    {
        base.OnEnable();
        OnClick.AddListener(LockToRay);
        OnClickRelease.AddListener(UnlockFromRay);
    }

    void LockToRay(HaptikosExoskeleton hand)
    {
        if (locked)
        {
            return;
        }

        rayLayerMask = raycast.targetLayers;
        raycast.targetLayers = 0;//We want the ray to not interact with other object while it is locked to this object

        originalDirection = raycast.direction;
        originalDistanceVector = transform.position - raycast.startingPoint;
        curRaycast = raycast;
        locked = true;
        rayMaxLength = curRaycast.rayRange;
        curRaycast.rayRange = originalDistanceVector.magnitude;
        raycast.drawDefaultRay = false;
        line = raycast.Line;
        lastVelocities.Clear();
        int count = (int)(Mathf.Pow(Time.fixedDeltaTime,-1f)/3f);
        
        for(int i = 0; i < count; i++)
        {
            lastVelocities.Enqueue(Vector3.zero);
        }
    }



    void FollowRay()
    {
        Quaternion relativeRotation = Quaternion.FromToRotation(originalDirection, curRaycast.direction);
        Vector3 distanceVector = relativeRotation * originalDistanceVector;
        Vector3 targetPosition = curRaycast.startingPoint + distanceVector;
        
        Vector3 direction = targetPosition - transform.position;

        velocity = direction * followSpeed;
        rb.velocity = velocity;
    }

    void UnlockFromRay(HaptikosExoskeleton hand)
    {
        locked = false;
        curRaycast.rayRange = rayMaxLength;
        Throw();
        curRaycast = null;
        raycast.drawDefaultRay = true;
        raycast.targetLayers = rayLayerMask;
    }

    void Throw()//TODO: Give momentum when released while moving
    {
        Vector3 exitVelocity = Vector3.zero;
        int count = lastVelocities.Count;
        while (lastVelocities.Count > 0)
        {
            exitVelocity += lastVelocities.Dequeue();
        }
        exitVelocity /= count;
        rb.velocity = exitVelocity*throwSpeed;
    }

    void Update()
    {
        
        if (locked)
        {
            if (curRaycast == null)
            {
                locked = false;
                return;
            }

            FollowRay();
            line.positionCount = 2;
            Vector3[] positions = { raycast.startingPoint, transform.position };
            line.SetPositions(positions);
        }
    }

    void FixedUpdate()
    {
        if (locked)
        {
            lastVelocities.Dequeue();
            lastVelocities.Enqueue(velocity);
        }
    }
}
