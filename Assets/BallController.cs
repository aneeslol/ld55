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

    void Start()
    {
    }

    void Update()
    {
        Body.velocity = Direction * Speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.tag == "Wall")
        {
            Speed = Math.Min(Speed + 1, MaxSpeed);
            SetDirection(Vector3.Reflect(Direction, collision.contacts[0].normal));
        }
        else if (collision.collider.gameObject.tag == "Goal")
        {
            var goal = collision.collider.gameObject.GetComponent<GoalController>();
            ScoredGoal?.Invoke(1 - goal.Player);
        }
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
