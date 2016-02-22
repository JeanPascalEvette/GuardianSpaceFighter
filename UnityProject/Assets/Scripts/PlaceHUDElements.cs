using UnityEngine;
using System.Collections;

//This class is used to place the differend HUD elements and make sure they move based on the screen size
public class PlaceHUDElements : MonoBehaviour {

    //SideLaserBar Coordinates
    private static float slcbXCoord;
    private static float healthXCoord;
    private Transform sideLaserCharge;
    // Use this for initialization
    void Start () {
        sideLaserCharge = transform.Find("SideLaserCharge");


        float minusScreenBounds = 0; //Overworld HUD
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Overworld")
        {
            slcbXCoord = -OverworldLogic.ScreenBounds + 1.5f;
            Transform shopButton = transform.Find("ShopButton").transform;
            shopButton.position = new Vector3(0.0f, shopButton.position.y, shopButton.position.z);
            minusScreenBounds = -OverworldLogic.ScreenBounds;
        } //Game's SideLaser & Health bars
        else if(sideLaserCharge != null)
        {
            slcbXCoord = -GameLogic.ScreenBounds + 1.0f;
            minusScreenBounds = -GameLogic.ScreenBounds;
            Transform slcbbg1 = sideLaserCharge.transform.Find("Background");
            Transform slcbbg2 = sideLaserCharge.transform.Find("Background2");
            Transform slct = sideLaserCharge.transform.Find("Text");
            slcbbg1.position = new Vector3(-GameLogic.ScreenBounds + 5.0f, slcbbg1.position.y, slcbbg1.position.z);
            slcbbg2.position = new Vector3(-GameLogic.ScreenBounds + 5.0f, slcbbg2.position.y, slcbbg2.position.z);
            slct.position = new Vector3(slcbXCoord + 0.2f, slct.position.y, slct.position.z);
            Transform healthBar = transform.Find("HPBar");
            healthXCoord = GameLogic.ScreenBounds - 9.05f;
            Transform hbbg1 = healthBar.transform.Find("Background");
            Transform hbbg2 = healthBar.transform.Find("Background2");
            Transform hbbgText = healthBar.transform.Find("Text");
            hbbg1.position = new Vector3(GameLogic.ScreenBounds - 5.0f, hbbg1.position.y, hbbg1.position.z);
            hbbg2.position = new Vector3(GameLogic.ScreenBounds - 5.0f, hbbg2.position.y, hbbg2.position.z);
            hbbgText.position = new Vector3(healthXCoord + 4.2f, hbbgText.position.y, hbbgText.position.z);
        }

        //Point display
        Transform points = transform.Find("Points");
        Transform pointsIcon = transform.Find("Points").Find("Money Icon");
        points.position = new Vector3(minusScreenBounds + 4.0f, points.position.y, points.position.z);
        pointsIcon.position = new Vector3(minusScreenBounds + 3.0f, pointsIcon.position.y, pointsIcon.position.z);
        Transform pb = transform.Find("BestScore");
        if(pb != null) //Game's Best Score Display
        { 
            Transform pbIcon = transform.Find("BestScore").Find("Money Icon");
            pb.position = new Vector3(-minusScreenBounds - 6.0f, pb.position.y, pb.position.z);
            if (pbIcon == null || UserData.LevelMaxPoints == null) return;
            pbIcon.position = new Vector3(-minusScreenBounds - 4.5f - (0.47f * UserData.LevelMaxPoints[UserData.CurrentLevel].ToString().Length), pb.position.y - 3.3f, pbIcon.position.z);
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (sideLaserCharge != null)
        {
            if (UserData.GetSideLaser() != 0) //Updating the SideLaser Progression if it has been enabled.
            {
                sideLaserCharge.gameObject.SetActive(true);
            }
            else
                sideLaserCharge.gameObject.SetActive(false);
        }
    }

    //This function modifies the scale and position of the side laser bar to simulate it growing as the cooldwon reduces
    public static void UpdateSideLaserBar(float LaserCD, float LaserLastUse)
    {
        float barLength = 0.5f;
        Transform slcb = GameObject.Find("HUD").transform.Find("SideLaserCharge").transform.Find("Bar");
        float percentage = Mathf.Min(100.0f - ((LaserCD - LaserLastUse) / LaserCD) * 100.0f, 100.0f);
        slcb.localScale = new Vector3(percentage * barLength, slcb.localScale.y, slcb.localScale.z);
        slcb.position = new Vector3(slcbXCoord + percentage / ((1 / barLength) * 12.5f), slcb.position.y, slcb.position.z);

    }

    //the function modifies the scale and position of the HP bar to reflect the current status of the player's HP
    public static void UpdateHPBar(float currentHP, float maxHP)
    {
        float barLength = 0.502f;
        Transform hpb = GameObject.Find("HUD").transform.Find("HPBar").transform.Find("Bar");
        float percentage = Mathf.Max(Mathf.Min(((currentHP) / maxHP) * 100.0f, 100.0f), 0.0f);
        hpb.localScale = new Vector3(percentage * barLength, hpb.localScale.y, hpb.localScale.z);
        hpb.position = new Vector3(healthXCoord + percentage / ((1 / barLength) * 12.5f), hpb.position.y, hpb.position.z);

    }
}
