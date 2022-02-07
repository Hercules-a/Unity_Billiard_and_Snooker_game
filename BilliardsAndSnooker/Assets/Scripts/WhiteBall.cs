using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WhiteBall : MonoBehaviour
{
    public List<GameObject> ColidedObjects = new List<GameObject>();
    public List<GameObject> BallsStriped = new List<GameObject>();
    public List<GameObject> BallsSolid = new List<GameObject>();
    public List<GameObject> Bands = new List<GameObject>();
    public GameObject BlackBall;
    public bool IsInKitchen;
    public GameObject TheKitchen;

    List<GameObject> OtherBalls;

    // Start is called before the first frame update
    void Start()
    {
        IsInKitchen = true;

        OtherBalls = new List<GameObject>(BallsSolid.Concat(BallsStriped))
        {
            BlackBall
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        ColidedObjects.Add(collision.gameObject);
    }

    public bool WhiteCollidedWithBandAfterCollidedWithBall()
    {
        bool collidedWithBall = false;

        foreach (GameObject element in ColidedObjects)
        {
            if (BallsSolid.Contains(element) || BallsStriped.Contains(element)) 
            {
                collidedWithBall = true;
            }
            if (collidedWithBall && Bands.Contains(element))
            {
                return true;
            }
        }
        return false;
    }

    public bool WhiteCollidedWithBallAfterLeftKitchen()
    {
        bool leftKitchen = false;

        foreach (GameObject element in ColidedObjects)
        {
            if (TheKitchen == element)
            {
                leftKitchen = true;
            }
            if (leftKitchen && (BallsSolid.Contains(element) || BallsStriped.Contains(element)))
            {
                return true;
            }
        }
        return false;
    }

    public string FirstContactWithBall()
    {
        foreach (GameObject element in ColidedObjects)
        {
            if (BallsSolid.Contains(element))
            {
                return "solids";
            }
            if (BallsStriped.Contains(element))
            {
                return "stripes";
            }
            if (element == BlackBall)
            {
                return "black";
            }
        }
        return "null";
    }

    public void resetList()
    {
        ColidedObjects.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == TheKitchen)
        {
            IsInKitchen = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == TheKitchen)
        {
            IsInKitchen = false;
            ColidedObjects.Add(TheKitchen);
        }
    }

    public bool CollideWithOther()
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
}
