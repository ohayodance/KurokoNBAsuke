using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{

    public Transform Ball;
    public Transform PosDribble;
    public Transform PosOverHead;
    public Transform Arms;
    public Transform Target;

    public bool IsBallInHands { get; private set; }
    public bool IsBallFlying { get; private set; }
    public bool IsShooting { get; private set; }
    public bool TriggerShoot { get; private set; }
    public Vector3 TargetPosition => Target.position;


    private float shootPressTime;
    private float flightTime;

    public void PickUpBall(Transform ballTransform)
    {
        if (IsBallInHands || IsBallFlying) return;

        Ball = ballTransform;
        IsBallInHands = true;
        IsBallFlying = false;
        Ball.GetComponent<Rigidbody>().isKinematic = true;
        Ball.SetParent(Arms); // ‘иксируем м€ч к рукам
    }

    public void HandleBallInput(bool inputShoot, bool shootReleased)
    {
        if (!IsBallInHands) return;

        const float holdThreshold = 0.5f;

        if (inputShoot)
        {
            IsShooting = true;
            Ball.position = PosOverHead.position;
            shootPressTime = Time.time;
        }
        else
        {
            IsShooting = false;
            Dribble();
        }

        if (shootReleased)
        {
            if (Time.time - shootPressTime >= holdThreshold)
            {
                ReleaseBall();
            }
            else
            {
                TriggerShoot = true;
                ReleaseBall();
            }
        }
    }

    public void ResetShootTrigger() => TriggerShoot = false;

    private void Dribble()
    {
        Ball.position = PosDribble.position + Vector3.up * Mathf.Abs(Mathf.Sin(Time.time * 3));
        Arms.localEulerAngles = Vector3.zero;
        Arms.localPosition = Vector3.zero;
    }

    private void ReleaseBall()
    {
        IsBallInHands = false;
        IsBallFlying = true;
        flightTime = 0;
    }

    private void Update()
    {
        if (IsBallFlying)
        {
            UpdateBallFlight();
        }
    }

    private void UpdateBallFlight()
    {
        flightTime += Time.deltaTime;
        float duration = 0.66f;
        float t = flightTime / duration;

        Vector3 startPos = PosOverHead.position;
        Vector3 endPos = Target.position;
        Vector3 currentPos = Vector3.Lerp(startPos, endPos, t);
        Vector3 arc = Vector3.up * 5 * Mathf.Sin(t * Mathf.PI);

        Ball.position = currentPos + arc;

        if (t >= 1)
        {
            IsBallFlying = false;
            Ball.GetComponent<Rigidbody>().isKinematic = false;
        }
    }
}