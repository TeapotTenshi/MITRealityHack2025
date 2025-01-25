using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Haptikos.Gloves;
using Haptikos.Exoskeleton;

public class ExampleSceneController : MonoBehaviour
{
    HaptikosRaycast[] raycasts = new HaptikosRaycast[4];
    HaptikosGestureRecognizer[] recognizers = new HaptikosGestureRecognizer[2];
  
    // Start is called before the first frame update
    void Awake()
    {
        Transform child = transform.GetChild(0);
        
        for(int i = 0; i < 4; i++)
        {
            raycasts[i] = child.GetChild(i).GetComponent<HaptikosRaycast>();
        }
        raycasts[0].enabled = true;
        raycasts[1].enabled = true;
        raycasts[2].enabled = false;
        raycasts[3].enabled = false;

        recognizers = GetComponents<HaptikosGestureRecognizer>();
       
    }

    private void OnEnable()
    {
        foreach(HaptikosGestureRecognizer recognizer in recognizers)
        {
            recognizer.enabled = true;
            recognizer.OnActivate.AddListener(changeState);
        }
    }

    private void OnDisable()
    {
        foreach (HaptikosGestureRecognizer recognizer in recognizers)
        {
            recognizer.enabled = false;
            recognizer.OnActivate.RemoveListener(changeState);
        }
    }
    void changeState(HaptikosExoskeleton hand)
    {
        foreach(HaptikosRaycast raycast in raycasts)
        {
            raycast.enabled = !raycast.enabled;
        }
    }
}
