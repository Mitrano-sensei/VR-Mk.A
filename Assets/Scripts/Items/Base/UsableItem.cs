using System;
using UnityEngine;
using UnityEngine.Events;

public class UsableItem : Pickable
{
    [Header("Events")]
    [SerializeField] private OnUse _onUse = new OnUse();

    [Header("Item Properties")]
    [SerializeField] private bool _isUsableOnGround = true;
    public OnUse OnUse { get => _onUse; set => _onUse = value; }

    protected override void Start()
    {
        base.Start();

        OnUse.AddListener(HandleUse);
    }

    /**
     * Basic handle use. It checks if the item can be used on the given object and logs it (LogMode.Trace).
     */
    private void HandleUse(UseEvent useEvent)
    {
        var usedOnName = useEvent.UsedOn ? useEvent.UsedOn.name : "Ground";
        if (CanBeUsedOn(useEvent.UsedOn))
            _logger.Trace("Using " + name + " on " + usedOnName);
        else
            _logger.Trace("Tried to Use " + name + " on " + usedOnName + ", but it is impossible");
    }

    /**
     * Returns true if this item is usable in the given situation.
     */
    protected bool CanBeUsedOn(GameObject obj = null)
    {
        if (obj == null)
            return CanBeUsedOnGround();
        else return CanBeUsedOnObject(obj) || CanBeUsedOnGround();
    }

    /**
     * Returns true if this item is usable on the given object.
     * Subclasses can override this method to specify which objects can be used on.
     */
    protected virtual bool CanBeUsedOnObject(GameObject obj) => true;

    /**
     * Returns true if this item is usable on the ground.
     * Subclasses can override this method to specify if this item can be used on the ground.
     */
    protected virtual bool CanBeUsedOnGround() => _isUsableOnGround;

}
#region Event

[Serializable] public class OnUse : UnityEvent<UseEvent> { }
public class UseEvent {
    private GameObject _usedOn;
    public GameObject UsedOn { get => _usedOn; }
    
    public UseEvent(GameObject usedOn = null) => _usedOn = usedOn;
    public UseEvent() { }
}
#endregion