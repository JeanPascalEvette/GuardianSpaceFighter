using UnityEngine;
using System.Collections;

public class SideLaserHandler : MonoBehaviour {



	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    //Inflicts damage to enemies hit
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag != "Enemies") return;
        other.gameObject.GetComponent<EnemyBehaviour>().InflictDamage(PlayerCharacter.SideLaserDamage + (UserData.GetSideLaser() - 1) * 5 );
    }
}
