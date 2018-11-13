using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public float startDelay;

    public float phaseSafeZone;

    public float timeRemaining;

    public Text delayText;
    public RectTransform textTransform;

    private Vector2 origScale;

    public AnimationCurve startSizeCurve;

    public TextMeshProUGUI timerText;

    public bool paused;

    private float additionalTime;

    public Image blackScreen;

    public float fadeDuration;
    public float blackScreenDuration;

    public int currentPhase;

    public GameObject[] phases;

    public Caste currentCaste;
    private int score;

    public TextMeshProUGUI citizenText;
    public TextMeshProUGUI casteText;
    public TextMeshProUGUI gameOverText;

    public float AdditionalTime
    {
        get
        {
            return additionalTime;
        }

        set
        {
            additionalTime = value;
        }
    }

    public GameObject gameOverButton;

    private IEnumerator Start()
    {
        GenerateEmbryo();
        float timeLeft = timeRemaining;
        timerText.text = ((int)timeLeft / 60) + ":" + ((int)timeLeft % 60);

        origScale = textTransform.localScale;
        float timeRemainingBeforeStart = startDelay;
        delayText.text = "" + (int)startDelay;
        delayText.color = Color.white;

        float elapsedSinceLastSecond = 0;
        while (timeRemainingBeforeStart > 0)
        {
            float prevalue = timeRemainingBeforeStart;
            delayText.color = Color.Lerp(Color.white, new Color(1, 1, 1, 0), elapsedSinceLastSecond);
            textTransform.localScale = new Vector3(origScale.x * startSizeCurve.Evaluate(1f - elapsedSinceLastSecond), origScale.y * startSizeCurve.Evaluate(1f - elapsedSinceLastSecond), 1);

            yield return null;
            timeRemainingBeforeStart -= Time.deltaTime;
            elapsedSinceLastSecond -= Time.deltaTime;
            if ((int)prevalue != (int)timeRemainingBeforeStart)
            {
                elapsedSinceLastSecond = 1;
                textTransform.localScale = new Vector3(origScale.x, origScale.y, 1);
                delayText.text = (int)timeRemainingBeforeStart + 1 + "";
            }
        }
        phases[0].SetActive(true);

        textTransform.gameObject.SetActive(false);
        citizenText.gameObject.SetActive(false);
        casteText.gameObject.SetActive(false);
        timerText.gameObject.SetActive(true);
        timerText.text = ((int)timeLeft / 60) + ":" + ((int)timeLeft % 60);
        while (timeLeft > 0)
        {
            if (AdditionalTime > 0)
            {
                timeLeft += AdditionalTime;
                AdditionalTime = 0;
            }
            timerText.text = ((int)timeLeft / 60) + (((int)timeLeft % 60) < 10 ? ":0" : ":") + ((int)timeLeft % 60);

            if (paused)
            {
                yield return new WaitUntil(() => !paused);
            }
            yield return null;
            timeLeft -= Time.deltaTime;
            timerText.text = ((int)timeLeft / 60) + (((int)timeLeft % 60) < 10 ? ":0" : ":") + ((int)timeLeft % 60);

            timerText.color = Color.Lerp(Color.white, Color.red, Mathf.Clamp01((60 - timeLeft) / 60f));
        }
        for (int i = 0; i < phases.Length; i++)
        {
            phases[i].SetActive(false);
            textTransform.localScale = Vector3.one;

            gameOverText.gameObject.SetActive(true);
            gameOverButton.SetActive(true);
            citizenText.gameObject.SetActive(true);
        }
    }

    public void Restart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void SwitchPhases()
    {
        int prevPhase = currentPhase;
        if (currentPhase == 1)
        {
            timerText.gameObject.SetActive(true);
        }
        Fade(() =>
        {
            phases[prevPhase].SetActive(false);
        });

        StartCoroutine(PhaseDelay());
        currentPhase = (currentPhase + 1) % phases.Length;
        if (currentPhase == 0)
        {
            score++;
            GenerateEmbryo();
        }
        Fade(() =>
        {
            if (currentPhase == 1)
            {
                timerText.gameObject.SetActive(false);

                phases[currentPhase].GetComponent<Stunting>().Initialize(currentCaste, fadeDuration + (phaseSafeZone - (3 * fadeDuration)));
            }
            phases[currentPhase].SetActive(true);
        });
    }

    private void GenerateEmbryo()
    {
        currentCaste = (Caste)Random.Range(0, 5);
        casteText.text = "Caste : " + currentCaste.ToString();

        citizenText.text = "Citizens Made : " + score;
    }

    private IEnumerator PhaseDelay()
    {
        paused = true;
        yield return new WaitForSeconds(phaseSafeZone);
        paused = false;
    }

    public void Fade(System.Action callback)
    {
        StartCoroutine(FadeScreen(callback));
    }

    private IEnumerator FadeScreen(System.Action fadeToBlack)
    {
        float elapsed = fadeDuration;
        casteText.gameObject.SetActive(true);
        citizenText.gameObject.SetActive(true);

        while (elapsed > 0)
        {
            elapsed -= Time.deltaTime;
            blackScreen.color = Color.Lerp(Color.clear, Color.black, Mathf.Clamp01((fadeDuration - elapsed) / fadeDuration));
            yield return null;
        }
        if (fadeToBlack != null)
        {
            fadeToBlack();
        }

        yield return new WaitForSeconds(blackScreenDuration);
        casteText.gameObject.SetActive(false);
        citizenText.gameObject.SetActive(false);
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            blackScreen.color = Color.Lerp(Color.clear, Color.black, Mathf.Clamp01((fadeDuration - elapsed) / fadeDuration));
            yield return null;
        }
    }
}