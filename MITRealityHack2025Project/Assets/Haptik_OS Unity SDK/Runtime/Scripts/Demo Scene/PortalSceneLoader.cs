using Haptikos;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalSceneLoader : MonoBehaviour
{
    public int sceneNumber;
    HaptikosSelectable selectable;
    void LoadScene()
    {
        SceneInformation.calibrated = HaptikosPlayer.calibrated; 
        StartCoroutine(HapticFeedback.StopAllHaptics(0.3f));
        HaptikosPlayer.GetExoskeleton(HandType.LeftHand).uDPReciever.SendHapticData("quit");
        HaptikosPlayer.GetExoskeleton(HandType.RightHand).uDPReciever.SendHapticData("quit");
        SceneManager.LoadScene(sceneNumber);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hand"))
        {
            LoadScene();
        }
    }
}
