using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

public class CoinPickUp : MonoBehaviour
{
    public Transform coinTarget;
    public TextMeshProUGUI coinCountText;
    public int coinCount;
    public float animationSpeed = 0.3f;
    public GameObject coin;
    public List<GameObject> coins = new List<GameObject>();
    public GameObject coinContainer;
    public GameManager gm;
    public SoundController sc;

    void Start()
    {
        //for (int i = 0; i < 10; i++)
        //{
        //    var xPos = Random.Range(20, Screen.width-20);
        //    var yPos = Random.Range(20, Screen.height-20);

        //    var spawnPoint = new Vector3(xPos, yPos, 0f);//Camera.main.ScreenToWorldPoint(new Vector3(xPos, yPos, 0f));

        //    GameObject myCoins = Instantiate(coin, spawnPoint, Quaternion.identity) as GameObject;
        //    myCoins.transform.parent = GameObject.Find("Canvas/GameOverPanel/CoinContainer").transform;
        //    coins.Add(myCoins);

        //}

        for (int i = 0; i < 10; i++)
        {
            var xPos = Screen.width + 200;
            var yPos = Random.Range(20, Screen.height - 20);

            var spawnPoint = new Vector3(xPos, yPos, 0f);

            GameObject myCoins = Instantiate(coin, spawnPoint, Quaternion.identity) as GameObject;
            myCoins.transform.parent = coinContainer.transform;
            coins.Add(myCoins);

        }

    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0) && gm.isGameOver  )
        {
            Debug.Log(mouseClickCount);
            Vector2 mousePos2D = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            var realMousePos = Camera.main.WorldToScreenPoint(new Vector3(mousePos2D.x, mousePos2D.y, 0));
            StartCoroutine(CoinAnimation(coins[mouseClickCount], mousePos2D));

            if (mouseClickCount < 9)
            {
                mouseClickCount++;
            }
            else
            {
                mouseClickCount = 0;
            }
        }
    }

    float timeOfTravel = 1f; //time after object reach a target place 
    float currentTime = 0; // actual floting time 
    private float normalizedValue = 1;
    private bool animationFinished = true;
    private int mouseClickCount = 0;

    private IEnumerator CoinAnimation(GameObject currentCoin,Vector2 clickPoint)
    {
        currentCoin.transform.position = clickPoint;
        animationFinished = false;
        sc.Play("Star");
        while (currentTime <= timeOfTravel &&
               Vector2.Distance(currentCoin.transform.localPosition, coinTarget.localPosition) > 0.02f)
        {
            currentTime += Time.deltaTime;
            normalizedValue = currentTime / timeOfTravel; // we normalize our time 

            RectTransform rectTransform = currentCoin.GetComponent<RectTransform>();

            rectTransform.anchoredPosition =
                Vector2.Lerp(currentCoin.transform.localPosition, coinTarget.localPosition, normalizedValue);

            yield return null;
        }
        coins.RemoveAt(0);
        coins.Insert(coins.Count - 1, currentCoin);
        animationFinished = true;
        //currentCoin.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
        currentTime = 0;
        //coins[i].transform.position = coinTarget.position;

    }
    private IEnumerator CoinsAnimation()
    {
        for (int i = 0; i < coins.Count; i++)
        {
            Debug.Log("ilk for");
            yield return new WaitForSeconds(0.0f);
            //for (float t = 0f; t < 1f; t += Time.deltaTime * animationSpeed)
            //{
            //    Debug.Log(t);

            //    coins[i].transform.position = Vector2.Lerp(coins[i].transform.position, coinTarget.position, t);
            //    Debug.Log(t);
            //    yield return null;
            //}

            while (currentTime <= timeOfTravel && Vector2.Distance(coins[i].transform.position, coinTarget.position) > 0.02f)
            {
                currentTime += Time.deltaTime;
                normalizedValue = currentTime / timeOfTravel; // we normalize our time 

                RectTransform rectTransform = coins[i].GetComponent<RectTransform>();

                rectTransform.anchoredPosition = Vector2.Lerp(coins[i].transform.position, coinTarget.position, normalizedValue);

                //rectTransform.localScale =
                //    Vector2.Lerp(coins[i].transform.localScale, new Vector3(0.2f, 0.2f, 0.2f), normalizedValue);
                yield return null;
            }
            coins[i].transform.GetChild(0).GetComponent<ParticleSystem>().Play();
            timeOfTravel = .1f;
            currentTime = 0;
            //coins[i].transform.position = coinTarget.position;

        }

    }

    
}
