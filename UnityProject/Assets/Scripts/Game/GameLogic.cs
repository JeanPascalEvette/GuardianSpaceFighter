using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameLogic : MonoBehaviour
{
    [SerializeField] private TextMesh GameText = null;
    [SerializeField] private Camera GameplayCamera = null;
    [SerializeField]
    public float PlayerKillDistance = 10.0f;
    [SerializeField] public float BulletKillDistance = 10.0f;
    [SerializeField]
    private int pickupChance = 100;
    [SerializeField] private float WaitTime = 1.0f;
    //[SerializeField] private int MaxMissedEnemies = 3; 

    private enum State { TapToStart, Game, GameOver };

    private List<GameObject> mActiveEnemies;
    private DifficultyCurve mCurrentDifficulty;
    private PlayerCharacter mPlayerCharacter;
    private PowerUpFactory mPowerUpFactory;
    private float mGameOverTime;
    //private float mDistanceTravelled;
    private int mMissedEnemies;
    private State mGameStatus;

    public static float GameDeltaTime { get; private set; }
    public static float GameSpeed { get { return DifficultyCurve.GameSpeed; } }
    public static float PlayerSpeed { get { return DifficultyCurve.PlayerSpeed; } }
    public static float BulletSpeed { get { return DifficultyCurve.BulletSpeed; } }
    public static float ScreenBounds { get; private set; }
    public static float ScreenHeight { get; private set; }
    public static bool Paused { get; private set; }

    void Awake()
    {
        //This should only be true when running game from Unity as the game usually loads from the Overworld.
        if (GameObject.Find("UserData(Clone)") == null)
            Instantiate(Resources.Load("Prefabs/UserData"), new Vector3(0, 0, 0), Quaternion.identity);
        float distance = transform.position.z - GameplayCamera.transform.position.z;
        ScreenHeight = CameraUtils.FrustumHeightAtDistance(distance, GameplayCamera.fieldOfView);
        ScreenBounds = ScreenHeight * GameplayCamera.aspect * 0.5f;

        GameInput.OnTap += HandleOnTap;
        GameInput.OnSwipe += HandleOnSwipe;
        GameInput.OnTilt += HandleOnTilt;
        mActiveEnemies = new List<GameObject>();
        mCurrentDifficulty = GetComponentInChildren<DifficultyCurve>();
        mPlayerCharacter = GetComponentInChildren<PlayerCharacter>();
        mGameStatus = State.TapToStart;
        mGameOverTime = Time.timeSinceLevelLoad;
        mMissedEnemies = 0;
        Paused = false;
        UserData.ResetLevelPoints();
    }

    void Start()
    {
        mPowerUpFactory = GameObject.Find("PowerUpFactory").GetComponent<PowerUpFactory>();
        mPlayerCharacter.ResetHP();
    }

    void OnDestroy()
    {
        GameInput.OnTap -= HandleOnTap;
        GameInput.OnSwipe -= HandleOnSwipe;
        GameInput.OnTilt -= HandleOnTilt;
    }

	void Update()
	{
        if(UserData.LevelMaxPoints != null && UserData.LevelMaxPoints.Count > UserData.CurrentLevel)
        GameObject.Find("BestScore").GetComponent<TextMesh>().text = string.Format("Previous\nBest :\n{0}", UserData.LevelMaxPoints[UserData.CurrentLevel]);

        GameDeltaTime = Paused ? 0.0f : Time.deltaTime;

		if( mGameStatus == State.Game )
        {
            GameText.transform.Find("Points").GetComponent<TextMesh>().text = string.Format("{0}", UserData.LevelPoints);

            if (UserData.GetIsTutorialReady()) // Display some tutorials for a few sec depending on state of the game
            {
                UserData.EnableTutorialMovementControl();
                GameText.GetComponent<TextMesh>().text = string.Format("Control your ship by\ndragging it and try to earn as many\npoints as possible!");

            }
            else if (UserData.GetSideLaser() > 0 && UserData.GetIsTutorialSLReady())
            {
                UserData.EnableTutorialSideLaser();
                GameText.GetComponent<TextMesh>().text = string.Format("When your laser is charged,\ntilt your device or\npress Q/E to use it!");

            }
            else if (UserData.CountDownTutorialMovementControl() && UserData.CountDownTutorialSideLaser())//If tutorial has been displayed long enough
            {
                GameText.GetComponent<TextMesh>().text = string.Format("");
            }

            string enemies = mCurrentDifficulty.SpawnCount();
            int iEnemies;
            if (int.TryParse(enemies, out iEnemies))
            {
                if (enemies == "-2") // Boss level - boss already spawned
                {
                    if (mActiveEnemies.Count == 0) //Boss Defeated !
                            mCurrentDifficulty.FinishLevel(8.0f);
                }
                else if (DifficultyCurve.Levels[UserData.CurrentLevel].Length == 1) // Boss level
                {
                    mActiveEnemies.Add(EnemyFactory.Dispatch(int.Parse(enemies), EnemyFactory.Column.Two, true));
                }
                else if (enemies.Length == 1)
                {
                    mActiveEnemies.Add(EnemyFactory.Dispatch(int.Parse(enemies), (EnemyFactory.Column)Random.Range(0, 3)));
                }
                else if (enemies.Length == 2)
                {
                    int config = Random.Range(0, 3);
                    if (config == 0)
                    {
                        mActiveEnemies.Add(EnemyFactory.Dispatch(int.Parse(enemies[0].ToString()), EnemyFactory.Column.One));
                        mActiveEnemies.Add(EnemyFactory.Dispatch(int.Parse(enemies[1].ToString()), EnemyFactory.Column.Two));
                    }
                    else if (config == 1)
                    {
                        mActiveEnemies.Add(EnemyFactory.Dispatch(int.Parse(enemies[0].ToString()), EnemyFactory.Column.One));
                        mActiveEnemies.Add(EnemyFactory.Dispatch(int.Parse(enemies[1].ToString()), EnemyFactory.Column.Three));
                    }
                    else
                    {
                        mActiveEnemies.Add(EnemyFactory.Dispatch(int.Parse(enemies[0].ToString()), EnemyFactory.Column.Two));
                        mActiveEnemies.Add(EnemyFactory.Dispatch(int.Parse(enemies[1].ToString()), EnemyFactory.Column.Three));
                    }
                }
                else if (enemies.Length == 3)
                {
                    mActiveEnemies.Add(EnemyFactory.Dispatch(int.Parse(enemies.Substring(0, 1)), EnemyFactory.Column.One));
                    mActiveEnemies.Add(EnemyFactory.Dispatch(int.Parse(enemies.Substring(1, 1)), EnemyFactory.Column.Two));
                    mActiveEnemies.Add(EnemyFactory.Dispatch(int.Parse(enemies.Substring(2, 1)), EnemyFactory.Column.Three));
                }
            }
			// Update the position of each active enemy, keep a track of enemies which have gone off screen 
			List<GameObject> oldEnemys = new List<GameObject>(); 
			for( int count = 0; count < mActiveEnemies.Count; count++ )
			{
				Vector3 position = mActiveEnemies[count].transform.position;

                if(DifficultyCurve.Levels[UserData.CurrentLevel].Length == 1 && position.y <= ScreenHeight - 25.0f) // If boss, then force him to strafe back and forth after entering the screen
                {
                    position = mActiveEnemies[count].GetComponent<EnemyBehaviour>().StrafeBackAndForth();
                }
                else//If not boss then just go straight down (Could have some more logic here to implement different movement patterns)
				    position.y -= GameDeltaTime * GameSpeed;
				mActiveEnemies[count].transform.position = position;
                mActiveEnemies[count].GetComponent<EnemyBehaviour>().DoAction();
				if( position.y < ScreenHeight * -0.5f )
				{
					EnemyFactory.Return( mActiveEnemies[count] );
					oldEnemys.Add( mActiveEnemies[count] ); 
					mMissedEnemies++;
				}
			}


                    mPowerUpFactory.MovePowerups(GameDeltaTime * GameSpeed);

            /* Mechanic no longer in use - kept here if needed
			if( mMissedEnemies >= MaxMissedEnemies )
			{
                // Too many missed enemies - Game over
				mCurrentDifficulty.Stop();
                mGameOverTime = Time.timeSinceLevelLoad;
                mGameStatus = State.GameOver;
				GameText.text = string.Format( "You Been Invaded!\nTotal Distance: {0:0.0} m", mDistanceTravelled );
			}
            */

			for( int count = 0; count < oldEnemys.Count; count++ )
			{
				mActiveEnemies.Remove( oldEnemys[count] );
			}
		}
	}

    //This function is called to reduce player HP and handle death
    public void DamagePlayer(int dmg)
    {
        if (mPlayerCharacter.InflictDamage(dmg))
        {
            mCurrentDifficulty.Stop();
            mGameOverTime = Time.timeSinceLevelLoad;
            mGameStatus = State.GameOver;
            GameText.text = string.Format("You Died!");
        }
    }

    public void DestroyEnemy(GameObject enemy)
    {
        if (DifficultyCurve.Levels[UserData.CurrentLevel].Length != 1)
            SpawnPowerUp(enemy.transform.position);
        else // If the enemy is a boss, then spawn pickup with a worth equal to the # of the level (i.e : Boss 3 drops is worth 9 powerups)
        {
            PowerUpFactory.Dispatch(enemy.transform.position);
        }
        UserData.IncreaseLevelPoints(enemy.GetComponent<EnemyBehaviour>().GetPointsValue());
        ExplosionFactory.Dispatch(enemy.transform.position);
        EnemyFactory.Return(enemy);
        mActiveEnemies.Remove(enemy);
        mActiveEnemies.TrimExcess();
    }

    public void SpawnPowerUp(Vector3 position)
    {
        int roll = (int)(Random.value * 100);
        if(roll < pickupChance)
        {
            PowerUpFactory.Dispatch(position);
        }

    }

	private void Reset()
	{
		mPlayerCharacter.Reset();
		mCurrentDifficulty.Reset();
		EnemyFactory.Reset();
		mActiveEnemies.Clear();
		mMissedEnemies = 0;
		//mDistanceTravelled = 0.0f;
	}

	private void HandleOnTap( Vector3 position )
	{
		switch( mGameStatus )
		{ 
		case State.TapToStart:
			Paused = false;
			mGameStatus = State.Game;
			break;
		case State.Game:
			mPlayerCharacter.Fire();
			break;
		case State.GameOver:
            if (Time.timeSinceLevelLoad - mGameOverTime > WaitTime)
            {
                    UnityEngine.SceneManagement.SceneManager.LoadScene("Overworld");
			}
            break;
		}
	}


    private void HandleOnSwipe(Vector3 mouseLocation)
    {
        if (mGameStatus == State.Game)
        {
            Vector3 pos = GameplayCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 150.0f));
            mPlayerCharacter.Move(pos);
            mPlayerCharacter.Fire();
        }
    }

    private void HandleOnTilt(bool isLeft)
    {
        if (mGameStatus == State.Game)
        {
            mPlayerCharacter.FireSideLaser(isLeft);
        }
    }

    public bool isGameStatusGame()
    {
        return mGameStatus == State.Game;
    }
}
