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
	 * an input 2D setupArray[x,y] of int values. The following key should be used for the setup array:
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
		for (int c=0; c<numCols; c++){
			for (int r=0; r<numRows; r++){
				GameObject toInstantiate = null;
                bool wall = false;
                bool IsEnemy = false;
				switch (setupArray[c,r]){
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
					GameObject instance = Instantiate(toInstantiate, new Vector3(c,r,0f), Quaternion.identity) as GameObject;//r,numRows-1-c,0f), Quaternion.identity) as GameObject;
					instance.transform.SetParent(boardHolder);
                    if (IsEnemy)
                    {
                        Enemy thisEnemy = instance.GetComponent<Enemy>();
                        if (Enemies == null)
                        {
                            Enemies = new List<Enemy>();
                        }
                        Enemies.Add(thisEnemy);
                        thisEnemy.TileX = c + 1;
                        thisEnemy.TileY = r + 1;
                    }
                    GameManager.SpecialPathNode thisNode = new GameManager.SpecialPathNode();
                    thisNode.X = c+1;
                    thisNode.Y = r+1;
                    thisNode.tile = instance;
                    thisNode.IsWall = wall;
					boardArray[c+1,r+1] = thisNode;
				}
			}
		}
		return boardArray;
	}

	T[,] rotateArrayCW<T>(T[,] inArray){
		int outRows = inArray.GetLength(1); //Number of rows in output is number of cols in input.
		int outCols = inArray.GetLength(0); //Number of cols in output is number of rows in input.
		T[,] outArray = new T[outRows,outCols];
		for (int r=0; r<outRows; r++){
			for (int c=0; c<outCols; c++){
				outArray[r,c] = inArray[outRows-c-1,r];
			}
		}
		return outArray;
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
		setupArray = rotateArrayCW(setupArray);
		boardArray = LayoutBoardFromArray(setupArray, boardArray);
		return boardArray;
	}

	///*
	///SetupPCGScene builds a procedurally generated room based on the input level. This function is intended
	///for main gameplay use over SetupDefaultScene.
	///*
	public GameManager.SpecialPathNode[,] SetupPCGScene(int level, int optSeed=int.MinValue){
		if(optSeed!=int.MinValue)
			Random.seed = optSeed;
		optSeed = Random.seed;
		print ("setup seed:"+optSeed);

		GameManager.SpecialPathNode[,] blankMap = null;
		GameManager.SpecialPathNode[,] walledMap = null;

		blankMap = GenerateBlankBlobMap(200, optSeed);
		walledMap = AddWallsAndDoors(blankMap, optSeed);
		Vector3 playerLoc = new Vector3();
		foreach (GameManager.SpecialPathNode node in walledMap){
			if(node!=null){
				if (node.tile==entrance)
					playerLoc = new Vector3(node.X, node.Y+1, 0f);
				GameObject instance = Instantiate(node.tile, new Vector3(node.X, node.Y, 0f), Quaternion.identity) as GameObject;
				instance.transform.SetParent(boardHolder);
			}
		}
		if (playerLoc!=null)
			Instantiate(player, playerLoc, Quaternion.identity);

		GameManager.SpecialPathNode[,] filledMap = walledMap;

		return filledMap;
	}

	public Vector3 GetPlayerStart(GameManager.SpecialPathNode[,] boardArray){
		for (int x=0; x<boardArray.GetLength(0); x++){
			for (int y=0; y<boardArray.GetLength(1); y++){
				if (boardArray[x, y].tile.tag == "Enter")
					return new Vector3(boardArray[x,y+1].X, boardArray[x,y+1].Y, 0f);
			}
		}
		return new Vector3();
	}

/*////////////////////PROCEDURAL MAP GENERATION FUNCTIONS////////////////////////////////*/


	/// 
	/// Adds the walls and doors to the border of an input blank map. Places the 
	/// enter door on a south-most wall, and places the exit door on one of the 
	/// north-most walls.
	/// NOTE: input blankMap should be array of nodes in blankMap[x,y] format
	/// 
	GameManager.SpecialPathNode[,] AddWallsAndDoors(GameManager.SpecialPathNode[,] blankMap, int optSeed=int.MinValue){
		if(optSeed!=int.MinValue)
			Random.seed = optSeed;
		print ("walls/doors seed:"+Random.seed);

		GameManager.SpecialPathNode[,] walledMap = blankMap;
		int xNum = walledMap.GetLength(0);
		int yNum = walledMap.GetLength(1);
		for (int x=0; x<xNum; x++){
			int leftInd = (x>0)?(x-1):(0);
			int rightInd = (x<xNum-1)?(x+1):(xNum-1);
			for (int y=0; y<yNum; y++){
				int downInd = (y>0)?(y-1):(0);
				int upInd = (y<yNum-1)?(y+1):(yNum-1);
				int[,] cases = new int[4,2]{ //Four cases with (x,y) in each
					{leftInd, y}, //Case 0: right edge of floor
					{rightInd, y}, //Case 1: left edge of floor
					{x, downInd}, //Case 2: upper edge of floor
					{x, upInd}}; //Case 3: lower edge of floor
				//If any of the cases are true create an obstacle tile.
				for (int caseNum=0; caseNum<cases.GetLength(0); caseNum++){
					int caseX = cases[caseNum,0];
					int caseY = cases[caseNum,1];
					if (isFloorNode(blankMap[caseX,caseY]) && blankMap[x,y]==null){ //current cell is empty and one above it is floor
						GameManager.SpecialPathNode node = new GameManager.SpecialPathNode();
						node.X = x;
						node.Y = y;
						node.tile = obstacleTile;
						node.IsWall = true;
						walledMap[x,y] = node;
					}
				}
			}
		}

		//Now add the enter and exit doors...

		bool[,] travSpace = getTraversableSpace(walledMap);

		//Exit door:
		int exitY = walledMap.GetLength(1);
		List<int> exitOptions = new List<int>();
		while (exitOptions.Count==0 && exitY>0){
			exitY--;
			for (int i=0; i<walledMap.GetLength(0); i++){
				if (walledMap[i,exitY]!=null && walledMap[i,exitY].tile==obstacleTile && travSpace[i,exitY-1]){
					exitOptions.Add(i);
				}
			}
		}
		int exitX = exitOptions[Random.Range (0,exitOptions.Count)];
		walledMap[exitX,exitY].tile = exit;

		//Enter door:
		int enterY = -1;
		List<int> enterOptions = new List<int>();
		while (enterOptions.Count==0 && enterY<walledMap.GetLength(1)-1){
			enterY++;
			for (int i=0; i<walledMap.GetLength(0); i++){
				if (walledMap[i,enterY]!=null && walledMap[i,enterY].tile==obstacleTile && travSpace[i,enterY+1]){
					enterOptions.Add(i);
				}
			}
		}
		int enterX = enterOptions[Random.Range (0,enterOptions.Count)];
		walledMap[enterX,enterY].tile = entrance;

		return walledMap;
	}

	/// 
	/// Gets the traversable space on the map and returns a 2d bool array with the same size as the input walled map.
	/// Traversable space is defined as the largest group of adjoined floor tiles on the map. In the returned bool
	/// array, true values are traversable and false values are not. (Some floor tiles may be marked false if they
	/// are not adjoined to the biggest group of floor tiles)
	/// 
	bool[,] getTraversableSpace(GameManager.SpecialPathNode[,] walledMap){
		int xNum = walledMap.GetLength(0);
		int yNum = walledMap.GetLength(1);
		bool[,] travSpace = new bool[xNum,yNum];

		//groups is a dict of <key=groupNum, value=list of contained locations>
		Dictionary<int, List<Vector3>> groups = new Dictionary<int, List<Vector3>>();
		//parents provides quick way to see what group a tile belongs to, <key=tile location, value=groupNum>
		Dictionary<Vector3, int> parents = new Dictionary<Vector3, int>();

		int groupNum = 0;
		for (int y=1; y<yNum; y++){
			for (int x=1; x<xNum; x++){
				travSpace[x,y] = false; //Fill the array with false to begin with, while organizing floor tiles into groups of adjoined tiles.
				if (walledMap[x,y]!=null && walledMap[x,y].tile==floorTile){
					Vector3 currLoc = new Vector3(x,y,0f);
					int parent=-1;
					if (walledMap[x,y-1].tile==floorTile){ //If tile below is floor tile, add (x,y) to its group.
						parent = parents[new Vector3(x,y-1,0f)];
						parents.Add (currLoc, parent);
						List<Vector3> groupList = groups[parent];
						groupList.Add (currLoc);
						groups[parent] = groupList;
					}
					if (walledMap[x-1,y].tile==floorTile){ //If tile to left is a floor tile...
						Vector3 leftLoc = new Vector3(x-1,y,0f);
						int leftParent = parents[leftLoc];
						if (parent!=-1 && parent!=leftParent){ //If there was a floor tile below and the one to left has a different parent, absorb left group into below group.
							List<Vector3> leftList = groups[leftParent];
							List<Vector3> downList = groups[parent];
							foreach (Vector3 loc in leftList){
								downList.Add (loc);
								parents[loc] = parent;
							}
							groups[parent] = downList;
							groups.Remove (leftParent);
						}else if(parent==-1){ //If there wasn't a floor tile below, add (x,y) to left group.
							parent = parents[leftLoc];
							parents.Add (currLoc, parent);
							List<Vector3> groupList = groups[parent];
							groupList.Add (currLoc);
							groups[parent] = groupList;
						}
					}
					if (parent==-1){ //If parent has not been determined from the below or left tiles, create a new group for (x,y)
						parent = groupNum++;
						List<Vector3> newList = new List<Vector3>();
						newList.Add(currLoc);
						groups.Add(parent, newList);
						parents.Add(currLoc, parent);
					}
				}
			}
		}

		//Now all the floor tiles have been organized into groups, find the largest group and declare those tiles as the traversable space.
		List<Vector3> biggestGroup = new List<Vector3>();
		foreach (List<Vector3> group in groups.Values){
			if (group.Count > biggestGroup.Count)
				biggestGroup = group;
		}

		foreach (Vector3 loc in biggestGroup){
			travSpace[(int)loc.x, (int)loc.y] = true;
		}
		return travSpace;
	}

	bool isFloorNode (GameManager.SpecialPathNode node){
		return (node!=null && node.tile==floorTile);
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
	/// NOTE: should output blank map with format blankMap[x,y]
	///*
	GameManager.SpecialPathNode[,] GenerateBlankBlobMap(int area, int optSeed=int.MinValue){
		if(optSeed!=int.MinValue)
			Random.seed = optSeed;
		print ("blob map seed:"+Random.seed);

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
				if (neighbors.Count>=4 && neighbors.Count>0){ //If this active cell has 3 or more active neighbors, add one randomly.
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
		int xNum = (int)sortByX[sortByX.Count-1].x - minX + 1;
		List<Vector3> sortByY = activeCells.Keys.ToList();
		sortByY.Sort((a, b) => a.y.CompareTo(b.y));
		int minY = (int)sortByY[0].y;
		int yNum = (int)sortByY[sortByY.Count-1].y - minY + 1;
		GameManager.SpecialPathNode[,] blankMap = new GameManager.SpecialPathNode[xNum+2,yNum+2];

		//Now convert the cells to the correct data type and fill the array.
		List<Cell> cells = activeCells.Values.ToList();
		for (int k=0; k<cells.Count; k++){
			GameManager.SpecialPathNode node = new GameManager.SpecialPathNode();
			//Calculate adjusted x-y coords so that all values are >0 (-minX/Y) and there is room for boundary around (+1).
			int adjustedX = (int)cells[k].location.x - minX + 1;
			int adjustedY = (int)cells[k].location.y - minY + 1;
			node.X = adjustedX;
			node.Y = adjustedY;
			node.tile = floorTile;
			node.IsWall = false;

			blankMap[adjustedX,adjustedY] = node;//y-minY+1,x-minX+1] = node; //+1 here shifts the tiles so there is room to add the walls around them.
		}
		return blankMap;
	}
}
