using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class BaseMinionController : MonoBehaviour
{
    [SerializeField] public int Player = 0;
    [SerializeField] public int Health = 1;
    [SerializeField] protected Rigidbody Body;
    [SerializeField] protected float Speed = 5;

    protected Vector3 Direction;
    public bool IsDamaged = false;

    public static event Action<BaseMinionController> OnDeath;

    protected void Start()
    {
        Body = GetComponent<Rigidbody>();
        SetDirection(new Vector3(Player == 0 ? 1 : -1, 0, 0));
    }

    protected void Update()
    {
        if (IsDamaged)
        {
            Body.velocity = Vector3.zero;
        }
        else
        {
            Body.velocity = Direction * Speed;
        }
    }

    public virtual void SetPlayer(int player)
    {
        Player = player;
    }

    public virtual void SetDirection(Vector3 direction)
    {
        Direction = direction.normalized;
    }

    public virtual void Die()
    {
        OnDeath?.Invoke(this);
        Destroy(this.gameObject);
    }

    public virtual void Damage(int damage)
    {
        Health -= damage;
        var material = gameObject.GetComponent<Renderer>().material;
        var color = material.color;
        material.color = Color.red;
        IsDamaged = true;
        material.DOColor(color, .2f)
            .OnComplete(() =>
            {
                IsDamaged = false;
                if (Health <= 0)
                    Die();
            });
    }
}
