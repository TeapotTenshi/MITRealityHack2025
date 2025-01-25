using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaptikosResources : MonoBehaviour
{
    #region Header MANAGERS
    [Space(10)]
    [Header("MANAGERS")]
    #endregion
    #region Tooltip
    [Tooltip("Singletons that may be missing during the game to prevent Errors")]
    #endregion
    public SceneLoader sceneLoader;
    public GameObject haptikosEventSystem;
    [Header("Prefabs")]
    public GameObject slider;
    public GameObject calibrationManager;

    [Header("Materials")]
    public Material raycastMaterial;
    [Header("Sprites")]
    public Sprite circle;
    private static HaptikosResources instance;
    
    public static HaptikosResources Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<HaptikosResources>("HaptikosResources");
            }
            return instance;
        }
    }
}
