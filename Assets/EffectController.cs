using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectController : MonoBehaviour
{
    [SerializeField] float Duration;

    void Start()
    {
        DOTween.Sequence()
            .OnComplete(() => Destroy(this.gameObject))
            .SetDelay(Duration);
    }
}
