using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//This Class is used to store the player's data (i.e.: Money, Upgrade levels, level unlocked, etc..) It is designed to reload the data 
public class UserData : MonoBehaviour {

    public static int CurrentLevel { get; private set; }
    public static int LevelUnlocked { get; private set; }
    public static int MultishotLevel { get; private set; }
    public static int BulletSpeedLevel { get; private set; }
    public static int FireRateLevel { get; private set; }
    public static int SideLaserLevel { get; private set; }
    public static int Points { get; private set; }
    public static int LevelPoints { get; private set; }
    public static int RealMoneyPoints { get; private set; }
    public static int RealMoneyPurchases { get; private set; }
    public static List<int> LevelMaxPoints { get; private set; }

    public static float TutorialMovementControl { get; private set; }
    public static float TutorialOverworld { get; private set; }
    public static float TutorialSideLaser { get; private set; }

    void Awake()
    {
        DontDestroyOnLoad(this);
        if (LevelMaxPoints == null)
        {
            LevelMaxPoints = new List<int>();
            for (int i = 0; i < 99; i++)
                LevelMaxPoints.Add(0); 
            TutorialMovementControl = 0.0f;
            TutorialOverworld = 0.0f;
            TutorialSideLaser = 0.0f;
        }
    }
    void OnApplicationQuit()
    {
        SaveData();
    }
        void OnApplicationFocus(bool focusStatus)
    {
        if (focusStatus == false) 
            SaveData();
    }

    void SaveData()
    {
        PlayerPrefs.SetInt("CurrentLevel", CurrentLevel);
        PlayerPrefs.SetInt("LevelUnlocked", LevelUnlocked);
        PlayerPrefs.SetInt("MultishotLevel", MultishotLevel);
        PlayerPrefs.SetInt("BulletSpeedLevel", BulletSpeedLevel);
        PlayerPrefs.SetInt("FireRateLevel", FireRateLevel);
        PlayerPrefs.SetInt("SideLaserLevel", SideLaserLevel);
        PlayerPrefs.SetInt("RealMoneyPoints", RealMoneyPoints);
        PlayerPrefs.SetInt("RealMoneyPurchases", RealMoneyPurchases);
        PlayerPrefs.SetInt("Points", Points);
        PlayerPrefs.SetFloat("TutorialMovementControl", TutorialMovementControl);
        PlayerPrefs.SetFloat("TutorialOverworld", TutorialOverworld);
        PlayerPrefs.SetFloat("TutorialSideLaser", TutorialSideLaser);

        for (int i = 0; i < LevelMaxPoints.Count; i++)
            PlayerPrefs.SetInt("LevelMaxPoints" + i, LevelMaxPoints[i]);

    }

    // Use this for initialization
    void Start () {
        if (LevelMaxPoints == null || LevelMaxPoints.Count == 99)
        {
            LevelMaxPoints = new List<int>();
            for (int i = 0; DifficultyCurve.Levels!= null && i < DifficultyCurve.Levels.Count; i++)
                LevelMaxPoints.Add(PlayerPrefs.GetInt("LevelMaxPoints"+i));

            CurrentLevel = PlayerPrefs.GetInt("CurrentLevel");
            LevelUnlocked = PlayerPrefs.GetInt("LevelUnlocked");
            MultishotLevel =  PlayerPrefs.GetInt("MultishotLevel");
            BulletSpeedLevel = PlayerPrefs.GetInt("BulletSpeedLevel");
            FireRateLevel = PlayerPrefs.GetInt("FireRateLevel");
            SideLaserLevel = PlayerPrefs.GetInt("SideLaserLevel");
            RealMoneyPoints = PlayerPrefs.GetInt("RealMoneyPoints");
            RealMoneyPurchases = PlayerPrefs.GetInt("RealMoneyPurchases");
            Points = PlayerPrefs.GetInt("Points");
            TutorialMovementControl = PlayerPrefs.GetFloat("TutorialMovementControl");
            TutorialOverworld = PlayerPrefs.GetFloat("TutorialOverworld");
            TutorialSideLaser = PlayerPrefs.GetFloat("TutorialSideLaser");
        }
	}
	
    public static void Clear()
    {
        LevelMaxPoints = new List<int>();
        for (int i = 0; i < DifficultyCurve.Levels.Count; i++)
            LevelMaxPoints.Add(0);

        CurrentLevel = 0;
        LevelUnlocked = 0;
        MultishotLevel = 0;
        BulletSpeedLevel = 0;
        FireRateLevel = 0;
        SideLaserLevel = 0;
        TutorialMovementControl = 0.0f;
        TutorialOverworld = 0.0f;
        TutorialSideLaser = 0.0f;
        Points = 0;
        RealMoneyPoints = 5 * RealMoneyPurchases;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Overworld");
    }
	
	// Update is called once per frame
	void Update () {
        
}

    public static bool GetIsTutorialSLReady()
    {
        return TutorialSideLaser == 0.0f;
    }
    public static void EnableTutorialSideLaser()
    {
        TutorialSideLaser = 5.0f;
    }
    public static bool CountDownTutorialSideLaser()
    {
        if (TutorialSideLaser > 0)
        {
            TutorialSideLaser -= GameLogic.GameDeltaTime;
            return false;
        }
        return true;
    }

    public static bool GetIsTutorialReady()
    {
        return TutorialMovementControl == 0.0f;
    }
    public static void EnableTutorialMovementControl()
    {
        TutorialMovementControl = 5.0f;
    }
    public static bool CountDownTutorialMovementControl()
    {
        if (TutorialMovementControl > 0)
        {
            TutorialMovementControl -= GameLogic.GameDeltaTime;
            return false;
        }
        return true;
    }

    public static bool GetIsTutorialOWReady()
    {
        return TutorialOverworld == 0.0f;
    }
    public static void EnableTutorialOW()
    {
        TutorialOverworld = 5.0f;
    }
    public static bool CountDownTutorialOW()
    {
        if (TutorialOverworld > 0)
        {
            TutorialOverworld -= OverworldLogic.GameDeltaTime;
            return false;
        }
        return true;
    }

    public static void AddBonusPoints(int pts)
    {
        LevelPoints += pts;
    }

    public static void unlockNextLevel()
    {
        if (LevelUnlocked + 1 < DifficultyCurve.Levels.Count)
            LevelUnlocked++;
        else // Possibly trigger IWIN state here?
            return;
    }

    public static void ConvertLevelpoints()
    {
        if (LevelMaxPoints[CurrentLevel] < LevelPoints)
        {
            int ptsDiff = LevelPoints - LevelMaxPoints[CurrentLevel];
            LevelMaxPoints[CurrentLevel] = LevelPoints;
            Points += ptsDiff;
            LevelPoints = 0;
        }
    }

    public static void IncreaseMultishot()
    {
        if (MultishotLevel < 4)
            MultishotLevel++;
    }

    public static void IncreaseBulletSpeed()
    {
        if (BulletSpeedLevel < 4)
            BulletSpeedLevel++;
    }

    public static void IncreaseSideLaser()
    {
        if (SideLaserLevel < 4)
            SideLaserLevel++;
    }

    public static void IncreaseFireRate()
    {
        if (FireRateLevel < 4)
            FireRateLevel++;
    }

    public static void ResetLevelPoints()
    {
        LevelPoints = 0;
    }

    public static void IncreaseLevelPoints(int pts)
    {
        LevelPoints += pts;
    }

    public static void IncreaseCurrentLevel(int diff)
    {
        CurrentLevel += diff;
    }

    public static void SetCurrentLevel(int lvl)
    {
        CurrentLevel = lvl;
    }

    public static void SetLevelUnlocked(int lvl)
    {
        LevelUnlocked = lvl;
    }
    public static void BuyRealMoneyPoints()
    {
        RealMoneyPoints += 5;
        RealMoneyPurchases++;
    }

    public static void SetBulletSpeedLevel(int lvl)
    {
        BulletSpeedLevel = lvl;
    }
    public static void SetFireRate(int lvl)
    {
        FireRateLevel = lvl;
    }
    public static void SetMultishotLevel(int lvl)
    {
        MultishotLevel= lvl;
    }
    public static void SetSideLaserLevel(int lvl)
    {
        SideLaserLevel = lvl;
    }
    public static void SetPoints(int pts)
    {
        Points = pts;
    }
    public static void SetRealMoneyPoints(int pts)
    {
        RealMoneyPoints = pts;
    }
    public static void SetRealMoneyPurchases(int purchases)
    {
        RealMoneyPurchases = purchases;
    }
    public static void SetLevelPoints(int pts)
    {
        LevelPoints = pts;
    }

    public static int GetUnlockedLevel()
    {
        return LevelUnlocked;
    }

    public static int GetMultishot()
    {
        return MultishotLevel;
    }

    public static int GetBulletSpeed()
    {
        return BulletSpeedLevel;
    }

    public static int GetSideLaser()
    {
        return SideLaserLevel;
    }

    public static int GetFireRate()
    {
        return FireRateLevel;
    }

    public static bool TrySpendPoints(int price)
    {
        if (Points - price < 0) return false;

        Points -= price;
        return true;
    }

    public static bool TrySpendRealMoney(int price)
    {
        if (RealMoneyPoints - price < 0) return false;

        RealMoneyPoints -= price;
        return true;
    }
}
