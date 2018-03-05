﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class DataEventManager
{
    public int value1;
    public int value2;

    public DataEventManager(int a, int b)
    {
        value1 = a;
        value2 = b;
    }
}

/// <summary>
/// EventManager Description
/// </summary>
public class EventManager : MonoBehaviour
{
    #region Attributes

    private class UnityEventInt : UnityEvent<int>  {    }
    private class UnityEvent2Int : UnityEvent<int, int>  {    }
    private class UnityEventBool : UnityEvent<bool>  {    }
    private class UnityEventBoolInt : UnityEvent<bool, int> { }
    private class UnityEventData : UnityEvent<DataEventManager>  {    }

    private Dictionary<GameData.Event, UnityEvent> eventDictionary;
    private Dictionary<GameData.Event, UnityEventInt> eventDictionaryInt;
    private Dictionary<GameData.Event, UnityEvent2Int> eventDictionary2Int;
    private Dictionary<GameData.Event, UnityEventBool> eventDictionaryBool;
    private Dictionary<GameData.Event, UnityEventBoolInt> eventDictionaryBoolInt;
    private Dictionary<GameData.Event, UnityEventData> eventDictionaryData;

    private static EventManager eventManager;

    public static EventManager instance
    {
        get
        {
            if (!eventManager)
            {
                eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;

                if (!eventManager)
                {
                    Debug.LogError("There needs to be one active EventManger script on a GameObject in your scene.");
                }
                else
                {
                    eventManager.Init();
                }
            }

            return eventManager;
        }
    }
    #endregion

    #region Initialization
    /// <summary>
    /// test si on met le script en UNIQUE
    /// </summary>
    void Init()
    {
        if (eventDictionary == null)
            eventDictionary = new Dictionary<GameData.Event, UnityEvent>();
        if (eventDictionaryInt == null)
            eventDictionaryInt = new Dictionary<GameData.Event, UnityEventInt>();
        if (eventDictionary2Int == null)
            eventDictionary2Int = new Dictionary<GameData.Event, UnityEvent2Int>();
        if (eventDictionaryBool == null)
            eventDictionaryBool = new Dictionary<GameData.Event, UnityEventBool>();
        if (eventDictionaryBoolInt == null)
            eventDictionaryBoolInt = new Dictionary<GameData.Event, UnityEventBoolInt>();
        if (eventDictionaryData == null)
            eventDictionaryData = new Dictionary<GameData.Event, UnityEventData>();
    }
    #endregion

    #region Core
    /// <summary>
    /// ajoute un listener ?
    /// we look at the dictionary,
    /// we see if we have already a key value pair for what we are trying to add
    /// if so, we add to it,
    /// if not, we create a new unity event, we add the listener to it, 
    /// and we push that into the dictionary as the very first entry
    /// </summary>
    public static void StartListening(GameData.Event eventName, UnityAction listener)
    {
        UnityEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent();
            thisEvent.AddListener(listener);
            instance.eventDictionary.Add(eventName, thisEvent);
        }
    }
    public static void StartListening(GameData.Event eventName, UnityAction<int> listener)
    {
        UnityEventInt thisEvent = null;
        if (instance.eventDictionaryInt.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEventInt();
            thisEvent.AddListener(listener);
            instance.eventDictionaryInt.Add(eventName, thisEvent);
        }
    }
    public static void StartListening(GameData.Event eventName, UnityAction<int, int> listener)
    {
        UnityEvent2Int thisEvent = null;
        if (instance.eventDictionary2Int.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent2Int();
            thisEvent.AddListener(listener);
            instance.eventDictionary2Int.Add(eventName, thisEvent);
        }
    }
    public static void StartListening(GameData.Event eventName, UnityAction<bool, int> listener)
    {
        UnityEventBoolInt thisEvent = null;
        if (instance.eventDictionaryBoolInt.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEventBoolInt();
            thisEvent.AddListener(listener);
            instance.eventDictionaryBoolInt.Add(eventName, thisEvent);
        }
    }
    public static void StartListening(GameData.Event eventName, UnityAction<bool> listener)
    {
        UnityEventBool thisEvent = null;
        if (instance.eventDictionaryBool.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEventBool();
            thisEvent.AddListener(listener);
            instance.eventDictionaryBool.Add(eventName, thisEvent);
        }
    }
    public static void StartListening(GameData.Event eventName, UnityAction<DataEventManager> listener)
    {
        UnityEventData thisEvent = null;
        if (instance.eventDictionaryData.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEventData();
            thisEvent.AddListener(listener);
            instance.eventDictionaryData.Add(eventName, thisEvent);
        }
    }

    /// <summary>
    /// unregister
    /// </summary>
    public static void StopListening(GameData.Event eventName, UnityAction listener)
    {
        if (eventManager == null)   //au cas ou on a déja supprimé l'eventManager
            return;
        UnityEvent thisEvent = null;
        //si on veut unregister et que la clé existe dans le dico..
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }
    public static void StopListening(GameData.Event eventName, UnityAction<int> listener)
    {
        if (eventManager == null)   //au cas ou on a déja supprimé l'eventManager
            return;
        UnityEventInt thisEvent = null;
        //si on veut unregister et que la clé existe dans le dico..
        if (instance.eventDictionaryInt.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }
    public static void StopListening(GameData.Event eventName, UnityAction<int, int> listener)
    {
        if (eventManager == null)   //au cas ou on a déja supprimé l'eventManager
            return;
        UnityEvent2Int thisEvent = null;
        //si on veut unregister et que la clé existe dans le dico..
        if (instance.eventDictionary2Int.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }
    public static void StopListening(GameData.Event eventName, UnityAction<bool, int> listener)
    {
        if (eventManager == null)   //au cas ou on a déja supprimé l'eventManager
            return;
        UnityEventBoolInt thisEvent = null;
        //si on veut unregister et que la clé existe dans le dico..
        if (instance.eventDictionaryBoolInt.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }
    public static void StopListening(GameData.Event eventName, UnityAction<bool> listener)
    {
        if (eventManager == null)   //au cas ou on a déja supprimé l'eventManager
            return;
        UnityEventBool thisEvent = null;
        //si on veut unregister et que la clé existe dans le dico..
        if (instance.eventDictionaryBool.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }
    public static void StopListening(GameData.Event eventName, UnityAction<DataEventManager> listener)
    {
        if (eventManager == null)   //au cas ou on a déja supprimé l'eventManager
            return;
        UnityEventData thisEvent = null;
        //si on veut unregister et que la clé existe dans le dico..
        if (instance.eventDictionaryData.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    /// <summary>
    /// trigger un event
    /// </summary>
    public static void TriggerEvent(GameData.Event eventName)
    {
        UnityEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke();
        }
    }
    public static void TriggerEvent(GameData.Event eventName, int value)
    {
        UnityEventInt thisEvent = null;
        if (instance.eventDictionaryInt.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke(value);
        }
    }
    public static void TriggerEvent(GameData.Event eventName, int firstValue, int secondValue)
    {
        UnityEvent2Int thisEvent = null;
        if (instance.eventDictionary2Int.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke(firstValue, secondValue);
        }
    }
    public static void TriggerEvent(GameData.Event eventName, bool active, int secondValue)
    {
        UnityEventBoolInt thisEvent = null;
        if (instance.eventDictionaryBoolInt.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke(active, secondValue);
        }
    }
    public static void TriggerEvent(GameData.Event eventName, bool value)
    {
        UnityEventBool thisEvent = null;
        if (instance.eventDictionaryBool.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke(value);
        }
    }
    public static void TriggerEvent(GameData.Event eventName, DataEventManager data)
    {
        UnityEventData thisEvent = null;
        if (instance.eventDictionaryData.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke(data);
        }
    }
    #endregion

    #region Unity ending functions

    #endregion
}
