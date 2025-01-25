using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Haptikos.Gloves;
using UnityEngine.Events;
using Haptikos.Exoskeleton;

public class ExampleSceneMenuController : MonoBehaviour
{
    HaptikosRaycast[] raycasts = new HaptikosRaycast[4];
    HaptikosGestureRecognizer[] recognizers = new HaptikosGestureRecognizer[4];
    GameObject mainMenu;
    public int state = 0;
    float timer;
    Slider slider;
    GameObject visualization;
    [SerializeField]
    Sprite icon;
    Transform mainCamera;
    Transform rightMiddle;
    Transform leftMiddle;
    Button[] buttons;
    TMP_Text[] texts;
    HaptikosExoskeleton rightHand, leftHand;
    bool started;

    static public UnityEvent<HaptikosExoskeleton> OnRecognizedOpenHand = new();
    static public UnityEvent<HaptikosExoskeleton> OnMainMenuOpened = new();

     void Start()
    {
        mainMenu = transform.GetChild(0).gameObject;
        mainMenu.SetActive(false);
        Transform child = transform.GetChild(1);

        for (int i = 0; i < 4; i++)
        {
            raycasts[i] = child.GetChild(i).GetComponent<HaptikosRaycast>();
            recognizers[i] = transform.GetChild(i + 2).GetComponent<HaptikosGestureRecognizer>();
            raycasts[i].enabled = false;
        }

        recognizers[0].enabled = true;
        recognizers[2].enabled = true;
        recognizers[1].enabled = false;
        recognizers[3].enabled = false;

        visualization = Instantiate(HaptikosResources.Instance.slider);
        visualization.transform.parent = transform;
        visualization.SetActive(false);
        slider = visualization.GetComponentInChildren<Slider>();
        Image iconImage = visualization.transform.GetChild(0).GetChild(2).GetComponent<Image>();
        if (icon != null)
        {
            iconImage.sprite = icon;
        }
        else
        {
            iconImage.sprite = HaptikosResources.Instance.circle;
        }
        Image fillImage = visualization.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Image>();
        fillImage.color = Color.green;
        mainCamera = GameObject.FindWithTag("MainCamera").transform;

        rightHand = recognizers[0].Hand;
        leftHand = recognizers[2].Hand;
        rightMiddle = recognizers[0].Hand.transform.GetChild(0).GetChild(0).GetChild(2);
        leftMiddle = recognizers[2].Hand.transform.GetChild(0).GetChild(0).GetChild(2);

        buttons = new Button[5];
        texts = new TMP_Text[5];

        for (int i = 0; i < 5; i++)
        {
            buttons[i] = mainMenu.transform.GetChild(i).GetComponent<Button>();
            texts[i] = buttons[i].transform.GetComponentInChildren<TMP_Text>();
        }

        for (int i = 0; i < 4; i++)
        {
            texts[i].text = "Enable " + buttons[i].name;
        }

        texts[4].text = buttons[4].name;
        started = true;
        OnEnable();
    }
    void OnEnable()
    {
        if (!started)
        {
            return;
        }
        recognizers[1].OnActivate.AddListener(openMenu);
        recognizers[3].OnActivate.AddListener(openMenu);
        buttons[0].onClick.AddListener(LeftCursorHandler);
        buttons[1].onClick.AddListener(RightCursorHandler);
        buttons[2].onClick.AddListener(LeftTeleportHandler);
        buttons[3].onClick.AddListener(RightTeleportHandler);
        buttons[4].onClick.AddListener(ExitMenuHandler);
    }

    void OnDisable()
    {
        if (!started)
        {
            return;
        }

        recognizers[1].OnActivate.RemoveListener(openMenu);
        recognizers[3].OnActivate.RemoveListener(openMenu);
        buttons[0].onClick.RemoveListener(LeftCursorHandler);
        buttons[1].onClick.RemoveListener(RightCursorHandler);
        buttons[2].onClick.RemoveListener(LeftTeleportHandler);
        buttons[3].onClick.RemoveListener(RightTeleportHandler);
        buttons[4].onClick.RemoveListener(ExitMenuHandler);
    }

    void Update()
    {
        switch (state)
        {
            case 0:

                if (recognizers[0].Activated)
                {
                    state = 1;
                    recognizers[1].enabled = true;
                    timer = 1f;
                    visualization.SetActive(true);
                    visualization.transform.SetPositionAndRotation(rightMiddle.position + Vector3.up * 0.15f, mainCamera.rotation);
                    slider.value = 0f;
                    OnRecognizedOpenHand?.Invoke(rightHand);
                }
                else if (recognizers[2].Activated)
                {
                    state = 2;
                    recognizers[3].enabled = true;
                    timer = 1f;
                    visualization.SetActive(true);
                    visualization.transform.SetPositionAndRotation(leftMiddle.position + Vector3.up * 0.15f, mainCamera.rotation);
                    slider.value = 0f;
                    OnRecognizedOpenHand?.Invoke(leftHand);
                }
                break;

            case 1:
                if (!recognizers[0].Activated)
                {
                    timer -= Time.deltaTime;
                }
                else
                {
                    timer = 1f;
                }

                if(timer < 0 && recognizers[1].Recognized == false)
                {
                    recognizers[1].enabled = false;
                    visualization.SetActive(false);
                    state = 0;
                }
                else if(recognizers[1].Recognized)
                {
                    slider.value = recognizers[1].Timer / recognizers[1].timeToActivate;

                }
                visualization.transform.SetPositionAndRotation(rightMiddle.position + Vector3.up * 0.15f, mainCamera.rotation);
                break;

            case 2:
                if (!recognizers[2].Activated)
                {
                    timer -= Time.deltaTime;
                }
                else
                {
                    timer = 1f;
                }  

                if (timer < 0 && recognizers[3].Recognized == false)
                {
                    recognizers[1].enabled = false;
                    visualization.SetActive(false);
                    state = 0;
                }
                else if (recognizers[3].Recognized)
                {
                    slider.value = recognizers[3].Timer / recognizers[3].timeToActivate;
                }
                visualization.transform.SetPositionAndRotation(leftMiddle.position + Vector3.up * 0.15f, mainCamera.rotation);
                break;
            case 3:
                mainMenu.transform.LookAt(mainCamera);
                mainMenu.transform.rotation *= Quaternion.Euler(0, 180f, 0);
                mainMenu.transform.position = mainCamera.position + mainCamera.forward * 0.5f - mainCamera.right * 0.15f;
                break;
            default:
                break;

        }
    }

    void openMenu(HaptikosExoskeleton hand)
    {
        OnMainMenuOpened?.Invoke(hand);
        mainMenu.SetActive(true);
        mainMenu.transform.LookAt(mainCamera.position);
        mainMenu.transform.rotation *= Quaternion.Euler(0, 180f, 0);
        mainMenu.transform.position = mainCamera.position + mainCamera.forward * 0.8f;
        visualization.SetActive(false);
        state = 3;
        recognizers[1].enabled = false;
        recognizers[3].enabled = false;
    }

    void LeftCursorHandler()
    {
        raycastHandler(0);
    }

    void RightCursorHandler()
    {
        raycastHandler(1);
    }

    void LeftTeleportHandler()
    {
        raycastHandler(2);
    }

    void RightTeleportHandler()
    {
        raycastHandler(3);
    }

    void raycastHandler(int index)
    {
        HaptikosRaycast raycast = raycasts[index];
        HaptikosRaycast otherRaycast = raycasts[(index + 2) % 4];
        TMP_Text text = texts[index];
        Button button = buttons[index];
        TMP_Text otherText = texts[(index + 2) % 4];
        Button otherButton = buttons[(index + 2) % 4];
        if (raycast.enabled)
        {
            raycast.enabled = false;
            text.text = "Enable " + button.name;
        }
        else
        {
            raycast.enabled = true;
            otherRaycast.enabled = false;
            text.text = "Disable " + button.name;
            otherText.text = "Enable" + otherButton.name;
        }
    }
    public void ExitMenuHandler()
    {
        StartCoroutine(DisableMenu());
        state = 0;
    }

    IEnumerator DisableMenu()
    {
        yield return null;
        mainMenu.SetActive(false);
    }
}
