using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR;

public class Ball : MonoBehaviour
{
    public GameObject BallsContainer;
    
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

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;
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
            Invoke("TeleportBallAfterPocketed", 0.2f);
        }
    }

    void TeleportBallAfterPocketed()
    {
        StopObjectMovement();
        if (!BallsContainer)
        {
            transform.position = initialPosition;
        }
        else
        {
            if (OtherBalls.Any() && GameRules.gamePhase == "BreakShot")
            {
                gameObject.layer = 3;
                transform.position = initialPosition;
                while (CollideWithOther())
                {
                    transform.position -= new Vector3(0.01f, 0, 0);
                }
                gameObject.layer = 8;
            }
            else
            {
                int iterator = BallsContainer.GetComponent<BallsContainerSocketIteration>().AvaibleSocket;
                transform.position = BallsContainer.transform.GetChild(iterator).transform.position;
                iterator++;
                BallsContainer.GetComponent<BallsContainerSocketIteration>().AvaibleSocket = iterator;
            }
        }
    }

    bool CollideWithOther()
    {
        foreach (GameObject ball in OtherBalls)
        {
            if (Vector3.Distance(gameObject.transform.position, ball.transform.position) < ball.GetComponent<Collider>().bounds.size[0])
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
