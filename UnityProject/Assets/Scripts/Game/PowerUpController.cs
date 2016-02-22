using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//This class is used to apply the effects of the powerups
public class PowerUpController : MonoBehaviour
{
    public PowerUpFactory.PowerUpType myType;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            switch (myType)
            {
                case PowerUpFactory.PowerUpType.MONEY:
                    if(DifficultyCurve.Levels[UserData.CurrentLevel].Length == 1)
                        UserData.AddBonusPoints((UserData.CurrentLevel+1) *1000);
                    else
                        UserData.AddBonusPoints(1000);
                    PowerUpFactory.Return(this.gameObject);
                    break;
            }
        }
    }
}
