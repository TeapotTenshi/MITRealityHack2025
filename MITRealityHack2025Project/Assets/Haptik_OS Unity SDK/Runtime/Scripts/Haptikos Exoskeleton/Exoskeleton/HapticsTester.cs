using Haptikos.Exoskeleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace Haptikos.Gloves
{
    /// <summary>
    /// Haptics Tester class
    /// 
    /// This component can trigger haptics on a Haptikos glove during runtime
    /// Easily enable/disable specific fingertip's haptic and also modify its intensity via slider
    /// In order to work must be placed on a Haptikos glove gameobject (MagosGlove component required)
    /// </summary>
    public class HapticsTester : MonoBehaviour
    {
        private HaptikosExoskeleton glove = null;

        public bool indexOn = false;
        [Range(0, 100)]
        public int indexIntensity = 1;

        public bool middleOn = false;
        [Range(0, 100)]
        public int middleIntensity = 1;

        public bool ringOn = false;
        [Range(0, 100)]
        public int ringIntensity = 1;

        public bool pinkyOn = false;
        [Range(0, 100)]
        public int pinkyIntensity = 1;

        public bool thumbOn = false;
        [Range(0, 100)]
        public int thumbIntensity = 1;


        private void Start()
        {
            glove = this.GetComponent<HaptikosExoskeleton>();
        }

        private void OnValidate()
        {
            if (glove != null && glove.hand.ConnectionStatus != "")
            {
                Debug.Log("Validate - haptics sent.");

                if (indexOn)
                    glove.uDPReciever.SendHapticData("index3 on@" + indexIntensity);
                else
                    glove.uDPReciever.SendHapticData("index3 off@0");

                if (middleOn)
                    glove.uDPReciever.SendHapticData("middle3 on@" + middleIntensity);
                else
                    glove.uDPReciever.SendHapticData("middle3 off@0");

                if (ringOn)
                    glove.uDPReciever.SendHapticData("ring3 on@" + ringIntensity);
                else
                    glove.uDPReciever.SendHapticData("ring3 off@0");

                if (pinkyOn)
                    glove.uDPReciever.SendHapticData("pinky3 on@" + pinkyIntensity);
                else
                    glove.uDPReciever.SendHapticData("pinky3 off@0");

                if (thumbOn)
                    glove.uDPReciever.SendHapticData("thumb3 on@" + thumbIntensity);
                else
                    glove.uDPReciever.SendHapticData("thumb3 off@0");
            }
        }
    }
}
