using UnityEngine;

public class GameObjectsAnimationController : MonoBehaviour
{
    private GameObject windTurbine;
    public GameManager gm;
    public MapGenerator mapGenerate;
    private float animFinishTime = 5f;

    public void GameObjectAnimationPlay()
    {
        if (gm.playerDatas.whichScenario == 1)
        {
            WindTurbineAnimationPlay();
        }
        else if (gm.playerDatas.whichScenario == 2)
        {
            gm.Invoke("EndGame", animFinishTime);
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
}