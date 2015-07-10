using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using System.Linq;

public class BoardManager : MonoBehaviour {

	[Serializable]
	public class Count {
		public int minimum;
		public int maximum;

		public Count(int min, int max){
			minimum = min;
			maximum = max;
		}
	}

	public int columns = 10; //Only used for default map.
	public int rows = 10; //Only used for default map.
	public Count obstacleCount = new Count(6,10); //I arbitarily chose 6 and 10 for now, can change this later.
	public GameObject player;
	public GameObject exit;
	public GameObject entrance;
	public GameObject floorTile;
	public GameObject obstacleTile;
	public GameObject enemy;
	public GameObject boss;

    //Since the board is initializing enemies, we need to be able to access all of them.
    public List<Enemy> Enemies { get; set; }

	private Transform boardHolder;
	private List <Vector3> gridPositions = new List<Vector3>();

	void InitializeList(){
		gridPositions.Clear();
		for (int x=0; x<columns; x++){
			for (int y=0; y<rows; y++){
				gridPositions.Add(new Vector3(x,y,0f));
			}
		}
	}
	///*
	/// BoardSetup creates the floor and outer walls of the level and places the exit and enter doors on the 
	/// upper and lower walls, respectively. If no exitLoc and/or enterLoc is specified, the door
	/// will be placed at a random horizontal location on the upper/lower wall.
	/// NOTE: This function is only used to build the default map.
	///*
	GameManager.SpecialPathNode[,] BoardSetup(int exitLoc=-1, int enterLoc=-1){
		if (exitLoc==-1)
			exitLoc = Random.Range(0, columns);
		if (enterLoc==-1)
			enterLoc = Random.Range(0, columns);

		boardHolder = new GameObject("Board").transform;
        GameManager.SpecialPathNode[,] boardArray = new GameManager.SpecialPathNode[columns + 2, rows + 2];
		for (int x=-1; x<columns+1; x++){
			for (int y=-1; y<rows+1; y++){
				GameObject toInstantiate = floorTile;
				if (y==-1){
					if (x==enterLoc){
						toInstantiate = entrance;
						Instantiate(player, new Vector3(x,y+1,0f), Quaternion.identity);
                        /**
                        Debug.Log("player stuff: " + player.transform.position + ", " + new Vector3(x, y + 1, 0f) + ", " + (y + 1 + 1) + ", " + (x));
                        PlayerLoc = new GameManager.SpecialPathNode();
                        PlayerLoc.X = x + 1;
                        PlayerLoc.Y = y + 1 + 1;
                        PlayerLoc.tile = player;
                        PlayerLoc.IsWall = false;*/
					}else
						toInstantiate = obstacleTile;
				}else if(y==rows){
					if (x==exitLoc)
						toInstantiate = exit;
					else
						toInstantiate = obstacleTile;
				}else if(x==-1 || x==columns)
					toInstantiate = obstacleTile;

				GameObject instance = Instantiate(toInstantiate, new Vector3(x,y,0f), Quaternion.identity) as GameObject;
				instance.transform.SetParent(boardHolder);
                GameManager.SpecialPathNode thisNode = new GameManager.SpecialPathNode();
                thisNode.X = x+1;
                thisNode.Y = y+1;
                thisNode.tile = instance;
                thisNode.IsWall = true;
				boardArray[x+1,y+1] = thisNode;
			}
		}
		return boardArray;
	}
	/*
	 * RandomPosition generates a random board position based on the remaining available 
	 * positions in gridPositions.
	 */
	Vector3 RandomPosition(){
		int randomIndex = Random.Range (0, gridPositions.Count);
		Vector3 randomPosition = gridPositions[randomIndex];
		gridPositions.RemoveAt(randomIndex);
		return randomPosition;
	}

	GameObject[,] LayoutObjectAtRandom(GameObject tile, int min, int max, GameObject[,] boardArray, bool IsWall){
		int objectCount = Random.Range(min, max+1);
		for (int i=0; i<objectCount; i++){
			Vector3 randomPosition = RandomPosition();
			GameObject instance = Instantiate(tile, randomPosition, Quaternion.identity) as GameObject;
            GameManager.SpecialPathNode thisNode = new GameManager.SpecialPathNode();
            thisNode.X = (int)(randomPosition.x) + 1;
            thisNode.Y = (int)(randomPosition.y) + 1;
            thisNode.tile = instance;
            thisNode.IsWall = IsWall;
			boardArray[(int)(randomPosition.x)+1, (int)(randomPosition.y)+1] = instance;
		}
		return boardArray;
	}

	/*
	 * LayoutBoardFromArray creates the board area inside the outer wall layer according to 
	 * an input 2D setupArray of int values. The following key should be used for the setup array:
	 * -2 : empty (for unreachable area)
	 * -1 : obstacle
	 * 0  : floor
	 * 1  : base enemy
	 * 2  : boos enemy
	 * 
	 * Note: this function does not affect placement of doors or player.
	 */
    GameManager.SpecialPathNode[,] LayoutBoardFromArray(int[,] setupArray, GameManager.SpecialPathNode[,] boardArray)
    {

		int numRows = setupArray.GetLength(0);
		int numCols = setupArray.GetLength(1);
		for (int x=0; x<numCols; x++){
			for (int y=0; y<numRows; y++){
				GameObject toInstantiate = null;
                bool wall = false;
                bool IsEnemy = false;
				switch (setupArray[x,y]){
				case 0:
					toInstantiate = floorTile;
					break;
				case -1:
					toInstantiate = obstacleTile;
                    wall = true;
					break;
				case 1:
					toInstantiate = enemy;
                    wall = true;
                    IsEnemy = true;
					break;
				case 2:
					toInstantiate = boss;
					break;
				}
				if (toInstantiate != null){
					GameObject instance = Instantiate(toInstantiate, new Vector3(y,numRows-1-x,0f), Quaternion.identity) as GameObject;
                    if (IsEnemy)
                    {
                        Enemy thisEnemy = instance.GetComponent<Enemy>();
                        if (Enemies == null)
                        {
                            Enemies = new List<Enemy>();
                        }
                        Enemies.Add(thisEnemy);
                        thisEnemy.TileX = x + 1;
                        thisEnemy.TileY = y + 1;
                    }
                    GameManager.SpecialPathNode thisNode = new GameManager.SpecialPathNode();
                    thisNode.X = x+1;
                    thisNode.Y = y+1;
                    thisNode.tile = instance;
                    thisNode.IsWall = wall;
					boardArray[x+1,y+1] = thisNode;
				}
			}
		}
		return boardArray;
	}


	///*
	/// SetupDefaultScene builds a manually determined room with no PCG elements. This is intended to be used
	/// for testing/debugging purposes only.
	///*
    public GameManager.SpecialPathNode[,] SetupDefaultScene(){
        GameManager.SpecialPathNode[,] boardArray = BoardSetup(5, 5);
		InitializeList();

		int[,] setupArray = new int[10, 10] {
			{1, 0, 0, 0, 0, 0, 0, 0, 0, 0},
			{0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
			{0, 0,-1, 0, 0, 0, 0,-1, 0, 0},
			{0, 0,-1, 0, 0, 0, 0,-1, 1, 0},
			{0, 0,-1, 0, 0, 0, 0,-1, 0, 0},
			{0, 0,-1, 0, 0, 1, 0,-1, 0, 0},
			{0, 0,-1, 0, 0, 0, 0,-1, 0, 0},
			{0, 0,-1,-1,-1,-1,-1,-1, 0, 0},
			{0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
			{0, 0, 0, 0, 0, 0, 0, 0, 0, 0}};

		boardArray = LayoutBoardFromArray(setupArray, boardArray);
		return boardArray;
	}

	///*
	///SetupPCGScene builds a procedurally generated room based on the input level. This function is intended
	///for main gameplay use over SetupDefaultScene.
	///*
    public GameManager.SpecialPathNode[,] SetupPCGScene(int level){
		GameManager.SpecialPathNode[,] blankMap = GenerateBlankBlobMap(200);
		foreach (GameManager.SpecialPathNode node in blankMap){
			if(node!=null)
				Instantiate(node.tile, new Vector3(node.X, node.Y, 0f), Quaternion.identity);
		}
		return blankMap;
	}

	public Vector3 GetPlayerStart(GameObject[,] boardArray){
		for (int row=boardArray.GetLength(0)-1; row>0; row--){
			for (int col=0; col<boardArray.GetLength(1); col++){
				if (boardArray[row, col].tag == "Enter")
					return boardArray[row-1, col].transform.position;
			}
		}
		return new Vector3();
	}

/*////////////////////PROCEDURAL MAP GENERATION FUNCTIONS////////////////////////////////*/

	GameManager.SpecialPathNode[,] AddWallsAndDoors(GameManager.SpecialPathNode[,] blankMap, int optSeed=int.MinValue){
		if(optSeed!=int.MinValue)
			Random.seed = optSeed;

		GameManager.SpecialPathNode[,] walledMap = null;

		return walledMap;
	}

	class Cell{
		private bool active = false;
		public Vector3 location;

		public Cell(Vector3 loc){
			location = loc;
		}
		public void activate(){
			active = true;
		}
		public void deactivate(){
			active = false;
		}
	}

	List<Cell> GetNeighbors(Cell c, Dictionary<Vector3, Cell> activeCells, bool active=false){
		List<Cell> neighbors = new List<Cell>();
		for (int i=(int)c.location.x-1; i<(int)c.location.x+2; i++){
			for (int j=(int)c.location.y-1; j<(int)c.location.y+2; j++){
				if (i!=0 || j!=0){
					Vector3 loc = new Vector3(i, j, 0f);
					if (activeCells.ContainsKey(loc) && active)
						neighbors.Add(activeCells[loc]);
					else if (!activeCells.ContainsKey(loc) && !active)
						neighbors.Add(new Cell(loc));
				}
			}
		}
		return neighbors;
	}

	///*
	/// GenerateBlankBlobMap uses a procedural generation algorithm (cell automaton) to create the 
	/// base floor layout of a map with a number of floor tiles equal to the input area.
	///*
	GameManager.SpecialPathNode[,] GenerateBlankBlobMap(int area, int optSeed=int.MinValue){
		if(optSeed!=int.MinValue)
			Random.seed = optSeed;

		Dictionary<Vector3, Cell> activeCells = new Dictionary<Vector3, Cell>();
		Vector3 startLoc = new Vector3(0,0,0f);
		activeCells.Add(startLoc, new Cell(startLoc));
		int currArea = 1;

		
		List<Cell> toBeActivated = new List<Cell>();
		while (currArea < area){
			toBeActivated.Clear();
			List<Cell> allActive = activeCells.Values.ToList();
			for (int i=0; i<allActive.Count; i++){
				List<Cell> neighbors = GetNeighbors(allActive[i], activeCells, false);
				if (neighbors.Count<=3 && neighbors.Count>0){ //If this active cell has 3 or more active neighbors, add one randomly.
					toBeActivated.Add(neighbors[Random.Range(0,neighbors.Count)]); //Get a random inactive neighbor and set it to be active on the next generation.
				}
			}
			if (toBeActivated.Count==0){
				List<Cell> neighbors = GetNeighbors(allActive[Random.Range(0,allActive.Count)], activeCells, false);
				if(neighbors.Count>0)
					toBeActivated.Add(neighbors[Random.Range(0,neighbors.Count)]);
			}
			for (int j=0; j<toBeActivated.Count; j++){
				Cell c = toBeActivated[j];
				if (!activeCells.ContainsKey(c.location)){
					c.activate();
					activeCells.Add(c.location, c);
					currArea++;
				}
			}
		}

		////Now there is a dictionary full of cells to be turned into floor tiles, create the output array.
		//First, figure out how big the 2d array needs to be to contain the map, then create the empty array.
		List<Vector3> sortByX = activeCells.Keys.ToList();
		sortByX.Sort((a, b) => a.x.CompareTo(b.x));
//		for (int i=0; i<sortByX.Count; i++)
//			print (sortByX[i]);
		int minX = (int)sortByX[0].x;
		int numCols = (int)sortByX[sortByX.Count-1].x - minX + 1;
		List<Vector3> sortByY = activeCells.Keys.ToList();
		sortByY.Sort((a, b) => a.y.CompareTo(b.y));
		int minY = (int)sortByY[0].y;
		int numRows = (int)sortByY[sortByY.Count-1].y - minY + 1;
		GameManager.SpecialPathNode[,] blankMap = new GameManager.SpecialPathNode[numRows,numCols];

		//Now convert the cells to the correct data type and fill the array.
		List<Cell> cells = activeCells.Values.ToList();
		for (int k=0; k<cells.Count; k++){
			GameManager.SpecialPathNode node = new GameManager.SpecialPathNode();
			int x = (int)cells[k].location.x;
			int y = (int)cells[k].location.y;
			node.X = x;
			node.Y = y;
			node.tile = floorTile;
			node.IsWall = false;

			blankMap[y-minY,x-minX] = node;
		}
		return blankMap;
	}
}
