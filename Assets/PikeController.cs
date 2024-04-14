using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PikeController : BaseMinionController
{
    [SerializeField] SkinnedMeshRenderer ShieldRenderer;

    new void Start()
    {
        Health = 2;
    }

    new void Update()
    {
        base.Update();

        if (Health < 2)
            ShieldRenderer.gameObject.SetActive(false);
    }

    public override void SetPlayer(int player)
    {
        base.SetPlayer(player);
        ShieldRenderer.material.color = Player == 0 ? darkRed : Color.blue;
        gameObject.transform.rotation = Quaternion.Euler(0, Player == 0 ? 90 : -90, 0);
        AnimatorController?.Play("Idle");
    }

    public override void Damage(int damage)
    {
        AnimatorController?.Play("Hit");
        base.Damage(damage);
    }

    public override void DamageEvent()
    {
        AnimatorController?.Play("Idle");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsActive())
            return;

        if (other.gameObject.tag == "Runner" || other.gameObject.tag == "Fetcher")
        {
            var runner = other.gameObject.GetComponent<BaseMinionController>();
            if (runner.Player != Player && runner.IsActive())
            {
                AnimatorController?.Play("Attack");
                DOTween.Sequence()
                    .OnComplete(() =>
                    {
                        runner.Damage(1);
                    }).SetDelay(.3f);
            }
        }
    }
}
