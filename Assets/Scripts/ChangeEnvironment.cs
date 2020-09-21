using TMPro;
using UnityEngine;

public class ChangeEnvironment : MonoBehaviour
{
    public MapGenerator mapGenerator;
    public PathGenarator pathGenarator;

    #region MoveSenario

    [Space(8f)]
    [Header("Move Senario")]
    public Color[] signColors;
    public void AddSignNumber()
    {
        var whichLevelIndex = pathGenarator.gm.currentLevel.levelIndex;
        var whichSubLevelIndex = pathGenarator.gm.currentSubLevel.subLevelIndex;

        var number = (whichLevelIndex - 1) * 3 + whichSubLevelIndex;

        mapGenerator.targetHome.transform.Find("Sign").GetComponentInChildren<TextMeshPro>().text = number.ToString();

        mapGenerator.targetHome.transform.Find("Sign/Group_005").GetComponent<Renderer>().material.color = signColors[whichLevelIndex - 1];

        mapGenerator.targetNewHome.transform.Find("Sign").GetComponentInChildren<TextMeshPro>().text = number.ToString();

        mapGenerator.targetNewHome.transform.Find("Sign/Group_005").GetComponent<Renderer>().material.color = signColors[whichLevelIndex - 1];
    }

    #endregion

    #region LoopSenario
    [Space(8f)]
    [Header("Loop Senario")]
    public GameObject[] forest;
    public Material loopMapPathMaterial;
    public GameObject streetLightPrefab;

    public void AddForestEmptyTiles()
    {
        ChangeLightSettings();
        for (int i = 0; i < mapGenerator.allOpenCoords.Count; i++)
        {
            var openCoord = mapGenerator.allOpenCoords[i];
            var spawnPosition = new Vector3(openCoord.x, 0.5f, openCoord.y) * mapGenerator.tileSize;

            var rnd = Random.Range(0, forest.Length);
            var forestSpawn = Instantiate(forest[rnd], spawnPosition, Quaternion.identity);
        }
    }

    private void ChangeLightSettings()
    {
        var directionalLight = GameObject.Find("Directional Light");
        directionalLight.GetComponent<Light>().color = new Color(.5f,.5f,.5f);
    }

    public void DestroyObstaclesInPath()
    {
        for (int i = 0; i < pathGenarator.Path.Count; i++)
        {
            var index = mapGenerator.allObstacleCoord.FindIndex(v => (v.x == pathGenarator.Path[i].x) && (v.y == pathGenarator.Path[i].y));

            var PathIndex = mapGenerator.allTileCoords.FindIndex(v => (v.x == pathGenarator.Path[i].x) && (v.y == pathGenarator.Path[i].y));

            if (PathIndex > 0)
            {
                mapGenerator.allTileGameObject[PathIndex].gameObject.GetComponent<Renderer>().material =
                    loopMapPathMaterial;
            }

            if (index >= 0)
            {
                DestroyImmediate(mapGenerator.obstacleGameObject[index]);
            }
        }
    }

    public void AddStreetLightsToPath()
    {
        for (int i = 0; i < pathGenarator.PathLength; i++)
        {
            var pathGenaratorPath = pathGenarator.Path;

            if (i == pathGenarator.PathLength-1) return;

            Vector3 LightPos = Vector3.zero;
            Vector3 LightPos1 = Vector3.zero;

            if (pathGenaratorPath[i].pathDirection == Direction.Left || pathGenaratorPath[i].pathDirection == Direction.Right)
            {
                
                if (pathGenaratorPath[i+1].pathDirection != Direction.Forward)
                {
                    LightPos = new Vector3(
                        pathGenaratorPath[i].x * mapGenerator.tileSize,
                        0,
                        pathGenaratorPath[i].y * mapGenerator.tileSize + 1);
                }
                if (pathGenaratorPath[i + 1].pathDirection != Direction.Backward)
                {
                    LightPos1 = new Vector3(
                        pathGenaratorPath[i].x * mapGenerator.tileSize,
                        0,
                        pathGenaratorPath[i].y * mapGenerator.tileSize - 1);
                }

                var lamp = Instantiate(streetLightPrefab,LightPos,Quaternion.identity);
                var lamp1 = Instantiate(streetLightPrefab, LightPos1, Quaternion.identity);

                var lookPos = new Vector3(
                    pathGenaratorPath[0].x * mapGenerator.tileSize,
                    0,
                    pathGenaratorPath[0].y * mapGenerator.tileSize + 1);
                var lookPos1 = new Vector3(
                    pathGenaratorPath[0].x * mapGenerator.tileSize,
                    0,
                    pathGenaratorPath[0].y * mapGenerator.tileSize - 1);
                lamp.transform.LookAt(lookPos);
                lamp1.transform.LookAt(lookPos1);
            }
            else if (pathGenaratorPath[i].pathDirection == Direction.Forward || pathGenaratorPath[i].pathDirection == Direction.Backward)
            {
                if (pathGenaratorPath[i + 1].pathDirection != Direction.Left)
                {
                    LightPos = new Vector3(
                        pathGenaratorPath[i].x * mapGenerator.tileSize + 1,
                        0,
                        pathGenaratorPath[i].y * mapGenerator.tileSize);
                }
                if (pathGenaratorPath[i + 1].pathDirection != Direction.Right)
                {
                    LightPos1 = new Vector3(
                    pathGenaratorPath[i].x * mapGenerator.tileSize- 1,
                    0,
                    pathGenaratorPath[i].y * mapGenerator.tileSize );
                }


                var lamp = Instantiate(streetLightPrefab, LightPos, Quaternion.identity);
                var lamp1 = Instantiate(streetLightPrefab, LightPos1, Quaternion.identity);

                var lookPos = new Vector3(
                    pathGenaratorPath[0].x * mapGenerator.tileSize+ 1,
                    0,
                    pathGenaratorPath[0].y * mapGenerator.tileSize );
                var lookPos1 = new Vector3(
                    pathGenaratorPath[0].x * mapGenerator.tileSize- 1,
                    0,
                    pathGenaratorPath[0].y * mapGenerator.tileSize );
                lamp.transform.LookAt(lookPos);
                lamp1.transform.LookAt(lookPos1);
            }
        }
    }

    #endregion

    

}
