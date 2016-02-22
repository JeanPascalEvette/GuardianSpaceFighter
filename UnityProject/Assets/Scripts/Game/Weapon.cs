using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Weapon : MonoBehaviour
{
    private static Weapon mInstance;
    [SerializeField]
    private float BulletScale = 0.5f;
    [SerializeField]
    private float RechargeTime = 0.25f;
    [SerializeField]
    private float FireRateUpgradeDifference = 0.01f;
    [Range(1, 100)]
    [SerializeField]
    private int BulletPoolSize = 10;
    [SerializeField]
    private RuntimeAnimatorController missileAnimController = null;



    private GameObject[] mPool;
    private List<GameObject> mActive;
    private List<GameObject> mInactive;
    private float mCharging;

    private List<List<int>> multishotAngle;
    private List<List<int>> multishotXPos;
    private List<int> fastBulletSpeed;

    public List<GameObject> ActiveBullets { get { return mActive; } }



    void Start()
    {
        if (mInstance == null)
        {
            mInstance = this;
            // Create the bullets, initialise the active and available lists, put all bullets in the available list
            mActive = new List<GameObject>();
            mInactive = new List<GameObject>();
            mPool = new GameObject[BulletPoolSize];
            multishotAngle = new List<List<int>>();
            multishotAngle.Add(new List<int> { 0 });
            multishotAngle.Add(new List<int> { 0, 0 });
            multishotAngle.Add(new List<int> { 30, 0, -30 });
            multishotAngle.Add(new List<int> { 0, 30, -30, 0 });
            multishotAngle.Add(new List<int> { 60, 30, 0, -30, -60 });
            multishotXPos = new List<List<int>>();
            multishotXPos.Add(new List<int> { 0 });
            multishotXPos.Add(new List<int> { 1, -1 });
            multishotXPos.Add(new List<int> { 2, 0, -2 });
            multishotXPos.Add(new List<int> { 1, 2, -2, -1 });
            multishotXPos.Add(new List<int> { 2, 1, 0, -1, -2 });


            fastBulletSpeed = new List<int> { 0, 30, 60, 90, 120 };


            for (int count = 0; count < mPool.Length; count++)
            {
                GameObject bullet = new GameObject("Bullet_PoolID" + (count + 1));
                bullet.transform.localScale = new Vector3(BulletScale, BulletScale, BulletScale);
                bullet.transform.parent = transform;
                Animator anim = bullet.AddComponent<Animator>();
                anim.runtimeAnimatorController = missileAnimController;
                bullet.AddComponent<SpriteRenderer>();
                bullet.AddComponent<BoxCollider2D>();
                bullet.AddComponent<BulletMovement>();
                bullet.layer = 1;
                mPool[count] = bullet;
                mInactive.Add(bullet);
                bullet.SetActive(false);
            }
            mCharging = 0.0f;
        }
        else
        {
            Debug.LogError("Only one Weapon allowed - destorying duplicate");
            Destroy(this.gameObject);
        }
    }

    void Update()
    {
        if (DifficultyCurve.GameSpeed == 0.0f) return;
        // Update the position of each active bullet, keep a track of bullets which have gone off screen 
        List<GameObject> oldBullets = new List<GameObject>();
        for (int count = 0; count < mActive.Count; count++)
        {
            Vector3 oldPosition = mActive[count].transform.position;
            Vector3 dispacement = new Vector3(0, 0, 0);
            BulletMovement bm = mActive[count].GetComponent<BulletMovement>();
            dispacement.y += GameLogic.GameDeltaTime * bm.GetYVel();
            dispacement.x += GameLogic.GameDeltaTime * bm.GetXVel();
            mActive[count].transform.position = oldPosition + dispacement;
             

            RaycastHit2D hit = Physics2D.Raycast(oldPosition, dispacement, dispacement.magnitude, 1 << LayerMask.NameToLayer("Enemy"));
            //Use Raycast to make sure we are not missing any collision
            if (hit.collider != null && mActive[count].GetComponent<BulletMovement>().FromPlayer)
            {//Only the player's bullet will deal damage to enemies
                hit.collider.gameObject.GetComponent<EnemyBehaviour>().InflictDamage(PlayerCharacter.BulletDamage);
                mActive[count].SetActive(false);
                oldBullets.Add(mActive[count]);
            }

            else if (mActive[count].transform.position.y > GameLogic.ScreenHeight * 0.5f)
            {
                mActive[count].SetActive(false);
                oldBullets.Add(mActive[count]);
            }

            else if (mActive[count].transform.position.y < GameLogic.ScreenHeight * -0.5f)
            {
                mActive[count].SetActive(false);
                oldBullets.Add(mActive[count]);
            }
            else
            {
                Debug.DrawLine(oldPosition, oldPosition + dispacement, Color.red);
            }
        }

        for (int bullet = 0; bullet < mActive.Count; bullet++)
        {
            if (mActive[bullet].activeInHierarchy)
            {
                if (!mActive[bullet].GetComponent<BulletMovement>().FromPlayer)
                {
                    Vector3 diffToBullet = transform.parent.transform.position - mActive[bullet].transform.position;
                    if (diffToBullet.sqrMagnitude < GameObject.Find("Game").GetComponent<GameLogic>().BulletKillDistance)
                    {
                        // Touched Bullet
                        GameObject.Find("Game").GetComponent<GameLogic>().DamagePlayer(10);

                        mActive[bullet].SetActive(false);
                        oldBullets.Add(mActive[bullet]);
                    }
                }
            }
        }
        // Remove the bullets which have gone off screen, return them to the available list
        for (int count = 0; count < oldBullets.Count; count++)
        {
            oldBullets[count].transform.parent = transform;
            mActive.Remove(oldBullets[count]);
            mInactive.Add(oldBullets[count]);
        }

        if (mCharging > 0.0f)
        {
            mCharging -= GameLogic.GameDeltaTime;
        }
    }

    public bool Fire(Vector3 position)
    {
        // Look for a free bullet and then fire it from the player position
        bool result = false;
        if (mCharging > 0.0f) return result;
        int multishotLevel = UserData.GetMultishot();
        int bulletSpeedLevel = UserData.GetBulletSpeed();
        for (int i = 0; i < multishotLevel + 1; i++)
        {
            if (mInactive.Count > 0)
            {
                GameObject bullet = mInactive[0];
                bullet.transform.parent = null;
                bullet.transform.position = position + new Vector3(multishotXPos[multishotLevel][i], 0,0);
                BulletMovement bm = bullet.GetComponent<BulletMovement>();
                bm.SetXVel(multishotAngle[multishotLevel][i]);
                bm.SetYVel(fastBulletSpeed[bulletSpeedLevel] + GameLogic.BulletSpeed);
                Vector3 direction = (new Vector3(multishotAngle[multishotLevel][i], fastBulletSpeed[bulletSpeedLevel] + GameLogic.BulletSpeed, 0) - new Vector3(0, 0, 0)).normalized;

                float rot_z = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                bullet.transform.localRotation = Quaternion.Euler(0f, 0f, rot_z - 90);
                if (bullet.transform.localScale.y < 0)
                    bullet.transform.localScale *= -1;
                bm.AssignToPlayer(true);
                bullet.SetActive(true);
                mActive.Add(bullet);
                mInactive.Remove(bullet);
                mCharging = RechargeTime - (UserData.GetFireRate() * FireRateUpgradeDifference);
                result = true;
            }
        }

        // Returns true if a free bullet was found and fired
        return result;
    }

    //This function is used to fire one of the enemies' bullets
    public static bool FireEnemyBullet(Vector3 position, Vector3 velocity)
    {
        GameObject bullet = mInstance.mInactive[0];
        bullet.transform.parent = null;
        bullet.transform.position = position;
        if (bullet.transform.localScale.y > 0)
            bullet.transform.localScale *= -1;
        BulletMovement bm = bullet.GetComponent<BulletMovement>();
        bm.SetXVel(velocity.x);
        bullet.transform.localRotation = Quaternion.AngleAxis(-velocity.x, new Vector3(0, 0, 1));
        bm.SetYVel(velocity.y);
        bm.AssignToPlayer(false);
        bullet.SetActive(true);
        mInstance.mActive.Add(bullet);
        mInstance.mInactive.Remove(bullet);
        return true;
    }
}
