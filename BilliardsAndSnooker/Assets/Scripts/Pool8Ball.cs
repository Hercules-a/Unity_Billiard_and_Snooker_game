using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Pool8Ball : MonoBehaviour
{
    public Text CustomDebug;
    public Renderer Diamonds;
    public Renderer Table;

    public GameObject MenuInfo;
    public GameObject MenuFoulInBreakShot;
    public GameObject MenuChooseBalls;

    public GameObject LeftHandDirect;
    public GameObject LeftHandRayStreightLine;

    public PlayerCustom Player1;
    public PlayerCustom Player2;
    public PlayerCustom ActivePlayer;
    public PlayerCustom InActivePlayer;

    public Cue Cue;
    public List<Ball> BallsStriped = new List<Ball>();
    public List<Ball> BallsSolid = new List<Ball>();
    public GameObject WhiteBall;
    public Ball BlackBall;

    public BallsContainerSocketIteration BallsContainerSolid;
    public BallsContainerSocketIteration BallsContainerStriped;
    public BallsContainerSocketIteration BallsContainerBlack;

    public string gamePhase;

    public List<GameObject> PocketButtons = new List<GameObject>();

    bool ballsAreMoving;
    bool whiteInPocketAfterBreakShot;
    int rememberLayerOfTheCueBall;
    GameObject Pocket;

    List<string> logs = new List<string>();
    // Start is called before the first frame update

    void Start()
    {
        whiteInPocketAfterBreakShot = false;
        if (PlayerPrefs.GetString("gameMode") == "Practice")
        {
            gamePhase = "Practice";
        }
        else
        {
            gamePhase = "BreakShot";
        }
        Player1.PlayersBalls = "";
        Player2.PlayersBalls = "";
        ActivePlayer = Player1;
        InActivePlayer = Player2;

        MenuFoulInBreakShot.SetActive(false);
        MenuChooseBalls.SetActive(false);

        ballsAreMoving = false;
        InfoMenu($"{ActivePlayer.PlayersName} is breaking!", ActivePlayer.PlayersColor, "Good luck!", Color.white);

        foreach (GameObject element in PocketButtons)
        {
            element.SetActive(false);
        }
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
        if (gamePhase == "ClosedTable")
        {
            if (CheckThisIsTheLastBall())
            {
                LastBlackBall();
            }
            else
            {
                ClosedTable();
            }
        }
        else if (gamePhase == "OpenTable")
        {
            OpenTable();
        }
        else if (gamePhase == "BreakShot")
        {
            BreakShot();
        }

        ResetBallsBoolins();
    }

    void BreakShot()
    {
        if (!CheckAllColourBallIsInPocket() || WhiteBall.GetComponent<Ball>().TouchedPocket)
        {
            ChangeActivePlayer();
        }

        if ((CheckHowManyBallsTouchedBank() < 4 && !CheckAllColourBallIsInPocket()) || BlackBall.TouchedPocket)
        {
            MenuFoulInBreakShot.SetActive(true);
            ChangeHandToRay(true);
            // change WhiteBall layer to WhiteBall frozen - to not interact with hand or cue
            WhiteBall.layer = 11;
            MenuInfo.SetActive(false);

            if (BlackBall.TouchedPocket)
            {
                MenuFoulInBreakShot.transform.GetChild(0).gameObject.GetComponent<Text>().text =
                     $"8-ball is pocketed";
                MenuFoulInBreakShot.transform.GetChild(0).gameObject.GetComponent<Text>().color =
                    Color.white;
            }
            else
            {
                MenuFoulInBreakShot.transform.GetChild(0).gameObject.GetComponent<Text>().text =
                     $"Foul - {InActivePlayer.PlayersName} less than 4 balls hits a rail";
                MenuFoulInBreakShot.transform.GetChild(0).gameObject.GetComponent<Text>().color =
                    InActivePlayer.PlayersColor;
            }

            MenuFoulInBreakShot.transform.GetChild(1).gameObject.GetComponent<Text>().text = 
                $"{ActivePlayer.PlayersName} choose:";

            MenuFoulInBreakShot.transform.GetChild(1).gameObject.GetComponent<Text>().color = 
                ActivePlayer.PlayersColor;

            MenuFoulInBreakShot.transform.GetChild(3).gameObject.GetComponentInChildren<Text>().text = 
                $"Re-rack - {ActivePlayer.PlayersName} is breaking";

            MenuFoulInBreakShot.transform.GetChild(3).gameObject.GetComponentInChildren<Text>().color = 
                ActivePlayer.PlayersColor;

            MenuFoulInBreakShot.transform.GetChild(4).gameObject.GetComponentInChildren<Text>().text = 
                $"Re-rack - {InActivePlayer.PlayersName} is breaking";

            MenuFoulInBreakShot.transform.GetChild(4).gameObject.GetComponentInChildren<Text>().color = 
                InActivePlayer.PlayersColor;
        }

        // White Ball in pocket
        if (WhiteBall.GetComponent<Ball>().TouchedPocket)
        {
            whiteInPocketAfterBreakShot = true;

            if (WhiteBall.layer != 11)
            {
                InfoMenu($"Foul - {InActivePlayer.PlayersName} pocketed the Cue Ball", InActivePlayer.PlayersColor,
                    $"{ActivePlayer.PlayersName} has the cue ball in hand", ActivePlayer.PlayersColor);
                // change WhiteBall layer to grabbable
                WhiteBall.layer = 3;
            }
        }
        if (WhiteBall.layer != 11)
        {
            InfoMenu("Open Table", Color.white, $"{ActivePlayer.PlayersName}s turn", ActivePlayer.PlayersColor);
        }
        gamePhase = "OpenTable";
    }

    void OpenTable()
    {
        // Black Ball in pocket - game over
        if (BlackBall.TouchedPocket)
        {
            WhiteBall.layer = 11;
            InfoMenu($"{ActivePlayer.PlayersName} pocketed the 8-ball", ActivePlayer.PlayersColor,
                $"{InActivePlayer.PlayersName} is the WINNER! Congratulations :)", InActivePlayer.PlayersColor);
        }


        // If colour ball is not in pocket - change player.
        else if (!CheckAllColourBallIsInPocket() || 
            WhiteBall.GetComponent<WhiteBall>().FirstContactWithBall() == "black" ||
            WhiteBall.GetComponent<Ball>().TouchedPocket)
        {
            ChangeActivePlayer();

            if (WhiteBall.GetComponent<WhiteBall>().FirstContactWithBall() == "black")
            {
                // change WhiteBall layer to grabbable
                WhiteBall.layer = 3;
                InfoMenu($"Foul - {InActivePlayer.PlayersName} first contact with 8-ball", 
                    InActivePlayer.PlayersColor,
                    $"Open table, {ActivePlayer.PlayersName} has the cue ball in hand", ActivePlayer.PlayersColor);
            }
            else if (WhiteBall.GetComponent<WhiteBall>().FirstContactWithBall() == "null")
            {
                // change WhiteBall layer to grabbable
                WhiteBall.layer = 3;
                InfoMenu($"Foul - {InActivePlayer.PlayersName}, the cue ball didn't strike other ball", 
                    InActivePlayer.PlayersColor,
                    $"Open table, {ActivePlayer.PlayersName} has the cue ball in hand", ActivePlayer.PlayersColor);
            }
            else if (WhiteBall.GetComponent<Ball>().TouchedPocket)
            {
                // change WhiteBall layer to grabbable
                WhiteBall.layer = 3;
                InfoMenu($"Foul - {InActivePlayer.PlayersName} pocketed the cue ball",
                    InActivePlayer.PlayersColor,
                    $"Open table, {ActivePlayer.PlayersName} has the cue ball in hand", ActivePlayer.PlayersColor);
            }
            else if (CheckHowManyBallsTouchedBank() < 1 &&
                !WhiteBall.GetComponent<WhiteBall>().WhiteCollidedWithBandAfterCollidedWithBall())
            {
                // change WhiteBall layer to grabbable
                WhiteBall.layer = 3;
                InfoMenu($"Foul - {InActivePlayer.PlayersName} no ball hitted rail after first contact",
                    InActivePlayer.PlayersColor,
                    $"Open table, {ActivePlayer.PlayersName} has the cue ball in hand", ActivePlayer.PlayersColor);
            }
            else if (!WhiteBall.GetComponent<WhiteBall>().WhiteCollidedWithBallAfterLeftKitchen() &&
                whiteInPocketAfterBreakShot)
            {
                // change WhiteBall layer to grabbable
                WhiteBall.layer = 3;
                InfoMenu($"Foul - {InActivePlayer.PlayersName} first contact behind headstring",
                    InActivePlayer.PlayersColor,
                    $"Open table, {ActivePlayer.PlayersName} has the cue ball in hand. ", ActivePlayer.PlayersColor);
            }
            else
            {
                InfoMenu("Open table",
                    Color.white,
                    $"{ActivePlayer.PlayersName}s turn", ActivePlayer.PlayersColor);
            }
        }
        else
        {
            gamePhase = "ClosedTable";

            bool solid = false;
            bool striped = false;
            foreach (Ball ball in BallsSolid)
            {
                if (ball.TouchedPocket)
                {
                    solid = true;
                    break;
                }
            }
            foreach (Ball ball in BallsStriped)
            {
                if (ball.TouchedPocket)
                {
                    striped = true;
                    break;
                }
            }
            if (solid && striped)
            {
                MenuChooseBalls.SetActive(true);
                ChangeHandToRay(true);
                // change WhiteBall layer to WhiteBall frozen - to not interact with hand or cue
                WhiteBall.layer = 11;
                MenuInfo.SetActive(false);

                MenuChooseBalls.transform.GetChild(0).gameObject.GetComponent<Text>().text =
                    $"{ActivePlayer.PlayersName} pocketed balls from both groups";
                MenuChooseBalls.transform.GetChild(0).gameObject.GetComponent<Text>().color =
                    ActivePlayer.PlayersColor;
                MenuChooseBalls.transform.GetChild(1).gameObject.GetComponent<Text>().text =
                    $"{ActivePlayer.PlayersName} choose:";
                MenuChooseBalls.transform.GetChild(1).gameObject.GetComponent<Text>().color =
                    ActivePlayer.PlayersColor;
            }
            else
            {
                if (solid)
                {
                    ActivePlayer.PlayersBalls = "solids";
                    InActivePlayer.PlayersBalls = "stripes";
                }
                else
                {
                    InActivePlayer.PlayersBalls = "solids";
                    ActivePlayer.PlayersBalls = "stripes";
                }
                InfoMenu("Closed table", Color.white,
                    $"{ActivePlayer.PlayersName} is {ActivePlayer.PlayersBalls}", ActivePlayer.PlayersColor);
            }
        }
        whiteInPocketAfterBreakShot = false;
    }

    void ClosedTable()
    {
        // Black Ball in pocket - game over
        if (BlackBall.TouchedPocket)
        {
            WhiteBall.layer = 11;
            InfoMenu($"{ActivePlayer.PlayersName} pocketed 8-ball",
                ActivePlayer.PlayersColor,
                $"{InActivePlayer.PlayersName} is WINNER!", InActivePlayer.PlayersColor);
        }
        else if (!CheckSpecyficBallsGroupInPocket() ||
            WhiteBall.GetComponent<WhiteBall>().FirstContactWithBall() == "black" ||
            WhiteBall.GetComponent<Ball>().TouchedPocket ||
            WhiteBall.GetComponent<WhiteBall>().FirstContactWithBall() != ActivePlayer.PlayersBalls)
        {
            ChangeActivePlayer();

            if (WhiteBall.GetComponent<WhiteBall>().FirstContactWithBall() == "black")
            {
                InfoMenu($"Foul - {InActivePlayer.PlayersName} first contact with 8-ball",
                    InActivePlayer.PlayersColor,
                    $"{ActivePlayer.PlayersName} is {ActivePlayer.PlayersBalls}, the cue ball in hand", ActivePlayer.PlayersColor);
                // change WhiteBall layer to grabbable
                WhiteBall.layer = 3;
            }

            else if (WhiteBall.GetComponent<WhiteBall>().FirstContactWithBall() == ActivePlayer.PlayersBalls)
            {
                InfoMenu($"Foul - {InActivePlayer.PlayersName} first contact with oponents ball",
                    InActivePlayer.PlayersColor,
                    $"{ActivePlayer.PlayersName} is {ActivePlayer.PlayersBalls}, the cue ball in hand", ActivePlayer.PlayersColor);
                // change WhiteBall layer to grabbable
                WhiteBall.layer = 3;
            }

            else if (WhiteBall.GetComponent<WhiteBall>().FirstContactWithBall() == "null")
            {
                InfoMenu($"Foul - {InActivePlayer.PlayersName}, the cue ball didn't strike other ball",
                    InActivePlayer.PlayersColor,
                    $"{ActivePlayer.PlayersName} is {ActivePlayer.PlayersBalls}, the cue ball in hand", ActivePlayer.PlayersColor);
                // change WhiteBall layer to grabbable
                WhiteBall.layer = 3;
            }

            else if (CheckHowManyBallsTouchedBank() < 1 &&
               !WhiteBall.GetComponent<WhiteBall>().WhiteCollidedWithBandAfterCollidedWithBall())
            {
                InfoMenu($"Foul - {InActivePlayer.PlayersName} no ball hitted rail after first contact",
                    InActivePlayer.PlayersColor,
                    $"{ActivePlayer.PlayersName} is {ActivePlayer.PlayersBalls}, the cue ball in hand", ActivePlayer.PlayersColor);
                // change WhiteBall layer to grabbable
                WhiteBall.layer = 3;
            }

            else if (WhiteBall.GetComponent<Ball>().TouchedPocket)
            {
                InfoMenu($"Foul - {InActivePlayer.PlayersName} pocketed the cue ball",
                    InActivePlayer.PlayersColor,
                    $"{ActivePlayer.PlayersName} is {ActivePlayer.PlayersBalls}, the cue ball in hand", ActivePlayer.PlayersColor);
                // change WhiteBall layer to grabbable
                WhiteBall.layer = 3;
            }
            else
            {
                InfoMenu($"{ActivePlayer.PlayersName}s turn",
                    ActivePlayer.PlayersColor,
                     $"{ActivePlayer.PlayersName} is {ActivePlayer.PlayersBalls}", ActivePlayer.PlayersColor);
            }
        }
        else
        {
            InfoMenu($"{ActivePlayer.PlayersName}s turn", ActivePlayer.PlayersColor,
                    $"{ActivePlayer.PlayersName} is {ActivePlayer.PlayersBalls}", ActivePlayer.PlayersColor);
        }
                
        
        if (CheckThisIsTheLastBall())
        {
            displayMenuChoosePocket();
        }
    }

    void LastBlackBall()
    {
        if (BlackBall.Pocket == Pocket && !WhiteBall.GetComponent<Ball>().TouchedPocket)
        {
            WhiteBall.layer = 11;
            InfoMenu($"{ActivePlayer.PlayersName} pocketed 8-ball",
                ActivePlayer.PlayersColor,
                $"{ActivePlayer.PlayersName} is WINNER!", ActivePlayer.PlayersColor);
        }
        else if (BlackBall.TouchedPocket && BlackBall.Pocket != Pocket)
        {
            WhiteBall.layer = 11;
            InfoMenu($"Foul - {ActivePlayer.PlayersName} pocketed 8-ball to a wrong pocket",
                ActivePlayer.PlayersColor,
                $"{InActivePlayer.PlayersName} is WINNER!", InActivePlayer.PlayersColor);
        }
        else if (BlackBall.TouchedPocket && WhiteBall.GetComponent<Ball>().TouchedPocket)
        {
            WhiteBall.layer = 11;
            InfoMenu($"Foul - {ActivePlayer.PlayersName} pocketed the cue ball",
                ActivePlayer.PlayersColor,
                $"{InActivePlayer.PlayersName} is WINNER!", InActivePlayer.PlayersColor);
        }
        else if (BlackBall.TouchedPocket && WhiteBall.GetComponent<WhiteBall>().FirstContactWithBall() != "black")
        {
            WhiteBall.layer = 11;
            InfoMenu($"Foul - {ActivePlayer.PlayersName} first contact with oponents ball",
                ActivePlayer.PlayersColor,
                $"{InActivePlayer.PlayersName} is WINNER!", InActivePlayer.PlayersColor);
        }
        else
        {
            ChangeActivePlayer();

            if (WhiteBall.GetComponent<WhiteBall>().FirstContactWithBall() == "null")
            {
                InfoMenu($"Foul - {InActivePlayer.PlayersName}, the cue ball didn't strike other ball",
                    InActivePlayer.PlayersColor,
                    $"{ActivePlayer.PlayersName} is {ActivePlayer.PlayersBalls}, the cue ball in hand", ActivePlayer.PlayersColor);
                // change WhiteBall layer to grabbable
                WhiteBall.layer = 3;
            }

            else if (WhiteBall.GetComponent<WhiteBall>().FirstContactWithBall() != "black")
            {
                InfoMenu($"Foul - {InActivePlayer.PlayersName} first contact with oponents ball",
                    InActivePlayer.PlayersColor,
                    $"{ActivePlayer.PlayersName} is {ActivePlayer.PlayersBalls}, the cue ball in hand", ActivePlayer.PlayersColor);
                // change WhiteBall layer to grabbable
                WhiteBall.layer = 3;
            }
            else if (CheckHowManyBallsTouchedBank() < 1 &&
                !WhiteBall.GetComponent<WhiteBall>().WhiteCollidedWithBandAfterCollidedWithBall())
            {
                InfoMenu($"Foul - {InActivePlayer.PlayersName}, no ball hitted rail after first contact",
                    InActivePlayer.PlayersColor,
                    $"{ActivePlayer.PlayersName} is {ActivePlayer.PlayersBalls}, the cue ball in hand", ActivePlayer.PlayersColor);
                // change WhiteBall layer to grabbable
                WhiteBall.layer = 3;
            }
            else if (WhiteBall.GetComponent<Ball>().TouchedPocket)
            {
                InfoMenu($"Foul - {InActivePlayer.PlayersName} pocketed the cue ball",
                    InActivePlayer.PlayersColor,
                    $"{ActivePlayer.PlayersName} is {ActivePlayer.PlayersBalls}, the cue ball in hand", ActivePlayer.PlayersColor);
                // change WhiteBall layer to grabbable
                WhiteBall.layer = 3;
            }
            else
            {
                InfoMenu($"{ActivePlayer.PlayersName}s turn",
                    ActivePlayer.PlayersColor,
                    $"{ActivePlayer.PlayersName} is {ActivePlayer.PlayersBalls}", ActivePlayer.PlayersColor);
            }
        }


        if (CheckThisIsTheLastBall() && WhiteBall.layer != 11)
        {
            displayMenuChoosePocket(); 
        }
    }

    void displayMenuChoosePocket()
    {
        ChangeHandToRay(true);
        InfoMenu("Last 8-ball", Color.white,
                    $"{ActivePlayer.PlayersName} choose pocket for the 8-ball", ActivePlayer.PlayersColor);

        foreach (GameObject element in PocketButtons)
        {
            element.SetActive(true);
        }
        rememberLayerOfTheCueBall = WhiteBall.layer;
        Debug.Log($"Check rememberLayerOfTheCUeBall before change: {rememberLayerOfTheCueBall}");
        WhiteBall.layer = 11;
        Debug.Log($"Check rememberLayerOfTheCUeBall after change: {rememberLayerOfTheCueBall}");
    }

    bool CheckThisIsTheLastBall()
    {
        if ((ActivePlayer.PlayersBalls == "stripes" && BallsContainerStriped.AvaibleSocket == 7) ||
            (ActivePlayer.PlayersBalls == "solids" && BallsContainerSolid.AvaibleSocket == 7))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool CheckAllColourBallIsInPocket()
    {
        List<Ball> Balls = new List<Ball>(BallsSolid.Concat(BallsStriped))
        {
            BlackBall
        };

        foreach (Ball ball in Balls)
        {  
            if (ball.TouchedPocket)
            {
                return true;
            }
        }
        return false;
    }

    bool CheckSpecyficBallsGroupInPocket()
    {
        List<Ball> balls;
        if (ActivePlayer.PlayersBalls == "solids")
        {
            balls = BallsSolid;
        }
        else
        {
            balls = BallsStriped;
        }

        foreach (Ball ball in balls)
        {
            if (ball.TouchedPocket)
            {
                return true;
            }
        }
        return false;
    }

    int CheckHowManyBallsTouchedBank()
    {
        int touchedBank = 0;

        List<Ball> Balls = new List<Ball>(BallsSolid.Concat(BallsStriped))
        {
            BlackBall
        };
        foreach (Ball ball in Balls)
        {
            if (ball.TouchedBank)
            {
                touchedBank += 1;
            }
        }
        return touchedBank;
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
        Diamonds.material.color = ActivePlayer.PlayersColor;
        Table.material.color = ActivePlayer.PlayersColor;
    }

    void ResetBallsBoolins()
    {
        List<Ball> Balls = new List<Ball>(BallsSolid.Concat(BallsStriped))
        {
            BlackBall
        };
        foreach (Ball ball in Balls)
        {
            ball.TouchedBank = false;
            ball.TouchedPocket = false;
        }
        WhiteBall.GetComponent<Ball>().TouchedBank = false;
        WhiteBall.GetComponent<Ball>().TouchedPocket = false;
        WhiteBall.GetComponent<WhiteBall>().resetList();
        BlackBall.Pocket = null;
    }

    public void NewGame()
    {
        List<Ball> Balls = new List<Ball>(BallsSolid.Concat(BallsStriped))
        {
            BlackBall
        };

        foreach (Ball ball in Balls)
        {
            ball.ResetGame();
        }
        WhiteBall.GetComponent<Ball>().ResetGame();

        BallsContainerSolid.AvaibleSocketSetToZero();
        BallsContainerStriped.AvaibleSocketSetToZero();
        BallsContainerBlack.AvaibleSocketSetToZero();

        if (gamePhase != "Practice")
        {
            Diamonds.material.color = ActivePlayer.PlayersColor;
            Table.material.color = ActivePlayer.PlayersColor;

            // change WhiteBall layer to grabbable
            WhiteBall.layer = 3;

            gamePhase = "BreakShot";
            Player1.PlayersBalls = "";
            Player2.PlayersBalls = "";

            MenuFoulInBreakShot.SetActive(false);
            MenuChooseBalls.SetActive(false);
            whiteInPocketAfterBreakShot = false;
            ballsAreMoving = false;

            foreach (GameObject element in PocketButtons)
            {
                element.SetActive(false);
            }

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
        MenuChooseBalls.SetActive(false);
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
                WhiteBall.GetComponent<Ball>().ResetGame();
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

        foreach (GameObject element in PocketButtons)
        {
            if (element == pocketButton)
            {
                continue;
            }
            element.SetActive(false);
        }
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
