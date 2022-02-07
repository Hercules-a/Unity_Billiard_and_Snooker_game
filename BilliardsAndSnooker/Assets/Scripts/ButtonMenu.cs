using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonMenu : MonoBehaviour
{
    public Pool8Ball Table;
    public CustomDeviceManager XRRig;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void ResetGame()
    {
        Table.NewGame();
        XRRig.OpenCloseMenuButton();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

}
