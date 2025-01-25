using UnityEngine;

[System.Serializable]
public class PlaneData
{
    public Vector3 position;
    public Vector3 rotation;
    public Vector2 size;
}

public class InstantiateOnPlane : MonoBehaviour
{
    public GameObject prefab; // Assign your prefab in the Inspector
    public string jsonFilePath = "Assets/Scripts/planeData.json"; // Path to your JSON file

    void Start()
    {
        // Load the JSON file
        string jsonPath = Application.dataPath + "/" + jsonFilePath;
        if (System.IO.File.Exists(jsonPath))
        {
            string jsonContent = System.IO.File.ReadAllText(jsonPath);
            PlaneData planeData = JsonUtility.FromJson<PlaneData>(jsonContent);

            // Create the plane
            CreatePlane(planeData);
        }
        else
        {
            Debug.LogError("JSON file not found at path: " + jsonPath);
        }
    }

    void CreatePlane(PlaneData planeData)
    {
        // Create a GameObject to represent the plane
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);

        // Set the plane's position, rotation, and scale
        plane.transform.position = planeData.position;
        plane.transform.eulerAngles = planeData.rotation;

        // Adjust plane size based on JSON data
        plane.transform.localScale = new Vector3(planeData.size.x / 10f, 1, planeData.size.y / 10f);

        // Instantiate the prefab on the plane
        Vector3 spawnPosition = plane.transform.position + Vector3.up; // Offset to avoid overlapping
        Instantiate(prefab, spawnPosition, Quaternion.identity);
    }
}
