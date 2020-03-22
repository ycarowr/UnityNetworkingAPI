using System;
using UnityEngine;

public static class Logger
{
    /// <summary>
    ///     Use "black", "red" or any other html code to set the color.
    /// </summary>
    public static void Log(object log, Color color, string context = "")
    {
        var colorHtml = ColorUtility.ToHtmlStringRGBA(color);
        log = $"[<b>{context}</b>]: <color=#{colorHtml}>{log}</color>";
        Debug.Log(log);
    }
}