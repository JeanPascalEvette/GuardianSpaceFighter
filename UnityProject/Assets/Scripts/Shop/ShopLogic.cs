using UnityEngine;
using System.Collections;


public class ShopLogic : MonoBehaviour {

    Purchaser mPurchaser = null;

    void Awake()
    {
        //This should only be true when running game from Unity as the game usually loads from the Overworld.
        if (GameObject.Find("UserData(Clone)") == null)
            Instantiate(Resources.Load("Prefabs/UserData"), new Vector3(0, 0, 0), Quaternion.identity);
        ShopInput.OnTap += HandleOnTap;
    }
	// Use this for initialization
	void Start () {
        mPurchaser = new Purchaser();

    }
	
	// Update is called once per frame
	void Update ()
    {
        //Update Money Text Meshes
        GameObject realMoney = GameObject.Find("RealMoney");
        if (realMoney != null)
        {
            realMoney.GetComponent<TextMesh>().text = UserData.RealMoneyPoints.ToString();
        }
        GameObject pointMoney = GameObject.Find("Points");
        if (pointMoney != null)
        {
            pointMoney.GetComponent<TextMesh>().text = UserData.Points.ToString();
        }

        //Update Upgrades levels
        GameObject mspu = GameObject.Find("MultishotPowerup");
        if (mspu != null)
        {
            string multishotPrice = UserData.GetMultishot() == 4 ? "MAX" : ((UserData.GetMultishot() + 1) * 5000).ToString();
            mspu.transform.Find("BuyUsingPoints").transform.Find("Text").GetComponent<TextMesh>().text = multishotPrice;
            if (multishotPrice == "MAX")
                mspu.SetActive(false);
        }
        GameObject frpu = GameObject.Find("FireRatePowerup");
        if (frpu != null)
        {
            string fireRatePrice = UserData.GetFireRate() == 4 ? "MAX" : ((UserData.GetFireRate() + 1) * 5000).ToString();
            frpu.transform.Find("BuyUsingPoints").transform.Find("Text").GetComponent<TextMesh>().text = fireRatePrice;
            if (fireRatePrice == "MAX")
                frpu.SetActive(false);
        }
        GameObject sspu = GameObject.Find("ShotSpeedPowerup");
        if (sspu != null)
        {
            string shotSpeedPrice = UserData.GetBulletSpeed() == 4 ? "MAX" : ((UserData.GetBulletSpeed() + 1) * 5000).ToString();
            sspu.transform.Find("BuyUsingPoints").transform.Find("Text").GetComponent<TextMesh>().text = shotSpeedPrice;
            if (shotSpeedPrice == "MAX")
                sspu.SetActive(false);
        }
        GameObject slpu = GameObject.Find("SideLasersPowerup");
        if (slpu != null)
        {
            string sideLaserPrice = UserData.GetSideLaser() == 4 ? "MAX" : ((UserData.GetSideLaser() + 1) * 5000).ToString();
            slpu.transform.Find("BuyUsingPoints").transform.Find("Text").GetComponent<TextMesh>().text = sideLaserPrice;
            if (sideLaserPrice == "MAX")
                slpu.SetActive(false);
        }
    }


    void OnDestroy()
    {
        ShopInput.OnTap -= HandleOnTap;
    }

    //This function detects clicks on the different buy options of the shop and calls the appropriate function.
    private void HandleOnTap(Vector3 position)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z) + new Vector3(0, 0, -15.0f));
        Vector3 pointHit = worldPos;

         
        if (isInside(GameObject.Find("ExitButton").GetComponent<BoxCollider2D>(), pointHit))
            UnityEngine.SceneManagement.SceneManager.LoadScene("Overworld");
        else if (GameObject.Find("MultishotPowerup") != null && isInside(GameObject.Find("MultishotPowerup").transform.Find("BuyUsingPoints").GetComponent<BoxCollider2D>(), pointHit))
        {
            if (UserData.TrySpendPoints((UserData.GetMultishot() + 1) * 5000))
                UserData.IncreaseMultishot();
        }
        else if (GameObject.Find("FireRatePowerup") != null && isInside(GameObject.Find("FireRatePowerup").transform.Find("BuyUsingPoints").GetComponent<BoxCollider2D>(), pointHit))
        {
            if (UserData.TrySpendPoints((UserData.GetFireRate() + 1) * 5000))
                UserData.IncreaseFireRate();
        }
        else if (GameObject.Find("ShotSpeedPowerup") != null && isInside(GameObject.Find("ShotSpeedPowerup").transform.Find("BuyUsingPoints").GetComponent<BoxCollider2D>(), pointHit))
        {
            if (UserData.TrySpendPoints((UserData.GetBulletSpeed() + 1) * 5000))
                UserData.IncreaseBulletSpeed();
        }
        else if (GameObject.Find("SideLasersPowerup") != null && isInside(GameObject.Find("SideLasersPowerup").transform.Find("BuyUsingPoints").GetComponent<BoxCollider2D>(), pointHit))
        {
            if (UserData.TrySpendPoints((UserData.GetSideLaser() + 1) * 5000))
                UserData.IncreaseSideLaser();

        }
        else if (GameObject.Find("MultishotPowerup") != null && isInside(GameObject.Find("MultishotPowerup").transform.Find("BuyUsingMoney").GetComponent<BoxCollider2D>(), pointHit))
        {
            if (UserData.TrySpendRealMoney(2))
                UserData.IncreaseMultishot();
            else
                mPurchaser.BuyCurrency();
        }
        else if (GameObject.Find("FireRatePowerup") != null && isInside(GameObject.Find("FireRatePowerup").transform.Find("BuyUsingMoney").GetComponent<BoxCollider2D>(), pointHit))
        {
            if (UserData.TrySpendRealMoney(2))
                UserData.IncreaseFireRate();
            else
                mPurchaser.BuyCurrency();
        }
        else if (GameObject.Find("ShotSpeedPowerup") != null && isInside(GameObject.Find("ShotSpeedPowerup").transform.Find("BuyUsingMoney").GetComponent<BoxCollider2D>(), pointHit))
        {
            if (UserData.TrySpendRealMoney(2))
                UserData.IncreaseBulletSpeed();
            else
                mPurchaser.BuyCurrency();
        }
        else if (GameObject.Find("SideLasersPowerup") != null && isInside(GameObject.Find("SideLasersPowerup").transform.Find("BuyUsingMoney").GetComponent<BoxCollider2D>(), pointHit))
        {
            if (UserData.TrySpendRealMoney(2))
                UserData.IncreaseSideLaser();
            else
                mPurchaser.BuyCurrency();
        }
    }

    //This function is used to calculate if a point is inside of a Collider (Axis-Aligned)
    private bool isInside(BoxCollider2D col, Vector3 point)
    {
        Vector3 min = col.bounds.min;
        Vector3 max = col.bounds.max;

        return point.x < max.x && point.x > min.x && point.y < max.y && point.y > min.y;
    }
}
