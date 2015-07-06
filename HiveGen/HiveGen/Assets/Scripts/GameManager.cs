using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public static GameManager instance = null; //This makes GameManager a singleton, so only one can exist.
	public BoardManager boardScript;

	private int level = 1;
	private GameObject[,] boardArray;

	// Use this for initialization
	void Awake () {
		if(instance == null) //Make sure only one instance of GameManager exists
			instance = this;
		else if (instance != this)
			Destroy(gameObject);

		DontDestroyOnLoad(gameObject); //Allow GameManager to persist when the level changes.
		boardScript = GetComponent<BoardManager>();
		InitGame();
	}

	void InitGame(){
		boardArray = boardScript.SetupPCGScene(level);
		if (boardArray==null)
			boardArray = boardScript.SetupDefaultScene();
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
