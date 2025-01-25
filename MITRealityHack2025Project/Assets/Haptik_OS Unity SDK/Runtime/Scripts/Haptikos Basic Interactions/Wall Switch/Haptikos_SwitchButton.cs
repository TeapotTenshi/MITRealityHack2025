using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Haptikos_SwitchButton : HapticItem
{
    // 2 states, on and off, user can choose initial position
    public float angle = 3.35f;
    public Collider areaOff;
    public Collider areaOn;

    // In which state is currently the button
    public bool isOn = false;

    private Vector3 initial;

    private void OnValidate()
    {
        // angle variable not to take negative values
        angle = Mathf.Max(angle, 0);
    }


    private void Start()
    {
        this.transform.localEulerAngles = new Vector3(this.transform.localEulerAngles.x, angle, this.transform.localEulerAngles.z);
        initial = this.transform.localEulerAngles;

        SubscribeToButtonAreaEvents();
    }

    private void SubscribeToButtonAreaEvents()
    {
        var haptikosSwitchArea = GetComponentsInChildren<Haptikos_SwitchButtonArea>();
        foreach (var switchArea in haptikosSwitchArea)
        {
            switchArea.OnButtonAreaTouched.AddListener((hand, finger) => { onHapticFeedbackStartAndEnd?.Invoke(true, finger.Name, hand.hand.HandType, true); });
        }
    }

    private void Update()
    {
        UpdateSwitch();
    }


    private void UpdateSwitch()
    {
        // Check which state the button is on start (positive or negative value)
        if (this.transform.localEulerAngles == initial)
            isOn = false;
        else
            isOn = true;

        if (isOn)
        {
            areaOn.enabled = true;
            areaOff.enabled = false;
        }
        else
        {
            areaOff.enabled = true;
            areaOn.enabled = false;
        }
    }
}
