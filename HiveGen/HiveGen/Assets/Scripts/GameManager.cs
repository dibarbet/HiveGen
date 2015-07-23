using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using AStar;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public static GameManager instance = null; //This makes GameManager a singleton, so only one can exist.
	public BoardManager boardScript;

    public static SpecialPathNode ExitTile;

	public static SpecialPathNode[,] boardArray;

    private Enemy[] enemies;
    private SpecialPathNode PlayerLocation;
    private SpecialPathNode PreviousPlayerLocation;
    private Player player;

    private static int PLAYERHP;

	//UI Elements:
	public float levelStartDelay = 2f;
	public Text levelText;
	public Text exitText;
	private GameObject levelImage;
	private int level = 1;
	private bool doingSetup;

	public GameObject exitBlock;
	public float exitMessageDelay = 2f;

	// Use this for initialization
	void Awake () {
        PLAYERHP = 100;
		if(instance == null) //Make sure only one instance of GameManager exists
			instance = this;
		else if (instance != this)
			Destroy(gameObject);

		DontDestroyOnLoad(gameObject); //Allow GameManager to persist when the level changes.
		boardScript = GetComponent<BoardManager>();
		exitText = GameObject.Find ("ExitText").GetComponent<Text>();
		InitGame();
        int rowLength = boardArray.GetLength(0);
        int colLength = boardArray.GetLength(1);
        
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

	void OnLevelWasLoaded(int index){
        PLAYERHP = player.HealthPoints;
		level++;
		InitGame();
	}

    void Start()
    {
        //Access enemies
        enemies = FindObjectsOfType<Enemy>();
        //Instantiate their search space
        if (enemies != null)
        {
            foreach (Enemy e in enemies)
            {
                //Make sure they are initialized to the correct tile.
                SpecialPathNode tile = e.GetTileOn();
                e.TileX = tile.X;
                e.TileY = tile.Y;
                //Debug.Log("Enemy: " + e.TileX + ", " + e.TileY + "; Player: " + tile.X + ", " + tile.Y);
                e.InstantiateAStar(boardArray);
                e.InstantiateDStar(boardArray);
            }
        }
        //Get location of exit tile
        foreach (SpecialPathNode n in boardArray)
        {
            if (n != null)
            {
                if (n.tile != null)
                {
                    if (n.tile.gameObject.tag == "Exit")
                    {
                        ExitTile = n;
                        n.IsWall = false;
                        Debug.Log("FOUND EXIT");
                    }
                }
                
            }
        }

        //Get player location
        player = FindObjectOfType<Player>();
        Debug.Log("PLAYER: " + player);
        player.HealthPoints = PLAYERHP;
        PlayerLocation = player.GetComponent<Player>().GetTileOn();


        
    }

	void InitGame(){
		doingSetup = true;
		if (exitText==null){
			exitText = GameObject.Find ("ExitText").GetComponent<Text>();
		}else if (exitText!=null && exitText.enabled)
			exitText.enabled = false;
		levelImage = GameObject.Find("LevelImage");
		levelText = levelImage.GetComponentInChildren<Text>();
//		levelText = GameObject.Find("LevelText").GetComponent<Text>();
		levelText.text = "Level: "+level;
		levelImage.SetActive(true);
		Invoke("HideLevelImage", levelStartDelay);

		//enemies.Clear();
		boardArray = boardScript.SetupPCGScene(level);
		if (boardArray==null)
			boardArray = boardScript.SetupDefaultScene();
        int xNum = boardArray.GetLength(0);
        int yNum = boardArray.GetLength(1);
        for (int y = 0; y < yNum; y++)
        {
            for (int x = 0; x < xNum; x++)
            {
                //Initialize any empty cells to non-walkable
                if (boardArray[x, y] == null)
                {
                    boardArray[x, y] = new SpecialPathNode();
                    boardArray[x, y].X = x;
                    boardArray[x, y].Y = y;
                    boardArray[x, y].IsWall = true;
                }
            }
        }
        Start();
	}

	private void HideLevelImage(){
		levelImage.SetActive(false);
		doingSetup = false;
	}
	
	// Update is called once per frame
	void Update () {
		int numEnemies = FindObjectsOfType<Enemy>().Length;
		if (exitBlock != null && numEnemies == 0){
			GameObject.Destroy(exitBlock);
			showExitMessage();
		}else if(ExitTile != null && numEnemies > 0 && exitBlock==null){
			exitBlock = Instantiate(boardScript.obstacleTile, new Vector3(ExitTile.X, ExitTile.Y, 0f), Quaternion.identity) as GameObject;
			hideExitMessage();
		}
        if (player == null || enemies.Length == 0)
        {
            Mover[] movers = FindObjectsOfType<Mover>();
            foreach (Mover m in movers)
            {
                m.Die();
            }
        }
        else
        {
            PlayerLocation = player.GetComponent<Player>().GetTileOn();
            if (!doingSetup)
            {
                if (PreviousPlayerLocation == null)
                {
                    PreviousPlayerLocation = PlayerLocation;
                    //PathToPlayer();
                }
                

                //Debug.Log("GameManager Player Location: " + PlayerLocation.X + ", " + PlayerLocation.Y);
                //Debug.Log(player.transform.position);
                if (PlayerLocation != PreviousPlayerLocation)
                {
                    //PathToPlayer();
                    PreviousPlayerLocation = PlayerLocation;
                }
            }
        }
		
	}

	public void GameOver(){
		doingSetup = true;
		levelText.text = "Game Over! You died on level "+level+".";
		levelImage.SetActive(true);
	}

    private void PathToPlayer()
    {
        if (enemies != null)
        {
            foreach (Enemy e in enemies)
            {
                bool success = e.MoveToTile(PlayerLocation);
                Debug.Log("Found Path: " + success);
            }
        }
    }

	private void showExitMessage(){
		exitText.enabled = true;
		Invoke("hideExitMessage", exitMessageDelay);
	}

	private void hideExitMessage(){
		exitText.enabled = false;
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
            
            if (tile == null)
            {
                return "F";
                //return "(" + String.Format("{0:00}", X) + ", " + String.Format("{0:00}", Y) + "), " + "N" + "  ||  ";
            }
            else if (tile.tag == "Enter" || tile.tag == "Exit")
            {
                return "D";
            }
            if (IsWall)
            {
                return "T";
                //return "(" + String.Format("{0:00}", X) + ", " + String.Format("{0:00}", Y) + "), " + "T" + "  ||  ";
            }
            else
            {
                return "F";
                //return "(" + String.Format("{0:00}", X) + ", " + String.Format("{0:00}", Y) + "), " + "F" + "  ||  ";
            }
        }
    }

}
