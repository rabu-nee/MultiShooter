//#define EVENTROUTER_THROWEXCEPTIONS
#if EVENTROUTER_THROWEXCEPTIONS
//#define EVENTROUTER_REQUIRELISTENER // Uncomment this if you want listeners to be required for sending events.
#endif

using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

// GameEvents are used throughout the game for general game events (game started, game ended, life lost, etc.)
/*


*/

namespace GameEvents
{

  /// This class handles event management, and can be used to broadcast events throughout the game, to tell one class (or many) that something's happened.
  /// Events are classes derived from GameEvent, you can define any kind of events you want. This manager comes with GameEvents, which are
  /// basically just made of a string, but you can work with more complex ones if you want.
  ///
  /// To trigger a new event, from anywhere, just call EventManager.TriggerEvent(YOUR_EVENT);
  /// For example : EventManager.TriggerEvent(new GameEvent("GameStart")); will broadcast an GameEvent named GameStart to all listeners.
  ///
  /// To start listening to an event from any class, there are 3 things you must do :
  ///
  /// 1 - tell that your class implements the IGameEventListener interface for that kind of event.
  /// For example: public class SoundManager : Singleton<SoundManager>, IGameEventListener<MyGameEvent>
  /// You can have more than one of these (one per event type).
  ///
  /// 2 - On Enable and Disable, respectively start and stop listening to the event :
  /// void OnEnable()
  /// {
  /// 	this.EventStartListening<MyGameEvent>();
  /// }
  /// void OnDisable()
  /// {
  /// 	this.EventStopListening<MyGameEvent>();
  /// }
  ///
  /// 3 - Implement the IGameEventListener interface for that event. For example :
  /// public void OnGameEvent(MyGameEvent gameEvent)
  /// {
  /// 	if (gameEvent.eventName == "GameOver")
  ///		{
  ///			// DO SOMETHING
  ///		}
  /// }
  /// will catch all events of type MyGameEvent emitted from anywhere in the game, and do something if it's named GameOver

  [ExecuteInEditMode]
  public static class GameEventManager
  {
    private static readonly Dictionary<Type, List<IGameEventListener>> _subscribersList;

    static GameEventManager() {
      _subscribersList = new Dictionary<Type, List<IGameEventListener>>();
    }

    /// Adds a new subscriber to a certain event.
    public static void AddListener<TEvent>(IGameEventListener<TEvent> listener) where TEvent : GameEvent {
      Type eventType = typeof(TEvent);

      if (!_subscribersList.ContainsKey(eventType))
        _subscribersList[eventType] = new List<IGameEventListener>();

      if (!SubscriptionExists(eventType, listener))
        _subscribersList[eventType].Add(listener);
    }

    /// Removes a subscriber from a certain event.
    public static void RemoveListener<TEvent>(IGameEventListener<TEvent> listener) where TEvent : GameEvent {
      Type eventType = typeof(TEvent);

      if (!_subscribersList.ContainsKey(eventType)) {
#if EVENTROUTER_THROWEXCEPTIONS
          throw new ArgumentException( string.Format( "Removing listener \"{0}\", but the event type \"{1}\" isn't registered.", listener, eventType.ToString() ) );
        #else
        return;
#endif
      }

      List<IGameEventListener> subscriberList = _subscribersList[eventType];
      bool listenerFound;
      listenerFound = false;


      for (int i = 0; i < subscriberList.Count; i++) {
        if (subscriberList[i] == listener) {
          subscriberList.Remove(subscriberList[i]);
          listenerFound = true;

          if (subscriberList.Count == 0)
            _subscribersList.Remove(eventType);

          return;
        }
      }

#if EVENTROUTER_THROWEXCEPTIONS
       if( !listenerFound )
       {
         throw new ArgumentException( string.Format( "Removing listener, but the supplied receiver isn't subscribed to event type \"{0}\".", eventType.ToString() ) );
       }
#endif
    }

    /// Triggers an event. All instances that are subscribed to it will receive it (and will potentially act on it).
    public static void TriggerEvent<TEvent>(TEvent newEvent) where TEvent : GameEvent {
      List<IGameEventListener> list;
      if (!_subscribersList.TryGetValue(typeof(TEvent), out list))
#if EVENTROUTER_REQUIRELISTENER
        throw new ArgumentException( string.Format( "Attempting to send event of type \"{0}\", but no listener for this type has been found. Make sure this.Subscribe<{0}>(EventRouter) has been called, or that all listeners to this event haven't been unsubscribed.", typeof( Event ).ToString() ) );
#else
        return;
#endif

      for (int i = 0; i < list.Count; i++) {
        (list[i] as IGameEventListener<TEvent>).OnGameEvent(newEvent);
      }
    }

    /// Checks if there are subscribers for a certain type of events
    private static bool SubscriptionExists(Type type, IGameEventListener receiver) {
      List<IGameEventListener> receivers;

      if (!_subscribersList.TryGetValue(type, out receivers)) return false;

      bool exists = false;

      for (int i = 0; i < receivers.Count; i++) {
        if (receivers[i] == receiver) {
          exists = true;
          break;
        }
      }
      
      return exists;
    }
  }

  /// Static extension class that allows any class to start or stop listening to events
  public static class GameEventRegister
  {
    public static void EventStartListening<EventType>(this IGameEventListener<EventType> caller)
      where EventType : GameEvent {
      GameEventManager.AddListener<EventType>(caller);
    }

    public static void EventStopListening<EventType>(this IGameEventListener<EventType> caller)
      where EventType : GameEvent {
      GameEventManager.RemoveListener<EventType>(caller);
    }
  }

  /// Event listener basic interface
  public interface IGameEventListener
  { };

  /// A public interface you'll need to implement for each type of event you want to listen to.
  public interface IGameEventListener<T> : IGameEventListener
  where T : GameEvent
  {
    void OnGameEvent(T eventType);
  }

}
