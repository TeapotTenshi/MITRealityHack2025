using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

using static UtilityMethods;
using System.Linq;
using SimpleJSON;

namespace Haptikos.RecordingPlayback {

    public class ReplayHand : MonoBehaviour
    {
        private List<Hand> instances = new List<Hand>();

        [Space(5)]
        [SerializeField] HandType hand;

        [Header("Assign the animated Joints of the Hand you want to move")]
        public Transform[] animatedJoints = new Transform[16];

        [Header("Determines how fast the recording is going to be played")]
        [Header("Smaller Samples --> Faster Playback")]
        public int sampleRate = 20;
        private int sampleRate_previous = 20;

        [HideInInspector]
        public float totalSamples = 0f;

        [HideInInspector]
        public float totalDuration = 0f;

        [HideInInspector]
        public int currentSample = 0;

        public bool pause = false;

        [Range(0.0f, 0.999f)]
        [Header("Use the bar to control the animation")]
        public float bar = 0f;
        private float barPrevious = 0f;

        public void CreateInstances(TextAsset jsonRecording)
        {
            instances = CreateRecordingListFromJson(jsonRecording.ToString());

        }


        public void OnValidate()
        {
            if (bar != barPrevious)
            {
                pause = true;

                currentSample = (int)(bar * totalSamples);
                PlayInstance(instances[currentSample]);

                //Has to wait for user to ungrab the slider in order to continue from the new currentSample
                pause = false;

                barPrevious = bar;
            }

            if (sampleRate != sampleRate_previous)
            {
                if (sampleRate < 1)
                    sampleRate = 1;

                sampleRate_previous = sampleRate;
            }
        }

        public void PlayRecording(TextAsset jsonRecording)
        {

            CreateInstances(jsonRecording);

            StartCoroutine(StartAnim(sampleRate));
        }

        private List<Hand> CreateRecordingListFromJson(string jsonString)
        {
            List<Hand> recordingsAsList = new();

            //SplitJson(jsonString);
            if (jsonString != null)
            {
                JSONNode root = JsonParse(jsonString);

                for (int i = 0; i < root.Count; i++)
                {
                    Hand instance = CreateNewHandInstance(root[i]);
                    recordingsAsList.Add(instance);
                }
            }

            var recordingsForHand = recordingsAsList.Where(x => x.HandType == hand).ToList();

            if (recordingsForHand.Count == 0)
            {
                
                Debug.LogWarning("This recording is not for this Hand! Hand: " + hand);

                hand = hand == HandType.LeftHand ? HandType.RightHand : HandType.LeftHand;
                recordingsAsList = recordingsAsList.Where(x => x.HandType == hand).ToList();

            }

            
            totalSamples = recordingsAsList.Count;
            totalDuration = totalSamples * ((float)sampleRate / 1000f);

            return recordingsAsList;
        }

        private void OnDisable()
        {
            instances.Clear();
        }

        private Hand CreateNewHandInstance(JSONNode node)
        {
            Hand newHand = new Hand();

            var deserializedHand = JsonConvert.DeserializeObject<DeserializedHandData>(node.ToString());

            newHand.BatteryLevel = deserializedHand.batteryLevel;
            newHand.HandType = (HandType)deserializedHand.handType;

            newHand.ConnectionStatus = deserializedHand.connectionStatus;
            newHand.Orientation = QuaternionFromList(deserializedHand.IMU_orientation);

            for (int i = 0; i < 17; i++)
            {
                newHand.joints[i] = QuaternionFromList(deserializedHand.joints[i]);
                newHand.jointPositions[i] = VectorFromList(deserializedHand.jointPositions[i]);
            }

            return newHand;
        }

        public IEnumerator StartAnim(int startSample)
        {
            currentSample = startSample;
            bar = ((float)currentSample / (float)totalSamples);

            while (currentSample < instances.Count)
            {
                if (!pause)
                {
                    ChangeReplayHand(hand);

                    PlayInstance(instances[currentSample]);
                    currentSample++;

                    if (currentSample >= instances.Count)
                        currentSample = 0;

                    bar = ((float)currentSample / (float)totalSamples);

                    float waitSeconds = ((float)sampleRate / 1000f);
                    yield return new WaitForSeconds(waitSeconds);
                }
                else
                    yield return new WaitForEndOfFrame();
            }
        }

        public void PlayInstance(Hand instance)
        {
            animatedJoints[3].parent.parent.parent.rotation = instance.Orientation;

            for (int i = 0; i < 17; i++)
            {
                animatedJoints[i].localRotation = instance.joints[i];
                animatedJoints[i].localPosition = instance.jointPositions[i];
            }

        }

        public void ChangeReplayHand(HandType hand)
        {
            var replayHand = transform.GetChild(0);

            if(hand == HandType.LeftHand)
            {
                replayHand.transform.localRotation = Quaternion.Euler(0, 90, 0);
                replayHand.transform.localScale = new Vector3(-1, 1, 1);

            } else if(hand == HandType.RightHand)
            {
                replayHand.transform.localRotation = Quaternion.Euler(0, -90, 0);
                replayHand.transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }
}


