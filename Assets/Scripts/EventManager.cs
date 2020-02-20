using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event
{
    //Keep track of what event type we want to used
    public string myEventID;
}

public static class EventManager
{
    //We'll need a delegate that'll represent the function tha
    //we call.
    public delegate void CallbackMethod();

    //The manager will hold a Dictionary that holds all listners
    private static Dictionary<string, CallbackMethod> listeners = new Dictionary<string, CallbackMethod>();

    /// <summary>
    /// Add a listener to receive an even to perform.
    /// </summary>
    /// <param name="myEvent">The event to call.</param>
    /// <param name="listener">The listener that waits for an event to occur.</param>
    public static void AddEventListener(string myEvent, CallbackMethod listener)
    {
        //If there is no listener for this event, add one
        if (!listeners.ContainsKey(myEvent))
            listeners.Add(myEvent, listener);
        else
            //Attach the callback method to the listner
            listeners[myEvent] += listener;
    }

    /// <summary>
    /// Remove a listener from the listener's list. If the listners has no more events, it'll be removed completely.
    /// </summary>
    /// <param name="myEvent">The event to call.</param>
    /// <param name="listener">The listener to remove.</param>
    public static void RemoveEventListener(string myEvent, CallbackMethod listener)
    {
        //If there is a delegate for this even, remove the listener passed in from the delegate
        if (listeners.ContainsKey(myEvent))
        {
            listeners[myEvent] -= listener;
            //If the linstener list is empty, remove the event completely
            if (listeners[myEvent] == null)
                listeners.Remove(myEvent);
        }
    }

    /// <summary>
    /// Trigger an event that has been registered
    /// </summary>
    /// <param name="myEvent">The event to trigger.</param>
    public static void TriggerEvent(string myEvent)
    {
        //If the event exists, we want to call the delegate for this event
        if (listeners.ContainsKey(myEvent))
            listeners[myEvent]();
    }
}
