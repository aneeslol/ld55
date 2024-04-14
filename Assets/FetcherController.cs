using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FetcherController : BaseMinionController
{
    public BallController Ball;

    new void Start()
    {

    }

    new void Update()
    {
        gameObject.transform.LookAt(Ball.gameObject.transform);
        SetDirection(gameObject.transform.forward);
        if (IsActive())
        {
            if (Ball.isActiveAndEnabled)
            {
                AnimatorController.Play("Walk");
                Body.velocity = Direction * Speed;
            }
            else
            {
                AnimatorController.Play("Stop");
                Body.velocity = Vector3.zero;
            }
        }
        else
        {
            Body.velocity = Vector3.zero;
        }
    }

    public override void SetPlayer(int player)
    {
        base.SetPlayer(player);
        AnimatorController?.Play("Walk");
        SetDirection(Vector3.forward);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsActive())
            return;

        if (other.gameObject.tag == "Ball")
        {
            var ball = other.gameObject.GetComponent<BallController>();

            var zVelocity = Direction.z;
            var xVelocity = Player == 0 ? 1 : -1;
            AnimatorController.Play("Attack");
            IsDamaged = true;
            DOTween.Sequence()
                .OnComplete(() => Die())
                .SetDelay(.625f);
            ball.AddSpeed(3);
            ball.SetDirection(new Vector3(xVelocity, 0, zVelocity));
        }
        else if (other.gameObject.tag == "Goal")
        {
            Die();
        }
        //else if (other.gameObject.tag == "Paddle")
        //{
        //    var paddle = other.gameObject.GetComponent<KeeperController>();
        //    if (paddle.Player != Player && paddle.IsActive())
        //    {
        //        AnimatorController?.Play("Attack");
        //        paddle.Damage(1);
        //        Speed = 0;
        //        IsDamaged = true;
        //        DOTween.Sequence()
        //            .OnComplete(() =>
        //            {
        //                Die();
        //            }).SetDelay(.3f);
        //    }
        //}
        //else if (other.gameObject.tag == "Runner")
        //{
        //    var runner = other.gameObject.GetComponent<RunnerController>();
        //    if (runner.Player != Player && runner.IsActive())
        //    {
        //        Speed = 0;
        //        AnimatorController?.Play("Attack");
        //        DOTween.Sequence()
        //            .OnComplete(() =>
        //            {
        //                runner.Damage(1);
        //            }).SetDelay(.3f);
        //    }
        //}
    }
}
