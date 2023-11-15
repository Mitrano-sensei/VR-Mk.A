using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LogManager : Singleton<LogManager>
{
    public enum LogMode
    {
        TRACE, INFO
    }

    [SerializeField] private LogMode logMode = LogMode.INFO;
    [SerializeField] private TextMeshPro textMeshPro;

    private void Start()
    {
        Clear();
    }

    public void Trace(string message)
    {
        if (logMode == LogMode.TRACE) Log(message + " by LogManager");
    }

    public void Log(string message)
    {
        Debug.Log(message);
        textMeshPro.text += message + '\n';
    }

    public void Clear()
    {
        textMeshPro.text = string.Empty;
    }

}
