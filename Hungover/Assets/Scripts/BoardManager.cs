using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour {
	
	[Serializable]
	public class Count
	{
		public int minimum;
		public int maximum;
		
		public Count (int min, int max)
		{
			minimum = min;
			maximum = max;
		}
		
	}
	
	public enum Level {
		Frosh,
		Pool,
		Dorm,
		Office,
		Wedding
	}
	
	public class Room 
	{
		public int widthRoom;
		public int heightRoom;
		public int xRoomPosition;
		public int yRoomPosition;
		
		public int xEnterPosition;
		public int yEnterPosition;
		public int xExitPosition;
		public int yExitPosition;
		
		private static Room biggestRoom;
		private static Room smallestRoom;
		
		public static Room GetBiggest()
		{
			if(biggestRoom != null) {
				return biggestRoom;
			}
			
			foreach(Room r in BoardManager.roomsList) {
				if(biggestRoom == null) {
					biggestRoom = r;
				} else {
					if((r.widthRoom * r.heightRoom) > (biggestRoom.widthRoom * biggestRoom.heightRoom)) {
						biggestRoom = r;
					}
					
					
				}
			}
			return biggestRoom;
			
		}

		public static Room SmallestRoom()
		{
			if(smallestRoom != null) {
				return smallestRoom;
			}
			
			foreach(Room r in BoardManager.roomsList) {
				if(smallestRoom == null) {
					smallestRoom = r;
				} else {
					if((r.widthRoom * r.heightRoom) < (smallestRoom.widthRoom * smallestRoom.heightRoom)) {
						smallestRoom = r;
					}
					
					
				}
			}

			return smallestRoom;
			
		}

		public int xCenterPos
		{
			get {return xRoomPosition + (widthRoom/2);}
		}
		
		public int yCenterPos
		{
			get {return yRoomPosition + (heightRoom/2);}
		}
		
		public Vector2 xyCenterPos
		{
			get{return new Vector2(xCenterPos, yCenterPos);}
		}
		
		public bool CollidesWith(Room otherRoom)
		{
			if(xRoomPosition > (otherRoom.xRoomPosition + otherRoom.widthRoom - 1)) // this room's left > the other room's right
				return false;
			
			if(yRoomPosition > (otherRoom.yRoomPosition + otherRoom.heightRoom + 1)) // this room's bottom > the other room's top
				return false;
			
			if((xRoomPosition + widthRoom - 1) < otherRoom.xRoomPosition) // this room's right < the other room's left
				return false;
			
			if((yRoomPosition + heightRoom + 1) < otherRoom.yRoomPosition) // this room's top < the other room's bottom
				return false;
			
			
			return true;
			
		}
		
	}
	
	public int columns = 80;                                         //Number of columns in our game board.
	public int rows = 80;                                            //Number of rows in our game board.
	public int roomNumber = 25;
	public Count wallCount = new Count (5, 9);                      //Lower and upper limit for our random number of walls per level.
	public Count foodCount = new Count (1, 5);                      //Lower and upper limit for our random number of food items per level.
	public Count enemyCount = new Count (5, 10);
	public GameObject exit;                                         //Prefab to spawn for exit.
	public GameObject enter;                                         //Prefab to spawn for enter.
	public GameObject[] floorTiles;                                 //Array of floor prefabs.
	public GameObject[] wallTiles;                                  //Array of wall prefabs.
	public GameObject[] clotheTiles;                                //Array of food prefabs.
	public GameObject[] enemyTiles;                                 //Array of enemy prefabs.
	public GameObject[] outerWallTiles;                             //Array of outer tile prefabs.
	public GameObject player;										//Player prefab.
	public GameObject[] foodTiles;
	
	// Wedding specefic objects
	public GameObject weddingTable;
	public GameObject danceFloor;
	public List<GameObject> weddingTables = new List<GameObject>();
	public List<GameObject> dancers = new List<GameObject>();
	public GameObject alarmSwitch;
	
	private Transform boardHolder; 
	private List <Vector3> gridPositions = new List <Vector3> ();
	
	public Level currentLevel = Level.Wedding;
	
	public static List<Room> roomsList = new List<Room>();
	
	void InitialiseList () //Clears our list gridPositions and prepares it to generate a new board.
	{
		gridPositions.Clear ();
		
		for(int x = 1; x < columns-1; x++)
		{
			for(int y = 1; y < rows-1; y++)
			{
				gridPositions.Add (new Vector3(x, y, 0f));
			}
		}
	}
	
	
	void BoardSetup () //Sets up the outer walls and floor (background) of the game board.
	{
		boardHolder = new GameObject ("Board").transform;
		
		//Room Generation
		for(int i = 0; i < roomNumber; i++)
		{
			Room r = new Room();
			
			r.widthRoom = Random.Range(4,8);
			r.heightRoom = Random.Range(4,8);
			
			if(currentLevel == Level.Wedding && roomsList.Count == 0) {
				r.widthRoom = 13;
				r.heightRoom = 18;
			}
			
			r.xRoomPosition = Random.Range(0, rows - r.widthRoom);
			r.yRoomPosition = Random.Range(0, columns - r.heightRoom);
			
			
			if(!RoomCollides(r))
			{
				for(int x = r.xRoomPosition; x <= (r.xRoomPosition + r.widthRoom); x++)
				{
					for(int y = r.yRoomPosition; y <= (r.yRoomPosition + r.heightRoom); y++)
					{
						GameObject toInstantiate = floorTiles[Random.Range (0,floorTiles.Length)];
						
						if(y == r.yRoomPosition) 	// bottom wall
							toInstantiate = outerWallTiles [x % 2 == 0 ? 7 : 8];
						if(x == (r.xRoomPosition + r.widthRoom))	// right wall
							toInstantiate = outerWallTiles [y % 2 == 0 ? 4 : 5];
						if(y == (r.yRoomPosition + r.heightRoom))	// top wall
							toInstantiate = outerWallTiles [x % 2 == 0 ? 1 : 2];
						if(x == r.xRoomPosition) 	// left wall
							toInstantiate = outerWallTiles [y % 2 == 0 ? 10 : 11];
						if(x == r.xRoomPosition && y == (r.yRoomPosition + r.heightRoom))	// top left corner
							toInstantiate = outerWallTiles [0];
						if(x == (r.xRoomPosition + r.widthRoom) && y == (r.yRoomPosition + r.heightRoom))	// top right corner
							toInstantiate = outerWallTiles [3];
						if(x == (r.xRoomPosition + r.widthRoom) && y == r.yRoomPosition)	// bottom right corner
							toInstantiate = outerWallTiles [6];
						if(x == r.xRoomPosition && y == r.yRoomPosition) 	// bottom left corner
							toInstantiate = outerWallTiles [9];
						
						if(x == r.xRoomPosition || x == (r.xRoomPosition + r.widthRoom) || y == r.yRoomPosition || y == (r.yRoomPosition + r.heightRoom))
						{
							//							toInstantiate = outerWallTiles [Random.Range (0, outerWallTiles.Length)];
						}
						
						if(toInstantiate != null) {
							GameObject instance = Instantiate (toInstantiate, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;
							instance.transform.SetParent (boardHolder);	
						}
						
					}
				}
				
				roomsList.Add (r);
			}	
		}
		
		//Sort rooms from left to right
		SortRoomsByPosition();
		
		//Instantiate enter and exit tiles in the rooms.
		//SpawnDoors();
		this.SpawnExit();
		
		//Create paths which connect the rooms.
		ConnectRooms();
	}
	
	bool RoomCollides(Room r1)
	{
		foreach(Room r2 in roomsList)
		{
			if(r1.CollidesWith(r2))
			{
				return true;
			}
		}
		
		return false;	
	}
	
	public Room getCurrentRoom()
	{
		this.player = GameObject.Find("AshFlashem(Clone)");
		
		foreach (Room room in roomsList)
		{
			if (
				this.player.transform.position.x >= room.xRoomPosition &&
				this.player.transform.position.x <= room.xRoomPosition + room.widthRoom &&
				this.player.transform.position.y >= room.yRoomPosition &&
				this.player.transform.position.y <= room.yRoomPosition + room.heightRoom)
			{
				return room;
			}
		}
		return null;
	}
	
	void SpawnDoors()
	{
		foreach(Room r in roomsList)
		{
			r.xEnterPosition = r.xRoomPosition + 1;
			r.yEnterPosition = r.yRoomPosition + 1;
			r.xExitPosition = r.xRoomPosition + r.widthRoom - 1;
			r.yExitPosition = r.yRoomPosition + r.heightRoom - 1;
			
			Instantiate (enter, new Vector3 (r.xEnterPosition, r.yEnterPosition, 0f), Quaternion.identity);
			Instantiate (exit, new Vector3 (r.xExitPosition, r.yExitPosition, 0f), Quaternion.identity);
		}
		
	}
	
	private void SpawnExit()
	{
		Room lastRoom = roomsList[roomsList.Count - 1];
		lastRoom.xExitPosition = lastRoom.xRoomPosition + lastRoom.widthRoom - 1;
		lastRoom.yExitPosition = lastRoom.yRoomPosition + lastRoom.heightRoom - 1;
		Instantiate(exit, new Vector3 (lastRoom.xExitPosition, lastRoom.yExitPosition, 0.0f), Quaternion.identity);
	}
	
	void SortRoomsByPosition()
	{
		Room temp = new Room();
		
		for(int i = 0; i < roomsList.Count - 1; i++)
		{
			for(int j = i + 1; j < roomsList.Count; j++)
			{
				if(roomsList[i].xCenterPos > roomsList[j].xCenterPos)
				{
					temp = roomsList[i];
					roomsList[i] = roomsList[j];
					roomsList[j] = temp;
				}
			}
		}
	}
	
	void ConnectRooms()
	{
		
		for(int i = 0; i < roomsList.Count - 1; i++)
		{
			int xPath = roomsList[i].xCenterPos;
			int yPath = roomsList[i].yCenterPos;
			
			if(!(roomsList[i].yCenterPos == (roomsList[i+1].yRoomPosition + roomsList[i+1].heightRoom) || roomsList[i+1].xCenterPos == (roomsList[i].xRoomPosition + roomsList[i].widthRoom)))
			{
				while (xPath != roomsList[i+1].xCenterPos)
				{
					
					// if you ran into a wall, place a door, else, place a floor tile.
					if((xPath == (roomsList[i].xRoomPosition + roomsList[i].widthRoom) && yPath > (roomsList[i].yRoomPosition) && yPath < (roomsList[i].yRoomPosition + roomsList[i].heightRoom)) || (xPath == (roomsList[i+1].xRoomPosition) && yPath > (roomsList[i+1].yRoomPosition) && yPath < (roomsList[i+1].yRoomPosition + roomsList[i+1].heightRoom)))
					{
						Instantiate (enter, new Vector3 (xPath, yPath, 0f), Quaternion.identity);
					}
					else
					{
						Instantiate (floorTiles[Random.Range (0,floorTiles.Length)], new Vector3 (xPath, yPath, 0f), Quaternion.identity);
					}
					
					
					if(xPath < roomsList[i+1].xCenterPos)
					{
						xPath++;
					}
					
					else 
					{
						xPath--;
					}
				}
				
				while (yPath != roomsList[i+1].yCenterPos)
				{
					// if you ran into a wall, place a door, else, place a floor tile.
					if((yPath == (roomsList[i].yRoomPosition + roomsList[i].heightRoom) && xPath > (roomsList[i].xRoomPosition) && xPath < (roomsList[i].xRoomPosition + roomsList[i].widthRoom)) || (yPath == (roomsList[i].yRoomPosition) && xPath > (roomsList[i].xRoomPosition) && xPath < (roomsList[i].xRoomPosition + roomsList[i].widthRoom))  || (yPath == (roomsList[i+1].yRoomPosition + roomsList[i+1].heightRoom) && xPath > (roomsList[i+1].xRoomPosition) && xPath < (roomsList[i+1].xRoomPosition + roomsList[i+1].widthRoom)) || (yPath == (roomsList[i+1].yRoomPosition) && xPath > (roomsList[i+1].xRoomPosition) && xPath < (roomsList[i+1].xRoomPosition + roomsList[i+1].widthRoom)))
					{
						Instantiate (enter, new Vector3 (xPath, yPath, 0f), Quaternion.identity);
					}
					else
					{
						Instantiate (floorTiles[Random.Range (0,floorTiles.Length)], new Vector3 (xPath, yPath, 0f), Quaternion.identity);
					}
					
					if(yPath < roomsList[i+1].yCenterPos)
					{
						yPath++;
					}
					
					else 
					{
						yPath--;
					}
				}
			}
			else //if(!(roomsList[i].yCenterPos == roomsList[i+1].yRoomPosition || roomsList[i+1].xCenterPos == (roomsList[i].xRoomPosition + roomsList[i].widthRoom)))
			{	
				while (yPath != roomsList[i+1].yCenterPos)
				{
					// if you ran into a wall, place a door, else, place a floor tile.
					if((yPath == (roomsList[i].yRoomPosition + roomsList[i].heightRoom) && xPath > (roomsList[i].xRoomPosition) && xPath < (roomsList[i].xRoomPosition + roomsList[i].widthRoom)) || (yPath == (roomsList[i].yRoomPosition) && xPath > (roomsList[i].xRoomPosition) && xPath < (roomsList[i].xRoomPosition + roomsList[i].widthRoom))  || (yPath == (roomsList[i+1].yRoomPosition + roomsList[i+1].heightRoom) && xPath > (roomsList[i+1].xRoomPosition) && xPath < (roomsList[i+1].xRoomPosition + roomsList[i+1].widthRoom)) || (yPath == (roomsList[i+1].yRoomPosition) && xPath > (roomsList[i+1].xRoomPosition) && xPath < (roomsList[i+1].xRoomPosition + roomsList[i+1].widthRoom)))
					{
						Instantiate (enter, new Vector3 (xPath, yPath, 0f), Quaternion.identity);
					}
					else
					{
						Instantiate (floorTiles[Random.Range (0,floorTiles.Length)], new Vector3 (xPath, yPath, 0f), Quaternion.identity);
					}
					
					if(yPath < roomsList[i+1].yCenterPos)
					{
						yPath++;
					}
					
					else 
					{
						yPath--;
					}
				}
				
				while (xPath != roomsList[i+1].xCenterPos)
				{
					// if you ran into a wall, place a door, else, place a floor tile.
					if((xPath == (roomsList[i].xRoomPosition + roomsList[i].widthRoom) && yPath > (roomsList[i].yRoomPosition) && yPath < (roomsList[i].yRoomPosition + roomsList[i].heightRoom)) || (xPath == (roomsList[i+1].xRoomPosition) && yPath > (roomsList[i+1].yRoomPosition) && yPath < (roomsList[i+1].yRoomPosition + roomsList[i+1].heightRoom)))
					{
						Instantiate (enter, new Vector3 (xPath, yPath, 0f), Quaternion.identity);
					}
					else
					{
						Instantiate (floorTiles[Random.Range (0,floorTiles.Length)], new Vector3 (xPath, yPath, 0f), Quaternion.identity);
					}
					
					if(xPath < roomsList[i+1].xCenterPos)
					{
						xPath++;
					}
					
					else 
					{
						xPath--;
					}
				}
			}
		}	
	}
	
	Vector3 RandomPosition () //RandomPosition returns a random position within a generated room.
	{
		Room room = roomsList[Random.Range(0, roomsList.Count)]; 
		
		Vector3 randomPosition = new Vector3(Random.Range(room.xRoomPosition + 1, room.xRoomPosition + room.widthRoom - 1 ), Random.Range(room.yRoomPosition + 1, room.yRoomPosition + room.heightRoom - 1 ), 0f);
		
		return randomPosition;
	}
	
	//LayoutObjectAtRandom accepts an array of game objects to choose from along with a minimum and maximum range for the number of objects to create.
	void LayoutObjectAtRandom (GameObject[] tileArray, int minimum, int maximum)
	{
		//Choose a random number of objects to instantiate within the minimum and maximum limits
		int objectCount = Random.Range (minimum, maximum+1);
		
		//Instantiate objects until the randomly chosen limit objectCount is reached
		for(int i = 0; i < objectCount; i++)
		{
			//Choose a position for randomPosition by getting a random position from our list of available Vector3s stored in gridPosition
			Vector3 randomPosition = Vector3.zero;
			do {
				randomPosition = RandomPosition();
			} while((randomPosition.x > Room.SmallestRoom().xRoomPosition && 
			          randomPosition.x < Room.SmallestRoom().xRoomPosition + Room.SmallestRoom().widthRoom && 
			          randomPosition.y > Room.SmallestRoom().yRoomPosition && 
			          randomPosition.y < Room.SmallestRoom().yRoomPosition + Room.SmallestRoom().heightRoom ));
			//Choose a random tile from tileArray and assign it to tileChoice
			GameObject tileChoice = tileArray[Random.Range (0, tileArray.Length)];
			
			//Instantiate tileChoice at the position returned by RandomPosition with no change in rotation
			Instantiate(tileChoice, randomPosition, Quaternion.identity);
		}
	}
	
	void LayoutClothes(GameObject[] clothes, List<Room> rooms) {
		
		foreach(GameObject clothe in clothes) {
			//Utils.Print ("Placing... " + clothe.tag + " in one of " + rooms.Count + " rooms");
			Vector3 randomPosition = Vector3.zero;
			do {
				randomPosition = RandomPosition();
			} while((randomPosition.x > Room.SmallestRoom().xRoomPosition && 
			         randomPosition.x < Room.SmallestRoom().xRoomPosition + Room.SmallestRoom().widthRoom && 
			         randomPosition.y > Room.SmallestRoom().yRoomPosition && 
			         randomPosition.y < Room.SmallestRoom().yRoomPosition + Room.SmallestRoom().heightRoom ));
			
			Instantiate(clothe, randomPosition, Quaternion.identity);
			
			if(clothe.gameObject.tag == "Wallet" && currentLevel == Level.Wedding) {
				// put wallet on dancefloor in the Wedding Level (muahaha)
				GameObject wallet = GameObject.FindGameObjectWithTag("Wallet");
				wallet.transform.position = danceFloor.transform.position;
			}
			
		}
		
	}
	
	public void SetupUpWedding(List<Room> rooms)
	{
		BoardManager.Room biggestRoom = Room.GetBiggest();
		
		// Place tables in biggestRoom
		for(int x = biggestRoom.xRoomPosition; x < (biggestRoom.xRoomPosition + biggestRoom.widthRoom); ++x) {
			for(int y = biggestRoom.yRoomPosition; y < (biggestRoom.yRoomPosition + biggestRoom.heightRoom); ++y) {
				int offset = 2;
				
				if(x >= biggestRoom.xRoomPosition + offset && x <= biggestRoom.xRoomPosition+biggestRoom.widthRoom - offset
				   && ((y >= biggestRoom.yRoomPosition + offset && y <= biggestRoom.yRoomPosition + 2*offset) || 
				    (y <= biggestRoom.yRoomPosition + biggestRoom.heightRoom - offset && y >= biggestRoom.yRoomPosition + biggestRoom.heightRoom - 2*offset)))
				if(x % 3 == 0 && y % 2 == 0) {
					GameObject table = (GameObject)Instantiate(weddingTable, new Vector3(x, y), Quaternion.identity);
					weddingTables.Add (table);
				}
				
			}
		}
		
		// Place dancefloor in biggestRoom's center
		danceFloor = (GameObject)Instantiate(danceFloor, biggestRoom.xyCenterPos, Quaternion.identity);
		
		// Place enemies on the dancefloor, same number of tables we have
		BoxCollider2D danceFloorBoxCollider = danceFloor.GetComponent<BoxCollider2D>();
		for(int i = 0; i < weddingTables.Count; ++i) {
			// random position within dance floor
			float offset = 0.5f;
			float x = Random.Range(danceFloorBoxCollider.bounds.min.x + offset, danceFloorBoxCollider.bounds.max.x - offset);
			float y = Random.Range(danceFloorBoxCollider.bounds.min.y + offset, danceFloorBoxCollider.bounds.max.y - offset);
			GameObject dancer = (GameObject)Instantiate(enemyTiles[0], new Vector3(x, y), Quaternion.identity);
			dancers.Add(dancer);
		}
		
		// Place the Alarm switch near the entrance door
		//		biggestRoom.
		
	}
	
	public void SetupScene (int level)
	{
		//Creates the outer walls and floor.
		BoardSetup ();
		
		//Reset our list of gridpositions.
		InitialiseList ();
		
		//Instantiates Player.
		Instantiate (player, new Vector3 (Room.SmallestRoom().xRoomPosition + 1, Room.SmallestRoom().yRoomPosition + 1, 0f), Quaternion.identity);

        //Instantiate a random number of wall tiles based on minimum and maximum, at randomized positions.
        //LayoutObjectAtRandom (wallTiles, wallCount.minimum, wallCount.maximum);

        //Instantiate a random number of food tiles based on minimum and maximum, at randomized positions.
        LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);
		
		//Instantiate a random number of enemies based on minimum and maximum, at randomized positions.
		LayoutObjectAtRandom (enemyTiles, enemyCount.minimum, enemyCount.maximum);
		
		// Let's setup the wedding level, shall we?
		if(currentLevel == Level.Wedding)
			SetupUpWedding(roomsList);
		
		// After level is setup, place clothes 'randomly' on it
		LayoutClothes(clotheTiles, roomsList);
	}
}