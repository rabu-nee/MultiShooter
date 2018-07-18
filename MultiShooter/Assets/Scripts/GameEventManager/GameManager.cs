using UnityEngine;
using System.Collections;
using Tools;
using GameEvents;
using System.Collections.Generic;




/// The game manager is a persistent singleton that handles global stuff, like e.g., scoring and time
public class GameManager : PersistentSingleton<GameManager>,
  IGameEventListener<GameEvent_Engine>
{
  
  
  void Awake() {
    GameEventManager.TriggerEvent(new GameEvent_Engine(GameEngineEventType.LevelStart));
  }


  public virtual void OnGameEvent(GameEvent_Engine e) { }
  
  /// <summary>
  /// OnDisable, we start listening to events.
  /// </summary>
  protected virtual void OnEnable()
  {
    this.EventStartListening<GameEvent_Engine> ();
  }

  /// <summary>
  /// OnDisable, we stop listening to events.
  /// </summary>
  protected virtual void OnDisable()
  {
    this.EventStopListening<GameEvent_Engine> ();
  }
  
  
  
  
  
  
}
