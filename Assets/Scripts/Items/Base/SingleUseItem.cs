using UnityEngine;

public class SingleUseItem : UsableItem
{
    protected override void Start()
    {
        base.Start();

        OnUse.AddListener(
            (e) => {
                if (!base.CanBeUsedOn(e.UsedOn)) return;
                AbstractControlWatcher.Instance.OnReleaseEvent.Invoke(new ReleasedEvent(this, null));
                gameObject.SetActive(false);
            });
    }
}
