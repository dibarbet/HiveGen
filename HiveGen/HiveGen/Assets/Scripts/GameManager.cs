using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public BoardManager boardScript;

	private int level = 1;
	private GameObject[,] boardArray;

	// Use this for initialization
	void Awake () {
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
