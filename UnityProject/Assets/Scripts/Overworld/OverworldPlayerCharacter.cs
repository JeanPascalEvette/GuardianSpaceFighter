using UnityEngine;
using System.Collections;

public class OverworldPlayerCharacter : MonoBehaviour
{
    [SerializeField]
    private Camera GameplayCamera;
    [SerializeField]
    private float OverworldMoveSpeed = 0.0f;


    

    private Vector3 mTargetPosition;
    



    void Start()
    {
        //Set Target to dummy data
        mTargetPosition = new Vector3(-20, -20, -20);

    }

    void Update()
    {
        //If Target hasn't been assigned yet...
        if (mTargetPosition == new Vector3(-20, -20, -20))
        {
            transform.position = GameObject.Find("Levels").transform.Find("Level" + UserData.CurrentLevel).transform.position;



            mTargetPosition = transform.position;

        }
        Vector3 position = transform.position;
        float distance = (mTargetPosition - position).magnitude;
        if (distance > 0.1f)
        {
            position.x = Mathf.SmoothStep(position.x, mTargetPosition.x, OverworldLogic.GameDeltaTime * OverworldMoveSpeed);
            position.y = Mathf.SmoothStep(position.y, mTargetPosition.y, OverworldLogic.GameDeltaTime * OverworldMoveSpeed);
            transform.position = position;
        }
    }
    
    //This function moves the player towards a different level
    public void Move(int difference)
    {
        UserData.IncreaseCurrentLevel(difference);
        mTargetPosition = GameObject.Find("Levels").transform.Find("Level" + UserData.CurrentLevel).transform.position;
        if (difference == -1 && transform.localScale.y > 0 || difference == 1 && transform.localScale.y < 0)
            transform.localScale = new Vector3(transform.localScale.x, -transform.localScale.y, transform.localScale.z);
    }

    

}
