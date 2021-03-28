﻿using System;
using System.Collections.Generic;
using UnityEngine;

public static class EventManager
{
    [Serializable]
    public delegate void CallBackMethod();

    [Serializable]
    public class Event
    {
        public int uniqueID;
        public string eventCode;
        public bool hasTriggered;
        public bool hasListeners;

        public CallBackMethod listeners;

        public Event(int uniqueID, string eventCode, CallBackMethod listener) {

            this.uniqueID = uniqueID;
            
            //Null-Checking
            if (string.IsNullOrEmpty(eventCode))
            {
                //There is no
                this.eventCode = "Unassigned";
            }
            else
            {
                this.eventCode = eventCode;
            }
            
            hasTriggered = false;
        }

        /// <summary>
        /// Return the uniqueId given to this event
        /// </summary>
        /// <returns></returns>
        public int GetUniqueID() => uniqueID;

        /// <summary>
        /// Return the eventCode given to this event
        /// </summary>
        /// <returns></returns>
        public string GetEventCode() => eventCode;

        public void AddNewListener(CallBackMethod listener)
        {
            if (listener == null)
            {
                listeners = new CallBackMethod(listener);
                Debug.Log(listeners.Method.Name);
            }

            listeners += listener;

            HasListerners();
        }

        public void RemoveListener(CallBackMethod listener)
        {
            if(listeners != null)
                listeners -= listener;
        }

        /// <summary>
        /// Trigger this event, executing all listeners assigned to it.
        /// </summary>
        public void Trigger()
        {
            if (listeners != null)
            {
                hasTriggered = true;
                listeners.Invoke();
                return;
            }

            Debug.LogError("There are no listeners in this event...");
            return;
        }

        /// <summary>
        /// Set HasTriggered to false, as if it hasn't been triggered
        /// </summary>
        public void Reset()
        {
            if (hasTriggered)
                hasTriggered = false;
        }

        /// <summary>
        /// Returns if this even has been triggered
        /// </summary>
        public bool HasTriggered()
        {
            return hasTriggered;
        }


        public bool HasListerners()
        {
            hasListeners = (listeners.GetInvocationList().Length != 0);
            return hasListeners;
        }
    }

    //This associated an event with
    static List<Event> Events = new List<Event>();

    /// <summary>
    /// Add a new event with a uniqueID, name, and defined listeners
    /// </summary>
    /// <param name="uniqueID"></param>
    /// <param name="name"></param>
    /// <param name="listeners"></param>
    public static Event AddNewEvent(int uniqueID, string name, params CallBackMethod[] listeners)
    {
        Event newEvent = new Event(uniqueID, name, null);
        foreach (CallBackMethod listener in listeners)
        {
            newEvent.AddNewListener(listener);
            Events.Add(newEvent);
        }

        return newEvent;
    }

    /// <summary>
    /// Remove an event based on it's eventCode
    /// </summary>
    /// <param name="eventCode"></param>
    public static void RemoveEvent(string eventCode)
    {
        for(int idIndex = 0; idIndex < Events.Count; idIndex++)
        {
            //If we found the event with this eventCode, remove it
            if (eventCode == Events[idIndex].GetEventCode())
            {
                //Now delete the event itself
                Events.Remove(Events[idIndex]);
                return;
            }
        }
    }

    /// <summary>
    /// Remove an event based on it's uniqueID
    /// </summary>
    /// <param name="eventCode"></param>
    public static void RemoveEvent(int uniqueId)
    {
        for (int idIndex = 0; idIndex < Events.Count; idIndex++)
        {
            //If we found the event with this eventCode, remove it
            if (uniqueId.Equals(Events[idIndex].GetUniqueID()))
            {
                //Now delete the event itself
                Events.Remove(Events[idIndex]);
            }
        }
    }

    /// <summary>
    /// Remove an event 
    /// </summary>
    /// <param name="eventCode"></param>
    public static void RemoveEvent(Event @event)
    {
        for (int idIndex = 0; idIndex < Events.Count; idIndex++)
        {
            //If we found the event with this eventCode, remove it
            if (@event.Equals(Events[idIndex]))
            {
                //Now delete the event itself
                Events.Remove(Events[idIndex]);
            }
        }
    }

    /// <summary>
    /// Retuns all events of this event code
    /// </summary>
    /// <param name="eventCode"></param>
    /// <returns></returns>
    public static Event[] FindEventsOfEventCode(string eventCode)
    {
        List<Event> foundEvents = new List<Event>();
        for (int idIndex = 0; idIndex < Events.Count; idIndex++)
        {
            //If we found the event with this eventCode, remove it
            if (eventCode.Equals(Events[idIndex].GetEventCode()))
            {
                //Add it to our discorvered events
                foundEvents.Add(Events[idIndex]);
            }
        }

        //Return the foundEvents
        return foundEvents.ToArray();
    }

    /// <summary>
    /// Check if all events of this kind have been triggered
    /// </summary>
    /// <param name="events"></param>
    /// <returns></returns>
    public static bool HaveAllTriggered(this Event[] events)
    {
        foreach(Event @event in events)
        {
            if (!@event.HasTriggered()) return false;
        }

        return true;
    }

    public static void TriggerEvent(int uniqueId)
    {
        for (int idIndex = 0; idIndex < Events.Count; idIndex++)
        {
            //If we found the event with this eventCode, remove it
            if (uniqueId.Equals(Events[idIndex].GetUniqueID()))
            {
                //Trigger events of this uniqueID
                Events[idIndex].Trigger();
            }
            else
            {
                Events[idIndex].hasTriggered = false;
            }
        }
    }

    public static void TriggerEvent(string eventCode)
    {
        for (int idIndex = 0; idIndex < Events.Count; idIndex++)
        {
            //If we found the event with this eventCode, remove it
            if (eventCode.Equals(Events[idIndex].GetEventCode()))
            {
                //Trigger events of this eventCode
                Events[idIndex].Trigger();
            }
            else
            {
                Events[idIndex].hasTriggered = false;
            }
        }
    }

    /// <summary>
    /// Returns all events of different IDs and EventCodes
    /// </summary>
    /// <returns></returns>
    public static Event[] GetAllEvents() => Events.ToArray();
}
