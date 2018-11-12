using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubblePop : MonoBehaviour
{
    public int spawnedBubbles;
    public int startShootBubbles;

    public Transform bubbleSpawnLocation;

    public Bubble[] bubblePrefabs;
    private Queue<Bubble> bubblesToShoot = new Queue<Bubble>();
    public Transform shootSpawn;
    public Animator launchAnimator;

    private Queue<Vector2> touchPositions = new Queue<Vector2>();

    private bool canFire = true;

    private bool complete = false;

    public float startDelay;
    public GameObject startCollider;

    private void OnEnable()
    {
        complete = false;
        StartCoroutine(StartUp());
    }

    private IEnumerator StartUp()
    {
        startCollider.SetActive(true);
        for (int i = 0; i < spawnedBubbles; i++)
        {
            Instantiate(bubblePrefabs[Random.Range(0, bubblePrefabs.Length)], bubbleSpawnLocation.position + (Vector3)Random.insideUnitCircle * .005f, Quaternion.identity, bubbleSpawnLocation).Initialize(startDelay);

            yield return null;
        }
        yield return new WaitForSeconds(startDelay);
        for (int i = 0; i < startShootBubbles; i++)
        {
            Bubble shootBubble = Instantiate(bubblePrefabs[Random.Range(0, bubblePrefabs.Length)], shootSpawn.position, Quaternion.identity);
            shootBubble.gameObject.SetActive(false);
            yield return null;
        }
        startCollider.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        if (complete)
        {
            return;
        }
        else
        {
            complete = spawnedBubbles <= 0;
        }
        if (canFire && bubblesToShoot.Count > 0 && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            canFire = false;
            launchAnimator.SetTrigger("Fire");
            touchPositions.Enqueue(Input.GetTouch(0).position);
        }
    }

    public void FireBall()
    {
        canFire = true;
        Bubble shotBubble = bubblesToShoot.Dequeue();
        shotBubble.gameObject.SetActive(true);
        shotBubble.Initialize(Camera.main.ScreenToWorldPoint(touchPositions.Dequeue()));
    }
}