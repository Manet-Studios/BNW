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

    private Bubble nextBubble;

    private bool started = false;
    private List<Bubble> spawnedBubblesList = new List<Bubble>();

    private GameManager gameManager;

    public float additionalTime;

    private void OnEnable()
    {
        foreach (Transform child in bubbleSpawnLocation)
        {
            GameObject.Destroy(child.gameObject);
        }

        foreach (Transform child in shootSpawn)
        {
            GameObject.Destroy(child.gameObject);
        }
        spawnedBubblesList.Clear();
        bubblesToShoot.Clear();
        gameManager = FindObjectOfType<GameManager>();
        complete = false;
        started = false;
        StartCoroutine(StartUp());
    }

    private IEnumerator StartUp()
    {
        startCollider.SetActive(true);
        for (int i = 0; i < spawnedBubbles; i++)
        {
            spawnedBubblesList.Add(Instantiate(bubblePrefabs[Random.Range(0, bubblePrefabs.Length)], bubbleSpawnLocation.position + (Vector3)Random.insideUnitCircle * .005f, Quaternion.identity, bubbleSpawnLocation));
            spawnedBubblesList[i].Initialize(startDelay);

            yield return null;
        }
        yield return new WaitForSeconds(startDelay);
        for (int i = 0; i < startShootBubbles; i++)
        {
            Bubble shootBubble = Instantiate(bubblePrefabs[Random.Range(0, bubblePrefabs.Length)], shootSpawn.position, Quaternion.identity, shootSpawn);
            shootBubble.dispersing = true;

            shootBubble.gameObject.SetActive(false);
            bubblesToShoot.Enqueue(shootBubble);
            yield return null;
        }
        startCollider.SetActive(false);
        canFire = true;
        started = true;
        nextBubble = bubblesToShoot.Dequeue();
        nextBubble.gameObject.SetActive(true);
    }

    // Update is called once per frame
    private void Update()
    {
        if (!complete && started && nextBubble == null && bubblesToShoot.Count == 0)
        {
            Complete();
            return;
        }

        if (complete)
        {
            return;
        }

        if (canFire && nextBubble != null && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            canFire = false;
            launchAnimator.SetTrigger("Fire");
            touchPositions.Enqueue(Input.GetTouch(0).position);
        }
    }

    private void Complete()
    {
        StartCoroutine(CompletePhase());
        complete = true;
        int bubblesPopped = spawnedBubbles;
        foreach (Bubble item in spawnedBubblesList)
        {
            if (item == null)
            {
                bubblesPopped--;
            }
        }

        gameManager.AdditionalTime += ((float)bubblesPopped / spawnedBubbles) * additionalTime;
    }

    public void FireBall()
    {
        nextBubble.Initialize(Camera.main.ScreenToWorldPoint(touchPositions.Dequeue()));
        nextBubble = null;
        if (bubblesToShoot.Count > 0)
        {
            StartCoroutine(SpawnBubble());
        }
    }

    private IEnumerator SpawnBubble()
    {
        yield return new WaitForSeconds(0.25f);
        nextBubble = bubblesToShoot.Dequeue();
        nextBubble.gameObject.SetActive(true);
        canFire = true;
    }

    private IEnumerator CompletePhase()
    {
        yield return new WaitForSeconds(2);
        gameManager.SwitchPhases();
    }
}