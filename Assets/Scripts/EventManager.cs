using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager
{
    Dictionary<EventName, HashSet<ISubscriber>> eventMap;
    private EventManager instance;

    private EventManager()
    {
        eventMap = new Dictionary<EventName, HashSet<ISubscriber>>();
    }

    public EventManager Instance()
    {
        if (instance == null)
            instance = new EventManager();

        return instance;
    }

    public void Notify(EventName eventName, IParam parameters)
    {
        HashSet<ISubscriber> subList;

        if (eventMap.TryGetValue(eventName, out subList))
        {
            foreach(ISubscriber subscriber in subList)
            {
                subscriber.Notify(eventName, parameters);
            }
        }
    }

    void Subscribe(EventName eventName, ISubscriber subscriber)
    {
        HashSet<ISubscriber> subList;
        
        if (eventMap.TryGetValue(eventName, out subList))
        {
            subList.Add(subscriber);
        }
        else
        {
            subList = new HashSet<ISubscriber>();
            subList.Add(subscriber);
            eventMap.Add(eventName, subList);
        }
    }

    void Unsubscribe(EventName eventName, ISubscriber subscriber)
    {
        HashSet<ISubscriber> subList;

        if (eventMap.TryGetValue(eventName, out subList))
        {
            if(subList.Contains(subscriber))
            {
                subList.Remove(subscriber);
            }
        }
    }
}
