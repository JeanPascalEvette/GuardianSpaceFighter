using UnityEngine;
using System.Collections;

//This class is used to draw the different levels in the overworld
public class OverworldLevelRenderer : MonoBehaviour {
    
    private int numberLevel;

    [SerializeField]
    private float levelDistance = 0.0f;

    [SerializeField]
    private Font pbFont = null;

    [SerializeField]
    private Material pbMat = null;

    [SerializeField]
    private Material levelMaterial = null;
    [SerializeField]
    private Material bossMaterial = null;

    public static float LevelDistance { get; private set; }

    void Awake()
    {
        LevelDistance = levelDistance;
    }

    // Use this for initialization
    void Start () {
        numberLevel = UserData.GetUnlockedLevel()+1;
        //For each level, draw a big disc
	    for(int i = 0; i < numberLevel; i++)
        {
            CreateLevel(i, new Vector3( (i-numberLevel/2)* levelDistance, 0.0f, 0.0f));
        }
	}
	
    void CreateLevel(int levelNum, Vector3 position)
    {
        

        GameObject Level = new GameObject("Level" + levelNum);
        Level.transform.parent = transform;
        CreateMesh m = Level.AddComponent<CreateMesh>();
        if(DifficultyCurve.Levels[levelNum].Length == 1) //If this is a boss level, use the boss material
            m.Material = bossMaterial;
        else
            m.Material = levelMaterial;
        Level.transform.position = position;
        Level.transform.localScale = new Vector3(5, 5, 5);

        if (levelNum > 0) //For each level after the first one - create a "path" of small discs
        {
            for (int i = 0; i <= levelDistance / 5.0f; i++)
                CreateLevelpath(i, Level);
        }

        //Write the best score below the level
        GameObject pbText = new GameObject("PreviousBest" + levelNum);
        pbText.transform.parent = Level.transform;
        TextMesh textMesh = pbText.AddComponent<TextMesh>();
        textMesh.font = pbFont;
        var meshRenderer = pbText.GetComponent<MeshRenderer>();
        meshRenderer.material = pbMat;
        textMesh.text = UserData.LevelMaxPoints[levelNum].ToString();
        pbText.transform.localScale = new Vector3(0.1f, 0.1f, 1);
        pbText.transform.position = Level.transform.position + new Vector3(-0.5f - (UserData.LevelMaxPoints[levelNum].ToString().Length * 0.85f), -5, 0);

    }


    void CreateLevelpath(int pathNum, GameObject parent)
    {
        GameObject Levelpath = new GameObject("Path");
        Levelpath.transform.parent = parent.transform;
        CreateMesh m = Levelpath.AddComponent<CreateMesh>();
        m.Material = levelMaterial;
        Levelpath.transform.position = parent.transform.position + new Vector3((-pathNum*levelDistance/5.0f), 0, 0);
        float localScale = 0.4f;
        Levelpath.transform.localScale = new Vector3(localScale, localScale, 1);
    }

	// Update is called once per frame
	void Update () {
	
	}
}
