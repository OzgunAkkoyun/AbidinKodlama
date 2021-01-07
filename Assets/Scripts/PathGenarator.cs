using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static MapGenerator;
using Random = System.Random;

public enum AnimalsInIfPath
{
    isAnimalCoord, isEmptyAnimalCoord,Empty
}
public class PathGenarator : MonoBehaviour
{
    public MapGenerator mapGenerator;
    public GameManager gm;
    public ChangeEnvironment changeEnvironment;
    public List<Coord> Path = new List<Coord>();
    public int expectedPathLength;
    public int PathLength => Path.Count;

    #region LoopPathGenerate
    //For Loop variables
    public enum forLoopDirections { Left, Right, Up, Down }

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
        Path.Add(new Coord(mapGenerator.currentMap.startPoint.x, mapGenerator.currentMap.startPoint.y));

        FindXDirection();
        FindYDirection();

        CreateMapWithDirections(ref currentPathIndex);
        SetPathDirections();

        ChangeEnvironment();
    }

    public void ChangeEnvironment()
    {
        changeEnvironment.DestroyObstaclesInPath();
        RemovePathinOpenCoord();
        InstantiateObstaclePathSide();
        changeEnvironment.AddForestEmptyTiles();
        changeEnvironment.AddStreetLightsToPath();
    }

    public void RemovePathinOpenCoord()
    {
        for (int i = 0; i < Path.Count; i++)
        {
            mapGenerator.allOpenCoords.Remove(Path[i]);
        }
    }

    public void InstantiateObstaclePathSide()
    {
        for (int i = 0; i < Path.Count; i++)
        {
            var currentPath = Path[i];
            var neightbours = currentPath.GetNeighbours();
            for (int j = 0; j < neightbours.Count; j++)
            {
                if (CellinBounds(neightbours[j]))
                {
                    var indexObstacle = mapGenerator.allObstacleCoord.FindIndex(v => (v.x == neightbours[j].x) && (v.y == neightbours[j].y));

                    var indexPath = Path.FindIndex(v => (v.x == neightbours[j].x) && (v.y == neightbours[j].y));

                    if (indexObstacle < 0 && indexPath < 0)
                    {
                        mapGenerator.ObstacleInstantiate(neightbours[j]);
                    }
                }
            }
        }
    }

    private void FindXDirection()
    {
        left = Mathf.Abs(mapGenerator.currentMap.startPoint.x);
        right = Mathf.Abs(mapGenerator.currentMap.mapSize.x - mapGenerator.currentMap.startPoint.x - 1);

        if (left >= right)
            directions.Add(forLoopDirections.Left);
        else
            directions.Add(forLoopDirections.Right);
    }

    private void FindYDirection()
    {
        up = Mathf.Abs(mapGenerator.currentMap.mapSize.y - mapGenerator.currentMap.startPoint.y - 1);
        down = Mathf.Abs(mapGenerator.currentMap.startPoint.y);

        if (up >= down)
            directions.Add(forLoopDirections.Up);
        else
            directions.Add(forLoopDirections.Down);
    }

    private void CreateMapWithDirections(ref int currentPathIndex)
    {
        var xLenght = UnityEngine.Random.Range(2, xSize);
        var yLenght = UnityEngine.Random.Range(2, ySize);

        if (gm.currentLevel.levelIndex == 1)
        {
            var random = UnityEngine.Random.Range(0, 2);

            if (random == 0)
                AddPathinXDirection(ref currentPathIndex, xLenght);
            else
                AddPathinYDirection(ref currentPathIndex, xLenght);
        }
        else if (gm.currentLevel.levelIndex == 2)
        {
            AddPathinXDirection(ref currentPathIndex, xLenght);
            AddPathinYDirection(ref currentPathIndex, yLenght);
        }
        else if (gm.currentLevel.levelIndex == 3)
        {
            AddPathXYDirectionsTogether(ref currentPathIndex, xLenght, yLenght);
        }
    }

    private void AddPathinXDirection(ref int currentPathIndex, int xLenght)
    {
        if (directions[0] == forLoopDirections.Left)
        {
            for (int i = 0; i < expectedPathLength; i++)
            {
                Path.Add(Path[currentPathIndex].GetNeighbours()[0]);
                currentPathIndex++;
            }
        }
        else if (directions[0] == forLoopDirections.Right)
        {
            for (int i = 0; i < expectedPathLength; i++)
            {
                Path.Add(Path[currentPathIndex].GetNeighbours()[1]);
                currentPathIndex++;
            }
        }
    }

    private void AddPathinYDirection(ref int currentPathIndex, int yLenght)
    {
        if (directions[1] == forLoopDirections.Down)
        {
            for (int i = 0; i < expectedPathLength; i++)
            {
                Path.Add(Path[currentPathIndex].GetNeighbours()[2]);
                currentPathIndex++;
            }
        }
        else if (directions[1] == forLoopDirections.Up)
        {
            for (int i = 0; i < expectedPathLength; i++)
            {
                Path.Add(Path[currentPathIndex].GetNeighbours()[3]);
                currentPathIndex++;
            }
        }
    }

    private void AddPathXYDirectionsTogether(ref int currentPathIndex, int xLenght, int yLenght)
    {
        for (int i = 0; i < expectedPathLength-1; i++)
        {
            if (directions[0] == forLoopDirections.Left)
            {
                Path.Add(Path[currentPathIndex].GetNeighbours()[0]);
                currentPathIndex++;
            }
            else if (directions[0] == forLoopDirections.Right)
            {
                Path.Add(Path[currentPathIndex].GetNeighbours()[1]);
                currentPathIndex++;
            }

            if (directions[1] == forLoopDirections.Down)
            {
                Path.Add(Path[currentPathIndex].GetNeighbours()[2]);
                currentPathIndex++;
            }
            else if (directions[1] == forLoopDirections.Up)
            {
                Path.Add(Path[currentPathIndex].GetNeighbours()[3]);
                currentPathIndex++;
            }
        }
    }

    public Coord GetRandomStartCoord()
    {
        int[] startCoordToSelect = new[] { 0, mapGenerator.currentMap.mapSize.x - 1 };
        var rnd = UnityEngine.Random.Range(0, 2);
        var rnd1 = UnityEngine.Random.Range(0, 2);

        var xPos = startCoordToSelect[rnd];
        var yPos = startCoordToSelect[rnd1];

        return new Coord(xPos, yPos);
    }
    #endregion

    #region MovePathGenerate

    public void CreatePath()
    {
        Path.Clear();
        var currentPathIndex = 0;
        Path.Add(new Coord(mapGenerator.currentMap.startPoint.x, mapGenerator.currentMap.startPoint.y));

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
        //changeEnvironment.AddSignNumber();
        SetPathDirections();
    }
    private void OneStepBackinList(Coord currentCell, ref int currentPathIndex)
    {
        Path.RemoveAt(currentPathIndex);
        currentPathIndex--;
        Path[currentPathIndex].UnavaliableNeighbours.Add(new Coord(currentCell.x, currentCell.y));
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
            selectedNeighbour = new Coord(0, 0);
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
        if (neighbours.x < 0 || neighbours.x >= mapGenerator.currentMap.mapSize.x || neighbours.y < 0 || neighbours.y >= mapGenerator.currentMap.mapSize.y)
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
        if (mapGenerator.allObstacleCoord.Contains(neighbours) || cell.UnavaliableNeighbours.Contains(neighbours))
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    #endregion

    #region IfPathGenerate
    //[Serializable]
    //public class IfObjects
    //{
    //    [Serializable]
    //    public class IfObjectsForLevel
    //    {
    //        public string ifName;
    //        public GameObject ifGameObjects;
    //        public Sprite ifGameObjectsImage;
    //    }
    //    public IfObjectsForLevel[] ifObjectsForLevels;

    //}
    //[Space(15f)]
    //[Header("If Objects")]
    //public IfObjects[] ifObjects;

    public IfObjectsScriptable ifObjectsScriptable;
    public GameObject smoke;

    public IfObjectsScriptable.IfObjects.IfObjectsForLevel currentIfObject;
    public IfObjectsScriptable.IfObjects allIfObjects;

    public List<IfObjectsScriptable.IfObjects.IfObjectsForLevel> selectedAnimals = new List<IfObjectsScriptable.IfObjects.IfObjectsForLevel>();
    public RotateToyUi rotateToyUi;
    public List<GameObject> animals = new List<GameObject>();
    public List<GameObject> justSmoke = new List<GameObject>();
    
    public void PrepareAnimals()
    {
        currentIfObject = ifObjectsScriptable.GetCurrentIfObjects(gm.currentLevel.levelIndex, gm.currentSubLevel.subLevelIndex);
        allIfObjects = ifObjectsScriptable.GetAllIfObjects(gm.currentLevel.levelIndex);
    }
    public void CreatePathWithIfStatement()
    {
        Path = mapGenerator.allOpenCoords;
        PrepareAnimals();
        //rotateToyUi.SetAllIfObjectsInContainer(3);
        rotateToyUi.SetAllIfObjectsInWheel(3);
        SetIfObjects();
    }

    private void SetIfObjects()
    {
        var maxObject = gm.currentSubLevel.maxIfObjectCount;
        var howManyObject = gm.currentSubLevel.ifObjectCount;
        var levelIndex = gm.currentLevel.levelIndex;
        var subLevelIndex = gm.currentSubLevel.subLevelIndex;
        
        for (int i = 0; i < maxObject-howManyObject; i++)
        {
            var whichPathHaveObject = UnityEngine.Random.Range(1, PathLength - 1);
            var selectedPath = Path[whichPathHaveObject];

            if (selectedPath.whichCoord == AnimalsInIfPath.Empty)
            {
                Path[whichPathHaveObject].whichCoord = AnimalsInIfPath.isEmptyAnimalCoord;
                var spawnPosition = mapGenerator.CoordToPosition(selectedPath.x, selectedPath.y);
                var onlySmoke = Instantiate(smoke, spawnPosition+Vector3.up, Quaternion.identity);
                justSmoke.Add(onlySmoke);
            }
        }
        
        for (int i = 0; i < howManyObject;)
        {
            var whichPathHaveObject = UnityEngine.Random.Range(1, PathLength - 1);

            var selectedPath = Path[whichPathHaveObject];

            var selectedAnimal = currentIfObject;

            if (selectedPath.whichCoord == AnimalsInIfPath.Empty)
            {
                Path[whichPathHaveObject].whichCoord = AnimalsInIfPath.isAnimalCoord;
                var spawnPosition = mapGenerator.CoordToPosition(selectedPath.x, selectedPath.y);
                var animal = Instantiate(selectedAnimal.ifGameObjects, spawnPosition, Quaternion.identity);
                animals.Add(animal);
                i++;
            }
        }
    }

    public void SetIfAnimalsForLoad()
    {
        PrepareAnimals();
        //rotateToyUi.SetAllIfObjectsInContainer(3);
        rotateToyUi.SetAllIfObjectsInWheel(3);
        for (int i = 0; i < PathLength; i++)
        {
            var selectedPath = Path[i];
            if (Path[i].whichCoord == AnimalsInIfPath.isEmptyAnimalCoord)
            {
                var spawnPosition = mapGenerator.CoordToPosition(selectedPath.x, selectedPath.y);
                var onlySmoke = Instantiate(smoke, spawnPosition + Vector3.up, Quaternion.identity);
                justSmoke.Add(onlySmoke);
            }
            else if (Path[i].whichCoord == AnimalsInIfPath.isAnimalCoord)
            {
                var levelIndex = gm.currentLevel.levelIndex;
                var subLevelIndex = gm.currentSubLevel.subLevelIndex;
                var selectedAnimal = currentIfObject;
                var spawnPosition = mapGenerator.CoordToPosition(selectedPath.x, selectedPath.y);
                var animal = Instantiate(selectedAnimal.ifGameObjects, spawnPosition, Quaternion.identity);
                animals.Add(animal);
            }
        }
    }

    #endregion

    #region WaitPathGenerate
    
    [Serializable]
    public class WaitObjects
    {
        [Serializable]
        public class DirtsForLevel
        {
            public string waitName;
            public int seconds;
        }
        public DirtsForLevel[] dirts;
    }
    [Space(15f)]
    [Header("Wait Objects")]
    public WaitObjects[] waitObjects;

    public List<WaitObjects.DirtsForLevel> currentDirts = new List<WaitObjects.DirtsForLevel>();
    public WaitObjects currentDirtObject;

    [Serializable]
    public class WaitMetarials
    {
        public string waitName;
        public Material dirtMaterial;
        
    }
    [Space(5f)]
    [Header("Wait Metarials")]
    public WaitMetarials[] waitMetarials;
    public void CreatePathForWait()
    {
        Path = mapGenerator.allOpenCoords;
    }

    public void SetDirtInPath()
    {
        var levelIndex = gm.currentLevel.levelIndex;
        var subLevelIndex = gm.currentSubLevel.subLevelIndex;

        var dirtCount = gm.currentSubLevel.dirtCount;

        currentDirtObject = waitObjects[gm.currentLevel.levelIndex - 1];
        var selectedDirtIndex = -1;

        for (int i = 0; i < dirtCount; )
        {
            selectedDirtIndex = GetDirtMetarialIndex();

            var selectedDirtObject = currentDirtObject.dirts.ToList().Find(v => v.waitName == waitMetarials[selectedDirtIndex].waitName);
            
            currentDirts.Add(selectedDirtObject);

            var whichPathHaveObject = UnityEngine.Random.Range(1, PathLength - 1);
            var selectedPath = Path[whichPathHaveObject];

            if (selectedPath.whichDirt == null)
            {
                selectedPath.whichDirt = selectedDirtObject;
               
                var PathIndex = mapGenerator.allTileCoords.FindIndex(v =>
                    (v.x == selectedPath.x) && (v.y == selectedPath.y));

                var selectedTile = mapGenerator.allTileGameObject[PathIndex].gameObject;

               selectedTile.GetComponent<Renderer>().material = waitMetarials[selectedDirtIndex].dirtMaterial;
                i++;
            }
        }
        
    }

    private int GetDirtMetarialIndex()
    {
        var index = -1;

        if (gm.currentLevel.levelIndex == 1)
        {
            index = 0;
        }
        else if (gm.currentLevel.levelIndex == 2)
        {
            index = UnityEngine.Random.Range(1, 3);
        }
        else if (gm.currentLevel.levelIndex == 3)
        {
            index = UnityEngine.Random.Range(3, 6);
        }

        return index;
    }

    public void SetDirtForLoad()
    {
        var levelIndex = gm.currentLevel.levelIndex;
        var subLevelIndex = gm.currentSubLevel.subLevelIndex;

        var dirtCount = gm.currentLevel.subLevels[subLevelIndex - 1].dirtCount;

        currentDirtObject = waitObjects[gm.currentLevel.levelIndex - 1];

       var allDirtCoords = gm.gameDatas[gm.gameDatas.Count - 1].Path.FindAll(v => v.whichDirt != null);

        for (int i = 0; i < allDirtCoords.Count; i++)
        {
            var PathIndex = mapGenerator.allTileCoords.FindIndex(v =>
                (v.x == allDirtCoords[i].x) && (v.y == allDirtCoords[i].y));

            currentDirts.Add(allDirtCoords[i].whichDirt);
            mapGenerator.allTileGameObject[PathIndex].gameObject.GetComponent<Renderer>().material = waitMetarials.ToList().Find(v => v.waitName == allDirtCoords[i].whichDirt.waitName).dirtMaterial;
        }
    }

    #endregion

    #region WholePathGenerate

    [Serializable]
    public class WholeObjects
    {
        [Serializable]
        public class IfObjects
        {
            public string ifName;
            public int gameObjectIndex;
        }
        public IfObjects[] ifObjects;

        [Serializable]
        public class WaitObjects
        {
            public string waitName;
            public int seconds;
        }
        public WaitObjects[] waitObjects;
    }
    [Space(15f)]
    [Header("Whole Objects")]
    public WholeObjects[] wholeObjects;


    [Serializable]
    public class WholeMetarials
    {
        [Serializable]
        public class IfMetarials
        {
            public string ifName;
            public GameObject ifGameObject;
            public Sprite ifImages;
        }

        public IfMetarials[] ifMetarials;
        [Serializable]
        public class WaitMetarials
        {
            public string waitName;
            public Material waitMaterial;
        }
        public WaitMetarials[] waitMetarials;
    }
    [Space(5f)]
    [Header("Whole Metarials")]
    public WholeMetarials[] wholeMetarials;

    public WholeObjects.IfObjects currentWholeIfObjects;
    public WholeMetarials.IfMetarials currentWholeIfMaterials;

    public WholeObjects.WaitObjects currentWholeWaitObjects;
    public WholeMetarials.WaitMetarials currentWholeWaitMaterials;

    public GameObject emtyMushroomPrefab;

    public List<GameObject> wholeIfObjectsList = new List<GameObject>();

    public List<AnimalsInIfPath> selectedMushrooms = new List<AnimalsInIfPath>();
    public void CreatePathWithWhole()
    {
        Path = mapGenerator.allOpenCoords;

        currentWholeIfObjects = wholeObjects[gm.currentLevel.levelIndex-1].ifObjects[gm.currentSubLevel.subLevelIndex-1];
        currentWholeIfMaterials = wholeMetarials[gm.currentLevel.levelIndex-1].ifMetarials[gm.currentSubLevel.subLevelIndex-1];

        currentWholeWaitObjects = wholeObjects[gm.currentLevel.levelIndex-1].waitObjects[gm.currentSubLevel.subLevelIndex-1];
        currentWholeWaitMaterials = wholeMetarials[gm.currentLevel.levelIndex-1].waitMetarials[gm.currentSubLevel.subLevelIndex-1];

        rotateToyUi.SetWholeIfObjectsInWheel(2);
    }

    public void SetWholeObjectsInPath()
    {
        var maxObject = gm.currentSubLevel.maxIfObjectCount;
        var howManyObject = gm.currentSubLevel.ifObjectCount;
        var levelIndex = gm.currentLevel.levelIndex;
        var subLevelIndex = gm.currentSubLevel.subLevelIndex;

        SetWholeIfObjects(maxObject, howManyObject, levelIndex, subLevelIndex);
        SetWholeWaitObjects(levelIndex, subLevelIndex);
    }

    private void SetWholeWaitObjects(int levelIndex, int subLevelIndex)
    {
        var dirtCount = gm.currentSubLevel.dirtCount;

        for (int i = 0; i < dirtCount;)
        {
            var selectedDirtObject = currentWholeWaitMaterials;

            var whichPathHaveObject = UnityEngine.Random.Range(1, PathLength - 1);
            var selectedPath = Path[whichPathHaveObject];

            if (selectedPath.whichDirt == null)
            {
                var PathIndex = mapGenerator.allTileCoords.FindIndex(v =>
                    (v.x == selectedPath.x) && (v.y == selectedPath.y));

                Path[PathIndex].whichWaitObject = currentWholeWaitObjects;

                var selectedTile = mapGenerator.allTileGameObject[PathIndex].gameObject;

                selectedTile.GetComponent<Renderer>().material = currentWholeWaitMaterials.waitMaterial;
                i++;
            }
        }
    }

    private void SetWholeIfObjects(int maxObject, int howManyObject, int levelIndex, int subLevelIndex)
    {
        for (int i = 0; i < maxObject - howManyObject; i++)
        {
            var whichPathHaveObject = UnityEngine.Random.Range(1, PathLength - 1);
            var selectedPath = Path[whichPathHaveObject];

            if (selectedPath.whichCoord == AnimalsInIfPath.Empty)
            {
                Path[whichPathHaveObject].whichCoord = AnimalsInIfPath.isEmptyAnimalCoord;
                var spawnPosition = mapGenerator.CoordToPosition(selectedPath.x, selectedPath.y);
                var emptyMushroom = Instantiate(emtyMushroomPrefab, spawnPosition + Vector3.up, Quaternion.identity);
                wholeIfObjectsList.Add(emptyMushroom);
            }
        }

        for (int i = 0; i < howManyObject;)
        {
            var whichPathHaveObject = UnityEngine.Random.Range(1, PathLength - 1);
            var selectedPath = Path[whichPathHaveObject];
            var selectedAnimal = currentWholeIfMaterials;

            if (selectedPath.whichCoord == AnimalsInIfPath.Empty)
            {
                Path[whichPathHaveObject].whichCoord = AnimalsInIfPath.isAnimalCoord;

                var spawnPosition = mapGenerator.CoordToPosition(selectedPath.x, selectedPath.y);
                var mushroom = Instantiate(selectedAnimal.ifGameObject, spawnPosition, Quaternion.identity);

                wholeIfObjectsList.Add(mushroom);
                i++;
            }
        }
    }

    #endregion
    public Coord GetRandomOpenCoord()
    {
        var rnd = UnityEngine.Random.Range(0, mapGenerator.allOpenCoords.Count);
        
        return new Coord(mapGenerator.allOpenCoords[rnd].x, mapGenerator.allOpenCoords[rnd].y);
    }

    private float GetDistance(Coord point1, Coord point2) => Vector3.Distance(mapGenerator.CoordToPosition(point1.x, point1.y), mapGenerator.CoordToPosition(point2.x, point2.y)) / mapGenerator.tileSize;

    private void SetPathDirections()
    {
        for (int i = 0; i < Path.Count; i++)
        {
            if (i != 0)
            {
                var dist = FindMinusTwoCoord(Path[i], Path[i - 1]);
                if (dist.x < 0)
                {
                    Path[i].pathDirection = Direction.Left;
                }
                else if (dist.x > 0)
                {
                    Path[i].pathDirection = Direction.Right;
                }
                else if (dist.y < 0)
                {
                    Path[i].pathDirection = Direction.Backward;
                }
                else if (dist.y > 0)
                {
                    Path[i].pathDirection = Direction.Forward;
                }
            }
        }
    }

    private Vector2 FindMinusTwoCoord(Coord c1, Coord c2) => new Vector2(c1.x - c2.x, c1.y - c2.y);


}