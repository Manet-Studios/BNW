using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum Caste
{
    Epsilon,
    Delta,
    Gamma,
    Beta,
    Alpha
}

public class Stunting : MonoBehaviour
{
    public Transform baseTransform;
    public Transform ropeTransform;
    public Transform eggTransform;

    public float verticalSpeed;

    public Vector2 bounds;

    private int alcoholPercentage;

    public TextMeshProUGUI percentageText;
    public TextMeshProUGUI targetText;
    public TextMeshProUGUI timerText;

    public int targetPercentage;

    public float levelTime;

    private float timeRemaining;

    private Coroutine suffocateCoroutine;

    private bool completed;

    private GameManager gameManager;
    private Coroutine timerCoroutine;

    public float alcoholDrainPerSecond;

    public float[] targets;
    public Color[] colors;

    public int AlcoholPercentage
    {
        get
        {
            return alcoholPercentage;
        }

        set
        {
            alcoholPercentage = Mathf.Clamp(value, 0, 100);

            percentageText.text = string.Format("Oxygen : {0}%", alcoholPercentage);
        }
    }

    private void OnEnable()
    {
        AlcoholPercentage = 100;
        timeRemaining = levelTime;

        completed = false;
        targetText.text = string.Format("Target : {0}%", targetPercentage);

        timerText.text = "" + levelTime;

        eggTransform.localPosition = new Vector3(0, bounds.y, 0);
    }

    public void Initialize(Caste caste, float startDelay)
    {
        gameManager = FindObjectOfType<GameManager>();

        if ((int)caste > 2)
        {
            levelTime = .1f;
        }
        else
        {
            levelTime = 10f;
        }

        eggTransform.GetComponent<SpriteRenderer>().color = colors[(int)caste];

        targetPercentage = (int)targets[(int)caste];
        targetText.text = "" + targetPercentage;

        gameManager.StartCoroutine(Delay(startDelay));
    }

    private IEnumerator Delay(float startDelay)
    {
        yield return new WaitForSeconds(startDelay);
        if (gameObject.activeInHierarchy)
        {
            timerCoroutine = StartCoroutine(Timer());
            suffocateCoroutine = StartCoroutine(Suffocate());
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (completed)
        {
            return;
        }

        if (!completed && timeRemaining < 0)
        {
            Complete();
        }

        float effectiveSpeed = verticalSpeed;
        if (Input.touchCount > 0)
        {
            effectiveSpeed *= -1;
        }
        eggTransform.localPosition += new Vector3(0, effectiveSpeed * Time.deltaTime, 0);
        eggTransform.localPosition = new Vector3(0, Mathf.Clamp(eggTransform.localPosition.y, bounds.x, bounds.y), 0);

        float distance = Vector2.Distance(baseTransform.position, eggTransform.position);
        ropeTransform.localPosition = new Vector3(ropeTransform.localPosition.x, eggTransform.localPosition.y + distance / 2f, 0);
        ropeTransform.localScale = new Vector3(ropeTransform.localScale.x, distance, 1);
    }

    private void Complete()
    {
        completed = true;
        if (AlcoholPercentage < 70)
        {
            gameManager.AdditionalTime -= 20f;
        }
        else
        {
            gameManager.AdditionalTime += Mathf.Clamp(10 - Mathf.Abs(targetPercentage - AlcoholPercentage), 0, 10);
        }

        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }

        if (suffocateCoroutine != null)
        {
            StopCoroutine(suffocateCoroutine);
        }

        gameManager.SwitchPhases();
    }

    private IEnumerator Timer()
    {
        while (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            yield return null;
            timerText.text = "" + (int)timeRemaining;
        }
    }

    private IEnumerator Suffocate()
    {
        while (!completed)
        {
            yield return new WaitForSeconds(1f / alcoholDrainPerSecond);
            if (eggTransform.localPosition.y < -2f)
            {
                AlcoholPercentage--;
            }
        }
    }
}