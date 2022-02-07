using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR;

public class BallSnooker : MonoBehaviour
{
    public GameObject BallsContainerRed1;
    public GameObject BallsContainerRed2;
    public GameObject BallsContainerColour;

    public Transform YellowBall;
    public Transform GreenBall;
    public Transform BrownBall;
    public Transform BlueBall;
    public Transform PinkBall;
    public Transform BlackBall;

    Vector3 initialPosition;
    Rigidbody rb;

    public AudioClip HitBallSound;
    public AudioClip HitTabbleSound;
    public AudioClip HitCueSound;
    public AudioClip HitPocketSound;
    AudioSource hitBall;
    AudioSource hitTabble;
    AudioSource hitCue;
    AudioSource hitPocket;

    public CustomDeviceManager XRRig;
    public Cue Cue;

    public Pool8Ball GameRules;
    public bool TouchedBank;
    public bool TouchedPocket;
    public GameObject Pocket;

    public List<GameObject> OtherBalls = new List<GameObject>();
    public GameObject UpperRail;

    List<Vector3> spots;
    bool CollideWithTheUpperRail;

    // Start is called before the first frame update
    void Start()
    {
        CollideWithTheUpperRail = false;

        initialPosition = transform.position;
        spots = new List<Vector3>
        {
            initialPosition,
            BlackBall.position,
            PinkBall.position,
            BlueBall.position,
            BrownBall.position,
            GreenBall.position,
            YellowBall.position
        };
        rb = GetComponent<Rigidbody>();
        // Set random rotation
        transform.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));

        hitBall = AddAudio(false, false, 1f, HitBallSound);
        hitTabble = AddAudio(false, false, 1f, HitTabbleSound);
        hitCue = AddAudio(false, false, 1f, HitCueSound);
        hitPocket = AddAudio(false, false, 1f, HitPocketSound);

        TouchedBank = false;
        TouchedPocket = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StopObjectMovement()
    {
        rb.velocity = new Vector3(0, 0, 0);
        rb.angularVelocity = new Vector3(0, 0, 0);

        Invoke("StopAgain", 0.1f);
    }
    void StopAgain()
    {
        rb.velocity = new Vector3(0, 0, 0);
        rb.angularVelocity = new Vector3(0, 0, 0);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Pocket")
        {
            hitPocket.Play();
            Pocket = other.gameObject;
            TouchedPocket = true;
            //Invoke("TeleportBallAfterPocketed", 0.2f);
        }
        if (other == UpperRail)
        {

        }
    }

    public void TeleportBallAfterPocketed()
    {
        StopObjectMovement();
        if (name == "CueBall")
        {
            gameObject.layer = 3;
            transform.position = initialPosition;
        }
        else if (name.Substring(0,3) != "Red")
        {
            if (BallsContainerRed1.GetComponent<BallsContainerSocketIteration>().AvaibleSocket <= 6)
            {
                TeleportBallToContainer(BallsContainerRed1);
            }
            else if (BallsContainerRed2.GetComponent<BallsContainerSocketIteration>().AvaibleSocket <= 6)
            {
                TeleportBallToContainer(BallsContainerRed2);
            }
            else
            {
                TeleportBallToContainer(BallsContainerColour);
            }

        }
        else
        {
            if (BallsContainerColour.GetComponent<BallsContainerSocketIteration>().AvaibleSocket == 1 && name == "Yellow")
            {
                TeleportBallToContainer(BallsContainerColour);
            }
            else if (BallsContainerColour.GetComponent<BallsContainerSocketIteration>().AvaibleSocket == 2 && name == "Green")
            {
                TeleportBallToContainer(BallsContainerColour);
            }
            else if (BallsContainerColour.GetComponent<BallsContainerSocketIteration>().AvaibleSocket == 3 && name == "Brown")
            {
                TeleportBallToContainer(BallsContainerColour);
            }
            else if (BallsContainerColour.GetComponent<BallsContainerSocketIteration>().AvaibleSocket == 4 && name == "Blue")
            {
                TeleportBallToContainer(BallsContainerColour);
            }
            else if (BallsContainerColour.GetComponent<BallsContainerSocketIteration>().AvaibleSocket == 5 && name == "Pink")
            {
                TeleportBallToContainer(BallsContainerColour);
            }
            else if (BallsContainerColour.GetComponent<BallsContainerSocketIteration>().AvaibleSocket == 6 && name == "Black")
            {
                TeleportBallToContainer(BallsContainerColour);
            }
            else
            {
                gameObject.layer = 3;


                foreach (Vector3 spot in spots)
                {
                    transform.position = spot;
                    if (!CollideWithOther())
                    {
                        break;
                    }
                }
                transform.position = initialPosition;
                while (CollideWithOther())
                {
                    transform.position -= new Vector3(0.01f, 0, 0);
                    if (CollideWithTheUpperRail)
                    {
                        break;
                    }
                }
                if (CollideWithTheUpperRail)
                {
                    transform.position = initialPosition;
                    while (CollideWithOther())
                    {
                        transform.position += new Vector3(0.01f, 0, 0);
                    }
                }
                gameObject.layer = 8;
            }
        }
    }

    void TeleportBallToContainer(GameObject container)
    {
        int iterator = container.GetComponent<BallsContainerSocketIteration>().AvaibleSocket;
        transform.position = BallsContainerRed1.transform.GetChild(iterator).transform.position;
        iterator++;
        BallsContainerRed1.GetComponent<BallsContainerSocketIteration>().AvaibleSocket = iterator;
    }

    bool CollideWithOther()
    {
        foreach (GameObject ball in OtherBalls)
        {
            if (Vector3.Distance(transform.position, ball.transform.position) < ball.GetComponent<Collider>().bounds.size[0])
            {
                return true;
            }
        }
        return false;
    }

    public void ResetGame()
    {
        StopObjectMovement();
        transform.position = initialPosition;
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ball")
        {
            hitBall.Play();
        }
        if (collision.gameObject.tag == "Tabble")
        {
            TouchedBank = true;
            hitTabble.Play();
        }
        if (collision.gameObject.tag == "Cue")
        {
            hitCue.Play();
            XRRig.HapticManager("left", 0.8f, 0.1f);
            XRRig.HapticManager("right", 0.8f, 0.1f);

            //Deactivate box collider in cue
            Invoke("TriggerDeactivate", 0.1f);        
        }
        if (collision.gameObject.tag == "Ground")
        {
            hitPocket.Play();
            Invoke("TeleportBallAfterPocketed", 0.2f);
        }
    }

    void TriggerDeactivate()
    {
        Cue.TriggerDeactivate();
        GameRules.StartHit();
    }

    public AudioSource AddAudio(bool loop, bool playAwake, float vol, AudioClip clip)
    {
        AudioSource newAudio = gameObject.AddComponent<AudioSource>();
        newAudio.loop = loop;
        newAudio.playOnAwake = playAwake;
        newAudio.volume = vol;
        newAudio.clip = clip;
        newAudio.spatialBlend = 1f;
        newAudio.minDistance = 1f;
        newAudio.maxDistance = 10f;
        return newAudio;
    }

}
