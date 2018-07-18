using UnityEngine;
using System.Collections;
using UnityEngine.Analytics;
using UnityEngine.Experimental.UIElements;


namespace GameEvents {
    /// <summary>
    ///	base class for a game action / statechange / user input
    /// The event holds all data necessary to describe itself, so that listeners 
    /// are able to interpret the content.
    /// </summary>
    public abstract class GameEvent {
        static int _idSequencer = 0;
        public static int GetNextEventID() { return ++_idSequencer; }

        //	constructor
        public GameEvent() {
            uniqueEventID = GetNextEventID();
            time = Time.time;
        }

        /// <summary>
        /// it's always a good idea to have a unique identifier for collected information objects.
        /// </summary>
        public readonly int uniqueEventID;

        /// <summary>
        /// time the event occured
        /// </summary>
        public readonly float time;

        /// <summary>
        /// each event will be able to test its data.
        /// this way, it isn't important if a sounddesigner or programmer implement/change an event,
        /// as both will be notified in case a misunderstanding / miscommunication occured.
        /// This is a convenient way to optimize maintainability, as the code will tell you what's wrong.
        /// </summary>
        public virtual bool isValid() { return true; }
    }

    //---------------------------------------------------------------------------------------------------

    // sample events without additional arguments

    public class GameEvent_Click : GameEvent { }
    public class GameEvent_CancelAction : GameEvent { }
    public class GameEvent_FinishLevel : GameEvent { }

    //---------------------------------------------------------------------------------------------------

    // sample events with additional arguments. Make sure these implement Validate()


    /// <summary>
    /// A list of the possible simplified Game Engine base events
    /// </summary>
    public enum GameEngineEventType {
        LevelStart,
        LevelComplete,
        LevelEnd,
        Pause,
        UnPause,
        PlayerDeath,
        Respawn,
        StarPicked
    }

    public class GameEvent_Engine : GameEvent {
        public GameEngineEventType EventType;
        public GameEvent_Engine(GameEngineEventType t) {
            EventType = t;
        }

        public override bool isValid() {
            return EventType != null;
        }
    }

    public class GameEvent_SendSeed : GameEvent {
        public string seed;

        public GameEvent_SendSeed(string newSeed) {
            seed = newSeed;
            Debug.Log("GameEventManager called me");
        }

        public string GetHostSeed() {
            return seed;
        }
    }

    public class GameEvent_SendSeedToClient : GameEvent {
        public string seed;

        public GameEvent_SendSeedToClient(string newSeed) {
            seed = newSeed;
            Debug.Log("GameEventManager called me");
        }

        public string GetHostSeed() {
            return seed;
        }
    }

    public class GameEvent_RequestHostSeed : GameEvent {
        public string seed;

        public GameEvent_RequestHostSeed() {

        }

        public string GetHostSeed() {
            return seed;
        }
    }
}