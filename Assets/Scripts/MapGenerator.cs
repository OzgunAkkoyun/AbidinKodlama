using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Networking;
using Random = System.Random;
using SimpleJSON;

[Serializable]
public class Data
{
    public int index;
    public float r1;
    public float r2;
    public float r3;
}


public class MapGenerator : MonoBehaviour {
	
	public Map[] maps;
    [Space(40f)]
	public int mapIndex;

    [Space(15f)]
    [Header("Game Prefabs")]
    public Transform tilePrefab;
	public Transform[] obstaclePrefab;
    public Transform mapFloor;
	public Transform navmeshFloor;
	public Transform navmeshMaskPrefab;
    public GameObject vehiclePrefab;

    [Space(40f)]
    public Vector2 maxMapSize;
    
    [Range(0,1)]
	public float outlinePercent;
	
	public float tileSize;
	List<Coord> allTileCoords;
    List<Coord> allOpenCoords;
    List<Coord> allObstacleCoord = new List<Coord>();

    Queue<Coord> shuffledTileCoords;
	Queue<Coord> shuffledOpenTileCoords;

    public List<Coord> Path = new List<Coord>();
    public int expectedPathLength;
    public int PathLength => Path.Count;

    Transform[,] tileMap;
	[HideInInspector]
	public Map currentMap;

    string holderName = "Generated Map";
    private Transform mapHolder;
    private System.Random prng;

    //Connection variables

    private string url = "http://localhost:8080/UnityTest/getRatios.php";

    void Awake() {

        currentMap = maps[mapIndex];
        //StartCoroutine(GetConnection());
    }

    IEnumerator GetConnection()
    {

        WWW www = new WWW(url);
        yield return www;

        if (www.error != null)
        {
            Debug.Log(www.error);
        }
        else
        {
            //List<Data> data = new List<Data>();
            Debug.Log(www.text);

            var N = JSON.Parse(www.text);
            Debug.Log(N[0]["r1"]);
            //var data = new data();
            // Or retrieve results as binary data
            
        }

    }
    public void GenerateMapFromLoad(Coord _mapSize,int _seed,Coord _startPoint,Coord _targetPoint,List<Coord> _Path)
    {
        Debug.Log("Load");
        Debug.Log(_Path);
        tileMap = new Transform[_mapSize.x, _mapSize.y];
        currentMap.seed = _seed;
        prng = new System.Random(currentMap.seed);
        mapHolder = new GameObject(holderName).transform;
        GenerateAllTiles();
        if (transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }
        mapHolder.parent = transform;

        SpawnAllTiles();

        SpawnObstacle(prng);

        CreateMapLines();

        currentMap.startPoint = _startPoint;
        Path = _Path;
        currentMap.targetPoint = Path[Path.Count - 1];

        var start = currentMap.startPoint;
        var target = Path[Path.Count - 1];

        tileMap[start.x, start.y].gameObject.GetComponent<MeshRenderer>().material.color = Color.blue;
        tileMap[target.x, target.y].gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
        SpawnVehicle();
    }
	public void GenerateMap()
	{
	    Timer timer = new Timer();
		
        tileMap = new Transform[currentMap.mapSize.x,currentMap.mapSize.y];

        currentMap.seed = UnityEngine.Random.Range(0,200);
        prng = new System.Random (currentMap.seed);


        mapHolder = new GameObject(holderName).transform;
        GenerateAllTiles();

		// Create map holder object
		
		if (transform.Find (holderName)) {
			DestroyImmediate (transform.Find (holderName).gameObject);
		}
		mapHolder.parent = transform;

	    SpawnAllTiles();

	    SpawnObstacle(prng);

	    
        CreateMapLines();

	    CreateStartandTargetPoints();
	    SpawnVehicle();
        timer.Finish(false);

        //CreateNavmeshMask();

    }

    public void CreatePath()
    {
        Path.Add(new Coord(currentMap.startPoint.x,currentMap.startPoint.y));
        var currentPathIndex = 0;

        while (currentPathIndex < expectedPathLength)
        {
            var currentCell = Path[currentPathIndex];
            var neighbours = GetAvailableNeighbours(currentCell);
          
            if (SelectNextCell(neighbours, out Coord selectedNeighbour))
            {
                
                Path.Add(selectedNeighbour);
                currentPathIndex++;
            }
            else
            {
                OneStepBackinList(currentCell, ref currentPathIndex);
            }

        }
    }

    private void OneStepBackinList(Coord currentCell, ref int currentPathIndex)
    {
        Path.RemoveAt(currentPathIndex);
        currentPathIndex--;
        Path[currentPathIndex].UnavaliableNeighbours.Add(new Coord(currentCell.x,currentCell.y));
    }

    private bool SelectNextCell(List<Coord> neighbours, out Coord selectedNeighbour)
    {
        if (neighbours.Count > 0)
        {
            var rnd = UnityEngine.Random.Range(0, neighbours.Count);
            selectedNeighbour = neighbours[rnd];
            return true;
        }
        else
        {
            selectedNeighbour = new Coord();
            return false;
        }

    }
    private List<Coord> GetAvailableNeighbours(Coord cell)
    {
        var neighbours = cell.GetNeighbours();
        List<Coord> availableCells = new List<Coord>();
        foreach (var neighbour in neighbours)
        {
            if (IsCellOnPath(neighbour))
            {
                //Log("Cell On Path");
            }
            else
            {
                if (CellinBounds(neighbour))
                {
                    if (CellUnavaliableNeighboursGet(neighbour, cell))
                    {
                        //Log("Cell UnavaliableNeighbours");
                    }
                    else
                    {
                        availableCells.Add(neighbour);
                    }
                }
            }
        }
        return availableCells;
    }

    public bool CellinBounds(Coord neighbours)
    {
        if (neighbours.x < 0 || neighbours.x >= currentMap.mapSize.x || neighbours.y < 0 || neighbours.y >= currentMap.mapSize.y)
        {
            return false;
        }
        else
        {
            return true;
        }
        
    }

    public bool IsCellOnPath(Coord neighbours) => Path.Contains(neighbours);

    public bool CellUnavaliableNeighboursGet(Coord neighbours, Coord cell)
    {
        if (allObstacleCoord.Contains(neighbours) || cell.UnavaliableNeighbours.Contains(neighbours))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void CreateMapLines()
    {
        //Close all sides Left Right
        for (int i = -1; i <= currentMap.mapSize.x+1; i += currentMap.mapSize.x+1 )
        {
            for (int j = -1; j < currentMap.mapSize.y+1; j++)
            {
                Vector3 tilePosition = CoordToPosition(i, j);
                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90)) as Transform;
                newTile.localScale = Vector3.one * (1 - outlinePercent) * tileSize;
                newTile.parent = mapHolder;

                float obstacleHeight = Mathf.Lerp(currentMap.minObstacleHeight, currentMap.maxObstacleHeight, (float)prng.NextDouble());
                Transform newObstacle = Instantiate(obstaclePrefab[UnityEngine.Random.Range(0, obstaclePrefab.Length)], tilePosition + Vector3.up * obstacleHeight / 2, Quaternion.identity) as Transform;
                newObstacle.parent = mapHolder;
                newObstacle.localScale = new Vector3((1 - outlinePercent) * tileSize, obstacleHeight, (1 - outlinePercent) * tileSize);
            }
        }

        //Close all sides Top Bottom
        for (int j = -1; j <= currentMap.mapSize.y+1; j += currentMap.mapSize.y+1 )
        {
            for (int i = 0; i < currentMap.mapSize.x; i++)
            {
                Vector3 tilePosition = CoordToPosition(i, j);
                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90)) as Transform;
                newTile.localScale = Vector3.one * (1 - outlinePercent) * tileSize;
                newTile.parent = mapHolder;

                float obstacleHeight = Mathf.Lerp(currentMap.minObstacleHeight, currentMap.maxObstacleHeight, (float)prng.NextDouble());
                Transform newObstacle = Instantiate(obstaclePrefab[UnityEngine.Random.Range(0, obstaclePrefab.Length)], tilePosition + Vector3.up * obstacleHeight / 2, Quaternion.identity) as Transform;
                newObstacle.parent = mapHolder;
                newObstacle.localScale = new Vector3((1 - outlinePercent) * tileSize, obstacleHeight, (1 - outlinePercent) * tileSize);
            }
        }
    }

    void SpawnVehicle()
    {
        vehiclePrefab.transform.position = new Vector3(currentMap.startPoint.x, 0.6f, currentMap.startPoint.y)*tileSize;
    }

    private Coord GetRandomOpenCoord()
    {
        var rnd = UnityEngine.Random.Range(0, allOpenCoords.Count);
        return new Coord(allOpenCoords[rnd].x,allOpenCoords[rnd].y);
    }

    private float GetDistance(Coord point1, Coord point2) => Vector3.Distance(CoordToPosition(point1.x,point1.y), CoordToPosition(point2.x,point2.y))/tileSize;

   private void CreateStartandTargetPoints()
    {
        currentMap.startPoint = GetRandomOpenCoord();
        CreatePath();
        currentMap.targetPoint = Path[Path.Count - 1];

        var start = currentMap.startPoint;
        var target = Path[Path.Count-1];

        tileMap[start.x, start.y].gameObject.GetComponent<MeshRenderer>().material.color = Color.blue;
        tileMap[target.x, target.y].gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
        

    }

    private void SpawnObstacle(Random prng)
    {
        // Spawning obstacles
        bool[,] obstacleMap = new bool[(int)currentMap.mapSize.x, (int)currentMap.mapSize.y];

        int obstacleCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y * currentMap.obstaclePercent);
        int currentObstacleCount = 0;
        allOpenCoords = new List<Coord>(allTileCoords);

        for (int i = 0; i < obstacleCount; i++)
        {
            Coord randomCoord = GetRandomCoord();
            obstacleMap[randomCoord.x, randomCoord.y] = true;
            currentObstacleCount++;

            if (randomCoord != currentMap.mapCentre && MapIsFullyAccessible(obstacleMap, currentObstacleCount))
            {
                ObstacleInstantiate(randomCoord, prng);
            }
            else
            {
                obstacleMap[randomCoord.x, randomCoord.y] = false;
                currentObstacleCount--;
            }
        }
       
        shuffledOpenTileCoords = new Queue<Coord>(Utility.ShuffleArray(allOpenCoords.ToArray(), currentMap.seed));
    }

    public void ObstacleInstantiate(Coord randomCoord,Random prng)
    {
        float obstacleHeight = Mathf.Lerp(currentMap.minObstacleHeight, currentMap.maxObstacleHeight, (float)prng.NextDouble());
        Vector3 obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y);

        Transform newObstacle = Instantiate(obstaclePrefab[UnityEngine.Random.Range(0,obstaclePrefab.Length)], obstaclePosition + Vector3.up * obstacleHeight / 2, Quaternion.identity) as Transform;
        newObstacle.parent = mapHolder;
        newObstacle.localScale = new Vector3((1 - outlinePercent) * tileSize, obstacleHeight, (1 - outlinePercent) * tileSize);

        Renderer obstacleRenderer = newObstacle.GetComponent<Renderer>();
        Material obstacleMaterial = new Material(obstacleRenderer.sharedMaterial);
        float colourPercent = randomCoord.y / (float)currentMap.mapSize.y;
        obstacleMaterial.color = Color.Lerp(currentMap.foregroundColour, currentMap.backgroundColour, colourPercent);
        obstacleRenderer.sharedMaterial = obstacleMaterial;

        allObstacleCoord.Add(randomCoord);
        allOpenCoords.Remove(randomCoord);
    }

    private void SpawnAllTiles()
    {
        // Spawning tiles
        for (int x = 0; x < currentMap.mapSize.x; x++)
        {
            for (int y = 0; y < currentMap.mapSize.y; y++)
            {
                Vector3 tilePosition = CoordToPosition(x, y);
                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90)) as Transform;
                newTile.localScale = Vector3.one * (1 - outlinePercent) * tileSize;
                newTile.parent = mapHolder;
                tileMap[x, y] = newTile;
            }
        }
    }

    public void GenerateAllTiles()
    {
        // Generating coords
        allTileCoords = new List<Coord>();
        for (int x = 0; x < currentMap.mapSize.x; x++)
        {
            for (int y = 0; y < currentMap.mapSize.y; y++)
            {
                allTileCoords.Add(new Coord(x, y));
            }
        }
        shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray(allTileCoords.ToArray(), currentMap.seed));
    }

    public void CreateNavmeshMask()
    {
        // Creating navmesh mask
        Transform maskLeft = Instantiate(navmeshMaskPrefab, Vector3.left * (currentMap.mapSize.x + maxMapSize.x) / 4f * tileSize, Quaternion.identity) as Transform;
        maskLeft.parent = mapHolder;
        maskLeft.localScale = new Vector3((maxMapSize.x - currentMap.mapSize.x) / 2f, 1, currentMap.mapSize.y) * tileSize;

        Transform maskRight = Instantiate(navmeshMaskPrefab, Vector3.right * (currentMap.mapSize.x + maxMapSize.x) / 4f * tileSize, Quaternion.identity) as Transform;
        maskRight.parent = mapHolder;
        maskRight.localScale = new Vector3((maxMapSize.x - currentMap.mapSize.x) / 2f, 1, currentMap.mapSize.y) * tileSize;

        Transform maskTop = Instantiate(navmeshMaskPrefab, Vector3.forward * (currentMap.mapSize.y + maxMapSize.y) / 4f * tileSize, Quaternion.identity) as Transform;
        maskTop.parent = mapHolder;
        maskTop.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - currentMap.mapSize.y) / 2f) * tileSize;

        Transform maskBottom = Instantiate(navmeshMaskPrefab, Vector3.back * (currentMap.mapSize.y + maxMapSize.y) / 4f * tileSize, Quaternion.identity) as Transform;
        maskBottom.parent = mapHolder;
        maskBottom.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - currentMap.mapSize.y) / 2f) * tileSize;

        navmeshFloor.localScale = new Vector3(maxMapSize.x, maxMapSize.y) * tileSize;
        mapFloor.localScale = new Vector3(currentMap.mapSize.x * tileSize, currentMap.mapSize.y * tileSize);
    }
	
	bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount) {
		bool[,] mapFlags = new bool[obstacleMap.GetLength(0),obstacleMap.GetLength(1)];
		Queue<Coord> queue = new Queue<Coord> ();
		queue.Enqueue (currentMap.mapCentre);
		mapFlags [currentMap.mapCentre.x, currentMap.mapCentre.y] = true;
		
		int accessibleTileCount = 1;
		
		while (queue.Count > 0) {
			Coord tile = queue.Dequeue();
			
			for (int x = -1; x <= 1; x ++) {
				for (int y = -1; y <= 1; y ++) {
					int neighbourX = tile.x + x;
					int neighbourY = tile.y + y;
					if (x == 0 || y == 0) {
						if (neighbourX >= 0 && neighbourX < obstacleMap.GetLength(0) && neighbourY >= 0 && neighbourY < obstacleMap.GetLength(1)) {
							if (!mapFlags[neighbourX,neighbourY] && !obstacleMap[neighbourX,neighbourY]) {
								mapFlags[neighbourX,neighbourY] = true;
								queue.Enqueue(new Coord(neighbourX,neighbourY));
								accessibleTileCount ++;
							}
						}
					}
				}
			}
		}
		
		int targetAccessibleTileCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y - currentObstacleCount);
		return targetAccessibleTileCount == accessibleTileCount;
	}
	
	Vector3 CoordToPosition(int x, int y) {
		//return new Vector3 (-currentMap.mapSize.x / 2f + 0.5f + x, 0, -currentMap.mapSize.y / 2f + 0.5f + y) * tileSize;
		return new Vector3 (x , 0, y) * tileSize;
	}

	public Transform GetTileFromPosition(Vector3 position) {
		int x = Mathf.RoundToInt(position.x / tileSize + (currentMap.mapSize.x - 1) / 2f);
		int y = Mathf.RoundToInt(position.z / tileSize + (currentMap.mapSize.y - 1) / 2f);
		x = Mathf.Clamp (x, 0, tileMap.GetLength (0) -1);
		y = Mathf.Clamp (y, 0, tileMap.GetLength (1) -1);
		return tileMap [x, y];
	}
	
	public Coord GetRandomCoord() {
		Coord randomCoord = shuffledTileCoords.Dequeue ();
		shuffledTileCoords.Enqueue (randomCoord);
		return randomCoord;
	}

	public Transform GetRandomOpenTile() {
		Coord randomCoord = shuffledOpenTileCoords.Dequeue ();
		shuffledOpenTileCoords.Enqueue (randomCoord);
		return tileMap[randomCoord.x,randomCoord.y];
	}
	
	[System.Serializable]
	public struct Coord:IEquatable<Coord>{

		public int x;
		public int y;
	    public List<Coord> UnavaliableNeighbours;

        public List<Coord> GetNeighbours()
	    {
	        return new List<Coord>
	        {
	            new Coord(x - 1, y),
	            new Coord(x + 1, y),
	            new Coord(x, y - 1),
	            new Coord(x, y + 1)
	        };
	    }

	    public bool Equals(Coord other)
	    {
	        return x == other.x && y==other.y;
	    }

        public Coord(int _x, int _y) {
			x = _x;
			y = _y;
            UnavaliableNeighbours = new List<Coord>();
        }
		
		public static bool operator ==(Coord c1, Coord c2) {
			return c1.x == c2.x && c1.y == c2.y;
		}
		
		public static bool operator !=(Coord c1, Coord c2) {
			return !(c1 == c2);
		}
		
	}
	
	[System.Serializable]
	public class Map {
		
		public Coord mapSize;
		[Range(0,1)]
		public float obstaclePercent;
		public int seed;
		public float minObstacleHeight;
		public float maxObstacleHeight;
		public Color foregroundColour;
		public Color backgroundColour;
	    public Coord startPoint;
	    public Coord targetPoint;

        public Coord mapCentre => new Coord(mapSize.x/2,mapSize.y/2);

    }

    public class Timer
    {
        private float StartTime;
        public float Duration;

        public Timer()
        {
            StartTime = Time.realtimeSinceStartup;
        }

        public void Finish(bool log)
        {
            var endTime = Time.realtimeSinceStartup;
            Duration = endTime - StartTime;

            if (log)
            {
                Debug.Log($"Calculations took {TimeSpan.FromSeconds(Duration).ToString("g")}");
            }
        }
    }
}
