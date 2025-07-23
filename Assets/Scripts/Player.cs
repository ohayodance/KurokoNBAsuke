using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Transform Ball;
    public Transform PosDribble;
    public Transform PosOverHead;
    public Transform Arms;
    public Transform Target;
    Animator animator;
    float jumpElapsedTime = 0;
    bool isJumping;

    // variable

    private bool IsBallInHands = true;
    private bool IsBallFlying = false;
    private float T = 0;
    private bool isShooting;
    private bool is3Point;
    private float timePressed;

    // Update is called once per frame
    void Update()
    {

        // ball in hands
        if (IsBallInHands)
        {
            animator.SetBool("HaveBall", true);

            // hold over head
            if (Input.GetKey(KeyCode.Mouse1))
            {
                isShooting = true;
                animator.SetBool("AimingShoot", true);
                Ball.position = PosOverHead.position;
                if (!is3Point)
                {
                    // Начало отсчета времени
                    is3Point = true;
                    timePressed = 0f;
                }
                timePressed += Time.deltaTime;



                // look towards the target
                transform.LookAt(Target.parent.position);
            }

            // dribbling
            else
            {
                Ball.position = PosDribble.position + Vector3.up * Mathf.Abs(Mathf.Sin(Time.time * 3));
                Arms.localEulerAngles = Vector3.right * 0;
                Arms.localPosition = Vector3.zero;
            }

            // throw ball
            if (Input.GetKeyUp(KeyCode.Space))
            {
                animator.SetTrigger("ShootOver");
                animator.SetBool("AimingShoot", false);
                animator.SetBool("HaveBall", false);
                IsBallInHands = false;
                IsBallFlying = true;
                T = 0;
            }
        }

        // ball in the air
        if (IsBallFlying)
        {
            T += Time.deltaTime;
            float duration = 0.66f;
            float t01 = T / duration;

            // move to target
            Vector3 A = PosOverHead.position;
            Vector3 B = Target.position;
            Vector3 pos = Vector3.Lerp(A, B, t01);

            // move in arc
            Vector3 arc = Vector3.up * 5 * Mathf.Sin(t01 * 3.14f);

            Ball.position = pos + arc;

            // moment when ball arrives at the target
            if (t01 >= 1)
            {
                IsBallFlying = false;
                Ball.GetComponent<Rigidbody>().isKinematic = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (!IsBallInHands && !IsBallFlying)
        {

            IsBallInHands = true;
            Ball.GetComponent<Rigidbody>().isKinematic = true;
        }
    }
}
