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
    [SerializeField] protected Animator AnimatorController;
    [SerializeField] protected SkinnedMeshRenderer Renderer;

    protected Vector3 Direction;
    public bool IsDamaged = false;
    public bool IsDead = false;

    public static event Action<BaseMinionController> OnDeath;
    protected Color darkRed = new Color(.5f, 0f, 0f);

    protected void Start()
    {
        Body = GetComponent<Rigidbody>();
        SetDirection(new Vector3(Player == 0 ? 1 : -1, 0, 0));
    }

    protected void Update()
    {
        if (IsActive())
        {
            Body.velocity = Direction * Speed;
        }
        else
        {
            Body.velocity = Vector3.zero;
        }
    }

    public virtual void SetPlayer(int player)
    {
        Player = player;
        Renderer.material.color = Player == 0 ? darkRed : Color.blue;
    }

    public virtual void SetDirection(Vector3 direction)
    {
        Direction = direction.normalized;
    }

    public virtual void Die()
    {
        IsDead = true;
        AnimatorController?.Play("Die");
        DOTween.Sequence().OnComplete(() =>
        {
            OnDeath?.Invoke(this);
            Destroy(this.gameObject);
        })
        .SetDelay(1);
    }

    public virtual bool IsActive()
    {
        return !IsDead && !IsDamaged;
    }

    public virtual void DamageEvent()
    {
        
    }

    public virtual void Damage(int damage)
    {
        Health -= damage;
        var material = Renderer.material;
        var color = material.color;
        material.color = Color.white;
        IsDamaged = true;
        material.DOColor(color, .2f)
            .OnComplete(() =>
            {
                IsDamaged = false;
                DamageEvent();
                if (Health <= 0)
                    Die();
            });
    }
}
