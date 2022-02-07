using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToPlayer : MonoBehaviour
{
    public Transform target;
    public float speed = 1f;
    public bool LockAxisX;
    public bool LockAxisY;
    public bool LockAxisZ;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Transform targetsPosistion = target;

        Vector3 direction =  transform.position - target.position;
        Quaternion rotation = Quaternion.LookRotation(direction);

        if (LockAxisX)
        {
            rotation = new Quaternion(transform.rotation.x, rotation.y, rotation.z, rotation.w);
        }
        if (LockAxisY)
        {
            rotation = new Quaternion(rotation.x, transform.rotation.y, rotation.z, rotation.w);
        }
        if (LockAxisZ)
        {
            rotation = new Quaternion(rotation.x, rotation.y, transform.rotation.z, rotation.w);
        }
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, speed * Time.deltaTime);
    }
}
