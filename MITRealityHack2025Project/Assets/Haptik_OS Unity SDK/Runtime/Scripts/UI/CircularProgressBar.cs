using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CircularProgressBar : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] TMP_Text circularBarText;


    // Start is called before the first frame update
    void Start()
    {
        slider.onValueChanged.AddListener(percentage => {
            circularBarText.text = Mathf.FloorToInt(percentage) + "%";

            if(percentage == 100)
            {
                percentage = 0;
                gameObject.SetActive(false);
            }
        });
    }

    public void SetCircularProgressBarValue(float value)
    {
        slider.value = Mathf.FloorToInt(value);
    }
}
