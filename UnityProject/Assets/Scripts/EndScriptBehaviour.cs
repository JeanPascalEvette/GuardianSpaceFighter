using UnityEngine;
using System.Collections;

public class EndScriptBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine(GoToOverworld());
	}

    private IEnumerator GoToOverworld()
    {
        yield return new WaitForSeconds(5.0f);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Overworld");
    }
	
	// Update is called once per frame
	void Update () {
         
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Overworld");
        }
    }
}
