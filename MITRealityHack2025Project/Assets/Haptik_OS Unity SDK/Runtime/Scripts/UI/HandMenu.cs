using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandMenu : MonoBehaviour
{
    [SerializeField] GameObject handMenu;
    [SerializeField] GameObject circularProgressBar;
    
    public void OpenMenu()
    {
        handMenu.SetActive(true);
        circularProgressBar.SetActive(false);
    }

    public void OpenProgressBar()
    {
        circularProgressBar.SetActive(true);
        handMenu.SetActive(false);
    }
}
