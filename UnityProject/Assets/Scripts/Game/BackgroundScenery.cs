using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BackgroundScenery : MonoBehaviour 
{
	[SerializeField] private Material SceneryMaterial = null; 
	[SerializeField] private float SceneryMinScale = 0.25f; 
	[SerializeField] private float SceneryMaxScale = 0.75f; 
	[Range( 1, 1000 )]
	[SerializeField] private int SceneryPoolSize = 100; 
	
	private GameObject [] mPool;
	
	void Start()
    {
        switch(UserData.CurrentLevel) // Different star color based on current level
        {
            case 0:
                SceneryMaterial.color = Color.white;
                break;
            case 1:
                SceneryMaterial.color = Color.cyan;
                break;
            case 2:
                SceneryMaterial.color = new Color(114.0f / 255.0f, 189.0f / 255.0f, 100.0f / 255.0f);
                break;
            case 3:
                SceneryMaterial.color = new Color(255.0f / 255.0f, 145.0f / 255.0f, 26.0f / 255.0f);
                break;
            case 4:
                SceneryMaterial.color = new Color(255.0f / 255.0f, 26.0f / 255.0f, 230.0f / 255.0f);
                break;
        }
        // Create the scenery and position
        mPool = new GameObject[SceneryPoolSize];
		for( int count = 0; count < SceneryPoolSize; count++ )
		{
			GameObject sceneryItem = new GameObject( "Scenery_PoolID" + ( count + 1 ) );
			CreateMesh m = sceneryItem.AddComponent<CreateMesh>();
			m.Material = SceneryMaterial;
			float x = Random.Range( -GameLogic.ScreenBounds, GameLogic.ScreenBounds );
			float y = Random.Range( GameLogic.ScreenHeight * -0.5f, GameLogic.ScreenHeight * 0.5f );
			float scale = Random.Range( SceneryMinScale, SceneryMaxScale );
			sceneryItem.transform.position = new Vector3( x, y, 0.0f );
			sceneryItem.transform.localScale = new Vector3( scale, scale, scale );
			sceneryItem.transform.localRotation = Quaternion.AngleAxis( 180.0f, Vector3.forward );
			sceneryItem.transform.parent = transform;
			mPool[count] = sceneryItem;
		}
	}
	
	void Update()
	{
		// Update the position of each active sceneryItem, keep a track of scenery which have gone off screen 
		for( int count = 0; count < mPool.Length; count++ )
		{
			Vector3 position = mPool[count].transform.position;
			float scale = mPool[count].transform.localScale.x;
			position.y -= GameLogic.GameDeltaTime * GameLogic.GameSpeed * scale;

			if( position.y < GameLogic.ScreenHeight * -0.5f )
			{
				position.y = GameLogic.ScreenHeight * 0.5f;

                //Randomize x position to prevent pattern recognition
                position.x = Random.Range(-GameLogic.ScreenBounds, GameLogic.ScreenBounds);
            }

			mPool[count].transform.position = position;
		}
	}
}
