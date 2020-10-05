using System;
using System.Collections.Generic;
using UnityEngine;
using static MapGenerator;
using Random = System.Random;

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
    [Serializable]
    public class IfObjects
    {
        [Serializable]
        public class AnimalForLevel
        {
            public string name;
            public GameObject animalsGameObjects;

        }
        public AnimalForLevel[] animals;

    }

    public IfObjects[] ifObjects;
        
    public void CreatePathWithIfStatement()
    {
        //CreatePath();
        Path = mapGenerator.allOpenCoords;
        SetIfObjects();
    }

    private void SetIfObjects()
    {
        var howManyObject = UnityEngine.Random.Range(1, gm.currentSubLevel.pathLenght);
        var levelIndex = gm.currentLevel.levelIndex;
        var subLevelIndex = gm.currentSubLevel.subLevelIndex;

        //for (int i = 0; i < 1;)
        //{
            var whichPathHaveObject = UnityEngine.Random.Range(1, PathLength - 1);
            var currentPath = Path[whichPathHaveObject];
            var selectedAnimal = ifObjects[levelIndex-1].animals[subLevelIndex-1];
           
            if (currentPath.animal == null)
            {
                Path[whichPathHaveObject].animal = selectedAnimal;
                var spawnPosition = mapGenerator.CoordToPosition(currentPath.x, currentPath.y);
                Instantiate(selectedAnimal.animalsGameObjects, spawnPosition, Quaternion.identity);
                //i++;
            }
        //}

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