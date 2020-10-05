using DG.Tweening;
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
    public Color changeTargetColor;

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

            if (i == pathGenarator.PathLength - 1)
            {
                mapGenerator.allTileGameObject[PathIndex].gameObject.GetComponent<Renderer>().material.DOColor(changeTargetColor, 3).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
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
            
            Vector3 LightPos = Vector3.zero;

            var neighbours = pathGenaratorPath[i].GetNeighbours();

            for (int j = 0; j < neighbours.Count; j++)
            {
                if (!pathGenaratorPath.Contains(neighbours[j]))
                {
                    var diff = new Vector3(
                        neighbours[j].x - pathGenaratorPath[i].x,
                        0,
                        neighbours[j].y - pathGenaratorPath[i].y);

                    var originalPos = new Vector3(
                        pathGenaratorPath[i].x * mapGenerator.tileSize,
                        0, 
                        pathGenaratorPath[i].y * mapGenerator.tileSize);

                    LightPos = originalPos + diff;

                    var lamp = Instantiate(streetLightPrefab, LightPos, Quaternion.identity);

                    var lookPos = new Vector3(
                        pathGenaratorPath[0].x * mapGenerator.tileSize + 1,
                        0,
                        pathGenaratorPath[0].y * mapGenerator.tileSize);

                    lamp.transform.LookAt(lookPos);
                }
            }
            
        }
    }
    

    #endregion

    

}
