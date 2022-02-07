using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCustom : MonoBehaviour
{
    public string PlayersName;
    public string PlayersBalls;
    public Color PlayersColor;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetString("gameMode") == "OneDeviceDuel")
        {
            if (gameObject.name == "Player1")
            {
                PlayersName = PlayerPrefs.GetString("Player1");
            }
            else
            {
                PlayersName = PlayerPrefs.GetString("Player2");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
