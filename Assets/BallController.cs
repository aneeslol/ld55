using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    [SerializeField] Rigidbody Body;
    [SerializeField] float Speed = 5;
    [SerializeField] float BaseSpeed = 5;
    [SerializeField] float MaxSpeed = 25;

    Vector3 Direction;
    public static event Action<int> ScoredGoal;
    float Rotation;

    float StuckTimer = 0;

    void Start()
    {
    }

    void Update()
    {
        Body.velocity = Direction * Speed;
        Rotation += Speed / 5;
        if (Rotation > 360) Rotation -= 360;
        gameObject.transform.rotation = Quaternion.Euler(0, Rotation, 0);

        if (gameObject.transform.position.z == 8.625f || gameObject.transform.position.z == -8.625f)
            StuckTimer++;
        else
            StuckTimer = 0;

        if (StuckTimer > 30)
        {
            SetDirection(new Vector3(Direction.x, Direction.y, -Math.Sign(gameObject.transform.position.z)));
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.tag == "Wall")
        {
            AddSpeed(2);
            SetDirection(Vector3.Reflect(Direction, collision.contacts[0].normal));
        }
        else if (collision.collider.gameObject.tag == "Goal")
        {
            var goal = collision.collider.gameObject.GetComponent<GoalController>();
            ScoredGoal?.Invoke(1 - goal.Player);
        }
    }

    public void AddSpeed(int amount)
    {
        Speed = Math.Min(Speed + amount, MaxSpeed);
    }

    public void ResetSpeed()
    {
        Speed = BaseSpeed;
    }

    public void SetDirection(Vector3 direction)
    {
        Direction = direction.normalized;
    }
}
