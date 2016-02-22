using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//This Class is used for every Enemy. It modifies some of the enemies properties and gives them the ability to do an action (i.e FIring)
public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField]
    private float mActionCD = 1.0f;
    [SerializeField]
    private float mTimeUntilNextAction = 0.0f;
    [SerializeField]
    private float mBossSpeed = 0.5f;
    [SerializeField]
    private float mBulletSpeed = 3.0f;
    [SerializeField]
    private int mHP = 20;
    [SerializeField]
    private int PointsValue = 50;

    private int sequence = 0; //used for bosses firing patterns

    private GameObject mHpBar;

    [SerializeField]
    private int currentHP;

    private bool isGoingLeft;

	void Start()
	{
        currentHP = mHP;
        isGoingLeft = true;
	}
    
    public void ResetHP()
    {
        currentHP = mHP;
    }

    public void SetPointsValue(int val)
    {
        PointsValue = val;
    }

    public void SetActionCD(float cd)
    {
        mActionCD = cd;
        mTimeUntilNextAction = mActionCD;
    }

    public void SetBulletSpeed(float speed)
    {
        mBulletSpeed = speed;
    }

    void Update()
    {
        mTimeUntilNextAction -= GameLogic.GameDeltaTime;
        if (mHpBar == null)
            mHpBar = transform.Find("HPBar(Clone)").gameObject;

        else if (this.name.Substring(0, 4) == "Boss") //Update HP Bar
        {
            float barLength = 0.507f;
            float xCoord = transform.position.x - 3.9f;
            Transform hpb = mHpBar.transform.Find("Bar");
            float percentage = Mathf.Max(Mathf.Min((((float)currentHP) / (float)mHP) * 100.0f, 100.0f), 0.0f);
            hpb.localScale = new Vector3(percentage * barLength, hpb.localScale.y, hpb.localScale.z);
            hpb.position = new Vector3(xCoord + percentage / ((1 / barLength) * 13.0f), hpb.position.y, hpb.position.z);
        }
        else
        {
            float barLength = 0.502f;
            float xCoord = transform.position.x - 2.0f;
            Transform hpb = mHpBar.transform.Find("Bar");
            float percentage = Mathf.Max(Mathf.Min((((float)currentHP) / (float)mHP) * 100.0f, 100.0f), 0.0f);
            hpb.localScale = new Vector3(percentage * barLength, hpb.localScale.y, hpb.localScale.z);
            hpb.position = new Vector3(xCoord + percentage / ((1 / barLength) * 25.0f), hpb.position.y, hpb.position.z);
        }
    }

    public void SetHP(int hp)
    {
        mHP = hp;
    }

    public void InflictDamage(int dmg)
    {
        currentHP -= dmg;
        if(currentHP <= 0)
            GameObject.Find("Game").GetComponent<GameLogic>().DestroyEnemy(this.gameObject);
    }

    public int GetPointsValue()
    {
        return PointsValue;
    }

    //Fire based on boss firing pattern
    public void DoActionBoss()
    {
        if (transform.position.y > GameLogic.ScreenHeight - 25.0f) return;
        if (mTimeUntilNextAction > 0) return;

        int enemyType = int.Parse(this.name.Substring(5, this.name.IndexOf('_', 5) - 5));


        Vector3 playerPos = GameObject.Find("Player").transform.position;
        switch(enemyType) //Using sequence as a way to have more interesting firing patterns
        {
            case 0:
                Weapon.FireEnemyBullet(this.transform.position, ShootTowards(playerPos + new Vector3(0, 0, 0)));
                Weapon.FireEnemyBullet(this.transform.position, ShootTowards(playerPos + new Vector3(2.5f, 0, 0)));
                Weapon.FireEnemyBullet(this.transform.position, ShootTowards(playerPos + new Vector3(-2.5f, 0, 0)));
                Weapon.FireEnemyBullet(this.transform.position, ShootTowards(playerPos + new Vector3(5f, 0, 0)));
                Weapon.FireEnemyBullet(this.transform.position, ShootTowards(playerPos + new Vector3(-5f, 0, 0)));
                Weapon.FireEnemyBullet(this.transform.position, ShootTowards(playerPos + new Vector3(7.5f, 0, 0)));
                Weapon.FireEnemyBullet(this.transform.position, ShootTowards(playerPos + new Vector3(-7.5f, 0, 0)));
                break;
            case 1:
                switch (sequence++ % 10)
                {
                    case 0:
                        Weapon.FireEnemyBullet(this.transform.position + new Vector3(0, 0, 0), ShootTowards(playerPos + new Vector3(0, 0, 0)));
                        break;
                    case 2:
                        Weapon.FireEnemyBullet(this.transform.position + new Vector3(3, 0, 0), ShootTowards(playerPos + new Vector3(0, 0, 0)));
                        Weapon.FireEnemyBullet(this.transform.position + new Vector3(-3, 0, 0), ShootTowards(playerPos + new Vector3(0, 0, 0)));
                        break;
                    case 4:
                        Weapon.FireEnemyBullet(this.transform.position + new Vector3(6, 0, 0), ShootTowards(playerPos + new Vector3(0, 0, 0)));
                        Weapon.FireEnemyBullet(this.transform.position + new Vector3(-6, 0, 0), ShootTowards(playerPos + new Vector3(0, 0, 0)));
                        break;
                    case 6:
                        Weapon.FireEnemyBullet(this.transform.position + new Vector3(9, 0, 0), ShootTowards(playerPos + new Vector3(0, 0, 0)));
                        Weapon.FireEnemyBullet(this.transform.position + new Vector3(9, 0, 0), ShootTowards(playerPos + new Vector3(0, 0, 0)));
                        break;
                    case 8:
                        Weapon.FireEnemyBullet(this.transform.position + new Vector3(12, 0, 0), ShootTowards(playerPos + new Vector3(0, 0, 0)));
                        Weapon.FireEnemyBullet(this.transform.position + new Vector3(-12, 0, 0), ShootTowards(playerPos + new Vector3(0, 0, 0)));
                        break;
                }
                break;
            case 2:
                switch(sequence++ % 10)
                {
                    case 0:
                        Weapon.FireEnemyBullet(this.transform.position + new Vector3(0, 0, 0), ShootTowards(playerPos + new Vector3(0, 0, 0)));
                        break;
                    case 2:
                        Weapon.FireEnemyBullet(this.transform.position + new Vector3(0, 0, 0), ShootTowards(playerPos + new Vector3(3, 0, 0)));
                        Weapon.FireEnemyBullet(this.transform.position + new Vector3(0, 0, 0), ShootTowards(playerPos + new Vector3(-3, 0, 0)));
                        break;
                    case 4:
                        Weapon.FireEnemyBullet(this.transform.position + new Vector3(0, 0, 0), ShootTowards(playerPos + new Vector3(6, 0, 0)));
                        Weapon.FireEnemyBullet(this.transform.position + new Vector3(0, 0, 0), ShootTowards(playerPos + new Vector3(-6, 0, 0)));
                        break;
                    case 6:
                        Weapon.FireEnemyBullet(this.transform.position + new Vector3(0, 0, 0), ShootTowards(playerPos + new Vector3(9, 0, 0)));
                        Weapon.FireEnemyBullet(this.transform.position + new Vector3(0, 0, 0), ShootTowards(playerPos + new Vector3(-9, 0, 0)));
                        break;
                    case 8:
                        Weapon.FireEnemyBullet(this.transform.position + new Vector3(0, 0, 0), ShootTowards(playerPos + new Vector3(12, 0, 0)));
                        Weapon.FireEnemyBullet(this.transform.position + new Vector3(0, 0, 0), ShootTowards(playerPos + new Vector3(-12, 0, 0)));
                        break;
                }
                if (sequence % 2 == 0)
                    Weapon.FireEnemyBullet(this.transform.position + new Vector3(0, 0, 0), ShootTowards(playerPos + new Vector3(0, 0, 0), 5.0f));
                break;
        }
        if (sequence == 100000) sequence = 0;
        mTimeUntilNextAction = mActionCD;
    }

    public Vector3 ShootTowards(Vector3 direction, float extraSpeed = 0f)
    {
        Vector3 output = direction - (transform.position);
        output.Normalize();
        output *= mBulletSpeed + extraSpeed;
        return output;
    }

    //This function is called on every iteration. It will check that the action cooldown has elapsed and if it has it will allow the enemy to shoot towards the player.
    public void DoAction()
    {
        if (mTimeUntilNextAction > 0) return;
        if (transform.position.y > GameLogic.ScreenHeight - 1.0f) return;

        if (this.name.Substring(0, 4) == "Boss")
        {
            DoActionBoss();
            return;
        }
        int enemyType = int.Parse(this.name.Substring(6, this.name.IndexOf('_', 6) - 6));
        switch(enemyType)
        {
            case 1:
                Weapon.FireEnemyBullet(this.transform.position, new Vector3(0, -20.0f, 0));
                break;
            case 2:
                Weapon.FireEnemyBullet(this.transform.position, new Vector3(5, -20.0f, 0));
                Weapon.FireEnemyBullet(this.transform.position, new Vector3(-5, -20.0f, 0));
                break;
            case 3:
                Weapon.FireEnemyBullet(this.transform.position, new Vector3(5, -20.0f, 0));
                Weapon.FireEnemyBullet(this.transform.position, new Vector3(-5, -20.0f, 0));
                Weapon.FireEnemyBullet(this.transform.position, new Vector3(0, -20.0f, 0));
                break;
            case 4:
                Weapon.FireEnemyBullet(this.transform.position, new Vector3(10, -20.0f, 0));
                Weapon.FireEnemyBullet(this.transform.position, new Vector3(-10, -20.0f, 0));
                Weapon.FireEnemyBullet(this.transform.position, new Vector3(5, -20.0f, 0));
                Weapon.FireEnemyBullet(this.transform.position, new Vector3(-5, -20.0f, 0));
                Weapon.FireEnemyBullet(this.transform.position, new Vector3(0, -20.0f, 0));
                break;

        }
        mTimeUntilNextAction = mActionCD;
    }


    //This is a movement pattern for bosses that makes them strafe back and forth
    public Vector3 StrafeBackAndForth()
    {
        Vector3 position = transform.position;
        if (isGoingLeft && position.x > -GameLogic.ScreenBounds + GetComponent<SpriteRenderer>().bounds.size.y / 2)
            position.x -= GameLogic.GameDeltaTime * GameLogic.GameSpeed * mBossSpeed;
        else if (!isGoingLeft && position.x < GameLogic.ScreenBounds - GetComponent<SpriteRenderer>().bounds.size.y / 2)
            position.x += GameLogic.GameDeltaTime * GameLogic.GameSpeed * mBossSpeed;
        else
            isGoingLeft = !isGoingLeft;

        return position;

    }
}
