using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//This class generate the stars in the background.
public class OverworldScenery : MonoBehaviour 
{
	[SerializeField] private Material SceneryMaterial = null; 
	[SerializeField] private float SceneryMinScale = 0.25f; 
	[SerializeField] private float SceneryMaxScale = 0.75f; 
	[Range( 1, 1000 )]
	[SerializeField] private int SceneryPoolSize = 100; 
	
	private GameObject [] mPool;
	
	void Start()
	{
        float ScreenBounds = OverworldLevelRenderer.LevelDistance * (DifficultyCurve.Levels.Count +2 );

        //Increase pool size as the overworld corresponds to more than a single screensize
        int actualSceneryPoolSize = (int)((float)SceneryPoolSize / OverworldLogic.ScreenBounds * ScreenBounds);
		// Create the scenery and position
		mPool = new GameObject[actualSceneryPoolSize];
		for( int count = 0; count < actualSceneryPoolSize; count++ )
		{
			GameObject sceneryItem = new GameObject( "Scenery_PoolID" + ( count + 1 ) );
			CreateMesh m = sceneryItem.AddComponent<CreateMesh>();
			m.Material = SceneryMaterial;
			float x = Random.Range( -ScreenBounds, ScreenBounds);
			float y = Random.Range(OverworldLogic.ScreenHeight * -0.5f, OverworldLogic.ScreenHeight * 0.5f );
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


			if( position.y < OverworldLogic.ScreenHeight * -0.5f )
			{
				position.y = OverworldLogic.ScreenHeight * 0.5f;

                //Randomize x position to prevent pattern recognition
                position.x = Random.Range(-OverworldLogic.ScreenBounds, OverworldLogic.ScreenBounds);
            }

			mPool[count].transform.position = position;
		}
	}
}
