using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public GameObject Keyboard;

    public GameObject MainMenu;
    public GameObject OneDeviceDuelMenu;
    public GameObject OnlineMenu;

    public InputField GamePassword;
    

    public NetworkManager NetworkManager;

    private string password;

    // Start is called before the first frame update
    void Start()
    {
        password = GamePassword.text.ToString();
    }

    public void Practice()
    {
        PlayerPrefs.SetString("gameMode", "Practice");
        SceneManager.LoadScene(1);
    }

    public void Online()
    {
        PlayerPrefs.SetString("gameMode", "Online");
        MainMenu.SetActive(false);
        OnlineMenu.SetActive(true);
        NetworkManager.ConnectToServer();
    }

    public void OneDeviceDuelStart()
    {
        PlayerPrefs.SetString("gameMode", "OneDeviceDuel");
        if (OneDeviceDuelMenu.transform.Find("Player1").GetComponent<InputField>().text.ToString() == "")
        {
            PlayerPrefs.SetString("Player1", "Player1");
        }
        else
        {
            PlayerPrefs.SetString("Player1", OneDeviceDuelMenu.transform.Find("Player1").GetComponent<InputField>().text.ToString());
        }
        if (OneDeviceDuelMenu.transform.Find("Player2").GetComponent<InputField>().text.ToString() == "")
        {
            PlayerPrefs.SetString("Player2", "Player2");
        }
        else
        {
            PlayerPrefs.SetString("Player2", OneDeviceDuelMenu.transform.Find("Player2").GetComponent<InputField>().text.ToString());
        }
        SceneManager.LoadScene(1);
    }

    public void OnlineCasual()
    {
        joinToRoom("Casualrandomxiox");
    }

    public void OnlineFriend()
    {
        if (GamePassword.text.ToString() != password)
        {
            NetworkManager.InitializeRoom(GamePassword.text.ToString());
        }
    }

    public void OnlineRanked()
    {
        joinToRoom("Rankedrandomxiox");
    }

    void joinToRoom(string roomType)
    {
        if (NetworkManager.RoomList.Count < 1)
        {
            NetworkManager.InitializeRoom($"{roomType}1");
            Debug.Log("first room");
        }
        else
        {
            List<RoomInfo> rankedRoomList = new List<RoomInfo>();
            foreach (RoomInfo room in NetworkManager.RoomList)
            {
                if (room.Name.Substring(0, 16) == roomType)
                {
                    rankedRoomList.Add(room);
                }
            }
            if (rankedRoomList.Count < 1)
            {
                NetworkManager.InitializeRoom($"{roomType}1");
                Debug.Log("first room");
            }
            else
            {
                foreach (RoomInfo room in rankedRoomList)
                {
                    Debug.Log(room.Name);
                    if (room.PlayerCount < 2)
                    {
                        NetworkManager.InitializeRoom(room.Name);
                        return;
                    }
                    string name = room.Name;
                }
                int roomIteration = Int32.Parse(name.Substring(16, name.Length)) + 1;
                if (roomIteration > 888)
                {
                    roomIteration = 1;
                }
                NetworkManager.InitializeRoom($"{roomType}{roomIteration}");
            }
        }
    }
}
