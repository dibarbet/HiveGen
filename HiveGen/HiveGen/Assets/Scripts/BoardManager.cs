using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

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

	public int columns = 10;
	public int rows = 10;
	public Count obstacleCount = new Count(6,10); //I arbitarily chose 6 and 10 for now, can change this later.
	public GameObject player;
	public GameObject exit;
	public GameObject entrance;
	public GameObject floorTile;
	public GameObject obstacleTile;
	public GameObject enemy;
	public GameObject boss;

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
	/*
	 * BoardSetup creates the floor and outer walls of the level and places the exit and enter doors on the 
	 * upper and lower walls, respectively. If no exitLoc and/or enterLoc is specified, the door
	 * will be placed at a random horizontal location on the upper/lower wall.
	 */
	GameObject[,] BoardSetup(int exitLoc=-1, int enterLoc=-1){
		if (exitLoc==-1)
			exitLoc = Random.Range(0, columns);
		if (enterLoc==-1)
			enterLoc = Random.Range(0, columns);

		boardHolder = new GameObject("Board").transform;
		GameObject[,] boardArray = new GameObject[columns+2,rows+2];
		for (int x=-1; x<columns+1; x++){
			for (int y=-1; y<rows+1; y++){
				GameObject toInstantiate = floorTile;
				if (y==-1){
					if (x==enterLoc){
						toInstantiate = entrance;
						Instantiate(player, new Vector3(x,y+1,0f), Quaternion.identity);
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
				boardArray[x+1,y+1] = instance;
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

	GameObject[,] LayoutObjectAtRandom(GameObject tile, int min, int max, GameObject[,] boardArray){
		int objectCount = Random.Range(min, max+1);
		for (int i=0; i<objectCount; i++){
			Vector3 randomPosition = RandomPosition();
			GameObject instance = Instantiate(tile, randomPosition, Quaternion.identity) as GameObject;
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
	GameObject[,] LayoutBoardFromArray(int[,] setupArray, GameObject[,] boardArray){

		int numRows = setupArray.GetLength(0);
		int numCols = setupArray.GetLength(1);
		for (int x=0; x<numCols; x++){
			for (int y=0; y<numRows; y++){
				GameObject toInstantiate = null;
				switch (setupArray[x,y]){
				case 0:
					toInstantiate = floorTile;
					break;
				case -1:
					toInstantiate = obstacleTile;
					break;
				case 1:
					toInstantiate = enemy;
					break;
				case 2:
					toInstantiate = boss;
					break;
				}
				if (toInstantiate != null){
					GameObject instance = Instantiate(toInstantiate, new Vector3(y,numRows-1-x,0f), Quaternion.identity) as GameObject;
					boardArray[x+1,y+1] = instance;
				}
			}
		}
		return boardArray;
	}

//	static int[,] RotateMatrixCounterClockwise(int[,] oldMatrix){
//
//	}

	/*
	 * SetupDefaultScene builds a manually determined room with no PCG elements. This is intended to be used
	 * for testing/debugging purposes only.
	 */
	public GameObject[,] SetupDefaultScene(){
		GameObject[,] boardArray = BoardSetup (5, 5);
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
			{0, 0, 0, 0, 0, 0, 0, 0, 0, 0},};
//		for (int i=0; i<10; i++)
//			print("setupArray["+i+",3]:"+setupArray[i,3]);

		boardArray = LayoutBoardFromArray(setupArray, boardArray);
		return boardArray;
	}

	/*
	 * SetupPCGScene builds a procedurally generated room based on the input level. This function is intended
	 * for main gameplay use over SetupDefaultScene.
	 */
	public GameObject[,] SetupPCGScene(int level){
		generateBlankBlobMap(3);
		return null;
	}

	public Vector3 getPlayerStart(GameObject[,] boardArray){
		for (int row=boardArray.GetLength(0)-1; row>0; row--){
			for (int col=0; col<boardArray.GetLength(1); col++){
				if (boardArray[row, col].tag == "Enter")
					return boardArray[row-1, col].transform.position;
			}
		}
		return new Vector3();
	}

/////////////////////PROCEDURAL MAP GENERATION FUNCTIONS////////////////////////////////

	private GameObject[,] generateBlankBlobMap(int area, int optSeed=int.MinValue){
		if(optSeed!=int.MinValue)
			Random.seed = optSeed;
		
		return null;
	}
}
