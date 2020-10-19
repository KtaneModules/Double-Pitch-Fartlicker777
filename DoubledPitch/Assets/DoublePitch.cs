using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;

public class DoublePitch : MonoBehaviour {

    public KMBombInfo Bomb;
    public KMAudio Audio;
    public KMSelectable[] Keypad;
    public KMSelectable[] PitchSelector;
    public KMSelectable SoundStarter;
    public TextMesh CurrentPitchLevel;
    public TextMesh CurrentInput;
    public GameObject ConenctsTheSoundButtonAndBox;
    public Material[] Color;

    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    int WordSelector;
    int CaesarShift;
    int NumericalAnswer;
    int Answer;
    int IndexForLettersListening;

    string[] Calls = {"GAMEOVER", "DYNAMITE", "BINARIES", "DOUBLEOH", "UNEMPLOY", "HALFLIFE", "PREPARED", "DEBUGLOG", "BARACUDA", "HANGTHEM", "LIGHTSPD", "THISMODS", "TROPICAL", "XENOLITH", "KNOCKOUT", "DETONATE", "ENCOUNTR", "YOURMAMA", "CHEKMATE", "BLANANAS", "THEWTNES", "TETRAVEX", "FUNNYMAN", "NTICHMBR", "KEEPTALK", "SOLOTHIS", "WEREDEAD", "GREATJOB", "ZULUKILO", "RADIATOR", "MRPEANUT", "ALCOHOLS", "ROYLFLSH", "JAPANESE", "URZODIAC", "TIMERSUP"};
    string[] Responses = {"LOSER", "IMTNT", "PITCH", "ZEROO", "GTJOB", "THREE", "READY", "ERROR", "SNAKE", "MAFIA", "BLAST", "BUGGD", "FRUIT", "ALIEN", "PUNCH", "BOMBS", "KANYE", "FATTY", "CHESS", "UHHHH", "DREAM", "MOVED", "TROLL", "GUNNR", "BUSTR", "IWILL", "LMFAO", "FUTWO", "QUERY", "CHILL", "TIRCH", "DRINK", "POKER", "KANJI", "ARIES", "XPLDE"};
    string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    string GivenCall = "";
    string ShuffledCallButAsAString = "";
    string UnencryptedAnswerResponse = "";
    string EncryptedAnswer = "";

    bool[][] BinaryRepresentations = new bool[26][] {
      new bool[5] {false, false, false, false, true},
      new bool[5] {false, false, false, true, false},
      new bool[5] {false, false, false, true, true},
      new bool[5] {false, false, true, false, false},
      new bool[5] {false, false, true, false, true},
      new bool[5] {false, false, true, true, false},
      new bool[5] {false, false, true, true, true},
      new bool[5] {false, true, false, false, false},
      new bool[5] {false, true, false, false, true},
      new bool[5] {false, true, false, true, false},
      new bool[5] {false, true, false, true, true},
      new bool[5] {false, true, true, false, false},
      new bool[5] {false, true, true, false, true},
      new bool[5] {false, true, true, true, false},
      new bool[5] {false, true, true, true, true},
      new bool[5] {true, false, false, false, false},
      new bool[5] {true, false, false, false, true},
      new bool[5] {true, false, false, true, false},
      new bool[5] {true, false, false, true, true},
      new bool[5] {true, false, true, false, false},
      new bool[5] {true, false, true, false, true},
      new bool[5] {true, false, true, true, false},
      new bool[5] {true, false, true, true, true},
      new bool[5] {true, true, false, false, false},
      new bool[5] {true, true, false, false, true},
      new bool[5] {true, true, false, true, false}
    };
    private bool[] animatingFlag = new bool[12];
    private bool[] animatingFlagTwo = new bool[2];
    bool animatingFlagThree = false;
    bool Activated = false;

    char[] ShuffledCall = new char[10];

    void Awake () {
        moduleId = moduleIdCounter++;
        foreach (KMSelectable Key in Keypad) {
            Key.OnInteract += delegate () { KeyPress(Key); return false; };
        }
        foreach (KMSelectable Arrow in PitchSelector) {
            Arrow.OnInteract += delegate () { ArrowPress(Arrow); return false; };
        }
        SoundStarter.OnInteract += delegate () { SoundStarterPress(); return false; };
    }

    void Start () {
      WordSelector = UnityEngine.Random.Range(0, Calls.Length);
      GivenCall = Calls[WordSelector];
      UnencryptedAnswerResponse = Responses[WordSelector];
      Debug.LogFormat("[Double Pitch #{0}] The unscrambled call is {1}.", moduleId, GivenCall);
      Debug.LogFormat("[Double Pitch #{0}] The unencrypted response is {1}.", moduleId, UnencryptedAnswerResponse);
      for (int i = 0; i < UnencryptedAnswerResponse.Length; i++)
        EncryptedAnswer += Alphabet[(Alphabet.IndexOf(UnencryptedAnswerResponse[i]) + Bomb.GetSerialNumberNumbers().Last()) % 26].ToString();
      Debug.LogFormat("[Double Pitch #{0}] The response caesar ciphered is {1}.", moduleId, EncryptedAnswer);
      for (int i = 0; i < EncryptedAnswer.Length; i++) {
        NumericalAnswer *= 10;
        switch (EncryptedAnswer[i]) {
          case 'A':case 'B':case 'C':
          NumericalAnswer += 1;
          break;
          case 'D':case 'E':case 'F':
          NumericalAnswer += 2;
          break;
          case 'G':case 'H':case 'I':
          NumericalAnswer += 3;
          break;
          case 'J':case 'K':case 'L':
          NumericalAnswer += 4;
          break;
          case 'M':case 'N':case 'O':
          NumericalAnswer += 5;
          break;
          case 'P':case 'Q':case 'R':
          NumericalAnswer += 6;
          break;
          case 'S':case 'T':
          NumericalAnswer += 7;
          break;
          case 'U':case 'V':
          NumericalAnswer += 8;
          break;
          case 'W':case 'X':
          NumericalAnswer += 9;
          break;
        }
      }
      for (int i = 0; i < 10; i++) {
        if (i == 8 || i == 9)
          ShuffledCall[i] = Alphabet[UnityEngine.Random.Range(0, 26)];
        else
          ShuffledCall[i] = GivenCall[i];
      }
      ShuffledCall.Shuffle();
      for (int i = 0; i < ShuffledCall.Length; i++)
        ShuffledCallButAsAString += ShuffledCall[i].ToString();
      Debug.LogFormat("[Double Pitch #{0}] The scrambled call is {1}.", moduleId, ShuffledCallButAsAString);
      Debug.LogFormat("[Double Pitch #{0}] The answer number is {1}.", moduleId, NumericalAnswer);
      IndexForLettersListening = UnityEngine.Random.Range(0, 10);
      CurrentPitchLevel.text = IndexForLettersListening.ToString();
    }

    void KeyPress (KMSelectable Key) {
      for (int i = 0; i < Keypad.Length; i++) {
        if (Key == Keypad[i] && i < 10) {
          Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Key.transform);
          StartCoroutine(keyAnimation(i));
          if (Answer.ToString().Length != 5) {
            Answer *= 10;
            Answer += i;
          }
          CurrentInput.text = Answer.ToString("00000");
        }
        else if (Key == Keypad[i] && i == 10) {
          StartCoroutine(keyAnimation(i));
          CurrentInput.text = "00000";
          Answer &= 0;
        }
        else if (Key == Keypad[i] && i == 11) {
          StartCoroutine(keyAnimation(i));
          if (Answer == 69420) {
            Audio.PlaySoundAtTransform("WakaFlaca", transform);
            goto funni;
          }
          else if (Answer == 80085) {
            Audio.PlaySoundAtTransform("tits", transform);
            goto funni;
          }
          if (Answer != NumericalAnswer) {
            GetComponent<KMBombModule>().HandleStrike();
            CurrentInput.text = "ERROR";
          }
          funni:
          if (Answer == NumericalAnswer)
            GetComponent<KMBombModule>().HandlePass();
        }
      }
    }

    void ArrowPress (KMSelectable Arrow) {
      if (Arrow == PitchSelector[0]) {
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Arrow.transform);
        StartCoroutine(keyAnimationForDifferentButtons(0));
        IndexForLettersListening++;
        IndexForLettersListening %= 10;
        CurrentPitchLevel.text = IndexForLettersListening.ToString();
      }
      else {
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Arrow.transform);
        StartCoroutine(keyAnimationForDifferentButtons(1));
        IndexForLettersListening--;
        if (IndexForLettersListening < 0)
          IndexForLettersListening += 10;
        CurrentPitchLevel.text = IndexForLettersListening.ToString();
      }
    }

    void SoundStarterPress () {
      Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, SoundStarter.transform);
      StartCoroutine(keyAnimationForDifferentButtonsButDifferentNow());
      if (!Activated) {
        ConenctsTheSoundButtonAndBox.GetComponent<MeshRenderer>().material = Color[1];
        Activated = true;
        StartCoroutine(Listen());
      }
      else {
        ConenctsTheSoundButtonAndBox.GetComponent<MeshRenderer>().material = Color[0];
        Activated = false;
        StopCoroutine(Listen());
      }
    }

    IEnumerator Listen () {
      while (Activated) {
        for (int i = 0; i < 5; i++) {
          if (BinaryRepresentations[Alphabet.IndexOf(ShuffledCall[IndexForLettersListening])][i]) {
            if (!Activated)
              goto NotActivatedLol;
            Audio.PlaySoundAtTransform("High", transform);
          }
          else {
            if (!Activated)
              goto NotActivatedLol;
            Audio.PlaySoundAtTransform("Low", transform);
          }
          yield return new WaitForSecondsRealtime(.287f);
        }
      }
      NotActivatedLol:
      yield return null;
    }

    private IEnumerator keyAnimation(int HiKavin)
    {
        animatingFlag[HiKavin] = true;
        Keypad[HiKavin].AddInteractionPunch(0.125f);
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        for (int i = 0; i < 5; i++)
        {
            Keypad[HiKavin].transform.localPosition += new Vector3(0, -0.15F, 0);
            yield return new WaitForSeconds(0.005F);
        }
        for (int i = 0; i < 5; i++)
        {
            Keypad[HiKavin].transform.localPosition += new Vector3(0, +0.15F, 0);
            yield return new WaitForSeconds(0.005F);
        }
        animatingFlag[HiKavin] = false;
    }

    private IEnumerator keyAnimationForDifferentButtons(int HiKavin)
    {
        animatingFlagTwo[HiKavin] = true;
        PitchSelector[HiKavin].AddInteractionPunch(0.125f);
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        for (int i = 0; i < 5; i++)
        {
            PitchSelector[HiKavin].transform.localPosition += new Vector3(0, -0.15F, 0);
            yield return new WaitForSeconds(0.005F);
        }
        for (int i = 0; i < 5; i++)
        {
            PitchSelector[HiKavin].transform.localPosition += new Vector3(0, +0.15F, 0);
            yield return new WaitForSeconds(0.005F);
        }
        animatingFlagTwo[HiKavin] = false;
    }

    private IEnumerator keyAnimationForDifferentButtonsButDifferentNow()
    {
        animatingFlagThree = true;
        SoundStarter.AddInteractionPunch(0.125f);
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        for (int i = 0; i < 5; i++)
        {
            SoundStarter.transform.localPosition += new Vector3(0, -0.0005F, 0);
            yield return new WaitForSeconds(0.005F);
        }
        for (int i = 0; i < 5; i++)
        {
            SoundStarter.transform.localPosition += new Vector3(0, +0.0005F, 0);
            yield return new WaitForSeconds(0.005F);
        }
        animatingFlagThree = false;
    }

    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"Use !{0} toggle to toggle the audio. Use !{0} raise/lower to press the buttons that adjust the pitch playing. Use !{0} ##### to submit a five digit number.";
    #pragma warning restore 414

    IEnumerator ProcessTwitchCommand (string Command) {
      int Result;
      Command = Command.Trim().ToUpper();
      yield return null;
      if (Command == "TOGGLE")
        SoundStarter.OnInteract();
      else if (Command == "LOWER")
        PitchSelector[1].OnInteract();
      else if (Command == "RAISE")
        PitchSelector[0].OnInteract();
      else if (Command.Length == 5 && Int32.TryParse(Command, out Result)) {
        for (int i = 0; i < 5; i++) {
          Keypad[int.Parse(Command[i].ToString())].OnInteract();
          yield return new WaitForSeconds(.1f);
        }
        Keypad[11].OnInteract();
      }
      else
        yield return "sendtochaterror I don't understand!";
    }

    IEnumerator TwitchHandleForcedSolve () {
      Keypad[10].OnInteract();
      for (int i = 0; i < 5; i++) {
        Keypad[int.Parse(NumericalAnswer.ToString("00000")[i].ToString())].OnInteract();
        yield return new WaitForSeconds(.1f);
      }
      Keypad[11].OnInteract();
    }
}
