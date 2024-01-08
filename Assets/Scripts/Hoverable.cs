using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Hoverable : MonoBehaviour
{
    [SerializeField] private OnHoverEnter _onHoverEnter = new OnHoverEnter();
    [SerializeField] private OnHover _onHover = new OnHover();
    [SerializeField] private OnHoverExit _onHoverExit = new OnHoverExit();

    public OnHoverEnter OnHoverEnter { get => _onHoverEnter; set => _onHoverEnter = value; }
    public OnHover OnHover { get => _onHover; set => _onHover = value; }
    public OnHoverExit OnHoverExit { get => _onHoverExit; set => _onHoverExit = value; }
}

#region Events

[Serializable] public class OnHoverEnter: UnityEvent<OnHoverEnterEvent> {  }
public class OnHoverEnterEvent { }

[Serializable] public class OnHover : UnityEvent<OnHoverEvent> { }
public class OnHoverEvent { }

[Serializable] public class OnHoverExit : UnityEvent<OnHoverExitEvent> { }
public class OnHoverExitEvent { }

#endregion
