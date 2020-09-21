using UnityEngine;

public class GameObjectsAnimationController : MonoBehaviour
{
    private GameObject windTurbine;
    public GameManager gm;
    public MapGenerator mapGenerate;

    public void GameObjectAnimationPlay()
    {
        if (gm.playerDatas.whichScenario == 1)
        {
            WindTurbineAnimationPlay();
        }
        else if (gm.playerDatas.whichScenario == 2)
        {
            gm.Invoke("EndGame", 3.5f);
        }
        else if (gm.playerDatas.whichScenario == 3)
        {
        }
        else if (gm.playerDatas.whichScenario == 4)
        {
        }
        else if (gm.playerDatas.whichScenario == 5)
        {
        }
    }
    public void WindTurbineAnimationPlay()
    {
        gm.sc.Play("Sparkle");

        windTurbine = mapGenerate.targetHome;

        windTurbine.transform.Find("Fx_Sparkle").gameObject.SetActive(true);

        var sparkleDuration = windTurbine.transform.Find("Fx_Sparkle").GetChild(0).GetComponent<ParticleSystem>().main.duration;

        Invoke("WindTurbuneSetActive", sparkleDuration - 3f);
    }

    public void WindTurbuneSetActive()
    {
        windTurbine.SetActive(false);
        mapGenerate.targetNewHome?.SetActive(true);
        gm.Invoke("EndGame", 3.5f);
    }
}