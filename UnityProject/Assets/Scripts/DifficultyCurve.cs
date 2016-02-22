using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class DifficultyCurve : MonoBehaviour 
{
	[SerializeField] private float GameStartSpeed = 10f;
	[SerializeField] private float GameSpeedRamp = 0.1f;
	[SerializeField] private float PlayerStartSpeed = 20f;
    [SerializeField] private float PlayerSpeedRamp = 0.1f;
    [SerializeField] private bool  PlayerInstantMoves = true;
    [SerializeField] private float BulletStartSpeed = 20f;
	[SerializeField] private float BulletSpeedRamp = 0.1f;
	[SerializeField] private float TimeBetweenRows = 5.0f;
	[SerializeField] private float TimeBetweenWaves = 40.0f;

    private List<EnemyWave[]> levels;
	private float mTimeToNextRow;
	private float mTimeToNextWave;
    private int mCurrentRow;
	private int mCurrentWave;

	public static float GameSpeed { get; private set; }
	public static float PlayerSpeed { get; private set; }
    public static float BulletSpeed { get; private set; }
    public static List<EnemyWave[]> Levels { get; private set; }

    void Awake()
    {

        //Generating levels
        //Levels with a single wave containing a single enemy are bosses.
        levels = new List<EnemyWave[]>();
        levels.Add(new EnemyWave[] {
            new EnemyWave( new string[]{ "0" } ),
            new EnemyWave( new string[]{ "010" }),
            new EnemyWave( new string[]{ "00", "00" }),
            new EnemyWave( new string[]{ "010", "010" }),
            new EnemyWave( new string[]{ "101", "010" } )
        }); 
        levels.Add(new EnemyWave[] {
            new EnemyWave( new string[]{ "111" } ),
            new EnemyWave( new string[]{ "101", "101" } ),
            new EnemyWave( new string[]{ "111", "111" } ),
            new EnemyWave( new string[]{ "111", "121", "111" } ),
            new EnemyWave( new string[]{ "111", "212", "222" } )
        });
        levels.Add(new EnemyWave[] {
            new EnemyWave( new string[]{ "0" } ) 
        });
        levels.Add(new EnemyWave[] {
            new EnemyWave( new string[]{ "020" } ),
            new EnemyWave( new string[]{ "121", "121" } ),
            new EnemyWave( new string[]{ "111", "121" } ),
            new EnemyWave( new string[]{ "12", "121" } ),
            new EnemyWave( new string[]{ "212", "22" } ),
            new EnemyWave( new string[]{ "121", "131", "131" } )
        });
        levels.Add(new EnemyWave[] {
            new EnemyWave( new string[]{ "222" } ),
            new EnemyWave( new string[]{ "212", "22" } ),
            new EnemyWave( new string[]{ "22", "232" } ),
            new EnemyWave( new string[]{ "232", "33" } ),
            new EnemyWave( new string[]{ "233", "33", "32", "22" } )
        });
        levels.Add(new EnemyWave[] {
            new EnemyWave( new string[]{ "1" } )
        });
        levels.Add(new EnemyWave[] {
            new EnemyWave( new string[]{ "333", "333" } ),
            new EnemyWave( new string[]{ "343", "343" } ),
            new EnemyWave( new string[]{ "44", "34", "333" } ),
            new EnemyWave( new string[]{ "121", "012", "11", "21" } ),
            new EnemyWave( new string[]{ "434", "333", "434" } )
        });
        levels.Add(new EnemyWave[] {
            new EnemyWave( new string[]{ "434" } ),
            new EnemyWave( new string[]{ "44", "4" } ),
            new EnemyWave( new string[]{ "444", "44" } ),
            new EnemyWave( new string[]{ "44", "44", "434" } ),
            new EnemyWave( new string[]{ "22", "33", "44", "33", "22", "44" } ),
            new EnemyWave( new string[]{ "44", "44", "44", "44", "44" } )
        });
        levels.Add(new EnemyWave[] {
            new EnemyWave( new string[]{ "2" } )
        });
        Levels = levels;
    }

    void Start()
	{
		Reset();


        GameSpeed = GameStartSpeed;
        PlayerSpeed = PlayerStartSpeed;
        //If InstantMoves is enabled, the player is always at the cursor's position & the PlayerSpeed is irrelevant. Else it's always trying to get there based on the speed. 
        if (PlayerInstantMoves)
            PlayerSpeed = -1;
        BulletSpeed = BulletStartSpeed;
    }
    

	public string SpawnCount()
	{
        //If this is a boss level - different cycle to make sure the boss is killed before going to next level
        if(Levels[UserData.CurrentLevel].Length == 1 && mCurrentRow > 0)
        {
            return "-2";
        }

		string enemiesToSpawn = "";
        if(mCurrentWave == -1) // If mCurrentWave is set to 1 - it means that the level is cleared and the player will go back to the overworld when the next wave should spawn
        {
            mTimeToNextWave -= GameLogic.GameDeltaTime;
            if (mTimeToNextWave > 0.0f)
                return enemiesToSpawn;
            FinishLevel();
        }
		else if(mCurrentRow <  Levels[UserData.CurrentLevel][mCurrentWave].NumberOfRows ) // If there are still some rows in this wave, return the current one.
		{
			mTimeToNextRow -= GameLogic.GameDeltaTime;
			if( mTimeToNextRow <= 0.0f )
			{
				mCurrentRow++;
				enemiesToSpawn = Levels[UserData.CurrentLevel][mCurrentWave].GetRow(mCurrentRow - 1);
				mTimeToNextRow = TimeBetweenRows;
			}
		}
		else // If you are at the last row of this wave, then go to next wave
		{
			mTimeToNextWave -= GameLogic.GameDeltaTime;
			if( mTimeToNextWave <= 0.0f )
			{
				if( ( mCurrentWave + 1 ) < Levels[UserData.CurrentLevel].Length )
				{
					GameSpeed += GameSpeedRamp; //Increase difficulty slightly at each wave
                    if (!PlayerInstantMoves)
                        PlayerSpeed += PlayerSpeedRamp;
					BulletSpeed += BulletSpeedRamp;
					mCurrentWave++;
				}
                else // If you have reached the last wave, wait for the normal time between waves and then go to overworld.
                {
                    mCurrentWave = -1;
                }
				mTimeToNextWave = TimeBetweenWaves;
				mCurrentRow = 0;
			}
		}

		return enemiesToSpawn;
	}

    //This function is going to finish the level. The optional parameter allows to set a delay before going to the overworld (used in boss levels to leave more time to collect rewards)
    public void FinishLevel(float time = 0.0f)
    {
        StartCoroutine(WaitThenFinishLevel(time));
    }

    //This function unlocks the next level if available, converts the level points into normal points if you have beaten your best score (inside UserData) and then loads the Overworld
    IEnumerator WaitThenFinishLevel(float time)
    {
        yield return new WaitForSeconds(time);
        if (UserData.GetUnlockedLevel() == UserData.CurrentLevel)
            UserData.unlockNextLevel();
        UserData.ConvertLevelpoints();

        if (UserData.CurrentLevel == Levels.Count - 1)
            UnityEngine.SceneManagement.SceneManager.LoadScene("EndGame");
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene("Overworld");
    }

	public void Stop()
	{
		GameSpeed = 0.0f;
		PlayerSpeed = 0.0f;
		BulletSpeed = 0.0f;
	}

	public void Reset()
	{
		GameSpeed = GameStartSpeed;
		PlayerSpeed = PlayerStartSpeed;
		BulletSpeed = BulletStartSpeed;
		mTimeToNextRow = TimeBetweenRows;
		mTimeToNextWave = TimeBetweenWaves;
		mCurrentRow = 0;
		mCurrentWave = 0;
	}
}
