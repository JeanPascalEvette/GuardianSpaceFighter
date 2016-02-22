using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerCharacter : MonoBehaviour
{
    [SerializeField]
    private float FireOffset = 0f;
    [SerializeField]
    private GameObject sideLaserPrefab = null;
    [SerializeField]
    private float LaserCD = 10.0f;

    private float LaserLastUse;
    

    private Weapon mGun;
    private Vector3 mTargetPosition;
    private float mStartY;

    private GameObject SideLaserLeft;
    private GameObject SideLaserRight;
    public static int SideLaserDamage;
    public static int BulletDamage;
    [SerializeField]
    private int sideLaserDamage = 15;
    [SerializeField]
    private int bulletDamage = 5;

    [SerializeField]
    private int mHP = 50;
    [SerializeField]
    private int mCurrentHP;

    public Weapon Weapon { get { return mGun; } }

    private List<Vector3> scaleSideLasers = new List<Vector3>() {
        new Vector3(0,0,0),
        new Vector3(1.5f,0.2f,1),
        new Vector3(2.2f,0.7f,1),
        new Vector3(2.5f,1.0f,1),
        new Vector3(3.0f,1.7f,1),
    };


    void Start()
    {
        BulletDamage = bulletDamage;
        SideLaserDamage = sideLaserDamage;
        Vector3 position = transform.position;
        position.y = GameLogic.ScreenHeight * -0.35f;
        mStartY = position.y;
        transform.position = position;

        mCurrentHP = mHP;

        // Look for the gun
        mGun = GetComponentInChildren<Weapon>();
        

        SideLaserLeft = (GameObject)Instantiate(sideLaserPrefab, new Vector3(transform.position.x - 1, transform.position.y, 0), Quaternion.identity);
        SideLaserRight = (GameObject)Instantiate(sideLaserPrefab, new Vector3(transform.position.x + 1, transform.position.y, 0), Quaternion.Euler(0, 180, 0));
        SideLaserLeft.transform.parent = transform;
        SideLaserRight.transform.parent = transform;
        SideLaserLeft.transform.name = "SideLaserLeft";
        SideLaserRight.transform.name = "SideLaserRight";
        SideLaserLeft.SetActive(false);
        SideLaserRight.SetActive(false);
        LaserLastUse = 0.0f;
    }

    void Update()
    {
        //Update SideLaser, and bars positions
        if(GameObject.Find("Game").GetComponent<GameLogic>().isGameStatusGame() && UserData.GetSideLaser() != 0)
        {
            PlaceHUDElements.UpdateSideLaserBar(LaserCD - (1*UserData.GetSideLaser()), LaserLastUse);
            LaserLastUse += Time.deltaTime;
        }
        PlaceHUDElements.UpdateHPBar(mCurrentHP, mHP);
        Vector3 position = transform.position;
        float distance = (mTargetPosition - position).magnitude;
        if (distance > 0.5f)
        {//If PlayerSpeed is -1, then the moves are instantaneous.
            if (GameLogic.PlayerSpeed != -1)
            {
                position.x = Mathf.SmoothStep(position.x, mTargetPosition.x, GameLogic.GameDeltaTime * GameLogic.PlayerSpeed);
                position.y = Mathf.SmoothStep(position.y, mTargetPosition.y, GameLogic.GameDeltaTime * GameLogic.PlayerSpeed);
            }
            else
            {
                position.x = mTargetPosition.x;
                position.y = mTargetPosition.y;
            }
            transform.position = position;
        }
    }


    public void Reset()
    {
        Vector3 position = new Vector3(0.0f, mStartY, 0.0f);
        transform.position = position;
        mTargetPosition = new Vector3(0.0f, 0.0f, 0.0f);

    }

    public void ResetHP()
    {
        mCurrentHP = mHP;
    }

    public bool InflictDamage(int dmg)
    {
        mCurrentHP -= dmg;
        if (mCurrentHP <= 0)
            return true;
        return false;
    }

    public void Fire()
    {
        if (mGun != null)
        {
            Vector3 position = transform.position;
            position.y += FireOffset;
            mGun.Fire(position);
        }
    }

    //This Fires the SideLasers if the cooldown is up
    public void FireSideLaser(bool isLeft)
    {
        if (LaserLastUse < LaserCD - (1 * UserData.GetSideLaser()) || UserData.GetSideLaser() == 0) return;
        LaserLastUse = 0.0f;


        GameObject SideLaser;
        if (isLeft)
            SideLaser = SideLaserLeft;
        else
            SideLaser = SideLaserRight;
        
        SideLaser.transform.localScale = scaleSideLasers[UserData.GetSideLaser()];
        

        SideLaser.SetActive(true);

        StartCoroutine(DisableSideLaser(isLeft));
    }

    IEnumerator DisableSideLaser(bool isLeft)
    {
        yield return new WaitForSeconds(0.2f);

        if (isLeft)
            SideLaserLeft.SetActive(false);
        else
            SideLaserRight.SetActive(false);
    }

    public void Move(Vector3 newTarget)
    {
        mTargetPosition = newTarget;
    }

    //Colliding with an enemy results in an instant death
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemies")
        {
            GameObject.Find("Game").GetComponent<GameLogic>().DamagePlayer(100);

        }
    }



}
