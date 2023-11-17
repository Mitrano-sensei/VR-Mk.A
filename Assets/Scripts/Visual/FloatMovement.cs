using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatMovement : MonoBehaviour
{
    [SerializeField] float _magnitude = .1f;
    [SerializeField] float _duration = 2f;
    [SerializeField] Vector3 _direction = Vector3.up;

    void Start()
    {
        var initialPosition = transform.localPosition;

        // Start Floating using dotween, looping forever
        DOTween.Sequence()
            .Append(transform.DOLocalMove(initialPosition + _direction * _magnitude, _duration/2).SetEase(Ease.InOutSine))
            .Append(transform.DOLocalMove(initialPosition, _duration/2).SetEase(Ease.InOutSine))
            .SetLoops(-1);
    }

}
