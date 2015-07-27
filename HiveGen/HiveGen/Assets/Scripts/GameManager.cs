using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using AStar;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public static GameManager instance = null; //This makes GameManager a singleton, so only one can exist.
	public BoardManager boardScript;
	public CameraUpdater cameraScript;

    public static SpecialPathNode ExitTile;

	public static SpecialPathNode[,] boardArray;

    private static Enemy[] enemies;
    private SpecialPathNode PlayerLocation;
    private SpecialPathNode PreviousPlayerLocation;
    private static Player player;

    private static int PLAYERHP;

    public static bool DoDstarLite;
    public static float MinimumDistance;

	//UI Elements:
	public float levelStartDelay = 2f;
	public Text levelText;
	public Text exitText;
	private static GameObject levelImage;
	private static int level = 1;
	private static int lastPlayedLevel = 1;
	public bool doingSetup;
	public bool titleBool = true;

	public GameObject exitBlock;
	public float exitMessageDelay = 2f;

	public static GameObject titleImage;

	// Use this for initialization
	void Awake () {
		print ("GameManager.Awake()");
        PLAYERHP = 100;
		if(instance == null) //Make sure only one instance of GameManager exists
			instance = this;
		else if (instance != this)
			Destroy(gameObject);

		DontDestroyOnLoad(gameObject); //Allow GameManager to persist when the level changes.
//		boardScript = GetComponent<BoardManager>();
		exitText = GameObject.Find ("ExitText").GetComponent<Text>();
		//InitGame();
//        int rowLength = boardArray.GetLength(0);
//        int colLength = boardArray.GetLength(1);
//        
//        //Prints grid in readable way
//        string final = "";
//        for (int i = 0; i < rowLength; i++)
//        {
//            for (int j = 0; j < colLength; j++)
//            {
//                final += string.Format("{0} ", boardArray[i, j]);
//            }
//            final += Environment.NewLine + Environment.NewLine;
//        }
//        Debug.Log(final);

        DoDstarLite = false;
        MinimumDistance = 5.0f;

		titleImage = GameObject.Find("TitleImage");
	}

	void OnLevelWasLoaded(int index){
		print ("GameManager.OnLevelWasLoaded()");
		print ("level: "+level+"  |  last played level: "+lastPlayedLevel);
		if(level==lastPlayedLevel){
			print ("loading next level...");
	        PLAYERHP = player.HealthPoints;
	        switch (level)
	        {
	            case 0:
	                MinimumDistance = 5.0f;
	                break;
	            case 1:
	                MinimumDistance = 5.5f;
	                break;
	            case 2:
	                MinimumDistance = 6.0f;
	                break;
	            case 3:
	                MinimumDistance = 6.5f;
	                break;
	            case 4:
	                //Debug.Log("level: " + level);
	                MinimumDistance = 7.0f;
	                break;
	            case 5:
	                MinimumDistance = 7.5f;
	                break;
	            case 6:
	                MinimumDistance = 8.0f;
	                break;
	            case 7:
	                MinimumDistance = 8.5f;
	                break;
	            case 8:
	                MinimumDistance = 9.0f;
	                break;
	            case 9:
	                MinimumDistance = 9.5f;
	                break;
	            case 10:
	                MinimumDistance = 10f;
	                break;
	            default:
	                MinimumDistance = 10f;
	                break;
	        }
			level++;
			InitGame();
		}
	}

    void Start()
	{
		print ("GameManager.Start()");
        //DoDstarLite = true;
		if(boardArray!=null){ //Only run if boardArray has been initialized (not if on title screen)
			print ("boardArray: "+boardArray);
	        //Access enemies
	        enemies = FindObjectsOfType<Enemy>();
	        //Instantiate their search space
	        if (enemies != null)
	        {
	            foreach (Enemy e in enemies)
	            {
					//print ("enemy: "+e);
	                //Make sure they are initialized to the correct tile.
	                SpecialPathNode tile = e.GetTileOn();
					//print ("tile: "+tile);
					if(tile!=null){
		                e.TileX = tile.X;
		                e.TileY = tile.Y;
					}
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
    }

	void InitGame(){
		print ("GameManager.InitGame()");
		doingSetup = true;
		if (exitText==null){
			exitText = GameObject.Find ("ExitText").GetComponent<Text>();
			exitText.enabled = false;
		}else if (exitText!=null && exitText.enabled)
			exitText.enabled = false;
		titleImage.SetActive(false);
		titleBool = false;
		levelImage = GameObject.Find("LevelImage");
		levelText = levelImage.GetComponentInChildren<Text>();
//		levelText = GameObject.Find("LevelText").GetComponent<Text>();
		levelText.text = "Level: "+level;
		levelImage.SetActive(true);
		Invoke("HideLevelImage", levelStartDelay);

		//cameraScript = GetComponent<CameraUpdater>();
		//cameraScript.target = null;

		boardScript = GetComponent<BoardManager>();
		//enemies.Clear();
		boardArray = null;
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
		print ("GameManager.HideLevelImage()");
		levelImage.SetActive(false);
		doingSetup = false;
	}
	
	// Update is called once per frame
	void Update () {
		//print ("GameManager.Update()");
		lastPlayedLevel = level;
		enemies = FindObjectsOfType<Enemy>();
		int numEnemies = enemies.Length;
		if (exitBlock != null && numEnemies == 0){
			GameObject.Destroy(exitBlock);
			showExitMessage();
		}else if(ExitTile != null && numEnemies > 0 && exitBlock==null){
			exitBlock = Instantiate(boardScript.obstacleTile, new Vector3(ExitTile.X, ExitTile.Y, 0f), Quaternion.identity) as GameObject;
			hideExitMessage();
		}
        if (player == null && enemies.Length == 0)
        {
			print ("update: null and length enemies==0");
            Mover[] movers = FindObjectsOfType<Mover>();
            foreach (Mover m in movers)
            {
				print ("("+(player==null)+" | "+(enemies.Length==0)+"killing mover from GameManager");
                m.Die();
            }
        }else if (player == null)
		{
			print ("update: player null");
			GameOver();
		}else
        {
			print ("update: player not null");
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
        /*
        for(int i = 0; i < boardArray.GetLength(0); i++)
        {
            for (int j = 0; j < boardArray.GetLength(1); j++)
            {
                if (boardArray != null && boardArray[i,j].tile != null && boardArray[i,j].tile.tag == "Floor")
                {
                    boardArray[i, j].tile.GetComponent<SpriteRenderer>().color = Color.white;
                }
            }
        }*/
		
	}

	public void GameOver(){
		//if(!doingSetup){
		doingSetup = true;
		levelText.text = "Game Over! You died on level "+level+".";
		levelImage.SetActive(true);
		//resetProperties();
		//Invoke("showTitleScreen", exitMessageDelay);
		//Application.LoadLevel(Application.loadedLevel); //Loading a new level deletes everything but the GameManager, since player is dead OnLevelWasLoaded will not move to next level.
		Invoke("quitGame",exitMessageDelay);
		//}
	}

	private void resetProperties(){
		level = 1;
		lastPlayedLevel = 0;

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

	private void showTitleScreen(){
		doingSetup = true;
		titleBool = true;
		titleImage.SetActive(true);
	}

	public void quitGame(){
		print ("quitting game");
		Application.Quit();
	}

	public void StartDStarButton_Click(){
		DoDstarLite = true;
		InitGame();
	}

	public void StartAStarButton_Click(){
		DoDstarLite = false;
		InitGame();
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
