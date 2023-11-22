using UnityEngine;

public abstract class SingleUseItem : UsableItem
{
    protected override void Start()
    {
        base.Start();

        OnUse.AddListener(
            (e) => {
                if (!CanBeUsedOn(e.UsedOn)) return;
                FPSControlsWatcher.Instance.OnReleaseEvent.Invoke(new ReleasedEvent(this, null));
                gameObject.SetActive(false);
            });
    }
}
