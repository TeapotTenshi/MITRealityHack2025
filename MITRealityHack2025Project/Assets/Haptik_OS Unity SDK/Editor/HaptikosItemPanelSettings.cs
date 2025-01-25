using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Haptikos Panel Settings", menuName = "Haptikos Panel/Generate Item Settings")]
public class HaptikosItemPanelSettings : ScriptableObject
{
    //Haptikos player prefab
    public string haptikosPlayerPath;
    public string haptikosPlaybackPlayerPath;

    //Haptikos UI prefabs
    public string buttonPath;
    public string sliderPath;
    public string dimmerPath;
    public string leverPath;
    public string switchPath;
    public string wallSwitchPath;

    //Haptikos grabbable prefabs
    public string basicGrabbablePath;

    //Haptikos Advanced Haptic Items
    public string beatingHeartPath;
    public string waterSinkPath;

    //Gesture Recognition Prefabs
    public string recognizerPath;
    public string teleportRaycasterPath;
    public string cursorRaycasterPath;
    public string teleportAreaPath;
    public string followRayPath;
    public string uiButtonPath;
    public string raycastMenuPath;

    //Misc
    public string portalPath;
    public string teleportationAreaPath;
}
