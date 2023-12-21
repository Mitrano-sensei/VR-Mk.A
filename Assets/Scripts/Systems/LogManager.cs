using System;
using TMPro;
using UnityEngine;

public class LogManager : Singleton<LogManager>
{
    public enum LogMode
    {
        TRACE, INFO
    }

    [SerializeField] private LogMode _logMode = LogMode.INFO;
    [SerializeField] private TextMeshProUGUI _inGameConsole;

    private void Start()
    {
        Clear();
    }

    /**
     * Logs in Debug Console AND in-game Console if LogMode is Trace
     */
    public void Trace(string message)
    {
        if (_logMode == LogMode.TRACE) Log(message /*+ " by LogManager"*/);
    }

    /**
     * Logs on in-game console
     */
    public void LogInGame(string message)
    {
        string date = System.DateTime.Now.ToString("HH:mm:ss");

        _inGameConsole.text = date + " " + message + '\n' + _inGameConsole.text;
    }

    /**
     * Logs in Debug console AND in-game console
     */
    public void Log(string message)
    {
        Debug.Log(message);
        LogInGame(message);
    }

    public void Clear()
    {
        _inGameConsole.text = string.Empty;
    }

    internal void Error(string message)
    {
        Debug.LogError(message);
        LogInGame(message); // TODO : Make it red ? 
    }

}
