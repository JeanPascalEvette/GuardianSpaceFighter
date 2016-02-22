using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//This Factory generates explosions when an enemy is killed
public class ExplosionFactory : MonoBehaviour 
{
    public enum Column { One, Two, Three, NumColumns }

	private static ExplosionFactory mInstance; 
    
	[Range( 1, 100 )]
    [SerializeField]
    private int EnemyPoolSize = 10;
    [SerializeField]
    private RuntimeAnimatorController EnemyExplosionAnim = null;

    private GameObject [] mPool;
	private List<GameObject> mActive;
	private List<GameObject> mInactive;
	
	void Start()
	{
		if( mInstance == null )
		{
			mInstance = this;
            

			// Create the explosions, initialise the active and available lists, put all enemies in the available list
			mActive = new List<GameObject>();
			mInactive = new List<GameObject>();
			mPool = new GameObject[EnemyPoolSize];
            for (int count = 0; count < mPool.Length; count++)
			{
				GameObject explosion = new GameObject( "Explosion_PoolID" + ( count + 1 ) );
                Animator anim = explosion.AddComponent<Animator>();
                anim.runtimeAnimatorController = EnemyExplosionAnim;
                explosion.AddComponent<SpriteRenderer>();
                explosion.transform.localScale = new Vector3(20, 20, 1);

                explosion.transform.parent = transform;
                mPool[count] = explosion;
				mInactive.Add(explosion);
                explosion.SetActive( false );
			}
		}
		else
		{
			Debug.LogError( "Only one ExplosionFactory allowed - destorying duplicate" );
			Destroy( this.gameObject );
		}
	}

	public static GameObject Dispatch( Vector3 position )
	{
		if( mInstance != null )
		{
			return mInstance.DoDispatch(position);
		}
		return null;
	}

	public static bool Return( GameObject explosion )
	{
		if( mInstance != null )
		{
			if( mInstance.mActive.Remove(explosion) )
			{
                explosion.SetActive( false );
				mInstance.mInactive.Add(explosion); 
			}
		}
		return false;
	}

	public static void Reset()
	{
		if( mInstance != null )
		{
			for( int count = 0; count < mInstance.mPool.Length; count++ )
			{
                mInstance.mPool[count].SetActive(false);
                mInstance.mInactive.Add(mInstance.mPool[count]); 
			}

			mInstance.mActive.Clear();
		}
	}

	private GameObject DoDispatch( Vector3 position)
	{
        // Look for a free explosion and then dispatch them 
        GameObject result = null;
		if( mInactive.Count > 0 )
		{
			GameObject explosion = mInactive[0];
            explosion.transform.position = position;
            if (DifficultyCurve.Levels[UserData.CurrentLevel].Length != 1)
                explosion.transform.localScale = new Vector3(20, 20, 1);
            else
                explosion.transform.localScale = new Vector3(60, 60, 1);
            explosion.SetActive(true);
			mActive.Add(explosion);
			mInactive.Remove(explosion);
			result = explosion;
            //explosion.GetComponent<Animator>().Play("Explosion");
            StartCoroutine(WaitForAnimation(explosion.GetComponent<Animator>()));
        }

        // Returns true if a free explosion was found and dispatched
        return result;
	}

    private IEnumerator WaitForAnimation(Animator anim)
    {
        while (!anim.IsInTransition(0))
        {
            yield return null;
        }
        Return(anim.gameObject);
        yield break;
    }
}
