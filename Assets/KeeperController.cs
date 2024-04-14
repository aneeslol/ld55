using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeeperController : BaseMinionController
{
    new void Start()
    {
        base.Start();
        SetDirection(GetRandomDirection());
    }

    new void Update()
    {
        base.Update();
    }

    public override void SetPlayer(int player)
    {
        base.SetPlayer(player);
        SetDirection(GetRandomDirection());
        AnimatorController?.Play("Walk");
        gameObject.transform.rotation = Quaternion.Euler(0, Player == 0 ? 0 : 180, 0);
    }

    private Vector3 GetRandomDirection()
    {
        var direction = Random.Range(0, 2) == 0 ? Vector3.back : Vector3.forward;
        if (gameObject.transform.position.z > 7.75f)
            direction = Vector3.forward;
        else if (gameObject.transform.position.z < -7.75f)
            direction = Vector3.back;

        return direction;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (!IsActive())
            return;

        if (collision.gameObject.tag == "Ball")
        {
            var ball = collision.gameObject.GetComponent<BallController>();

            var zVelocity = Direction.z;
            var xVelocity = Player == 0 ? 1 : -1;
            AnimatorController.Play("Attack");
            DOTween.Sequence()
                .OnComplete(() => AnimatorController.Play("Walk"))
                .SetDelay(.625f);
            ball.AddSpeed(2);
            ball.PlayHitSound();
            ball.SetDirection(new Vector3(xVelocity, 0, zVelocity));
        }
        else if (collision.gameObject.tag == "Wall")
        {
            AnimatorController.Play("Walk");
            SetDirection(Direction * -1);
        }
        else if (collision.gameObject.tag == "Paddle")
        {
            Damage(1);
        }
        else if (collision.gameObject.tag == "Fetcher")
        {
            var fetcher = collision.gameObject.GetComponent<FetcherController>();
            if (fetcher.Player != Player && fetcher.IsActive())
            {
                AnimatorController?.Play("Attack");
                DOTween.Sequence()
                    .OnComplete(() =>
                    {
                        fetcher.Damage(1);
                    }).SetDelay(.3f);
            }
        }
    }
}
