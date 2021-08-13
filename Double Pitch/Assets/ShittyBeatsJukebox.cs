using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Rnd = UnityEngine.Random;

public class ShittyBeatsJukebox : MonoBehaviour {
    enum LoopOptions
    {
        NoLoop,
        Loop,
        Shuffle
    }
    enum Status
    {
        Stopped,
        Paused,
        Playing
    }

    public KMSelectable left, right, play, pause, stop, loopOption, volUp, volDown;
    public SpriteRenderer loopDisp;
    public Sprite[] loopOptions;

    public TextMesh number, songTitle;
    public KMAudio Audio;
    public AudioSource audioPlayer;
    public AudioClip[] tracks;
    public Transform record;
    private int pos;
    private Status currentState = Status.Stopped;
    private LoopOptions currentLoop = LoopOptions.NoLoop;
    private float volume = 5;

    private int[] shuffleOrder;
    private int shufflePointer;
    public void Initiate()
    {
        Debug.Log("entering shitty beats jukebox");
        pos = Rnd.Range(0, tracks.Length);
        UpdateDisplay();
        audioPlayer.volume = volume / 10;

        left.OnInteract = () => Left();
        right.OnInteract = () => Right();
        play.OnInteract = () => Play();
        pause.OnInteract = () => Pause();
        stop.OnInteract = () => Stop();
        loopOption.OnInteract = () => Loop();
        volUp.OnInteract = () => VolUp();
        volDown.OnInteract = () => VolDown();
    }
    private void UpdateDisplay()
    {
        number.text = (pos + 1).ToString().PadLeft(2, '0');
        audioPlayer.clip = tracks[pos];
        string title = audioPlayer.clip.name;
        songTitle.text = title.Length > 27 ? title.Insert(27, "\n") : title;
        bool wasPlaying = false;
        if (currentState == Status.Playing)
        {
            audioPlayer.Stop();
            wasPlaying = true;
        }
        audioPlayer.clip = tracks[pos];
        if (wasPlaying)
            audioPlayer.Play();
    }
    private void Update()
    {
        if (audioPlayer.isPlaying)
        {
            record.localRotation *= Quaternion.Euler(0, 0, 100 * Time.deltaTime);
        }
        else if (!audioPlayer.isPlaying && currentState == Status.Playing)
        {
            if (currentLoop == LoopOptions.Loop)
                audioPlayer.Play();
            else if (currentLoop == LoopOptions.Shuffle)
            {
                pos = shuffleOrder[shufflePointer++];
                UpdateDisplay();
                audioPlayer.Play();
            }
        }
        
    }

    private void GenericButtonPress(KMSelectable btn)
    {
        btn.AddInteractionPunch(0.3f);
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, btn.transform);
    }
    private bool Left()
    {
        GenericButtonPress(left);
        pos += tracks.Length - 1;
        pos %= tracks.Length;
        UpdateDisplay();
        return false;
    }
    private bool Right()
    {
        GenericButtonPress(right);
        pos++;
        pos %= tracks.Length;
        UpdateDisplay();
        return false;
    }
    private bool Stop()
    {
        GenericButtonPress(stop);
        if (audioPlayer.isPlaying)
            audioPlayer.Stop();
        currentState = Status.Stopped;
        return false;
    }
    private bool Play()
    {
        GenericButtonPress(play);
        if (currentState == Status.Paused)
            audioPlayer.UnPause();
        else audioPlayer.Play();
        currentState = Status.Playing;
        return false;
    }
    private bool Pause()
    {
        GenericButtonPress(pause);
        if (currentState == Status.Paused)
        {
            audioPlayer.UnPause();
            currentState = Status.Playing;
        }
        else if (currentState == Status.Playing)
        {
            audioPlayer.Pause();
            currentState = Status.Paused;
        }
        return false;
    }
    private bool VolUp()
    {
        GenericButtonPress(volUp);
        if (volume != 10)
            volume++;
        audioPlayer.volume = volume / 10;
        return false;
    }
    private bool VolDown()
    {
        GenericButtonPress(volDown);
        if (volume != 1)
            volume--;
        audioPlayer.volume = volume / 10;
        return false;
    }
    private bool Loop()
    {
        GenericButtonPress(loopOption);
        currentLoop = (LoopOptions)(((int)currentLoop + 1) % 3);
        loopDisp.sprite = loopOptions[(int)currentLoop];
        if (currentLoop == LoopOptions.Shuffle)
            ShuffleQueue();
        return false;
    }

    private void ShuffleQueue()
    {
        currentLoop = LoopOptions.Shuffle;
        shufflePointer = 0;
        shuffleOrder = Enumerable.Range(0, tracks.Length).ToArray().Shuffle();
    }

}
