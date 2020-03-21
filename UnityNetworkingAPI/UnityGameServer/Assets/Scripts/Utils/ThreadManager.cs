using System;
using System.Collections.Generic;
using UnityEngine;

public class ThreadManager : MonoBehaviour
{
    static readonly List<Action> MainQueue = new List<Action>();
    static readonly List<Action> CopyOfMainQueue = new List<Action>();
    static bool _isEnabled;

    void FixedUpdate()
    {
        UpdateThreads();
    }

    public static void Schedule(Action action)
    {
        if (action == null)
            return;

        lock (MainQueue)
        {
            MainQueue.Add(action);
            _isEnabled = true;
        }
    }

    static void UpdateThreads()
    {
        if (!_isEnabled)
            return;

        CopyOfMainQueue.Clear();
        lock (MainQueue)
        {
            CopyOfMainQueue.AddRange(MainQueue);
            MainQueue.Clear();
            _isEnabled = false;
        }

        foreach (var thread in CopyOfMainQueue)
            thread.Invoke();
    }
}