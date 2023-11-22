using OpenCover.Framework.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class UsableItem : Pickable
{
    [Header("Events")]
    [SerializeField] private OnUse _onUse = new OnUse();

    [Header("Usable On")]
    [SerializeField] private List<Type> _canBeUsedOn = new List<Type>();
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
        if (CanBeUsedOn(useEvent.UsedOn))
            _logger.Trace("Using " + name + " on " + useEvent.UsedOn.name);
        else
            _logger.Trace("Tried to Use " + name + " on " + useEvent.UsedOn.name + ", but it is impossible");
    }

    /**
     * Returns true if this item is usable on the given object.
     * Subclasses can override this method to specify which objects can be used on.
     */
    public virtual bool CanBeUsedOn(Interactable obj) { 
        return _canBeUsedOn.Any(c => obj.GetComponent(c.GetType()) != null);
    }

}
#region Event

[Serializable] public class OnUse : UnityEvent<UseEvent> { }
public class UseEvent {
    public Interactable UsedOn { get; set; }
}
#endregion