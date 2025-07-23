using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDetector : MonoBehaviour
{
    private ThirdPersonController controller;

    private void Awake()
    {
        controller = GetComponentInParent<ThirdPersonController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            controller.NotifyBallInRange(other.transform);
        }
    }
}