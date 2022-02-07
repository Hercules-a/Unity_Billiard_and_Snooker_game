using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Snooker : MonoBehaviour
{
    public Text CustomDebug;
    public Renderer Table;

    public GameObject MenuInfo;
    public GameObject MenuFoulInBreakShot;

    public GameObject LeftHandDirect;
    public GameObject LeftHandRayStreightLine;

    public PlayerCustom Player1;
    public PlayerCustom Player2;
    public PlayerCustom ActivePlayer;
    public PlayerCustom InActivePlayer;

    public Cue Cue;
    public List<BallSnooker> RedBalls = new List<BallSnooker>();
    public GameObject YellowBall;
    public GameObject GreenBall;
    public GameObject BrownBall;
    public GameObject BlueBall;
    public GameObject PinkBall;
    public GameObject BlackBall;
    public GameObject WhiteBall;

    public BallsContainerSocketIteration BallsContainerRed1;
    public BallsContainerSocketIteration BallsContainerRed2;
    public BallsContainerSocketIteration BallsContainerColour;

    public string gamePhase;

    bool ballsAreMoving;
    bool whiteInPocketAfterBreakShot;
    int rememberLayerOfTheCueBall;
    GameObject Pocket;

    List<BallSnooker> Balls = new List<BallSnooker>();

    List<string> logs = new List<string>();
    // Start is called before the first frame update

    void Start()
    {
        Balls = new List<BallSnooker>(RedBalls)
        {
            YellowBall.GetComponent<BallSnooker>(),
            GreenBall.GetComponent<BallSnooker>(),
            BrownBall.GetComponent<BallSnooker>(),
            BlueBall.GetComponent<BallSnooker>(),
            PinkBall.GetComponent<BallSnooker>(),
            BlackBall.GetComponent<BallSnooker>(),
        };

        whiteInPocketAfterBreakShot = false;
        if (PlayerPrefs.GetString("gameMode") == "Practice")
        {
            gamePhase = "Practice";
        }
        else
        {
            gamePhase = "Red";
        }
        Player1.PlayersBalls = "";
        Player2.PlayersBalls = "";
        ActivePlayer = Player1;
        InActivePlayer = Player2;

        MenuFoulInBreakShot.SetActive(false);

        ballsAreMoving = false;
        InfoMenu($"{ActivePlayer.PlayersName} is breaking!", ActivePlayer.PlayersColor, "Good luck!", Color.white);


    }

    // Update is called once per frame
    void Update()
    {
        if (ballsAreMoving && gamePhase != "Practice")
        {
            if (!Cue.BallsAreMoving())
            {
                ballsAreMoving = false;
                BallsStopped();           
            }
        }
    }

    public void StartHit()
    {
        ballsAreMoving = true;
    }

    void BallsStopped()
    {
        if (gamePhase == "Red")
        {
            RedPhase();
        }
        else if (gamePhase == "Colour")
        {
            ColourPhase();
        }
        else if (gamePhase == "LastBalls")
        {
            LastBallsPhase();
        }

        ResetBallsBoolins();
    }


    void RedPhase()
    {

    }

    void ColourPhase()
    {

    }

    void LastBallsPhase()
    {

    }




    void ChangeActivePlayer()
    { 

        if (ActivePlayer == Player1)
        {
            ActivePlayer = Player2;
            InActivePlayer = Player1;  
        }
        else
        {
            ActivePlayer = Player1;
            InActivePlayer = Player2;
        }
        Table.material.color = ActivePlayer.PlayersColor;
    }

    void ResetBallsBoolins()
    {
        foreach (BallSnooker ball in Balls)
        {
            if (ball.TouchedPocket)
            {
                ball.TeleportBallAfterPocketed();
            }
            ball.TouchedPocket = false;
        }
        WhiteBall.GetComponent<BallSnooker>().TouchedPocket = false;
        WhiteBall.GetComponent<WhiteBall>().resetList();
    }

    public void NewGame()
    {
        foreach (BallSnooker ball in Balls)
        {
            ball.ResetGame();
        }
        WhiteBall.GetComponent<Ball>().ResetGame();

        BallsContainerRed1.AvaibleSocketSetToZero();
        BallsContainerRed2.AvaibleSocketSetToZero();
        BallsContainerColour.AvaibleSocketSetToZero();

        if (gamePhase != "Practice")
        {

            Table.material.color = ActivePlayer.PlayersColor;

            // change WhiteBall layer to grabbable
            WhiteBall.layer = 3;

            gamePhase = "BreakShot";
            Player1.PlayersBalls = "";
            Player2.PlayersBalls = "";

            MenuFoulInBreakShot.SetActive(false);
            whiteInPocketAfterBreakShot = false;
            ballsAreMoving = false;


            InfoMenu($"{ActivePlayer.PlayersName} is breaking!", ActivePlayer.PlayersColor, "Good luck!", Color.white);
        }
    }

    public void WriteLog (string text)
    {
        logs.Add(text);
        if (logs.Count > 15)
        {
            logs.RemoveAt(0);
        }

        CustomDebug.text = "Debug: \n";

        foreach (string log in logs)
        {
            CustomDebug.text += $"{log}\n";
        }
    }

    public void MenuFoulInBreakShotContinueGame()
    {
        if (whiteInPocketAfterBreakShot)
        {
            // change White Ball layer to Grabbable
            WhiteBall.layer = 3;
        }
        else
        {
            // change White Ball layer to WhiteBall
            WhiteBall.layer = 10;
        }
        MenuFoulInBreakShot.SetActive(false);
        ChangeHandToRay(false);
        InfoMenu("Open Table", Color.white, $"{ActivePlayer.PlayersName}s turn", ActivePlayer.PlayersColor);
    }

    public void MenuFoulInBreakShotActivePlayerBreaking()
    {
        NewGame();
        ChangeHandToRay(false);
    }

    public void MenuFoulInBreakShotInActivePlayerBreaking()
    {
        ChangeActivePlayer();
        NewGame();
        ChangeHandToRay(false);
    }

    public void MenuChooseBallsAction(string balls)
    {
        if (balls == "stripes")
        {
            ActivePlayer.PlayersBalls = "stripes";
            InActivePlayer.PlayersBalls = "solids";
        }
        if (balls == "solids")
        {
            ActivePlayer.PlayersBalls = "solids";
            InActivePlayer.PlayersBalls = "stripes";
        }
        // change White Ball layer to WhiteBall
        WhiteBall.layer = 10;
        ChangeHandToRay(false);

        InfoMenu("Closed table", Color.white,
            $"{ActivePlayer.PlayersName} is {ActivePlayer.PlayersBalls}", ActivePlayer.PlayersColor);
    }

    public void ChangeHandToRay(bool ray)
    {
        if (ray)
        {
          //  LeftHandDirect.SetActive(false);
            LeftHandRayStreightLine.SetActive(true);
        }
        else
        {
            LeftHandRayStreightLine.SetActive(false);
          //  LeftHandDirect.SetActive(true);
        }
    }

    public void CheckIsCueBallInKitchen()
    {
        if (gamePhase == "BreakShot" || (gamePhase == "OpenTable" && whiteInPocketAfterBreakShot))
        {
            if (!WhiteBall.GetComponent<WhiteBall>().IsInKitchen)
            {
                WhiteBall.GetComponent<BallSnooker>().ResetGame();
                InfoMenu("The cue ball must be behind the headstring", Color.white,
                    $"{ActivePlayer.PlayersName} has the cue ball in hand", ActivePlayer.PlayersColor);
            }
        }
    }

    void InfoMenu(string info1, Color color1, string info2, Color color2)
    {
        if (gamePhase != "Practice")
        {
            MenuInfo.SetActive(true);

            MenuInfo.transform.GetChild(0).gameObject.GetComponent<Text>().text = info1;
            MenuInfo.transform.GetChild(0).gameObject.GetComponent<Text>().color = color1;

            MenuInfo.transform.GetChild(1).gameObject.GetComponent<Text>().text = info2;
            MenuInfo.transform.GetChild(1).gameObject.GetComponent<Text>().color = color2;
        }
    }

    public void MenuChoosePocket(GameObject pocket)
    {
        Pocket = pocket;
        GameObject pocketButton = pocket.transform.GetChild(0).gameObject;
        WhiteBall.layer = rememberLayerOfTheCueBall;
        ChangeHandToRay(false);

        if (rememberLayerOfTheCueBall == 3)
        {
            InfoMenu("Last 8-ball", Color.white,
               $"{ActivePlayer.PlayersName} has the cue ball in hand", ActivePlayer.PlayersColor);
        }
        else
        {
            InfoMenu("Last 8-ball", Color.white,
               $"{ActivePlayer.PlayersName}s turn", ActivePlayer.PlayersColor);
        }
    }
}
