using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Use it on a gameobject, that you want to be grabbable using the Haptikos Exoskeleton Gloves
/// </summary>
public class Haptikos_Grab : MonoBehaviour
{
    [Tooltip("Whether or not palm to be necessary in order to grab this gameObject.")]
    public bool palmNeeded = false;

    GameObject instatiated_trigger;

    private void Start()
    {
        if (GetComponent<Collider>() == null)
        {
            MeshCollider mc = this.gameObject.AddComponent<MeshCollider>();
            mc.convex = true;
        }
        if (GetComponent<Rigidbody>() == null)
        {
            Rigidbody rb = this.gameObject.AddComponent<Rigidbody>();
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }


        GameObject temp = this.gameObject;
        //this has to be destroyed instead of being disabled:
        temp.GetComponent<HapticFeedback>().enabled = false;
        Destroy(temp.GetComponent<Haptikos_Grab>());

        instatiated_trigger = Instantiate(temp);

        instatiated_trigger.transform.parent = this.transform;
        instatiated_trigger.transform.localPosition = new Vector3(0f, 0f, 0f);
        instatiated_trigger.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        instatiated_trigger.gameObject.tag = "Untagged";
        if (instatiated_trigger.transform.childCount > 0)
        {
            instatiated_trigger.transform.GetChild(0).gameObject.tag = "Untagged";          /*temp line, must be generic or removed! */
        }

        //Check if Mesh else if other collider
        if(instatiated_trigger.GetComponent<Renderer>() != null)
            instatiated_trigger.GetComponent<Renderer>().enabled = false;

        if(instatiated_trigger.GetComponent<MeshCollider>() != null)
        {
            instatiated_trigger.GetComponent<MeshCollider>().convex = true;
            instatiated_trigger.GetComponent<MeshCollider>().isTrigger = true;
        }
        

        Haptikos_Clone_Grab comp = instatiated_trigger.AddComponent<Haptikos_Clone_Grab>();
        Destroy(instatiated_trigger.GetComponent<Haptikos_Grab>());

        instatiated_trigger.GetComponent<HapticFeedback>().enabled = true;

        comp.palmNeeded = palmNeeded;

        Rigidbody r = instatiated_trigger.GetComponent<Rigidbody>();
        if (r != null)
        {
            DestroyImmediate(r);
        }


    }

}
