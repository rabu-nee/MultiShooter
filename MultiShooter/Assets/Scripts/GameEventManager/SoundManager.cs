using UnityEngine;
using System.Collections;
using Tools;
using GameEvents;
using System;

[Serializable]
public class SoundSettings {
    public bool MusicOn = true;
    public bool SfxOn = true;
}

public class SoundManager : PersistentSingleton<SoundManager>,
  IGameEventListener<GameEvent_Engine>,
    IGameEventListener<GameEvent_EnemyKill>,
    IGameEventListener<GameEvent_RespawnDeath>,
    IGameEventListener<GameEvent_Shoot>,
    IGameEventListener<GameEvent_Win>{
    /*
     * IGameEventListener<GameEvent_SOMETHINGSOMETHING>,
     * ...{
     */

    [Header("Settings")]
    public SoundSettings Settings;

    [Header("Music")]
    /// true if the music is enabled	
    //public bool MusicOn=true;
    /// the music volume
    [Range(0, 1)]
    public float MusicVolume = 0.3f;
    public AudioSource bgm;

    [Header("Sound Effects")]
    /// true if the sound fx are enabled
    //public bool SfxOn=true;
    /// the sound fx volume
    [Range(0, 1)]
    public float SfxVolume = 1f;
    

    protected const string _saveFolderName = "Engine/";
    protected const string _saveFileName = "sound.settings";

    protected AudioSource _backgroundMusic;

    /// <summary>
    /// Plays a background music.
    /// Only one background music can be active at a time.
    /// </summary>
    /// <param name="Clip">Your audio clip.</param>
    public virtual void PlayBackgroundMusic(AudioSource Music) {
        // if the music's been turned off, we do nothing and exit
        if (!Settings.MusicOn)
            return;
        // if we already had a background music playing, we stop it
        if (_backgroundMusic != null)
            _backgroundMusic.Stop();
        // we set the background music clip
        _backgroundMusic = Music;
        // we set the music's volume
        _backgroundMusic.volume = MusicVolume;
        // we set the loop setting to true, the music will loop forever
        _backgroundMusic.loop = true;
        // we start playing the background music
        _backgroundMusic.Play();
    }

    /// <summary>
    /// Plays a sound
    /// </summary>
    /// <returns>An audiosource</returns>
    /// <param name="sfx">The sound clip you want to play.</param>
    /// <param name="location">The location of the sound.</param>
    /// <param name="loop">If set to true, the sound will loop.</param>
    public virtual AudioSource PlaySound(AudioClip sfx, Vector3 location, bool loop = false) {
        if (!Settings.SfxOn)
            return null;
        // we create a temporary game object to host our audio source
        GameObject temporaryAudioHost = new GameObject("TempAudio");
        // we set the temp audio's position
        temporaryAudioHost.transform.position = location;
        // we add an audio source to that host
        AudioSource audioSource = temporaryAudioHost.AddComponent<AudioSource>() as AudioSource;
        // we set that audio source clip to the one in paramaters
        audioSource.clip = sfx;
        // we set the audio source volume to the one in parameters
        audioSource.volume = SfxVolume;
        // we set our loop setting
        audioSource.loop = loop;
        // we start playing the sound
        audioSource.Play();

        if (!loop) {
            // we destroy the host after the clip has played
            Destroy(temporaryAudioHost, sfx.length);
        }

        // we return the audiosource reference
        return audioSource;
    }

    /// <summary>
    /// Stops the looping sounds if there are any
    /// </summary>
    /// <param name="source">Source.</param>
    public virtual void StopLoopingSound(AudioSource source) {
        if (source != null) {
            Destroy(source.gameObject);
        }
    }

    protected virtual void SetMusic(bool status) {
        Settings.MusicOn = status;
        SaveSoundSettings();
    }

    protected virtual void SetSfx(bool status) {
        Settings.SfxOn = status;
        SaveSoundSettings();
    }

    public virtual void MusicOn() { SetMusic(true); }
    public virtual void MusicOff() { SetMusic(false); }
    public virtual void SfxOn() { SetSfx(true); }
    public virtual void SfxOff() { SetSfx(false); }

    protected virtual void SaveSoundSettings() {
        SaveLoadManager.Save(Settings, _saveFileName, _saveFolderName);
    }

    protected virtual void LoadSoundSettings() {
        SoundSettings settings = (SoundSettings)SaveLoadManager.Load(_saveFileName, _saveFolderName);
        if (settings != null) {
            Settings = settings;
        }
    }

    protected virtual void ResetSoundSettings() {
        SaveLoadManager.DeleteSave(_saveFileName, _saveFolderName);
    }

    // Event handlers -----------------------------------------------------------------------------

    public virtual void OnGameEvent(GameEvent_Engine e) {
        switch (e.EventType) {
            case (GameEngineEventType.LevelStart):
                Debug.Log("LevelStart");
                levelStartSound();
                PlayBackgroundMusic(bgm);
                break;
            case (GameEngineEventType.LevelComplete):
                Debug.Log("LevelComplete");
                levelCompleteSound();
                break;
            case (GameEngineEventType.LevelEnd):
                Debug.Log("LevelEnd");
                break;
            case (GameEngineEventType.Pause):
                Debug.Log("Pause");
                break;
            case (GameEngineEventType.UnPause):
                Debug.Log("UnPause");
                break;
            case (GameEngineEventType.PlayerDeath):
                Debug.Log("PlayerDeath");
                break;
            case (GameEngineEventType.Respawn):
                Debug.Log("Respawn");
                break;
            case (GameEngineEventType.StarPicked):
                Debug.Log("StarPicked");
                break;
        }
    }

    protected virtual void OnEnable() {
        this.EventStartListening<GameEvent_Engine>();
        this.EventStartListening<GameEvent_EnemyKill>();
        this.EventStartListening<GameEvent_RespawnDeath>();
        this.EventStartListening<GameEvent_Shoot>();
        this.EventStartListening<GameEvent_Win>();
        LoadSoundSettings();
    }

    protected virtual void OnDisable() {
        if (_enabled) {
            this.EventStopListening<GameEvent_Engine>();
            this.EventStopListening<GameEvent_EnemyKill>();
            this.EventStopListening<GameEvent_RespawnDeath>();
            this.EventStopListening<GameEvent_Shoot>();
            this.EventStopListening<GameEvent_Win>();
        }
    }

    public void OnGameEvent(GameEvent_EnemyKill e) {
        EnemyDeathSound();
    }

    public void OnGameEvent(GameEvent_RespawnDeath e) {
        RespawnSound();
    }

    public void OnGameEvent(GameEvent_Shoot e) {
        shootSound();
    }

    public void OnGameEvent(GameEvent_Win e) {
        levelCompleteSound();
    }

    //	Sound feedback methods -----------------------------------------------------------------------------

    public AudioClip soundLevelStart;
    public AudioClip soundLevelComplete;
    public AudioClip soundLevelEnd;
    public AudioClip soundPause;
    public AudioClip soundUnPause;
    public AudioClip soundPlayerDeath;
    public AudioClip soundEnemyDeath;
    public AudioClip soundRespawn;
    public AudioClip soundShoot;



    void levelStartSound() {
        PlaySound(soundLevelStart, this.transform.position);
    }

    void levelCompleteSound() {
        PlaySound(soundLevelComplete, this.transform.position);
    }

    void shootSound() {
        PlaySound(soundShoot, this.transform.position);
    }

    void EnemyDeathSound() {
        PlaySound(soundEnemyDeath, this.transform.position);
    }

    void RespawnSound() {
        PlaySound(soundRespawn, this.transform.position);
    }

}
