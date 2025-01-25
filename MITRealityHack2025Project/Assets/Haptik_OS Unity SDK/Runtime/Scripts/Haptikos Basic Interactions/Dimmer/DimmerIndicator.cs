using UnityEngine;
using Haptikos.Gloves;
using Haptikos.Exoskeleton;

namespace Haptikos.UI
{

    /// <summary>
    /// MagosDimmer indicator class.
    /// 
    /// This class handles the behavuiour of the indicator of the Dimmer. When it connects with a value index, it changes the value of the slider using collisions.
    /// </summary>
    public class DimmerIndicator : MonoBehaviour
    {
        private HaptikosDimmer dimmer;
        public bool indicationCollision = false;
        public HaptikosDimmer Dimmer
        {
            get { return dimmer; }
            set { dimmer = value; }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.name.Contains("Indication"))
            {
                indicationCollision = true;
                string[] sub = other.name.Split(',');

                if(sub.Length > 1 && int.TryParse(sub[1], out int value))
                { 
                dimmer.SetLastIndicatorValue(value);
                dimmer.DimmerValue = value;
                }

                if (dimmer.HandReference != null)
                {
                    foreach (HandPart part in dimmer.PartsIn)
                    {
                        if (part.Type == Hand_Part_Type.Finger_Tip)
                        {
                        }
                    }
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.name.Contains("Indication"))
            {
                indicationCollision = false;
            }
        }
    }
}