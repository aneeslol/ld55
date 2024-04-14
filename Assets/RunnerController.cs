using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunnerController : BaseMinionController
{
    new void Start()
    {
        SetDirection(new Vector3(Player == 0 ? 1 : -1, 0, 0));
    }

    new void Update()
    {
        base.Update();
    }

    public override void SetPlayer(int player)
    {
        base.SetPlayer(player);
        SetDirection(new Vector3(Player == 0 ? 1 : -1, 0, 0));
        AnimatorController?.Play("Walk");
        gameObject.transform.rotation = Quaternion.Euler(0, Player == 0 ? 90 : -90, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsActive())
            return;

        if (other.gameObject.tag == "Goal")
        {
            Die();
        }
        else if (other.gameObject.tag == "Paddle")
        {
            var paddle = other.gameObject.GetComponent<KeeperController>();
            if (paddle.Player != Player && paddle.IsActive())
            {
                AnimatorController?.Play("Attack");
                paddle.Damage(1);
                Speed = 0;
                IsDamaged = true;
                DOTween.Sequence()
                    .OnComplete(() =>
                    {
                        Die();
                    }).SetDelay(.3f);
            }
        }
        else if (other.gameObject.tag == "Runner" || other.gameObject.tag == "Fetcher")
        {
            var runner = other.gameObject.GetComponent<BaseMinionController>();
            if (runner.Player != Player && runner.IsActive())
            {
                Speed = 0;
                AnimatorController?.Play("Attack");
                DOTween.Sequence()
                    .OnComplete(() =>
                    {
                        runner.Damage(1);
                        if (other.gameObject.tag == "Fetcher")
                        {
                            Die();
                        }
                    }).SetDelay(.3f);
            }
        }
    }
}
