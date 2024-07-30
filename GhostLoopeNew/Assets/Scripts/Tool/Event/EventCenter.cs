using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public enum E_Event
{
    LoadScene, 
    input
}

public class EventCenter: BaseSingleton<EventCenter>
{
    // Key: �¼�������
    // Value: ��Ӧ��ί�к���
    private Dictionary<E_Event, UnityAction<object>> m_EventListeners = new Dictionary<E_Event, UnityAction<object>>();


    public void AddEventListener<T>(E_Event eventName, UnityAction<object> listener)
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


    public void RemoveEventListener(E_Event eventName, UnityAction<object> listener)
    {
        if (m_EventListeners.ContainsKey(eventName))
        {
            m_EventListeners[eventName] -= listener;
        }
    }

    public void EventTrigger(E_Event eventName, object info)
    {

        if (m_EventListeners.ContainsKey(eventName))
        {
            m_EventListeners[eventName].Invoke(info);
        }
    }

    // �����л�ʱ���ܻ��õ���պ���
    public void Clear()
    {
        m_EventListeners.Clear();
    }

}
