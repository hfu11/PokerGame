using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventCenter
{
    private static Dictionary<EventType, Delegate> m_EventTable = new Dictionary<EventType, Delegate>();

    private static void OnListenerAdding(EventType eventType, Delegate callBack)
    {
        if (!m_EventTable.ContainsKey(eventType))
        {
            m_EventTable.Add(eventType, null);
        }

        Delegate d = m_EventTable[eventType];
        if (d != null && d.GetType() != callBack.GetType())
        {
            throw new Exception(string.Format("Try add different delegate type to event {0}. Current event has delegate {1}, while the delegate to be added is type {2}", eventType, d.GetType(), callBack.GetType()));
        }

    }

    private static void OnListenerRemoving(EventType eventType, Delegate callBack)
    {
        if (m_EventTable.ContainsKey(eventType))
        {
            Delegate d = m_EventTable[eventType];
            if (d == null)
            {
                throw new Exception(string.Format("Event {0} has no delegate", eventType));
            }
            else if (d.GetType() != callBack.GetType())
            {
                throw new Exception(string.Format("Try remove different delegate type from event {0}.Current event has delegate {1}, while the delegate to be added is type {2}", eventType, d.GetType(), callBack.GetType()));
            }
        }
        else
        {
            throw new Exception(string.Format("Event {0} not Found", eventType));
        }
    }

    private static void OnListenerRemoved(EventType eventType)
    {
        if (m_EventTable[eventType] == null)
        {
            m_EventTable.Remove(eventType);
        }
    }

    public static void AddListener(EventType eventType, CallBack callBack)
    {
        OnListenerAdding(eventType, callBack);
        m_EventTable[eventType] = (CallBack)m_EventTable[eventType] + callBack;
    }

    public static void AddListener<T>(EventType eventType, CallBack<T> callBack)
    {
        OnListenerAdding(eventType, callBack);
        m_EventTable[eventType] = (CallBack<T>)m_EventTable[eventType] + callBack;
    }
    public static void AddListener<T,X>(EventType eventType, CallBack<T,X> callBack)
    {
        OnListenerAdding(eventType, callBack);
        m_EventTable[eventType] = (CallBack<T,X>)m_EventTable[eventType] + callBack;
    }

    //three parameters
    public static void AddListener<T, X, Y>(EventType eventType, CallBack<T, X, Y> callBack)
    {
        OnListenerAdding(eventType, callBack);
        m_EventTable[eventType] = (CallBack<T, X, Y>)m_EventTable[eventType] + callBack;
    }
    //four parameters
    public static void AddListener<T, X, Y, Z>(EventType eventType, CallBack<T, X, Y, Z> callBack)
    {
        OnListenerAdding(eventType, callBack);
        m_EventTable[eventType] = (CallBack<T, X, Y, Z>)m_EventTable[eventType] + callBack;
    }
    //five parameters
    public static void AddListener<T, X, Y, Z, W>(EventType eventType, CallBack<T, X, Y, Z, W> callBack)
    {
        OnListenerAdding(eventType, callBack);
        m_EventTable[eventType] = (CallBack<T, X, Y, Z, W>)m_EventTable[eventType] + callBack;
    }
    public static void RemoveListener(EventType eventType, CallBack callBack)
    {
        OnListenerRemoving(eventType, callBack);

        m_EventTable[eventType] = (CallBack)m_EventTable[eventType] - callBack;

        OnListenerRemoved(eventType);
    }
    public static void RemoveListener<T>(EventType eventType, CallBack<T> callBack)
    {
        OnListenerRemoving(eventType, callBack);

        m_EventTable[eventType] = (CallBack<T>)m_EventTable[eventType] - callBack;

        OnListenerRemoved(eventType);
    }
    public static void RemoveListener<T,X>(EventType eventType, CallBack<T,X> callBack)
    {
        OnListenerRemoving(eventType, callBack);

        m_EventTable[eventType] = (CallBack<T,X>)m_EventTable[eventType] - callBack;

        OnListenerRemoved(eventType);
    }
    //three parameters
    public static void RemoveListener<T, X, Y>(EventType eventType, CallBack<T, X, Y> callBack)
    {
        OnListenerRemoving(eventType, callBack);
        m_EventTable[eventType] = (CallBack<T, X, Y>)m_EventTable[eventType] - callBack;
        OnListenerRemoved(eventType);
    }
    //four parameters
    public static void RemoveListener<T, X, Y, Z>(EventType eventType, CallBack<T, X, Y, Z> callBack)
    {
        OnListenerRemoving(eventType, callBack);
        m_EventTable[eventType] = (CallBack<T, X, Y, Z>)m_EventTable[eventType] - callBack;
        OnListenerRemoved(eventType);
    }
    //five parameters
    public static void RemoveListener<T, X, Y, Z, W>(EventType eventType, CallBack<T, X, Y, Z, W> callBack)
    {
        OnListenerRemoving(eventType, callBack);
        m_EventTable[eventType] = (CallBack<T, X, Y, Z, W>)m_EventTable[eventType] - callBack;
        OnListenerRemoved(eventType);
    }

    public static void Broadcast(EventType eventType)
    {
        Delegate d;
        if(m_EventTable.TryGetValue(eventType, out d))
        {
            CallBack callBack = d as CallBack;
            if(callBack != null)
            {
                callBack();
            }
            else
            {
                throw new Exception(string.Format("Event {0} has different type of delegate", eventType));
            }
        }
    }
    public static void Broadcast<T>(EventType eventType, T arg)
    {
        Delegate d;
        if(m_EventTable.TryGetValue(eventType, out d))
        {
            CallBack<T> callBack = d as CallBack<T>;
            if(callBack != null)
            {
                callBack(arg);
            }
            else
            {
                throw new Exception(string.Format("Event {0} has different type of delegate", eventType));
            }
        }
    }
    public static void Broadcast<T,X>(EventType eventType, T arg1, X arg2)
    {
        Delegate d;
        if(m_EventTable.TryGetValue(eventType, out d))
        {
            CallBack<T,X> callBack = d as CallBack<T,X>;
            if(callBack != null)
            {
                callBack(arg1,arg2);
            }
            else
            {
                throw new Exception(string.Format("Event {0} has different type of delegate", eventType));
            }
        }
    }

    //three parameters
    public static void Broadcast<T, X, Y>(EventType eventType, T arg1, X arg2, Y arg3)
    {
        Delegate d;
        if (m_EventTable.TryGetValue(eventType, out d))
        {
            CallBack<T, X, Y> callBack = d as CallBack<T, X, Y>;
            if (callBack != null)
            {
                callBack(arg1, arg2, arg3);
            }
            else
            {
                throw new Exception(string.Format("广播事件错误：事件{0}对应委托具有不同的类型", eventType));
            }
        }
    }
    //four parameters
    public static void Broadcast<T, X, Y, Z>(EventType eventType, T arg1, X arg2, Y arg3, Z arg4)
    {
        Delegate d;
        if (m_EventTable.TryGetValue(eventType, out d))
        {
            CallBack<T, X, Y, Z> callBack = d as CallBack<T, X, Y, Z>;
            if (callBack != null)
            {
                callBack(arg1, arg2, arg3, arg4);
            }
            else
            {
                throw new Exception(string.Format("广播事件错误：事件{0}对应委托具有不同的类型", eventType));
            }
        }
    }
    //five parameters
    public static void Broadcast<T, X, Y, Z, W>(EventType eventType, T arg1, X arg2, Y arg3, Z arg4, W arg5)
    {
        Delegate d;
        if (m_EventTable.TryGetValue(eventType, out d))
        {
            CallBack<T, X, Y, Z, W> callBack = d as CallBack<T, X, Y, Z, W>;
            if (callBack != null)
            {
                callBack(arg1, arg2, arg3, arg4, arg5);
            }
            else
            {
                throw new Exception(string.Format("广播事件错误：事件{0}对应委托具有不同的类型", eventType));
            }
        }
    }
}
