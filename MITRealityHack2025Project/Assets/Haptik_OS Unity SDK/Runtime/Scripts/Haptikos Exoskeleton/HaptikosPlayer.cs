using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Haptikos.Gloves;
using Haptikos.Exoskeleton;
using System.Linq;


namespace Haptikos
{

    /// <summary>
    /// Haptikos Player Class
    /// 
    /// This class contains a pair of Haptikos Exoskeleton Gloves
    /// </summary>
    public class HaptikosPlayer : MonoBehaviour
    {

        /// <summary>
        /// The Exoskeleton Gloves of this player
        /// </summary>
        public static HaptikosExoskeleton[] exoskeletonGloves;

        public static bool calibrated;

        DataStreamingEvents dataStreamingEvents;

        //protected override void Awake()
        //{
        //    base.Awake();
        //    exoskeletonGloves = transform.GetComponentsInChildren<HaptikosExoskeleton>();
        //}

        private void Awake()
        {
            exoskeletonGloves = transform.GetComponentsInChildren<HaptikosExoskeleton>();
            dataStreamingEvents = GetComponent<DataStreamingEvents>();
            StartCoroutine(LoadCalibrated());
        }

        IEnumerator LoadCalibrated()
        {
            yield return new WaitForSeconds(3f);
            calibrated = SceneInformation.calibrated;
        }

        private void OnEnable()
        {
            dataStreamingEvents.OnDataReceived += SetCalibratedFalse;
            dataStreamingEvents.OnDataStoppedReceiving += SetCalibratedFalse;
            IMUCalibrationManager.FinishedCalibration.AddListener(SetCalibratedTrue);
        }

        private void OnDisable()
        {
            dataStreamingEvents.OnDataReceived -= SetCalibratedFalse;
            dataStreamingEvents.OnDataStoppedReceiving -= SetCalibratedFalse;
            IMUCalibrationManager.FinishedCalibration.RemoveListener(SetCalibratedTrue);
        }

        public static HaptikosExoskeleton GetExoskeleton(HandType hand)
        {
            return exoskeletonGloves.Where(glove => glove.hand.HandType == hand).First();
        }

        void SetCalibratedFalse(HaptikosExoskeleton hand)
        {
            calibrated = false;
        }

        void SetCalibratedTrue(HaptikosExoskeleton hand)
        {
            calibrated = true;
        }
    }
}