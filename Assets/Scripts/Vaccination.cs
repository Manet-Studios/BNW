using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vaccination : MonoBehaviour
{
    public Transform layOneTransform;
    public Transform layTwoTransform;
    public LayerMask eggLayer;

    public LayerMask ringLayer;

    private Collider2D layerOne;
    private Collider2D layerTwo;

    private bool canRotate;
    private bool complete;

    public float rotationDuration;

    public Transform eggTransform;

    public Transform[] raycastOrigins;

    private GameManager gameManager;
    public Stunting stunting;

    public SpriteRenderer egg;

    // Use this for initialization
    private void OnEnable()
    {
        gameManager = FindObjectOfType<GameManager>();
        egg.color = stunting.colors[(int)gameManager.currentCaste];
        layerOne = layOneTransform.GetComponent<Collider2D>();
        layerTwo = layTwoTransform.GetComponent<Collider2D>();
        List<Transform> children = new List<Transform>();
        List<Transform> childrenTwo = new List<Transform>();

        foreach (Transform child in layOneTransform)
        {
            children.Add(child);
            child.gameObject.SetActive(false);
        }

        foreach (Transform child in layTwoTransform)
        {
            childrenTwo.Add(child);
            child.gameObject.SetActive(false);
        }

        List<int> randomChildrenOne = new List<int>();
        List<int> randomChildrenTwo = new List<int>();

        int maxRandom = 3;
        for (int i = 0, j = Random.Range(1, 3); i < j; i++)
        {
            maxRandom = 3 - j;
            int randINdex = Random.Range(0, 3);
            while (randomChildrenOne.Contains(randINdex))
            {
                randINdex = Random.Range(0, 3);
            }
            randomChildrenOne.Add(randINdex);
        }
        for (int i = 0; i < maxRandom; i++)
        {
            int randINdex = Random.Range(0, 3);
            while (randomChildrenTwo.Contains(randINdex))
            {
                randINdex = Random.Range(0, 3);
            }
            randomChildrenTwo.Add(randINdex);
        }

        foreach (int item in randomChildrenOne)
        {
            children[item].gameObject.SetActive(true);
        }
        foreach (int item in randomChildrenTwo)
        {
            childrenTwo[item].gameObject.SetActive(true);
        }
        canRotate = true;

        if (CompletionCondition())
        {
            Rotate(layTwoTransform);
        }

        complete = false;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!canRotate || complete)
        {
            return;
        }
        if (!complete && CompletionCondition())
        {
            Complete();
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Collider2D hit = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position), ringLayer);
            if (hit == layerOne)
            {
                Rotate(layOneTransform);
            }
            else if (hit == layerTwo)
            {
                Rotate(layTwoTransform);
            }
        }
    }

    private void Complete()
    {
        complete = true;
        gameManager.SwitchPhases();
    }

    private bool CompletionCondition()
    {
        bool touched = false;

        foreach (Transform x in raycastOrigins)
        {
            RaycastHit2D hit = Physics2D.Linecast(x.position, eggTransform.position, eggLayer);
            touched = hit.collider != null && hit.collider.transform == eggTransform;
            if (touched)
            {
                break;
            }
        }

        return !touched;
    }

    public void Rotate(Transform rot)
    {
        canRotate = false;

        StartCoroutine(Rotation(rot));
    }

    private IEnumerator Rotation(Transform rot)
    {
        float elapsed = rotationDuration;
        float startZ = rot.localEulerAngles.z;
        while (elapsed > 0)
        {
            rot.localEulerAngles = new Vector3(0, 0, startZ + (120 * ((rotationDuration - elapsed) / rotationDuration)));
            elapsed -= Time.deltaTime;
            yield return null;
        }

        rot.localEulerAngles = new Vector3(0, 0, startZ + 120);
        canRotate = true;
    }
}