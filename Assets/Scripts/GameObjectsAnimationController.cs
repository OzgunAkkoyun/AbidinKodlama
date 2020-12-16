using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GameObjectsAnimationController : MonoBehaviour
{
    private GameObject windTurbine;
    public GameManager gm;
    public MapGenerator mapGenerate;
    public ChangeEnvironment changeEnvironment;
    private float animFinishTime = 5f;

    public void GameObjectAnimationPlay()
    {
        if (gm.playerDatas.whichScenario == 1)
        {
            WindTurbineAnimationPlay();
        }
        else if (gm.playerDatas.whichScenario == 2)
        {
            ForSenarioAnimationPlay();
        }
        else if (gm.playerDatas.whichScenario == 3)
        {
            gm.Invoke("EndGame", animFinishTime);
        }
        else if (gm.playerDatas.whichScenario == 4)
        {
            gm.Invoke("EndGame", animFinishTime);
        }
        else if (gm.playerDatas.whichScenario == 5)
        {
        }
    }

    public void WindTurbineAnimationPlay()
    {
        SoundController.instance.Play("Sparkle");

        windTurbine = mapGenerate.targetHome;

        windTurbine.transform.Find("Fx_Sparkle").gameObject.SetActive(true);

        var sparkleDuration = windTurbine.transform.Find("Fx_Sparkle").GetChild(0).GetComponent<ParticleSystem>().main.duration;

        Invoke("WindTurbuneSetActive", sparkleDuration - 3f);
    }

    public void WindTurbuneSetActive()
    {
        SoundController.instance.Play("Wind");
        windTurbine.SetActive(false);
        mapGenerate.targetNewHome?.SetActive(true);
        gm.Invoke("EndGame", animFinishTime);
        StartCoroutine(SoundController.instance.Pause("Wind", animFinishTime));
    }

    public Transform[] catMovePath = new Transform[2];
    public void ForSenarioAnimationPlay()
    {
        if (gm.currentLevel.levelIndex == 1)
        {
            var catPath = mapGenerate.vehiclePrefab.transform.Find("CatPath").transform;
            for (int i = 0; i < catPath.childCount; i++)
            {
                catMovePath[i] = catPath.GetChild(i);
            }

            StartCoroutine(CatMoveTargetPos());
        }
        gm.Invoke("EndGame", animFinishTime);
    }

    private IEnumerator CatMoveTargetPos()
    {
        var cat = changeEnvironment.realCarInsideObject;
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < catMovePath.Length; i++)
        {
            cat.transform.DOLookAt(catMovePath[i].position, 1f);
            yield return new WaitForSeconds(1f);
            cat.transform.DOMove(catMovePath[i].position, 1f);
            yield return new WaitForSeconds(1.2f);
        }
    }
}