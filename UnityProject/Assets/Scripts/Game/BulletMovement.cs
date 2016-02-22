using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//This class is used to store some data about the bullets.
public class BulletMovement : MonoBehaviour 
{

    private float xVel;
    private float yVel;
    public bool FromPlayer { get; private set; }


    void Start()
    {
    }

    void Update()
    {
    }

    public float GetXVel()
    {
        return xVel;
    }

    public float GetYVel()
    {
        return yVel;
    }

    public void AssignToPlayer(bool isFromPlayer)
    {
        FromPlayer = isFromPlayer;
    }

    public void SetXVel(float vel)
    {
        xVel = vel;
    }
    public void SetYVel(float vel)
    {
        yVel = vel;
    }
}
