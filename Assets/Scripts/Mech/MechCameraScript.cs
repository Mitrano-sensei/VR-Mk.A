using DG.Tweening;
using UnityEngine;

public class MechCameraScript : MonoBehaviour
{
    [SerializeField] private float _eagleViewHeight;

    private Vector3 _initialLocalPosition;
    private Quaternion _initialLocalRotation;
    private bool _isEagleView = false;

    private LogManager _logger;

    void Start()
    {
        _initialLocalPosition = transform.localPosition;
        _initialLocalRotation = transform.localRotation;

        _logger = LogManager.Instance;
    }

    #region Eagle View
    /**
     * Start Eagle View
     * If Eagle View is already active, do nothing
     */
    public void StartEagleView()
    {
        if (_isEagleView) return;

        _isEagleView = true;
        _logger.Trace("Starting Eagle View");
        var _startEagleViewSequence = DOTween.Sequence();
        _startEagleViewSequence.Append(transform.DOLocalMove(new Vector3(0, _eagleViewHeight, 0), 1f));
        _startEagleViewSequence.Join(transform.DOLocalRotate(new Vector3(90, 0, 0), 1f));
        _startEagleViewSequence.Play();
    }

    /**
     * Stop Eagle View
     * If Eagle View is already not active, do nothing
     */
    public void StopEagleView()
    {
        if (!_isEagleView) return;

        _isEagleView = false;
        _logger.Trace("Stopping Eagle View");
        var _stopEagleViewSequence = DOTween.Sequence();
        _stopEagleViewSequence.Append(transform.DOLocalMove(_initialLocalPosition, 1f));
        _stopEagleViewSequence.Join(transform.DOLocalRotate(_initialLocalRotation.eulerAngles, 1f));

        _stopEagleViewSequence.Play();
    }

    /**
     * Toggle Eagle View
     * If Eagle View is active, stop it, else start it
     */
    public void ToggleEagleView()
    {
        if (_isEagleView)
            StopEagleView();
        else
            StartEagleView();
    }
    #endregion
}


