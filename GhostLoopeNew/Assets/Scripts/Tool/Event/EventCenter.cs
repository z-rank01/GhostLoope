using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public enum E_Event
{
    LoadScene, 
    Conversation,
    ReceiveDamage,
    PlayerReceiveDamage, 

    BossShadeIncreaseMobOnScene,
    BossShadeDecreaseMobOnScene,
    TenacityReceiveDamage, 
    TenacityBulletReturn,

    BossWraithChainSuccess,
    BossWraithChainFail
}

// ����ת��ԭ�򣬻���װ����
public interface IEventInfo
{

}


public class EventInfo<T> : IEventInfo
{
    public UnityAction<T> actions;

    public EventInfo(UnityAction<T> action)
    {
        actions += action;
    }
}


public class EventInfo : IEventInfo
{
    public UnityAction actions;

    public EventInfo(UnityAction action)
    {
        actions += action;
    }
}

public class EventCenter: BaseSingleton<EventCenter>
{
    // Key: �¼�������
    // Value: ��Ӧ��ί�к���
    private Dictionary<E_Event, IEventInfo> m_EventListeners = new Dictionary<E_Event, IEventInfo>();

    // һ������������¼�
    public void AddEventListener<T>(E_Event eventName, UnityAction<T> listener)
    {
        Debug.Log("AddEventListener");
        if (m_EventListeners.ContainsKey(eventName))
        {
            (m_EventListeners[eventName] as EventInfo<T>).actions += listener;
        }
        else
        {
            m_EventListeners.Add(eventName, new EventInfo<T>(listener));
        }
    }

    // �޲���������¼�
    public void AddEventListener(E_Event eventName, UnityAction listener)
    {
        Debug.Log("AddEventListener");
        if (m_EventListeners.ContainsKey(eventName))
        {
            (m_EventListeners[eventName] as EventInfo).actions += listener;
        }
        else
        {
            m_EventListeners.Add(eventName, new EventInfo(listener));
        }
    }


    public void RemoveEventListener<T>(E_Event eventName, UnityAction<T> listener)
    {
        if (m_EventListeners.ContainsKey(eventName))
        {
            (m_EventListeners[eventName] as EventInfo<T>).actions -= listener;
        }
    }


    public void RemoveEventListener(E_Event eventName, UnityAction listener)
    {
        if (m_EventListeners.ContainsKey(eventName))
        {
            (m_EventListeners[eventName] as EventInfo).actions -= listener;
        }
    }

    public void EventTrigger<T>(E_Event eventName, T info)
    {

        if (m_EventListeners.ContainsKey(eventName) && (m_EventListeners[eventName] as EventInfo<T>).actions != null)
        {
            (m_EventListeners[eventName] as EventInfo<T>).actions.Invoke(info);
        }
    }


    public void EventTrigger(E_Event eventName)
    {

        if (m_EventListeners.ContainsKey(eventName) && (m_EventListeners[eventName] as EventInfo).actions != null)
        {
            (m_EventListeners[eventName] as EventInfo).actions.Invoke();
        }
    }


    // �����л�ʱ���ܻ��õ���պ���
    public void Clear()
    {
        m_EventListeners.Clear();
    }

}
