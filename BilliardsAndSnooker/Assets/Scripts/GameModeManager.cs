using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GameModeManager : MonoBehaviour
{
    public GameObject NetworkManager;
    public GameObject NetworkVoice;
    public GameObject MenuInfo;
    public List<XRGrabInteractable> Balls = new List<XRGrabInteractable>();
    public GameObject ResetButton;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetString("gameMode") == "Practice")
        {
            Debug.Log("practice");
            NetworkManager.SetActive(false);
            NetworkVoice.SetActive(false);
            MenuInfo.SetActive(false);
            foreach (XRGrabInteractable ball in Balls)
            {
                ball.enabled = true;
                ball.gameObject.layer = 13;
            }
        }
        else
        {
            ResetButton.SetActive(false);
        }

        if (PlayerPrefs.GetString("gameMode") == "OneDeviceDuel")
        {
            NetworkManager.SetActive(false);
            NetworkVoice.SetActive(false);

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
