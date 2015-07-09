using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using AStar;

public class GameManager : MonoBehaviour {

	public static GameManager instance = null; //This makes GameManager a singleton, so only one can exist.
	public BoardManager boardScript;

	private int level = 1;
	public static SpecialPathNode[,] boardArray;

    private List<Enemy> enemies;
    private SpecialPathNode PlayerLocation;

	// Use this for initialization
	void Awake () {
		if(instance == null) //Make sure only one instance of GameManager exists
			instance = this;
		else if (instance != this)
			Destroy(gameObject);

		DontDestroyOnLoad(gameObject); //Allow GameManager to persist when the level changes.
		boardScript = GetComponent<BoardManager>();
		InitGame();
        int rowLength = boardArray.GetLength(0);
        int colLength = boardArray.GetLength(1);
        //Access enemies
        enemies = boardScript.Enemies;
        PlayerLocation = boardScript.PlayerLoc;
        if (enemies != null)
        {
            foreach (Enemy e in enemies)
            {
                e.InstantiateAStar(boardArray);
                bool success = e.MoveToTile(PlayerLocation);
                Debug.Log("Found Path: " + success);
            }
        }
        
        //Prints grid in readable way
        string final = "";
        for (int i = 0; i < rowLength; i++)
        {
            for (int j = 0; j < colLength; j++)
            {
                final += string.Format("{0} ", boardArray[i, j]);
            }
            final += Environment.NewLine + Environment.NewLine;
        }
        Debug.Log(final);
	}

	void InitGame(){
		//boardArray = boardScript.SetupPCGScene(level);
		if (boardArray==null)
			boardArray = boardScript.SetupDefaultScene();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    
    public class SpecialPathNode : IPathNode<System.Object>
    {
        public Int32 X {get; set;}
        public Int32 Y {get; set;}
        public Boolean IsWall {get; set;}
        public GameObject tile { get; set; }

        public bool IsWalkable(System.Object unused)
        {
            return !IsWall;
        }

        public override string ToString()
        {
            if (IsWall)
            {
                return "(" + String.Format("{0:00}", X) + ", " + String.Format("{0:00}", Y) + "), " + "T" + "  ||  ";
            }
            else
            {
                return "(" + String.Format("{0:00}", X) + ", " + String.Format("{0:00}", Y) + "), " + "F" + "  ||  ";
            }
        }
    }
}
