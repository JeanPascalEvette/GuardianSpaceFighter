using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OverworldLogic : MonoBehaviour
{
    [SerializeField]
    private Camera GameplayCamera = null;
    private OverworldPlayerCharacter mPlayerCharacter;


    public static float GameDeltaTime { get; private set; }
    public static float ScreenBounds { get; private set; }
    public static float ScreenHeight { get; private set; }

    void Awake()
    {
        if (GameObject.Find("UserData(Clone)") == null)
            Instantiate(Resources.Load("Prefabs/UserData"), new Vector3(0, 0, 0), Quaternion.identity);

        float distance = transform.position.z - GameplayCamera.transform.position.z;
        ScreenHeight = CameraUtils.FrustumHeightAtDistance(distance, GameplayCamera.fieldOfView);
        ScreenBounds = ScreenHeight * GameplayCamera.aspect * 0.5f;
    }

    void Start()
    {
        mPlayerCharacter = transform.Find("Player").GetComponent<OverworldPlayerCharacter>();
        OverworldInput.OnTap += HandleOnTap;
        OverworldInput.OnSwipe += HandleOnSwipe;

    }

    void Update()
    {
        GameDeltaTime = Time.deltaTime;
        GameplayCamera.transform.position = mPlayerCharacter.transform.position + new Vector3(0,0,-150);
        GameObject.Find("Points").GetComponent<TextMesh>().text = string.Format("{0}", UserData.Points);



        GameObject.Find("TutorialMessage").transform.position = mPlayerCharacter.transform.position+ new Vector3(-GameObject.Find("TutorialMessage").GetComponent<MeshRenderer>().bounds.size.x/2, 12,0);


        //This tutorial message will display for a few second when reaching the overworld after beating the first level 
        var TutorialText = GameObject.Find("TutorialMessage");
        if (UserData.LevelUnlocked > 0 && UserData.GetIsTutorialOWReady())
        {
            UserData.EnableTutorialOW();
            TutorialText.GetComponent<TextMesh>().text = string.Format("Swipe Left/Right to change level.\nYou can repeat previous levels.\n\nIf you beat your best score, you\ncan earn some additional points\nto spend in the shop.");

        }
        else if (UserData.CountDownTutorialOW())
        {
            TutorialText.GetComponent<TextMesh>().text = string.Format("");
        }
    }
    
    void OnDestroy()
    {
        OverworldInput.OnTap -= HandleOnTap;
        OverworldInput.OnSwipe -= HandleOnSwipe;
    } 

    private void HandleOnTap(Vector3 position)
    {
        Vector3 worldPos = GameplayCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 150.0f));

        //Detect taps on Shop Button and on world
        if (GameObject.Find("ShopButton").GetComponent<BoxCollider2D>().bounds.Contains(worldPos))
            UnityEngine.SceneManagement.SceneManager.LoadScene("Shop");
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }


    private void HandleOnSwipe(OverworldInput.Direction direction)
    {
        //If swipe then go towards next/previous level
        if (direction == OverworldInput.Direction.Right && UserData.CurrentLevel < UserData.GetUnlockedLevel())
            mPlayerCharacter.Move(1);
        else if (direction == OverworldInput.Direction.Left && UserData.CurrentLevel != 0)
            mPlayerCharacter.Move(-1);
    }
    
    
    
}
