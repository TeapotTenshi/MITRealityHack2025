using Haptikos.Gloves;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Haptikos.RecordingPlayback
{
    [RequireComponent(typeof(ReplayHand))]
    public class PlayRecording : MonoBehaviour
    {
        [Header("Recording On Start")]
        [Space(5)]
        [SerializeField] bool startRecordingOnStart = true;

        [Space(5)]
        [SerializeField] TextAsset recordingJson;

        ReplayHand replay;

        private void Awake()
        {
            if(replay != null) 
            replay = GetComponent<ReplayHand>();
        }

        [ExecuteInEditMode]
        private void OnValidate()
        {
            replay = GetComponent<ReplayHand>();
            replay.CreateInstances(recordingJson);
        }

        // Start is called before the first frame update
        void Start()
        {
            if (recordingJson == null)
                gameObject.SetActive(false);

            if (startRecordingOnStart)
            {
                replay.PlayRecording(recordingJson);
            }

        }
    }
}

