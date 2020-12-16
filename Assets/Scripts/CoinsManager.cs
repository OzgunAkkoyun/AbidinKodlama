using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;

public class CoinsManager : MonoBehaviour
{
	//References
	[Header ("UI references")]
	[SerializeField] TMP_Text coinUIText;
	[SerializeField] GameObject animatedCoinPrefab;
	[SerializeField] Transform target;

	[Space]
	[Header ("Available coins : (coins to pool)")]
	[SerializeField] int maxCoins;
	public Queue<GameObject> coinsQueue = new Queue<GameObject> ();


	[Space]
	[Header ("Animation settings")]
	[SerializeField] [Range (0.5f, 0.9f)] float minAnimDuration;
	[SerializeField] [Range (0.9f, 2f)] float maxAnimDuration;

	[SerializeField] Ease easeType;
	[SerializeField] float spread;

	Vector3 targetPosition;

    private int _c = 0;

    public SoundController sc;
    public int amount;

	public int Coins {
		get{ return _c; }
		set {
			_c = value;
			//update UI text whenever "Coins" variable is changed
			coinUIText.text = Coins.ToString ();
		}
	}

	void Awake ()
	{
		targetPosition = target.position;
        //prepare pool
		PrepareCoins ();
	}

    public void StartAnimateCoins()
    {
        StartCoroutine(Animate(amount));
    }

    void PrepareCoins ()
	{
		GameObject coin;
		for (int i = 0; i < maxCoins; i++) {

            var xPos = Random.Range(20, Screen.width - 20);
            var yPos = Random.Range(20, Screen.height - 20);

            var spawnPoint = new Vector3(xPos, yPos, 0f);
            
            coin = Instantiate (animatedCoinPrefab, spawnPoint,Quaternion.identity);
			coin.transform.parent = transform;
			coin.SetActive (false);
			coinsQueue.Enqueue (coin);
		}
	}

	private IEnumerator Animate (int amount)
	{
        for (int j = 0; j < coinsQueue.Count; j++)
        {
            var collectedCoinPosition = coinsQueue.ToArray()[j].transform.position;
            yield return new WaitForSeconds(0.1f);

            sc.Play("Star");
            for (int i = 0; i < amount; i++)
            {
			    //check if there's coins in the pool
			    if (coinsQueue.Count > 0) {
				    //extract a coin from the pool
				    GameObject coin = coinsQueue.Dequeue ();
				    coin.SetActive (true);

				    //move coin to the collected coin pos
				    coin.transform.position = collectedCoinPosition + new Vector3 (Random.Range (-spread, spread), 0f, 0f);

				    //animate coin to target position
				    float duration = Random.Range (minAnimDuration, maxAnimDuration);

				    coin.transform.DOMove (targetPosition, duration).SetEase (easeType).SetLoops(2).OnComplete (() => {
					    //executes whenever coin reach target position
					    coin.SetActive (false);
					    coinsQueue.Enqueue (coin);

					    Coins++;
				    });
			    }
                yield return null;
            }
        }
    }
    public void AddCoins (Vector3 collectedCoinPosition, int amount)
	{
		Animate (amount);
	}
}
