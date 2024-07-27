using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class EventCenter : BaseSingleton<EventCenter>
{
    // Key: �¼�������
    // Value: ��Ӧ��ί�к���
    private Dictionary<string, UnityAction<object>> m_EventListeners = new Dictionary<string, UnityAction<object>>();


    public void AddEventListener(string eventName, UnityAction<object> listener)
    {
        Debug.Log("AddEventListener");
        if (m_EventListeners.ContainsKey(eventName))
        {
            m_EventListeners[eventName] += listener;
        }
        else
        {
            m_EventListeners.Add(eventName, listener);
        }
    }


    public void RemoveEventListener(string eventName, UnityAction<object> listener)
    {
        if (m_EventListeners.ContainsKey(eventName))
        {
            m_EventListeners[eventName] -= listener;
        }
    }

    public void EventTrigger(string eventName, object info)
    {

        if (m_EventListeners.ContainsKey(eventName))
        {
            Debug.Log("EventTrigger");
            m_EventListeners[eventName].Invoke(info);
        }
    }

    // �����л�ʱ���ܻ��õ���պ���
    public void Clear()
    {
        m_EventListeners.Clear();
    }

}
