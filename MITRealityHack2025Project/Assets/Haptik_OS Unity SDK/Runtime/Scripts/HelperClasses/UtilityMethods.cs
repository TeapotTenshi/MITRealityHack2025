using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UtilityMethods
{
    public static Quaternion QuaternionFromList(List<float> quaternionList)
    {
        return new Quaternion(quaternionList[0], quaternionList[1], quaternionList[2], quaternionList[3]);
    }

    public static Vector3 VectorFromList(List<float> vectorFromList)
    {
        return new Vector3(vectorFromList[0], vectorFromList[1], vectorFromList[2]);
    }

    public static JSONNode JsonParse(string json)
    {
        //https://discussions.unity.com/t/argumentexception-json-must-represent-an-object-type/237073/2
        JSONNode root = JSON.Parse(json);
        return root;
    }
}
