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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsDamaged)
            return; 

        if (other.gameObject.tag == "Wall" || other.gameObject.tag == "Goal")
        {
            Die();
        }
        else if (other.gameObject.tag == "Paddle")
        {
            var paddle = other.gameObject.GetComponent<PaddleController>();
            if (paddle.Player != Player && !paddle.IsDamaged)
            {
                Damage(1);
                paddle.Damage(1);
            }
        }
        else if(other.gameObject.tag == "Runner")
        {
            var runner = other.gameObject.GetComponent<RunnerController>();
            if (runner.Player != Player && !runner.IsDamaged)
            {
                Damage(1);
                runner.Damage(1);
            }
        }
    }
}
