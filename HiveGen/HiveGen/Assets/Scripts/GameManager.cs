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
    private SpecialPathNode PreviousPlayerLocation;
    private Player player;


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
        //Instantiate their search space
        if (enemies != null)
        {
            foreach (Enemy e in enemies)
            {
                e.InstantiateAStar(boardArray);
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

    void Start()
    {
        //Get player location
        player = FindObjectOfType<Player>();
        PlayerLocation = player.GetComponent<Player>().GetTileOn();
    }

	void InitGame(){
		boardArray = boardScript.SetupPCGScene(level);
		if (boardArray==null)
			boardArray = boardScript.SetupDefaultScene();
	}
	
	// Update is called once per frame
	void Update () {
        if (PreviousPlayerLocation == null)
        {
            PreviousPlayerLocation = PlayerLocation;
            PathToPlayer();
        }
        PlayerLocation = player.GetComponent<Player>().GetTileOn();
        
        //Debug.Log("GameManager Player Location: " + PlayerLocation.X + ", " + PlayerLocation.Y);
        //Debug.Log(player.transform.position);
        if (PlayerLocation != PreviousPlayerLocation)
        {
            PathToPlayer();
            PreviousPlayerLocation = PlayerLocation;
        }
	}

    private void PathToPlayer()
    {
        if (enemies != null)
        {
            foreach (Enemy e in enemies)
            {
                bool success = e.MoveToTile(PlayerLocation);
                //Debug.Log("Found Path: " + success);
            }
        }
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
