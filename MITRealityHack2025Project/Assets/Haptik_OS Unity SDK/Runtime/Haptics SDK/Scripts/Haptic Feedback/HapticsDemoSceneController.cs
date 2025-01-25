using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class HapticsDemoSceneController : MonoBehaviour
{

    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private GameObject[] gameobjects;






    // Start is called before the first frame update
    void Start()
    {
        dropdown.onValueChanged.AddListener(OnDropDownValueChanged);

        UpdateActiveObject(dropdown.value);
    }
    
    void OnDropDownValueChanged(int index)
    {
        UpdateActiveObject(index);
    }

    void UpdateActiveObject(int activeIndex)
    {
        for(int i=0; i< gameobjects.Length; i++)
        {
            if (gameobjects[i] != null)
            {
                gameobjects[i].SetActive(i == activeIndex);
            }
        }
    }
}
