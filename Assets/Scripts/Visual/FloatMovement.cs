using DG.Tweening;
using UnityEngine;

public class FloatMovement : MonoBehaviour
{
    [SerializeField] float _magnitude = .1f;
    [SerializeField] float _duration = 2f;
    [SerializeField] Vector3 _direction = Vector3.up;
    [SerializeField] bool _randomizeStart = true;

    public Vector3 Direction { get => _direction; private set => _direction = value; }

    void Start()
    {
        var initialPosition = transform.localPosition;

        // Start Floating using dotween, looping forever
        var sequence = DOTween.Sequence();
        var randomDelay = Random.Range(0f, _duration);

        if (_randomizeStart) sequence.PrependInterval(randomDelay);

        sequence
            .Append(transform.DOLocalMove(initialPosition + Direction * _magnitude, _duration/2).SetEase(Ease.InOutSine))
            .Append(transform.DOLocalMove(initialPosition, _duration/2).SetEase(Ease.InOutSine))
            .SetLoops(-1);
    }

}
