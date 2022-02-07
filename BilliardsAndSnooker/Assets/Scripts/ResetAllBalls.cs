using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetAllBalls : MonoBehaviour
{
    public List<GameObject> ElementsToReset = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetObjects()
    {
        foreach(GameObject item in ElementsToReset)
        {
            item.GetComponent<Ball>().ResetGame();
        }
    }
}
