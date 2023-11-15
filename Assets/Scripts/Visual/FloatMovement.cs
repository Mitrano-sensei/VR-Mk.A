using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatMovement : MonoBehaviour
{
    [SerializeField] float _magnitude = .1f;
    [SerializeField] float _duration = 2f;

    void Start()
    {
        var initialPosition = transform.localPosition;

        // Start Floating using dotween, looping forever
        DOTween.Sequence()
            .Append(transform.DOLocalMoveY(initialPosition.y + _magnitude, _duration/2).SetEase(Ease.InOutSine))
            .Append(transform.DOLocalMoveY(initialPosition.y, _duration/2).SetEase(Ease.InOutSine))
            .SetLoops(-1);
    }

}
