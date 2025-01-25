using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Haptikos.Gloves;
using Haptikos.Exoskeleton;

public class HaptikosHandposeCalculator : MonoBehaviour
{
    Transform wrist;

    //we don't need wrist
    Transform thumb1, thumb2, thumb3;
    Transform index1, index2, index3;
    Transform middle1, middle2, middle3;
    Transform ring1, ring2, ring3;
    Transform pinky1, pinky2, pinky3;
    Transform thumbTip, indexTip, middleTip, ringTip, pinkyTip;

    public HaptikosHandpose currentHandpose;
    Quaternion[] rotations = new Quaternion[15];
    Vector3 xAxisThumb;
    Vector3 yAxisWrist;
    Vector3 xAxisWrist;
    Vector3[] tips = new Vector3[5];

    float factor;

    void Awake()
    {
        HaptikosExoskeleton hand = GetComponent<HaptikosExoskeleton>();
        HandType handType = hand.hand.HandType;

        if (handType == HandType.RightHand)
        {
            factor = 1;
        }
        else if (handType == HandType.LeftHand)
        {
            factor = -1;
        }

        wrist = transform.GetChild(0).GetChild(0);

        thumb1 = wrist.transform.GetChild(4).GetChild(0);
        thumb2 = thumb1.transform.GetChild(0);
        thumb3 = thumb2.transform.GetChild(0);
        thumbTip = thumb3.transform.GetChild(1);

        index1 = wrist.transform.GetChild(3);
        index2 = index1.transform.GetChild(0);
        index3 = index2.transform.GetChild(0);
        indexTip = index3.transform.GetChild(1);

        middle1 = wrist.transform.GetChild(2);
        middle2 = middle1.transform.GetChild(0);
        middle3 = middle2.transform.GetChild(0);
        middleTip = middle3.transform.GetChild(1);

        ring1 = wrist.transform.GetChild(1);
        ring2 = ring1.transform.GetChild(0);
        ring3 = ring2.transform.GetChild(0);
        ringTip = ring3.transform.GetChild(1);

        pinky1 = wrist.transform.GetChild(0).GetChild(0);
        pinky2 = pinky1.transform.GetChild(0);
        pinky3 = pinky2.transform.GetChild(0);
        pinkyTip = pinky3.transform.GetChild(1);

        currentHandpose = new HaptikosHandpose();
    }

    // Update is called once per frame
    void Update()
    {
        xAxisThumb = thumb2.transform.right;
        xAxisWrist = wrist.transform.right;
        yAxisWrist = factor * wrist.transform.up;


        rotations[0] = thumb1.transform.localRotation;
        rotations[1] = thumb2.transform.localRotation;
        rotations[2] = thumb3.transform.localRotation;
        rotations[3] = index1.transform.localRotation;
        rotations[4] = index2.transform.localRotation;
        rotations[5] = index3.transform.localRotation;
        rotations[6] = middle1.transform.localRotation;
        rotations[7] = middle2.transform.localRotation;
        rotations[8] = middle3.transform.localRotation;
        rotations[9] = ring1.transform.localRotation;
        rotations[10] = ring2.transform.localRotation;
        rotations[11] = ring3.transform.localRotation;
        rotations[12] = Quaternion.Inverse(Quaternion.Euler(23.952f, -16.799f, -1.421f)) * pinky1.transform.localRotation;//We want rotation in relation to the wrist, not pinky0, might need changing, if we change the model this will break
        rotations[13] = pinky2.transform.localRotation;
        rotations[14] = pinky3.transform.localRotation;

        tips[0] = thumbTip.transform.position;
        tips[1] = indexTip.transform.position;
        tips[2] = middleTip.transform.position;
        tips[3] = ringTip.transform.position;
        tips[4] = pinkyTip.transform.position;

        currentHandpose.Update(xAxisThumb, yAxisWrist, xAxisWrist, tips, rotations, "Current Pose");
    }
}
