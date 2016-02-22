using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyFactory : MonoBehaviour 
{
    public enum Column { One, Two, Three, NumColumns }

	private static EnemyFactory mInstance; 

	[SerializeField] private Camera GameplayCamera = null;
	[SerializeField] private float EnemyScale = 1.5f;
    [Range(1, 100)]
    [SerializeField]
    private int EnemyPoolSize = 10;
    [SerializeField]
    private RuntimeAnimatorController[] enemyController = null;
    [SerializeField]
    private RuntimeAnimatorController[] bossController = null;


    private GameObject [][] mPool;
	private List<GameObject>[] mActive;
	private List<GameObject>[] mInactive;
	private float mColumnWidth;
	
	void Start()
	{
		if( mInstance == null )
		{
			mInstance = this;

            mActive = new List<GameObject>[enemyController.Length + bossController.Length];
            mInactive = new List<GameObject>[enemyController.Length + bossController.Length];
            mPool = new GameObject[enemyController.Length + bossController.Length][];

            // Work out the width of each column
            mColumnWidth = ( GameLogic.ScreenHeight * GameplayCamera.aspect * 0.8f ) / (int)Column.NumColumns;


            //Spawn the 3 different bosses
            for(int bossType = 0; bossType < bossController.Length; bossType++)
            {
                mActive[enemyController.Length + bossType] = new List<GameObject>();
                mInactive[enemyController.Length + bossType] = new List<GameObject>();
                mPool[enemyController.Length + bossType] = new GameObject[1];
                GameObject boss = new GameObject("Boss_" + bossType + "_PoolID0");
                var eb = boss.AddComponent<EnemyBehaviour>();
                Animator anim = boss.AddComponent<Animator>();
                anim.runtimeAnimatorController = bossController[bossType];
                boss.AddComponent<SpriteRenderer>();
                var pc = boss.AddComponent<PolygonCollider2D>();

                switch (bossType)
                {
                    case 0:
                        pc.SetPath(0, new Vector2[] { new Vector2(2.60f, -0.14f), new Vector2(3.52f, 2.17f), new Vector2(-3.73f, 2.19f), new Vector2(-2.91f, -0.12f), new Vector2(-1.95f, -0.69f), new Vector2(-0.26f, -1.07f), new Vector2(1.66f, -0.8f) });
                        eb.SetHP(250);
                        eb.SetActionCD(0.9f);
                        eb.SetBulletSpeed(10.0f);
                        break;
                    case 1:
                        pc.SetPath(0, new Vector2[] { new Vector2(1.1936f, -3.090f), new Vector2(1.1401f, -0.1680f), new Vector2(1.9069f, 1.1277f), new Vector2(1.9274f, 2.1667f), new Vector2(-1.9252f, 2.1992f), new Vector2(-1.9099f, 1.0355f), new Vector2(-1.1845f, 0.1712f), new Vector2(-1.2249f, -3.085f) });
                        eb.SetHP(700);
                        eb.SetActionCD(0.1f);
                        eb.SetBulletSpeed(10.0f);
                        break;
                    case 2:
                        pc.SetPath(0, new Vector2[] { new Vector2(3.34f, 1.46f), new Vector2(3.38f, -0.68f), new Vector2(2.28f, -2.6f), new Vector2(0f, -1.37f), new Vector2(-2.32f, -2.6f), new Vector2(-3.42f, -0.69f), new Vector2(-3.37f, 1.23f), new Vector2(-1.9f, 2.6f), new Vector2(1.58f, 2.55f) });
                        eb.SetHP(1500);
                        eb.SetActionCD(0.1f);
                        eb.SetBulletSpeed(10.0f);
                        break;

                }
                eb.SetPointsValue(1000);

                pc.isTrigger = true;

                
                boss.transform.localScale = new Vector3(EnemyScale, EnemyScale, EnemyScale);
                boss.transform.localRotation = Quaternion.AngleAxis(0.0f, Vector3.forward);
                boss.transform.parent = transform;
                boss.transform.tag = "Enemies";
                boss.layer = LayerMask.NameToLayer("Enemy");
                GameObject hpBar = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/HPBar"), new Vector3(0, 5, 0), Quaternion.identity);
                hpBar.transform.parent = boss.transform;
                hpBar.transform.localScale = new Vector3(0.1f, 0.02f, 1f);
                hpBar.transform.localPosition = new Vector3(0, 2.5f, 0);
                hpBar.transform.rotation = Quaternion.identity;
                var rb = boss.AddComponent<Rigidbody2D>();
                rb.gravityScale = 0f;
                mPool[enemyController.Length + bossType][0] = boss;
                mInactive[enemyController.Length + bossType].Add(boss);
                boss.SetActive(false);
                
            }

            // For each type of enemy....
            for (int enemyType = 0; enemyType < enemyController.Length; enemyType++)
            {
                mActive[enemyType] = new List<GameObject>();
                mInactive[enemyType] = new List<GameObject>();
                mPool[enemyType] = new GameObject[EnemyPoolSize];
                Vector2 offset = new Vector2(0,0);
                Vector2 size = new Vector2(0, 0);
                Vector2[] polyPath = new Vector2[] { };
                string colliderType = "";
                int HP = 20;
                int pointsVal = 20;
                switch(enemyType)
                {
                    case 0:
                        colliderType = "box";
                        offset = new Vector2(0.0f, -0.2411957f);
                        size = new Vector2(1.463096f, 1.173663f);
                        HP = 15;
                        pointsVal = 75;
                        break;
                    case 1:
                        colliderType = "box";
                        offset = new Vector2(0.0f, -0.2411957f);
                        size = new Vector2(1.463096f, 1.173663f);
                        HP = 20;
                        pointsVal = 100;
                        break;
                    case 2:
                        colliderType = "poly";
                        polyPath = new Vector2[] { new Vector2(0.9552f, 0.6247f), new Vector2(-0.9384f, 0.6395f), new Vector2(0.0063f, -1.0732f) };
                        HP = 30;
                        pointsVal = 150;
                        break;
                    case 3:
                        colliderType = "box";
                        offset = new Vector2(0, -0.1444435f);
                        size = new Vector2(1.62f, 0.9711132f);
                        HP = 25;
                        pointsVal = 250;
                        break;
                    case 4:
                        colliderType = "box";
                        offset = new Vector2(0, -0.1564808f);
                        size = new Vector2(1.56f, 1.127038f);
                        HP = 35;
                        pointsVal = 400;
                        break;

                }

                // ... Spawn "EnemyPoolSize" number of enemies 
                for (int count = 0; count < mPool[enemyType].Length; count++)
                {
                    GameObject enemy = new GameObject("Enemy_"+ enemyType + "_PoolID" + (count + 1));
                    var eb = enemy.AddComponent<EnemyBehaviour>();
                    eb.SetHP(HP);
                    Animator anim = enemy.AddComponent<Animator>();
                    anim.runtimeAnimatorController = enemyController[enemyType];
                    enemy.AddComponent<SpriteRenderer>();
                    eb.SetPointsValue(pointsVal);
                    if (colliderType == "box")
                    {
                        var bc = enemy.AddComponent<BoxCollider2D>();
                        bc.offset = offset;
                        bc.size = size;
                        bc.isTrigger = true;
                    }
                    else if (colliderType == "poly")
                    {
                        var pc = enemy.AddComponent<PolygonCollider2D>();
                        pc.SetPath(0, polyPath);
                        pc.isTrigger = true;
                    }
                    enemy.transform.localScale = new Vector3(EnemyScale, EnemyScale, EnemyScale);
                    enemy.transform.localRotation = Quaternion.AngleAxis(0.0f, Vector3.forward);
                    enemy.transform.parent = transform;
                    enemy.transform.tag = "Enemies";
                    var rb = enemy.AddComponent<Rigidbody2D>();
                    rb.gravityScale = 0f;
                    enemy.layer = LayerMask.NameToLayer("Enemy");
                    GameObject hpBar =(GameObject) GameObject.Instantiate(Resources.Load("Prefabs/HPBar"), new Vector3(0, 5, 0), Quaternion.identity);
                    hpBar.transform.parent = enemy.transform;
                    hpBar.transform.localScale = new Vector3(0.05f, 0.02f, 1f);
                    hpBar.transform.localPosition = new Vector3(0, 2, 0);
                    hpBar.transform.rotation = Quaternion.identity;
                    mPool[enemyType][count] = enemy;
                    mInactive[enemyType].Add(enemy);
                    enemy.SetActive(false);
                }
            }
		}
		else
		{
			Debug.LogError( "Only one EnemyFactory allowed - destorying duplicate" );
			Destroy( this.gameObject );
		}
	}
    
    

	public static GameObject Dispatch( int enemyType, Column column, bool isBoss = false)
	{
		if( mInstance != null )
		{
			return mInstance.DoDispatch(enemyType, column, isBoss);
		}
		return null;
	}

	public static bool Return( GameObject enemy )
	{
		if( mInstance != null )
		{
            if (enemy.name.Substring(0, 4) == "Boss")
            {
                int enemyType = int.Parse(enemy.name.Substring(5, enemy.name.IndexOf('_', 5) - 5));
                if (mInstance.mActive[mInstance.enemyController.Length + enemyType].Remove(enemy))
                {
                    enemy.SetActive(false);
                    mInstance.mInactive[mInstance.enemyController.Length + enemyType].Add(enemy);
                }
            }
            else
            {
                int enemyType = int.Parse(enemy.name.Substring(6, enemy.name.IndexOf('_', 6) - 6));
                if (mInstance.mActive[enemyType].Remove(enemy))
                {
                    enemy.SetActive(false);
                    mInstance.mInactive[enemyType].Add(enemy);
                }
            }
		}
		return false;
	}

	public static void Reset()
	{
		if( mInstance != null )
		{
            for (int i = 0; i < mInstance.mPool.Length; i++)
            {
                for (int count = 0; count < mInstance.mPool.Length; count++)
                {
                    mInstance.mPool[i][count].SetActive(false);
                    mInstance.mInactive[i].Add(mInstance.mPool[i][count]);
                }

                mInstance.mActive[i].Clear();
            }
		}
	}

	private GameObject DoDispatch(int enemyType, Column column, bool isBoss = false)
    {
        // Look for a free enemy and then dispatch them 
        GameObject result = null;
        if (isBoss)
        {
            Debug.Log("Spawn Boss");
            GameObject enemy = mInactive[enemyController.Length + enemyType][0];
            Vector3 position = enemy.transform.position;
            position.x = -mColumnWidth + (mColumnWidth * (float)column);
            position.y = GameLogic.ScreenHeight * 0.5f;
            position.z = 0.0f;
            enemy.transform.position = position;
            enemy.SetActive(true);
            mActive[enemyController.Length + enemyType].Add(enemy);
            mInactive[enemyController.Length + enemyType].Remove(enemy);
            enemy.GetComponent<EnemyBehaviour>().ResetHP();
            result = enemy;
        }
        else
        {
            if (mInactive[enemyType].Count > 0)
            {
                GameObject enemy = mInactive[enemyType][0];
                Vector3 position = enemy.transform.position;
                position.x = -mColumnWidth + (mColumnWidth * (float)column);
                position.y = GameLogic.ScreenHeight * 0.5f;
                position.z = 0.0f;
                enemy.transform.position = position;
                enemy.SetActive(true);
                mActive[enemyType].Add(enemy);
                mInactive[enemyType].Remove(enemy);
                enemy.GetComponent<EnemyBehaviour>().ResetHP();
                result = enemy;
            }
        }
		// Returns true if a free enemy was found and dispatched
		return result;
	}
}
