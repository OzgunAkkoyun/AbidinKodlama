using UnityEngine;

public class GameObjectsAnimationController : MonoBehaviour
{
    private GameObject windTurbine;
    public GameManager gm;
    public MapGenerator mapGenerate;

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
        mapGenerate.targetNewHome.SetActive(true);
        gm.Invoke("EndGame", 2);
    }
}