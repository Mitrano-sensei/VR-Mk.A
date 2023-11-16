using UnityEngine;
using UnityEngine.UI;

public class Scroller : MonoBehaviour
{
    private ScrollRect _scrollRect;
    private LogManager _logger;

    // Start is called before the first frame update
    void Start()
    {
        _logger = LogManager.Instance;
        _scrollRect = GetComponent<ScrollRect>();
    }


    /**
     * Scrolls the scrollview.
     * Note that positive value scrolls down, negative value scrolls up.
     */
    public void Scroll(float value = .3f)
    {
        _scrollRect.verticalNormalizedPosition -= value;
    }
}
