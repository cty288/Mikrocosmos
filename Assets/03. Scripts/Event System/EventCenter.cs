using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Obsolete]
public static class EventCenter
{
    private static Dictionary<EventType, Delegate> eventTable = new Dictionary<EventType, Delegate>();

    /// <summary>
    /// Add a listener to the corresponding event type, which must have at least one callback function
    /// </summary>
    /// <param name="eventType">The type of the event (create it in the EventType class)</param>
    /// <param name="callBack">A callback function, which can have 0-5 parameters</param>
    public static void AddListener(EventType eventType, CallBack callBack)
    {
        if (OnListenerAdding(eventType, callBack)) {
            //�ಥί�����
            eventTable[eventType] = (CallBack)eventTable[eventType] + callBack;
        }

    }


    /// <summary>
    ///  Add a listener to the corresponding event type, which must have at least one callback function with one parameter
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="eventType">The type of the event (create it in the EventType class)</param>
    /// <param name="callBack">A callback function, which can have 1 parameters</param>
    public static void AddListener<T>(EventType eventType, CallBack<T> callBack)
    {
        if (OnListenerAdding(eventType, callBack)) {
            //�ಥί�����
            eventTable[eventType] = (CallBack<T>) eventTable[eventType] + callBack;
        }
    }


    public static void AddListener<T, X>(EventType eventType, CallBack<T, X> callBack)
    {
        if (OnListenerAdding(eventType, callBack)) {
            //�ಥί�����
            eventTable[eventType] = (CallBack<T, X>) eventTable[eventType] + callBack;
        }
    }


    public static void AddListener<T, X, Y>(EventType eventType, CallBack<T, X, Y> callBack)
    {
        if (OnListenerAdding(eventType, callBack)) {
            //�ಥί�����
            eventTable[eventType] = (CallBack<T, X, Y>) eventTable[eventType] + callBack;
        }
    }


    public static void AddListener<T, X, Y, Z>(EventType eventType, CallBack<T, X, Y, Z> callBack)
    {
        if (OnListenerAdding(eventType, callBack)) {
            //�ಥί�����
            eventTable[eventType] = (CallBack<T, X, Y, Z>) eventTable[eventType] + callBack;
        }
    }


    public static void AddListener<T, X, Y, Z, W>(EventType eventType, CallBack<T, X, Y, Z, W> callBack)
    {
        if (OnListenerAdding(eventType, callBack)) {
            //�ಥί�����
            eventTable[eventType] = (CallBack<T, X, Y, Z, W>) eventTable[eventType] + callBack;
        }
    }

    /// <summary>
    ///  Remove a callback function from the corresponding event type, which must have at least one callback function
    /// </summary>
    /// <param name="eventType">The type of the event (create it in the EventType class)</param>
    /// <param name="callBack">A callback function with no parameters</param>
    public static void RemoveListener(EventType eventType, CallBack callBack)
    {
        OnListenerRemoving(eventType, callBack);
        //�Ƴ�ָ���Ļص�����
        eventTable[eventType] = (CallBack)eventTable[eventType] - callBack;
        OnListenerRemoved(eventType);
    }


    /// <summary>
    ///  Remove a callback function from the corresponding event type, which must have at least one callback function with one parameter
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="eventType">type of the event</param>
    /// <param name="callBack">callback function with one parameter</param>
    public static void RemoveListener<T>(EventType eventType, CallBack<T> callBack)
    {
        OnListenerRemoving(eventType, callBack);
        //�Ƴ�ָ���Ļص�����
        eventTable[eventType] = (CallBack<T>)eventTable[eventType] - callBack;
        OnListenerRemoved(eventType);
    }


    public static void RemoveListener<T, X>(EventType eventType, CallBack<T, X> callBack)
    {
        OnListenerRemoving(eventType, callBack);
        //�Ƴ�ָ���Ļص�����
        eventTable[eventType] = (CallBack<T, X>)eventTable[eventType] - callBack;
        OnListenerRemoved(eventType);
    }


    public static void RemoveListener<T, X, Y>(EventType eventType, CallBack<T, X, Y> callBack)
    {
        OnListenerRemoving(eventType, callBack);
        //�Ƴ�ָ���Ļص�����
        eventTable[eventType] = (CallBack<T, X, Y>)eventTable[eventType] - callBack;
        OnListenerRemoved(eventType);
    }


    public static void RemoveListener<T, X, Y, Z>(EventType eventType, CallBack<T, X, Y, Z> callBack)
    {
        OnListenerRemoving(eventType, callBack);
        //�Ƴ�ָ���Ļص�����
        eventTable[eventType] = (CallBack<T, X, Y, Z>)eventTable[eventType] - callBack;
        OnListenerRemoved(eventType);
    }


    public static void RemoveListener<T, X, Y, Z, W>(EventType eventType, CallBack<T, X, Y, Z, W> callBack)
    {
        OnListenerRemoving(eventType, callBack);
        //�Ƴ�ָ���Ļص�����
        eventTable[eventType] = (CallBack<T, X, Y, Z, W>)eventTable[eventType] - callBack;
        OnListenerRemoved(eventType);
    }



    /// <summary>
    /// Broadcast all callback functions with no parameters from an existing eventType
    /// </summary>
    /// <param name="eventType">The type of the event</param>
    public static void Broadcast(EventType eventType)
    {
        Delegate d;
        //��ȡ�������µ�ί��
        if (eventTable.TryGetValue(eventType, out d))
        {
            CallBack callback = d as CallBack;
            if (callback != null)
            {
                //����ί�е����лص�����
                callback();
            }
            else
            {
                throw new Exception(string.Format("Failed to broadcast event {0}: The delegate has a different type", eventType));
            }
        }
    }


    /// <summary>
    /// Broadcast an event with one parameter
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="eventType"></param>
    /// <param name="arg">parameter</param>
    public static void Broadcast<T>(EventType eventType, T arg)
    {
        Delegate d;
        //��ȡ�������µ�ί��
        if (eventTable.TryGetValue(eventType, out d))
        {
            CallBack<T> callback = d as CallBack<T>;
            if (callback != null)
            {
                //����ί�е����лص�����
                callback(arg);
            }
            else
            {
                throw new Exception(string.Format("Failed to broadcast event {0}: The delegate has a different type", eventType));
            }
        }
    }


    public static void Broadcast<T, X>(EventType eventType, T arg1, X arg2)
    {
        Delegate d;
        //��ȡ�������µ�ί��
        if (eventTable.TryGetValue(eventType, out d))
        {
            CallBack<T, X> callback = d as CallBack<T, X>;
            if (callback != null)
            {
                //����ί�е����лص�����
                callback(arg1, arg2);
            }
            else
            {
                throw new Exception(string.Format("Failed to broadcast event {0}: The delegate has a different type", eventType));
            }
        }
    }


    public static void Broadcast<T, X, Y>(EventType eventType, T arg1, X arg2, Y arg3)
    {
        Delegate d;
        //��ȡ�������µ�ί��
        if (eventTable.TryGetValue(eventType, out d))
        {
            CallBack<T, X, Y> callback = d as CallBack<T, X, Y>;
            if (callback != null)
            {
                //����ί�е����лص�����
                callback(arg1, arg2, arg3);
            }
            else
            {
                throw new Exception(string.Format("Failed to broadcast event {0}: The delegate has a different type", eventType));
            }
        }
    }

    public static void Broadcast<T, X, Y, Z>(EventType eventType, T arg1, X arg2, Y arg3, Z arg4)
    {
        Delegate d;
        //��ȡ�������µ�ί��
        if (eventTable.TryGetValue(eventType, out d))
        {
            CallBack<T, X, Y, Z> callback = d as CallBack<T, X, Y, Z>;
            if (callback != null)
            {
                //����ί�е����лص�����
                callback(arg1, arg2, arg3, arg4);
            }
            else
            {
                throw new Exception(string.Format("Failed to broadcast event {0}: The delegate has a different type", eventType));
            }
        }
    }

    public static void Broadcast<T, X, Y, Z, W>(EventType eventType, T arg1, X arg2, Y arg3, Z arg4, W arg5)
    {
        Delegate d;
        //��ȡ�������µ�ί��
        if (eventTable.TryGetValue(eventType, out d))
        {
            CallBack<T, X, Y, Z, W> callback = d as CallBack<T, X, Y, Z, W>;
            if (callback != null)
            {
                //����ί�е����лص�����
                callback(arg1, arg2, arg3, arg4, arg5);
            }
            else
            {
                throw new Exception(string.Format("Failed to broadcast event {0}: The delegate has a different type", eventType));
            }
        }
    }
    private static bool OnListenerAdding(EventType eventType, Delegate callBack)
    {
        //�¼���ָ���¼����͵�ֵΪ��ʱ, Ϊ���¼����ʹ���һ���µĿռ�, ������ί��
        if (!eventTable.ContainsKey(eventType))
        {
            eventTable.Add(eventType, callBack);
            return false;
        }

        //����һ��ί�м������е��¼�������ʱ, ���ж����ί�������Ƿ������ǰ������
        Delegate d = eventTable[eventType];
        if (d != null && d.GetType() != callBack.GetType())
        {
            throw new Exception(string.Format("A wrong type of delegate {0} was trying to be added to event {1}. The proper type is {2}", d.GetType(), eventType, callBack.GetType()));
            return false;
        }
        return true;
    }

    private static void OnListenerRemoving(EventType eventType, Delegate callBack)
    {
        //�������Ƿ��и��¼�����
        if (eventTable.ContainsKey(eventType))
        {
            Delegate d = eventTable[eventType];
            //�ж��¼������Ƿ�����κλص�����
            if (d == null)
            {
                throw new Exception(string.Format("The event {0} does not have corresponding delegate", eventType));
                //�ж��Ƴ����¼��������Ƿ�ͱ��е�����һ��
            }
            else if (d.GetType() != callBack.GetType())
            {
                throw new Exception(string.Format("The delegate type {0} is not the same as the delegate type {1} of event {2}", callBack.GetType(), d.GetType(), eventType));
            }
        }
        else
        {
            throw new Exception("Failed to remove the listener: no eventType");
        }
    }

    private static void OnListenerRemoved(EventType eventType)
    {
        if (eventTable[eventType] == null)
        {
            eventTable.Remove(eventType);
        }
    }
}
