using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeeperController : BaseMinionController
{
    new void Start()
    {
        base.Start();
        SetDirection(Random.Range(0, 2) == 0 ? Vector3.back : Vector3.forward);
    }

    new void Update()
    {
        base.Update();
    }

    public override void SetPlayer(int player)
    {
        base.SetPlayer(player);
        SetDirection(Random.Range(0, 2) == 0 ? Vector3.back : Vector3.forward);
        AnimatorController?.Play("Walk");
        gameObject.transform.rotation = Quaternion.Euler(0, Player == 0 ? 0 : 180, 0);
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
    }
}
