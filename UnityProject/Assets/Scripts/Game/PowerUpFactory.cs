using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//This class was first designed when upgrades where obtained through the powerup system instead of the shop. It now only creates the money drops, but could easily be extended if necessary.
public class PowerUpFactory : MonoBehaviour
{

    private static PowerUpFactory mInstance;
    private GameObject[] mPool;
    private List<GameObject> mActive;
    private List<GameObject> mInactive;
    [Range(1, 100)]
    [SerializeField]
    private int PowerUpPoolSize = 10;
    

    public enum PowerUpType
    {
        MONEY
    };

    // Use this for initialization
    void Start()
    {

        if (mInstance == null)
        {
            mInstance = this;


            // Create the enemies, initialise the active and available lists, put all enemies in the available list
            mActive = new List<GameObject>();
            mInactive = new List<GameObject>();
            mPool = new GameObject[PowerUpPoolSize];
            for (int count = 0; count < mPool.Length; count++)
            {
                for (int typeCount = 0; typeCount < System.Enum.GetNames(typeof(PowerUpType)).Length; typeCount++)
                {
                    string powerUpName = System.Enum.GetNames(typeof(PowerUpType))[typeCount];
                    

                    GameObject powerup = new GameObject("PowerUp_"+powerUpName+"PoolID" + (count + 1));

                    Animator anim = powerup.AddComponent<Animator>();
                    RuntimeAnimatorController animatorController = Resources.Load<RuntimeAnimatorController>("Animations/PowerUps/" + powerUpName + "/PU" + powerUpName);
                    if (animatorController == null) return;
                    anim.runtimeAnimatorController = animatorController;
                    powerup.AddComponent<SpriteRenderer>();
                    var collider = powerup.AddComponent<BoxCollider2D>();
                    collider.isTrigger = true;
                    collider.offset = new Vector2(-0.01864898f, -0.07832527f);
                    collider.size = new Vector2(1.857859f, 1.813102f);
                    var puc = powerup.AddComponent<PowerUpController>();
                    puc.myType = (PowerUpType)typeCount;
                    var rb = powerup.AddComponent<Rigidbody2D>();
                    rb.gravityScale = 0.0f;
                    powerup.transform.parent = transform;
                    mPool[count] = powerup;
                    mInactive.Add(powerup);
                    powerup.SetActive(false);
                }
            }
        }
        else
        {
            Debug.LogError("Only one PowerUpFactory allowed - destorying duplicate");
            Destroy(this.gameObject);
        }


    }

    public void MovePowerups(float speed)
    {
        List<GameObject> oldPowerUps = new List<GameObject>();
        for (int count = 0; count < mActive.Count; count++)
        {
            Vector3 position = mActive[count].transform.position;
            position.y -= speed;
            mActive[count].transform.position = position;
            if (position.y < GameLogic.ScreenHeight * -0.5f)
            {
                oldPowerUps.Add(mActive[count]);
                Return(mActive[count]);
            }

        }
        for (int count = 0; count < oldPowerUps.Count; count++)
        {
            mActive.Remove(oldPowerUps[count]);
        }
    }

    public static GameObject Dispatch(Vector3 pos)
    {
        if (mInstance != null)
        {
            return mInstance.DoDispatch(pos);
        }
        return null;
    }

    public static bool Return(GameObject enemy)
    {
        if (mInstance != null)
        {
            if (mInstance.mActive.Remove(enemy))
            {
                enemy.SetActive(false);
                mInstance.mInactive.Add(enemy);
            }
        }
        return false;
    }

    public static void Reset()
    {
        if (mInstance != null)
        {
            for (int count = 0; count < mInstance.mPool.Length; count++)
            {
                mInstance.mPool[count].SetActive(false);
                mInstance.mInactive.Add(mInstance.mPool[count]);
            }

            mInstance.mActive.Clear();
        }
    }

    private GameObject DoDispatch(Vector3 pos)
    {
        // Look for a free enemy and then dispatch them 
        GameObject result = null;
        if (mInactive.Count > 0)
        {
            GameObject powerup = mInactive[0];
            powerup.transform.position = pos;
            powerup.SetActive(true);
            mActive.Add(powerup);
            mInactive.Remove(powerup);
            result = powerup;
        }

        // Returns true if a free enemy was found and dispatched
        return result;
    }


    // Update is called once per frame
    void Update()
    {
        
    }

}
