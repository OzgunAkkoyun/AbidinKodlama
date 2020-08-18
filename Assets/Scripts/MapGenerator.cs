using UnityEngine;
using System.Collections.Generic;
using System;
using Random = System.Random;

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
    public ALLSpawnObjects allSpawnObjects;
    public Transform tilePrefab;
    public Transform[] obstaclePrefab;
    public Transform navmeshFloor;
    public Transform navmeshMaskPrefab;
    public GameObject vehiclePrefab;
    public Transform mapFloor;

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

    public GameObject[] home;
    public List<GameObject> obstacleGameObject;

    private GameManager gm;

    void Start()
    {
        
       
    }

    public void VariableAssign()
    {
        gm = FindObjectOfType<GameManager>();
        currentMap = maps[mapIndex];
        var senarioObjects = allSpawnObjects.allSpawnObjects[gm.playerDatas.whichScenario - 1];
        tilePrefab = senarioObjects.tileGameObject.transform;
        obstaclePrefab = new Transform[senarioObjects.environmentGameObjects.Length];
        for (int i = 0; i < senarioObjects.environmentGameObjects.Length; i++)
        {
            obstaclePrefab[i] = senarioObjects.environmentGameObjects[i].transform;
        }

        vehiclePrefab = senarioObjects.vehicleGameObject;
        home[0] = senarioObjects.startGameObject;
        home[1] = senarioObjects.targetGameObject;
    }
    public void GameStart()
    {
        VariableAssign();
        GenerateMap();
    }

    public void GameStartForLoad(Coord _mapSize, int _seed, Coord _startPoint, Coord _targetPoint, List<Coord> _Path)
    {
        VariableAssign();
        GenerateMapFromLoad(_mapSize, _seed, _startPoint, _targetPoint, _Path);
    }

    public void GenerateMapFromLoad(Coord _mapSize,int _seed,Coord _startPoint,Coord _targetPoint,List<Coord> _Path)
    {
        currentMap.mapSize = _mapSize;
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

        SpawnVehicle();
        SpawnHouses();
    }
	public void GenerateMap()
	{
	    Timer timer = new Timer();
        tileMap = new Transform[gm.playerDatas.lastMapSize, gm.playerDatas.lastMapSize];

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
        SpawnHouses();
        timer.Finish(false);
    }

    public void SpawnHouses()
    {
        var startHome = Instantiate(home[0], new Vector3((float)currentMap.startPoint.x * tileSize, 1f, (float)currentMap.startPoint.y * tileSize), Quaternion.identity);
        var targetHome = Instantiate(home[1], new Vector3((float)currentMap.targetPoint.x * tileSize, 1f, (float)currentMap.targetPoint.y * tileSize), Quaternion.identity);
        targetHome.name = "Target";
        startHome.transform.LookAt(new Vector3(Path[1].x * tileSize, 1, Path[1].y * tileSize));

        targetHome.transform.LookAt(new Vector3(Path[PathLength-2].x*tileSize, 1, Path[PathLength - 2].y*tileSize));
    }
    public void CreatePath()
    {
        Path.Clear();
        var currentPathIndex = 0;
        Path.Add(new Coord(currentMap.startPoint.x,currentMap.startPoint.y));
        
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

        //SetPathDirections();
    }

    //For Loop variables
    public enum forLoopDirections {Left,Right,Up,Down }

    public List<forLoopDirections> directions = new List<forLoopDirections>();

    public int xSize => Mathf.Max(left, right);
    public int ySize => Mathf.Max(up, down);
    public int left;
    public int right;
    public int up;
    public int down;
    public void CreatePathWithForLoop()
    {
        Path.Clear();
        var currentPathIndex = 0;
        Path.Add(new Coord(currentMap.startPoint.x, currentMap.startPoint.y));

        FindXDirection();
        FindYDirection();

        CreateMapWithDirections(ref currentPathIndex);

        for (int i = 0; i < Path.Count; i++)
        {
            var index = allObstacleCoord.FindIndex(v => (v.x == Path[i].x) && (v.y == Path[i].y));
            if (index >= 0)
            {
                DestroyImmediate(obstacleGameObject[index]);
            }
        }
    }

    private void CreateMapWithDirections(ref int currentPathIndex)
    {
        var xLenght = UnityEngine.Random.Range(2, xSize);
        var yLenght = UnityEngine.Random.Range(2, ySize);
        if (directions[0] == forLoopDirections.Left)
        {
            for (int i = 0; i < xLenght; i++)
            {
                Path.Add(Path[currentPathIndex].GetNeighbours()[0]);
                currentPathIndex++;
            }
        }
        else if (directions[0] == forLoopDirections.Right)
        {
            for (int i = 0; i < xLenght; i++)
            {
                Path.Add(Path[currentPathIndex].GetNeighbours()[1]);
                currentPathIndex++;
            }
        }
        if(directions[1] == forLoopDirections.Down)
        {
            for (int i = 0; i < yLenght; i++)
            {
                Path.Add(Path[currentPathIndex].GetNeighbours()[2]);
                currentPathIndex++;
            }
        }
        else if (directions[1] == forLoopDirections.Up)
        {
            for (int i = 0; i < yLenght; i++)
            {
                Path.Add(Path[currentPathIndex].GetNeighbours()[3]);
                currentPathIndex++;
            }
        }
    }

    private void FindXDirection()
    {
        left = Mathf.Abs(currentMap.startPoint.x);
        right = Mathf.Abs(currentMap.mapSize.x - currentMap.startPoint.x - 1);

        if (left >= right)
            directions.Add(forLoopDirections.Left);
        else
            directions.Add(forLoopDirections.Right);
    }

    private void FindYDirection()
    {
        up = Mathf.Abs(currentMap.mapSize.y - currentMap.startPoint.y - 1);
        down = Mathf.Abs(currentMap.startPoint.y);

        if (up >= down)
            directions.Add(forLoopDirections.Up);
        else
            directions.Add(forLoopDirections.Down);
    }

    //private void SetPathDirections()
    //{
    //    for (int i = 0; i < Path.Count; i++)
    //    {
    //        if (i == 0)
    //        {
    //            Path[i].direction = Coord.directions.Empty;
    //        }
    //    }
    //}
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

    private Coord GetRandomOpenCoord()
    {
        var rnd = UnityEngine.Random.Range(0, allOpenCoords.Count);
        return new Coord(allOpenCoords[rnd].x,allOpenCoords[rnd].y);
    }

    private float GetDistance(Coord point1, Coord point2) => Vector3.Distance(CoordToPosition(point1.x,point1.y), CoordToPosition(point2.x,point2.y))/tileSize;

   private void CreateStartandTargetPoints()
    {
        currentMap.startPoint = GetRandomOpenCoord();
        //CreatePath();
        CreatePathWithForLoop();
        currentMap.targetPoint = Path[Path.Count - 1];

        var start = currentMap.startPoint;
        var target = Path[Path.Count-1];

        //tileMap[start.x, start.y].gameObject.GetComponent<MeshRenderer>().material.color = Color.blue;
        //tileMap[target.x, target.y].gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
    }

    #region Spawners
    void SpawnVehicle()
    {
        var vehiclePos = new Vector3(currentMap.startPoint.x, 0.6f, currentMap.startPoint.y) * tileSize;

        vehiclePrefab = Instantiate(vehiclePrefab, vehiclePos, Quaternion.identity);
        gm.uh.mainCamera = vehiclePrefab.transform.Find("Main Camera").gameObject;
        gm.uh.cameraTarget = vehiclePrefab.transform.Find("CameraTarget");
        //vehiclePrefab.transform.position = new Vector3(currentMap.startPoint.x, 0.6f, currentMap.startPoint.y)*tileSize;
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

    public void ObstacleInstantiate(Coord randomCoord, Random prng)
    {
        float obstacleHeight = Mathf.Lerp(currentMap.minObstacleHeight, currentMap.maxObstacleHeight, (float)prng.NextDouble());
        Vector3 obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y);

        Transform newObstacle = Instantiate(obstaclePrefab[UnityEngine.Random.Range(0, obstaclePrefab.Length)], obstaclePosition + Vector3.up * obstacleHeight / 2, Quaternion.identity) as Transform;
        newObstacle.parent = mapHolder;
        newObstacle.localScale = new Vector3((1 - outlinePercent) * tileSize, obstacleHeight, (1 - outlinePercent) * tileSize);

        Renderer obstacleRenderer = newObstacle.GetComponent<Renderer>();
        Material obstacleMaterial = new Material(obstacleRenderer.sharedMaterial);
        float colourPercent = randomCoord.y / (float)currentMap.mapSize.y;
        obstacleMaterial.color = Color.Lerp(currentMap.foregroundColour, currentMap.backgroundColour, colourPercent);
        obstacleRenderer.sharedMaterial = obstacleMaterial;

        obstacleGameObject.Add(newObstacle.gameObject);
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

    #endregion

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
	
	public Vector3 CoordToPosition(int x, int y) {
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

    public Transform GetRandomOpenTile()
    {
        Coord randomCoord = shuffledOpenTileCoords.Dequeue();
        shuffledOpenTileCoords.Enqueue(randomCoord);
        return tileMap[randomCoord.x, randomCoord.y];
    }

    [System.Serializable]
    public struct Coord : IEquatable<Coord>
    {
        public int x;
        public int y;
        public List<Coord> UnavaliableNeighbours;

        //public enum directions
        //{
        //    Left,
        //    Right,
        //    Forward,
        //    Backward,
        //    Empty
        //};

        //public directions direction;

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
            return x == other.x && y == other.y;
        }

        public Coord(int _x, int _y)
        {
            x = _x;
            y = _y;
            UnavaliableNeighbours = new List<Coord>();
            //direction = directions.Empty;
        }

        public static bool operator ==(Coord c1, Coord c2)
        {
            return c1.x == c2.x && c1.y == c2.y;
        }

        public static bool operator !=(Coord c1, Coord c2)
        {
            return !(c1 == c2);
        }
    }

    [System.Serializable]
	public class Map
    {
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
