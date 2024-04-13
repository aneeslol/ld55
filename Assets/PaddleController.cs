using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaddleController : BaseMinionController
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

    private void OnTriggerEnter(Collider collision)
    {
        if (IsDamaged)
            return;

        if (collision.gameObject.tag == "Ball")
        {
            var ball = collision.gameObject.GetComponent<BallController>();

            var zVelocity = Direction.z;
            var xVelocity = Player == 0 ? 1 : -1;
            ball.SetDirection(new Vector3(xVelocity, 0, zVelocity));
        }
        else if (collision.gameObject.tag == "Wall")
        {
            SetDirection(Direction * -1);
        }
        else if (collision.gameObject.tag == "Paddle")
        {
            Damage(1);
        }
    }
}
