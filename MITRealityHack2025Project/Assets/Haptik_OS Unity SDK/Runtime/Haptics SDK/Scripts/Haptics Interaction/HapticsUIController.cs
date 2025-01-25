using Haptikos.Exoskeleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HapticsUIController : MonoBehaviour
{

    public HaptikosSelectableButton tableSwitchButton;
    public HaptikosSelectableButton wallSwitchButton;
    public HaptikosSelectableButton heartButton;
    public HaptikosSelectableButton sinkButton;
    public HaptikosSelectableButton buttonButton;
    public HaptikosSelectableButton dimmerButton;
    public HaptikosSelectableButton sliderButton;
    public HaptikosSelectableButton leverButton;
    public HaptikosSelectableButton openMenuButton;
    Transform buttons_canvas;
    Transform openmenu_canvas;
    [SerializeField] private GameObject[] gameobjects;
    private GameObject currentInstance;
    [SerializeField] private Transform[] spawnPoints;

    private void Awake()
    {
        buttons_canvas = transform.GetChild(0);
        openmenu_canvas = transform.GetChild(1);
        tableSwitchButton = buttons_canvas.GetChild(0).GetComponent<HaptikosSelectableButton>();
        wallSwitchButton = buttons_canvas.GetChild(1).GetComponent<HaptikosSelectableButton>();
        heartButton = buttons_canvas.GetChild(2).GetComponent<HaptikosSelectableButton>();
        sinkButton = buttons_canvas.GetChild(3).GetComponent<HaptikosSelectableButton>();
        buttonButton = buttons_canvas.GetChild(4).GetComponent<HaptikosSelectableButton>();
        dimmerButton = buttons_canvas.GetChild(5).GetComponent<HaptikosSelectableButton>();
        sliderButton = buttons_canvas.GetChild(6).GetComponent<HaptikosSelectableButton>();
        leverButton = buttons_canvas.GetChild(7).GetComponent<HaptikosSelectableButton>();
        openMenuButton = openmenu_canvas.GetChild(0).GetComponent<HaptikosSelectableButton>();

    }

    private void OnEnable()
    {
        tableSwitchButton.OnClick.AddListener( exoskeleton => CallActivateGameObject(0, exoskeleton));
        wallSwitchButton.OnClick.AddListener(exoskeleton => CallActivateGameObject(1, exoskeleton));
        heartButton.OnClick.AddListener(exoskeleton => CallActivateGameObject(2, exoskeleton));
        sinkButton.OnClick.AddListener(exoskeleton => CallActivateGameObject(3, exoskeleton));
        buttonButton.OnClick.AddListener(exoskeleton => CallActivateGameObject(4, exoskeleton));
        dimmerButton.OnClick.AddListener(exoskeleton => CallActivateGameObject(5, exoskeleton));
        sliderButton.OnClick.AddListener(exoskeleton => CallActivateGameObject(6, exoskeleton));
        leverButton.OnClick.AddListener(exoskeleton => CallActivateGameObject(7, exoskeleton));
        openMenuButton.OnClick.AddListener(CallOpenButtonsMenu);
    }

    private void OnDisable()
    {
        tableSwitchButton.OnClick.RemoveListener(exoskeleton => CallActivateGameObject(0, exoskeleton));
        wallSwitchButton.OnClick.RemoveListener(exoskeleton => CallActivateGameObject(1, exoskeleton));
        heartButton.OnClick.RemoveListener(exoskeleton => CallActivateGameObject(2, exoskeleton));
        sinkButton.OnClick.RemoveListener(exoskeleton => CallActivateGameObject(3, exoskeleton));
        buttonButton.OnClick.RemoveListener(exoskeleton => CallActivateGameObject(4, exoskeleton));
        dimmerButton.OnClick.RemoveListener(exoskeleton => CallActivateGameObject(5, exoskeleton));
        sliderButton.OnClick.RemoveListener(exoskeleton => CallActivateGameObject(6, exoskeleton));
        leverButton.OnClick.RemoveListener(exoskeleton => CallActivateGameObject(7, exoskeleton));
        openMenuButton.OnClick.RemoveListener(CallOpenButtonsMenu);
    }

    private IEnumerator OpenButtonsMenu()
    {
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(HapticFeedback.StopAllHaptics(0.1f));
        buttons_canvas.gameObject.SetActive(true);
        openmenu_canvas.gameObject.SetActive(false);
    }

    void CallOpenButtonsMenu(HaptikosExoskeleton exoskeleton)
    {
        StartCoroutine(OpenButtonsMenu());
    }


    private IEnumerator ActivateGameObject(int index)
    {
        yield return new WaitForSeconds(0.5f);

        if (currentInstance != null)
        {
            Destroy(currentInstance);
        }

        if (index >= 0 && index < gameobjects.Length && index < spawnPoints.Length)
        {
            currentInstance = Instantiate(gameobjects[index], spawnPoints[index].position, spawnPoints[index].rotation);
            currentInstance.SetActive(true);
        }

        buttons_canvas.gameObject.SetActive(false);
        openmenu_canvas.gameObject.SetActive(true);
        StartCoroutine(HapticFeedback.StopAllHaptics(0.1f));
    }

    void CallActivateGameObject(int index, HaptikosExoskeleton exoskeleton)
    {
        StartCoroutine(ActivateGameObject(index));
    }
}
