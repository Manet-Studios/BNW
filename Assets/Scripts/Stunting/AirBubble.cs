using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirBubble : MonoBehaviour
{
    public int bubbleReward;
    private Stunting stunting;

    private void Awake()
    {
        stunting = FindObjectOfType<Stunting>();
    }

    private void OnParticleCollision(GameObject other)
    {
        stunting.AlcoholPercentage += bubbleReward;
    }
}