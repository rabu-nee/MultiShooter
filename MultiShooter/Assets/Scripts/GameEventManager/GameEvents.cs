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
        }

        public string GetHostSeed() {
            return seed;
        }
    }

    public class GameEvent_RespawnNow : GameEvent {
        public Transform[] startPositions;
        public Transform[] enemyStartPos;
        public Transform[] obstacleStartPos;
        public Transform[] waypointStartPos;

        public GameEvent_RespawnNow(Transform[] newStartPositions, Transform[] newEnemyStartPos, Transform[] newObstacleStartPos, Transform[] newWaypointStartPos) {
            startPositions = newStartPositions;
            enemyStartPos = newEnemyStartPos;
            obstacleStartPos = newObstacleStartPos;
            waypointStartPos = newWaypointStartPos;
        }

        public Transform[] GetStartPos() {
            return startPositions;
        }

        public Transform[] GetEnemyPos() {
            return enemyStartPos;
        }

        public Transform[] GetObstaclePos() {
            return obstacleStartPos;
        }

        public Transform[] GetWaypointPos() {
            return waypointStartPos;
        }
    }

    public class GameEvent_RespawnDeath : GameEvent {
        public float scorePenalty;
        public GameObject instigator;

        public GameEvent_RespawnDeath(float newPenalty, GameObject newInstigator) {
            scorePenalty = newPenalty;
            instigator = newInstigator;
        }

        public float GetPenalty() {
            return scorePenalty;
        }

        public GameObject GetInstigator() {
            return instigator;
        }
    }

    public class GameEvent_EnemyKill : GameEvent {
        public float scoreAdd;
        public GameObject instigator;

        public GameEvent_EnemyKill(float newScoreAdd, GameObject newInstigator) {
            scoreAdd = newScoreAdd;
            instigator = newInstigator;
        }

        public float GetScoreAdd() {
            return scoreAdd;
        }

        public GameObject GetInstigator() {
            return instigator;
        }
    }

    public class GameEvent_PlayerKill : GameEvent {
        public float scoreAdd;
        public GameObject instigator;

        public GameEvent_PlayerKill(float newScoreAdd, GameObject newInstigator) {
            scoreAdd = newScoreAdd;
            instigator = newInstigator;
        }

        public float GetScoreAdd() {
            return scoreAdd;
        }

        public GameObject GetInstigator() {
            return instigator;
        }
    }

    public class GameEvent_Shoot : GameEvent {
        public GameEvent_Shoot() {
            
        }
    }

    public class GameEvent_Spawn : GameEvent {
        public int enemyNumber;

        public GameEvent_Spawn(int newEnemyNumber) {
            enemyNumber = newEnemyNumber;
        }

        public int GetEnemyNumber() {
            return enemyNumber;
        }
    }

    public class GameEvent_Win : GameEvent {

        public GameEvent_Win() {

        }
    }

}