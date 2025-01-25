using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;
using Haptikos.Exoskeleton.CommunicationLayer;
using static UtilityMethods;
using static Haptikos.Exoskeleton.CommunicationLayer.HandModelManipulator;

namespace Haptikos.Exoskeleton
{
    /// <summary>
    /// Haptikos Exoskeleton class
    /// 
    /// This class is the root of the Haptikos Exoskeleton communication layer, it is responsible for controlling every component that takes
    /// data from the device and translates them into a 3D representation of a human hand.
    /// </summary>
    public class HaptikosExoskeleton : MonoBehaviour
    {
        /// <summary>
        /// The transform of the controller on the scene
        /// </summary>
        public Transform positionReference;

        /// <summary>
        /// List containing all the Hand Parts of this Glove
        /// </summary>
        public List<HandPart> handParts = new List<HandPart>();

        /// <summary>
        /// Action that triggers each time the player pinches with his fingers.
        /// </summary>
        public Action<HandPart, HandPart> pinchAction;

        /// <summary>
        /// The thumb of the Hand.
        /// </summary>
        public HandPart thumbTip;

        /// <summary>
        /// The index finger of the thumb.
        /// </summary>
        public HandPart indexTip;

        /// <summary>
        /// Turns true if the use is pinching with this hand.
        /// </summary>
        public bool pinching = false;

        /// <summary>
        /// The Network communication port
        /// </summary>
        int middlewarePort;

        /// <summary>
        /// The Hand class of the glove is the deserialized data from the device.
        /// </summary>
        public Hand hand;

        /// <summary>
        /// The 3D representation of the hand.
        /// </summary>
        public HandModelManipulator handModelManipulator;

        /// <summary>
        /// Networking layer
        /// </summary>
        public UDPReceiver uDPReciever;

        /// <summary>
        /// if true, it applies the rotations taken from the device.
        /// </summary>
        public bool applyRotations = true;

        public int battery_level;

        CancellationTokenSource cts;
        CancellationToken ct;
        Task receive;
        float handScale = 0f;
        Rigidbody rb;

        const int left_glove_port = 272;
        const int right_glove_port = 273;

        DataStreamingEvents dataStreamingEvents;

        [SerializeField]
        List<HandPart> touchingFingers = new();

        public List<HandPart> TouchingFingers { get => touchingFingers; }


        /// <summary>
        /// The data recieved from the device.
        /// </summary>
        [TextArea]
        [SerializeField]
        private string json = "";
        private float factor;


        private void Awake()
        {
            if(hand.HandType == HandType.LeftHand)
            {
                middlewarePort = left_glove_port;
            }
            else if(hand.HandType == HandType.RightHand)
            {
                middlewarePort = right_glove_port;
            }

            uDPReciever = new UDPReceiver(middlewarePort);
            handModelManipulator = new HandModelManipulator(this);

            uDPReciever.InitializeConnection();
            handModelManipulator.IntializeTransforms(transform);

            rb = GetComponent<Rigidbody>();
            dataStreamingEvents = GetComponentInParent<DataStreamingEvents>();

            HandPart palm = transform.GetChild(0).transform.GetChild(0).transform.gameObject.AddComponent<HandPart>();
            palm.Type = Hand_Part_Type.Palm;
            palm.Name = "wrist";
            palm.TargetObject = transform.GetChild(0).transform.GetChild(0).transform.gameObject;
            palm.ParentHand = this;

            handParts.Add(palm);

            for (int i = 0; i < (handModelManipulator.fingerJoints.Length); i += 3)
            {
                if (i == 0 || i == 13)
                {
                    for (int j = i; j < i + 4; j++)
                    {
                        var finger = handModelManipulator.fingerJoints[j];

                        if (j == i + 3)
                        {
                            AddHandParts(finger, Hand_Part_Type.Finger_Base);
                            break;
                        }

                        AddHandParts(finger, (Hand_Part_Type)(j - i));

                    }
                    i++;
                }
                else
                {
                    for (int j = i; j < i + 3; j++)
                    {
                        var finger = handModelManipulator.fingerJoints[j];

                        AddHandParts(finger, (Hand_Part_Type)(j - i));
                    }
                }
            }
        }

        /// <summary>
        /// Populates each Joint of the 3D hand with a Hand Part to enable interactions.
        /// </summary>
        /// <param name="finger"> the finger that will get populated. </param>
        public void AddHandParts(JointObject finger, Hand_Part_Type handPartType)
        {
            var part_tip = finger.JointTransform.gameObject.AddComponent<HandPart>();
            part_tip.Type = handPartType;
            part_tip.Name = finger.JointTransform.name;
            part_tip.TargetObject = finger.JointTransform.gameObject;
            part_tip.ParentHand = this;

            handParts.Add(part_tip);
        }

        private void Start()
        {
            if (hand.HandType == HandType.LeftHand)
                middlewarePort = left_glove_port;
            else if (hand.HandType == HandType.RightHand)
                middlewarePort = right_glove_port;

            if (hand.HandType == HandType.LeftHand)
            {
                factor = -1;
            }
            else
            {
                factor = 1;
            }

            CreateTouchingFingers();

            cts = new CancellationTokenSource();
            ct = cts.Token;
            ReceiveJSON();
            StartCoroutine(CheckConnection());
        }

        IEnumerator CheckConnection()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.5f);
                long milliseconds = uDPReciever.stopwatch.ElapsedMilliseconds;
                if(hand.HandType == HandType.LeftHand)
                {
                    if (milliseconds > 200)
                    {
                        if (ExoskeletonConnectionController.LeftGloveConnected)
                        {
                            dataStreamingEvents.CallOnDataStoppedReceiving(this);
                        }
                        else
                        {
                            uDPReciever.SendHapticData("HERE " + middlewarePort);
                        }
                    }
                    else
                    {
                        if (!ExoskeletonConnectionController.LeftGloveConnected)
                        {
                            dataStreamingEvents.CallOnDataReceived(this);
                        }
                    }
                }
                else if (hand.HandType == HandType.RightHand)
                {
                    if (milliseconds > 200)
                    {
                        if (ExoskeletonConnectionController.RightGloveConnetected)
                        {
                            dataStreamingEvents.CallOnDataStoppedReceiving(this);
                        }
                        else
                        {
                            uDPReciever.SendHapticData("HERE " + middlewarePort);
                        }
                    }
                    else
                    {
                        if (!ExoskeletonConnectionController.RightGloveConnetected)
                        {
                            dataStreamingEvents.CallOnDataReceived(this);
                        }
                    }
                }
            }
        }

        private void CreateTouchingFingers()
        {
            touchingFingers.Add(handParts[1]);
            touchingFingers.Add(handParts[5]);
            touchingFingers.Add(handParts[8]);
            touchingFingers.Add(handParts[11]);
            touchingFingers.Add(handParts[14]);
        }

        private void OnEnable()
        {
            dataStreamingEvents.OnDataReceived += DataStreamingEvents_OnDataReceived;
            dataStreamingEvents.OnDataStoppedReceiving += DataStreamingEvents_OnDataStoppedReceiving;
        }

        private void DataStreamingEvents_OnDataStoppedReceiving(HaptikosExoskeleton hand)
        {
            if (hand != this)
            {
                return;
            }
            json = string.Empty;
            StopCoroutine(RotationsEnumerator());
        }

        private void DataStreamingEvents_OnDataReceived(HaptikosExoskeleton hand)
        {
            if (hand != this)
            {
                return;
            }
            StartCoroutine(RotationsEnumerator());
        }

        private void OnDisable()
        {
            cts.Cancel();
            uDPReciever.StopConnection();
            StopCoroutine(RotationsEnumerator());

            dataStreamingEvents.OnDataReceived -= DataStreamingEvents_OnDataReceived;
            dataStreamingEvents.OnDataStoppedReceiving -= DataStreamingEvents_OnDataStoppedReceiving;
        }


        /// <summary>
        /// Oppening a new Thread to recieve and Deserialize the data from the device.
        /// </summary>
        public void ReceiveJSON()
        {
            receive = new Task(() => { receiveTask(this); }, ct);
            receive.Start();
        }

        void receiveTask(HaptikosExoskeleton parent)
        {
            while (parent!=null)
            {
                
                //blocks execution
                json = uDPReciever.GetData();

                var handDataUDP = JsonConvert.DeserializeObject<DeserializedHandData>(json);


                battery_level = handDataUDP.batteryLevel;

                hand.ConnectionStatus = handDataUDP.connectionStatus;

                hand.Position = VectorFromList(handDataUDP.position);
                hand.Orientation = QuaternionFromList(handDataUDP.IMU_orientation);

                for (int i = 0; i < 17; i++)
                {
                    hand.joints[i] = QuaternionFromList(handDataUDP.joints[i]);
                    hand.jointPositions[i] = VectorFromList(handDataUDP.jointPositions[i]);
                }

                if (handScale == 0)
                    handScale = handDataUDP.handScale;

            }
            Debug.Log("parent is null");
        }

        private void OnApplicationQuit()
        {
            //Disable all haptics before exit
            uDPReciever.SendHapticData("index3 off@0");
            uDPReciever.SendHapticData("thumb3 off@0");
            uDPReciever.SendHapticData("middle3 off@0");
            uDPReciever.SendHapticData("pinky3 off@0");
            uDPReciever.SendHapticData("ring3 off@0");

            //Send message to middleware to inform that application just exited
            uDPReciever.SendHapticData("quit");
        }

        private void Update()
        {
            Vector3 targetPosition = positionReference.position;

            Vector3 dir = targetPosition - transform.position;

            Vector3 pos = dir.normalized * dir.magnitude * 20;

            rb.velocity = pos;

            CalculatePinchDistance();//?????

        }

        /// <summary>
        /// Determines if the user is pinching.
        /// </summary>
        public void CalculatePinchDistance()
        {
            JointObject thumb = handModelManipulator.fingerJoints[0];
            JointObject index = handModelManipulator.fingerJoints[4];

            HandPart thumbTip = thumb.JointTransform.GetComponent<HandPart>();
            HandPart indexTip = index.JointTransform.GetComponent<HandPart>();

            this.thumbTip = thumbTip;
            this.indexTip = indexTip;

            float distance = Vector3.Distance(thumbTip.TargetObject.transform.GetChild(0).position, indexTip.TargetObject.transform.GetChild(0).position);

            if (distance <= 0.02 && !pinching)
            {
                pinchAction?.Invoke(thumbTip, indexTip);
                pinching = true;
            }

            if (distance > 0.02 && pinching)
            {
                pinching = false;
            }
        }

        /// <summary>
        /// Enumerator that applies the recieved data to the 3D hand using the Hand Manipulator class.
        /// </summary>
        /// <returns></returns>
        private IEnumerator RotationsEnumerator()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.02f);

                if (applyRotations)
                {
                    handModelManipulator.ApplyTransforms();

                    if (handScale != 0)
                        transform.localScale = new Vector3(factor * handScale, handScale, handScale);
                }

            }

        }   
    }
}