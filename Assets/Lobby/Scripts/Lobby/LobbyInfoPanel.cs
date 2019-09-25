using UnityEngine;
using UnityEngine.UI;
using System.Collections;



public class LobbyInfoPanel : MonoBehaviour
{
    public Text infoText;
    public Text buttonText;
    public Button singleButton;
    public GameObject settings;

    public void Display(string info, string buttonInfo, UnityEngine.Events.UnityAction buttonClbk)
    {
        infoText.text = info;

        buttonText.text = buttonInfo;

        singleButton.onClick.RemoveAllListeners();

        if (buttonClbk != null)
        {
            singleButton.onClick.AddListener(buttonClbk);
        }

        singleButton.onClick.AddListener(() => { gameObject.SetActive(false); settings.SetActive(false); });

        gameObject.SetActive(true);
    }
}
