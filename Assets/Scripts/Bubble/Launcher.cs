using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    private BubblePop manager;

    public void Awake()
    {
        manager = GetComponentInParent<BubblePop>();
    }

    public void LaunchBall()
    {
        manager.FireBall();
    }
}