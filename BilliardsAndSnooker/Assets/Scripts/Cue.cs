using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Cue : MonoBehaviour
{
    public BoxCollider HitCylinder;
    public GameObject ColorCylinder;
    public CustomDeviceManager XRRig;
    public List<GameObject> Balls = new List<GameObject>();
    public GameObject WhiteBall;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TriggerActivate()
    {
        if (PlayerPrefs.GetString("gameMode") == "Practice")
        {
            HitCylinder.enabled = true;
            ColorCylinder.GetComponent<Renderer>().material.color = Color.white;
            XRRig.HapticManager("left", 0.5f, 0.01f);
            XRRig.HapticManager("right", 0.5f, 0.01f);

            WhiteBall.layer = 13;
        }
        if (!BallsAreMoving() && WhiteBall.layer != 11 && !WhiteBall.GetComponent<WhiteBall>().CollideWithOther())
        {
            foreach(GameObject ball in Balls)
            {
                ball.GetComponent<PhotonView>().RequestOwnership();
            }

            HitCylinder.enabled = true;
            ColorCylinder.GetComponent<Renderer>().material.color = Color.white;
            XRRig.HapticManager("left", 0.5f, 0.01f);
            XRRig.HapticManager("right", 0.5f, 0.01f);
            
            // change White Ball layer from grababble to WhiteBall
            WhiteBall.layer = 10;
        }
        else
        {
            XRRig.HapticManager("left", 1f, 0.5f);
            XRRig.HapticManager("right", 1f, 0.5f);
        }
    }

    public void TriggerDeactivate()
    {
        HitCylinder.enabled = false;
        ColorCylinder.GetComponent<Renderer>().material.color = Color.red;
    }

    public bool BallsAreMoving()
    {
        foreach(GameObject ball in Balls)
        {
            
            if (ball.GetComponent<Rigidbody>().velocity.magnitude > 0.01f)
            {
                return true;
            }
        }
        return false;
    }
}