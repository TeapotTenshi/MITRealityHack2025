using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class HaptikosRaycastCurved : HaptikosRaycast
{
    [Range(0f, 1f)]
    public float archAngleModifier;

    protected override bool CreateRay(Vector3[] originalPositions, out Vector3[] positions, out RaycastHit hit) //Returns true a selectable object was hit
    {
        Vector3 velocity = direction.normalized;
        float gravity =  archAngleModifier;
        float distance = 0.03f;
        int size = (int)(rayRange/distance);
 
        Vector3[] positionsArray = new Vector3[size];
        positionsArray[0] = originalPositions[0];

        int counter = 1;
        while (!Physics.Raycast(positionsArray[counter - 1], velocity, out hit, distance + 0.05f, includeLayers))
        {
            positionsArray[counter] = positionsArray[counter - 1] + velocity * distance;
            float time = distance / velocity.magnitude;
            velocity.y -= time * gravity;

            counter++;
            if (counter == size - 1)
            {
                positions = new Vector3[counter];
                for (int i = 0; i < counter; i++)
                {
                    positions[i] = positionsArray[i];
                }
                return false;
            }
        }

        positionsArray[counter] = hit.point;
        positions = new Vector3[counter + 1];
        for (int i = 0; i < counter + 1; i++)
        {
            positions[i] = positionsArray[i];
        }
        
        if (((1 << hit.transform.gameObject.layer) & targetLayers) != 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}

[CustomEditor(typeof(HaptikosRaycastCurved))]

public class HaptikosRaycastCurvedEditor : Editor
{
    HaptikosRaycast raycast;
    public override void OnInspectorGUI()
    {
        raycast = (HaptikosRaycastCurved)target;
        DrawDefaultInspector();
        if (raycast.Warning)
        {
            EditorGUILayout.HelpBox(raycast.getStatus, MessageType.Warning);
        }

    }
}
