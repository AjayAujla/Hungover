﻿using UnityEngine;
using System;
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
	
	protected class Room 
	{
		public int widthRoom;
		public int heightRoom;
		public int xRoomPosition;
		public int yRoomPosition;
		
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
    public GameObject[] floorTiles;                                 //Array of floor prefabs.
    public GameObject[] wallTiles;                                  //Array of wall prefabs.
    public GameObject[] foodTiles;                                  //Array of food prefabs.
    public GameObject[] enemyTiles;                                 //Array of enemy prefabs.
    public GameObject[] outerWallTiles;                             //Array of outer tile prefabs.
	
	private Transform boardHolder; 
	private List <Vector3> gridPositions = new List <Vector3> ();
	
	List<Room> roomsList = new List<Room>();

	
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
			r.xRoomPosition = Random.Range(0, rows - r.widthRoom);
			r.yRoomPosition = Random.Range(0, columns - r.heightRoom);
			
			if(!RoomCollides(r))
			{
				for(int x = r.xRoomPosition; x <= (r.xRoomPosition + r.widthRoom); x++)
				{
					for(int y = r.yRoomPosition; y <= (r.yRoomPosition + r.heightRoom); y++)
					{
						GameObject toInstantiate = floorTiles[Random.Range (0,floorTiles.Length)];
						
						if(x == r.xRoomPosition || x == (r.xRoomPosition + r.widthRoom) || y == r.yRoomPosition || y == (r.yRoomPosition + r.heightRoom))
						{
							toInstantiate = outerWallTiles [Random.Range (0, outerWallTiles.Length)];
						}
						
						GameObject instance = Instantiate (toInstantiate, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;
						
						roomsList.Add (r);
						
						instance.transform.SetParent (boardHolder);	
					}
				}
			}	
		}
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
			Vector3 randomPosition = RandomPosition();
			
			//Choose a random tile from tileArray and assign it to tileChoice
			GameObject tileChoice = tileArray[Random.Range (0, tileArray.Length)];
			
			//Instantiate tileChoice at the position returned by RandomPosition with no change in rotation
			Instantiate(tileChoice, randomPosition, Quaternion.identity);
		}
	}
	
	public void SetupScene (int level)
	{
		//Creates the outer walls and floor.
		BoardSetup ();
		
		//Reset our list of gridpositions.
		InitialiseList ();
		
		//Instantiate a random number of wall tiles based on minimum and maximum, at randomized positions.
		LayoutObjectAtRandom (wallTiles, wallCount.minimum, wallCount.maximum);
		
		//Instantiate a random number of food tiles based on minimum and maximum, at randomized positions.
		LayoutObjectAtRandom (foodTiles, foodCount.minimum, foodCount.maximum);
		
		//Instantiate a random number of enemies based on minimum and maximum, at randomized positions.
		LayoutObjectAtRandom (enemyTiles, enemyCount.minimum, enemyCount.maximum);
		
		//Instantiate the exit tile in the upper right hand corner of our game board
		Instantiate (exit, new Vector3 (columns - 1, rows - 1, 0f), Quaternion.identity);
	}


}