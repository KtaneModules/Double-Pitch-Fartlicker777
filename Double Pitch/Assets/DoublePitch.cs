using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using KModkit;

public class DoublePitch : MonoBehaviour {

   public KMBombInfo Bomb;
   public KMAudio Audio;

   AudioSource ShittyBeats;

   public KMSelectable[] Keypad;
   public KMSelectable[] PitchSelector;
   public KMSelectable SoundStarter;

   public GameObject[] LEDSegments;
   public GameObject ConenctsTheSoundButtonAndBox;

   public Material[] Color;
   public Material[] ColorLED;
   public ShittyBeatsJukebox jukeboxMode;
   public Transform modTF;

   static int moduleIdCounter = 1;
   int moduleId;
   bool ModuleSolved;
   bool JukeboxActive;

   int[] ErrorLED = { 7, 8, 10, 11, 13, 17, 18, 24, 25, 31, 32, 33, 34, 38, 39 };
   int[] GreatLED = { 7, 8, 11, 12, 13, 17, 18, 21, 22, 24, 25, 27, 28, 29, 30, 31, 32, 33, 36, 38, 39, 41 };
   int[] easterEggs = { 69420, 42069, 58008, 80085, 505, 45148 };
   int Answer;
   int CaesarShift;
   int IndexForLettersListening;
   int NumericalAnswer;
   int Presses;
   int WordSelector;

   string[] Calls = { "GAMEOVER", "DYNAMITE", "BINARIES", "DOUBLEOH", "UNEMPLOY", "HALFLIFE", "PREPARED", "DEBUGLOG", "BARACUDA", "HANGTHEM", "LIGHTSPD", "THISMODS", "TROPICAL", "XENOLITH", "KNOCKOUT", "DETONATE", "ENCOUNTR", "YOURMAMA", "CHEKMATE", "BLANANAS", "THEWTNES", "TETRAVEX", "FUNNYMAN", "NTICHMBR", "KEEPTALK", "SOLOTHIS", "WEREDEAD", "GREATJOB", "ZULUKILO", "RADIATOR", "MRPEANUT", "ALCOHOLS", "ROYLFLSH", "JAPANESE", "URZODIAC", "TIMERSUP", "IMPOSTER", "PLAYMUCK", "TOMBRADY" };
   readonly string[] Responses = { "LOSER", "IMTNT", "PITCH", "ZEROO", "GTJOB", "THREE", "READY", "ERROR", "SNAKE", "MAFIA", "BLAST", "BUGGD", "FRUIT", "ALIEN", "PUNCH", "BOMBS", "KANYE", "FATTY", "CHESS", "UHHHH", "DREAM", "MOVED", "TROLL", "GUNNR", "BUSTR", "IWILL", "LMFAO", "FUTWO", "QUERY", "CHILL", "TIRCH", "DRINK", "POKER", "KANJI", "ARIES", "XPLDE", "SUSSY", "GAMER", "TABLE" };
   string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
   string EncryptedAnswer = "";
   string GivenCall = "";
   string ShuffledCallButAsAString = "";
   string UnencryptedAnswerResponse = "";
   string Temp;
   readonly bool[][] BinaryRepresentations = new bool[26][] {
      new bool[5] {false, false, false, false, true}, new bool[5] {false, false, false, true, false}, new bool[5] {false, false, false, true, true}, new bool[5] {false, false, true, false, false}, new bool[5] {false, false, true, false, true}, new bool[5] {false, false, true, true, false}, new bool[5] {false, false, true, true, true}, new bool[5] {false, true, false, false, false}, new bool[5] {false, true, false, false, true}, new bool[5] {false, true, false, true, false}, new bool[5] {false, true, false, true, true}, new bool[5] {false, true, true, false, false}, new bool[5] {false, true, true, false, true}, new bool[5] {false, true, true, true, false}, new bool[5] {false, true, true, true, true}, new bool[5] {true, false, false, false, false}, new bool[5] {true, false, false, false, true}, new bool[5] {true, false, false, true, false}, new bool[5] {true, false, false, true, true}, new bool[5] {true, false, true, false, false}, new bool[5] {true, false, true, false, true}, new bool[5] {true, false, true, true, false}, new bool[5] {true, false, true, true, true}, new bool[5] {true, true, false, false, false}, new bool[5] {true, true, false, false, true}, new bool[5] {true, true, false, true, false}
    };
   bool Activated;
   bool Animating;
   bool TwitchPlaysActive;

   char[] ShuffledCall = new char[10];

   #region Calculations

   void Awake () {
      moduleId = moduleIdCounter++;
      foreach (KMSelectable Key in Keypad)
         Key.OnInteract += delegate () { KeyPress(Key); return false; };
      foreach (KMSelectable Arrow in PitchSelector)
         Arrow.OnInteract += delegate () { ArrowPress(Arrow); return false; };
      SoundStarter.OnInteract += delegate () { SoundStarterPress(); return false; };
   }

   void Start () {
      WordSelector = UnityEngine.Random.Range(0, Calls.Length);
      GivenCall = Calls[WordSelector];
      for (int i = 0; i < LEDSegments.Length; i++) {
         LEDSegments[i].gameObject.SetActive(false);
      }
      UnencryptedAnswerResponse = Responses[WordSelector];
      Debug.LogFormat("[Double Pitch #{0}] The unscrambled call is {1}.", moduleId, GivenCall);
      Debug.LogFormat("[Double Pitch #{0}] The unencrypted response is {1}.", moduleId, UnencryptedAnswerResponse);
      for (int i = 0; i < UnencryptedAnswerResponse.Length; i++) {
         EncryptedAnswer += Alphabet[(Alphabet.IndexOf(UnencryptedAnswerResponse[i]) + Bomb.GetSerialNumberNumbers().Last()) % 26].ToString();
      }
      Debug.LogFormat("[Double Pitch #{0}] The response caesar ciphered is {1}.", moduleId, EncryptedAnswer);
      for (int i = 0; i < EncryptedAnswer.Length; i++) {
         NumericalAnswer *= 10;
         switch (EncryptedAnswer[i]) {
            case 'A':
            case 'B':
            case 'C':
               NumericalAnswer++;
               break;
            case 'D':
            case 'E':
            case 'F':
               NumericalAnswer += 2;
               break;
            case 'G':
            case 'H':
            case 'I':
               NumericalAnswer += 3;
               break;
            case 'J':
            case 'K':
            case 'L':
               NumericalAnswer += 4;
               break;
            case 'M':
            case 'N':
            case 'O':
               NumericalAnswer += 5;
               break;
            case 'P':
            case 'Q':
            case 'R':
               NumericalAnswer += 6;
               break;
            case 'S':
            case 'T':
               NumericalAnswer += 7;
               break;
            case 'U':
            case 'V':
               NumericalAnswer += 8;
               break;
            case 'W':
            case 'X':
               NumericalAnswer += 9;
               break;
         }
      }
      for (int i = 0; i < 10; i++) {
         if (i == 8 || i == 9) {
            int choice = UnityEngine.Random.Range(0, 26);
            if (GivenCall == "IMPOSTER")
               while (Alphabet[choice] == 'U') choice = UnityEngine.Random.Range(0, 26);
            else if (GivenCall == "TIMERSUP")
               while (Alphabet[choice] == 'O') choice = UnityEngine.Random.Range(0, 26);
            ShuffledCall[i] = Alphabet[choice];
         }
         else {
            ShuffledCall[i] = GivenCall[i];
         }
      }
      ShuffledCall.Shuffle();
      for (int i = 0; i < ShuffledCall.Length; i++) {
         ShuffledCallButAsAString += ShuffledCall[i].ToString();
      }
      Debug.LogFormat("[Double Pitch #{0}] The scrambled call is {1}.", moduleId, ShuffledCallButAsAString);
      Debug.LogFormat("[Double Pitch #{0}] The answer number is {1}.", moduleId, NumericalAnswer.ToString("00000"));
      IndexForLettersListening = UnityEngine.Random.Range(0, 10);
      for (int i = 0; i < 7; i++) {
         LEDSegments[i].gameObject.SetActive(ShowingSegments(IndexForLettersListening)[i]);
      }
   }

   #endregion

   #region Buttons

   void KeyPress (KMSelectable Key) {
      //if (JukeboxActive)
      //   return;
      for (int i = 0; i < Keypad.Length; i++) {
         if (Key == Keypad[i] && i < 10) {
            Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Key.transform);
            StartCoroutine(keyAnimation(i));
            if (Animating) {
               return;
            }
            if (Answer.ToString().Length != 5) {
               Answer *= 10;
               Answer += i;
            }
            if (Presses == 5) {
               return;
            }
            Presses++;
            switch (Presses) {
               case 1: Temp = Answer.ToString("0"); break;
               case 2: Temp = Answer.ToString("00"); break;
               case 3: Temp = Answer.ToString("000"); break;
               case 4: Temp = Answer.ToString("0000"); break;
               case 5: Temp = Answer.ToString("00000"); break;
            }
            for (int x = 1; x < Temp.Length + 1; x++) {
               for (int j = 0; j < 7; j++) {
                  LEDSegments[j + (6 - x) * 7].gameObject.SetActive(ShowingSegments(int.Parse(Temp[Temp.Length - x].ToString()))[j]);
               }
            }
         }
         else if (Key == Keypad[i] && i == 10) {
            StartCoroutine(keyAnimation(i));
            if (Animating) {
               return;
            }
            Answer &= 0;
            StartCoroutine(ShutOff("Clear"));
         }
         else if (Key == Keypad[i] && i == 11) {
            StartCoroutine(keyAnimation(i));
            if (Answer == 69420) {
               Audio.PlaySoundAtTransform("WakaFlaca", this.transform);
               goto funni;
            }
            else if (Answer == 80085) {
               Audio.PlaySoundAtTransform("tits", this.transform);
               goto funni;
            }
            else if (Answer == 42069) {
               Audio.PlaySoundAtTransform("loudMan", this.transform);
               goto funni;
            }
            else if (Answer == 58008) {
               Audio.PlaySoundAtTransform("fart", this.transform);
               goto funni;
            }
            else if (Answer == 505) {
               Audio.PlaySoundAtTransform("AMOGUS Sound Effect", this.transform);
               goto funni;
            }
            else if (Answer == 45148) {
               Audio.PlaySoundAtTransform("Asian", this.transform);
               goto funni;
            }
            /*else if (ModuleSolved) {
               StartCoroutine(Flip());
            }*/
            else if (Answer != NumericalAnswer || Presses != 5) {
               GetComponent<KMBombModule>().HandleStrike();
               StartCoroutine(ShutOff("Strike"));
               return;
            }
            funni:
            if (Answer == NumericalAnswer) {
               GetComponent<KMBombModule>().HandlePass();
               StartCoroutine(ShutOff("Solve"));
               ModuleSolved = true;
            }
            if (TwitchPlaysActive && easterEggs.Contains(Answer)) {
               Answer &= 0;
               StartCoroutine(ShutOff("Clear"));
            }
         }
      }
   }

   void ArrowPress (KMSelectable Arrow) {
      if (Arrow == PitchSelector[0]) {
         Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Arrow.transform);
         StartCoroutine(keyAnimationForDifferentButtons(0));
         IndexForLettersListening++;
         IndexForLettersListening %= 10;
      }
      else {
         Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Arrow.transform);
         StartCoroutine(keyAnimationForDifferentButtons(1));
         IndexForLettersListening--;
         if (IndexForLettersListening < 0)
            IndexForLettersListening += 10;
      }
      for (int i = 0; i < 7; i++) {
         LEDSegments[i].gameObject.SetActive(ShowingSegments(IndexForLettersListening)[i]);
      }
   }

   void SoundStarterPress () {
      Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, SoundStarter.transform);
      StartCoroutine(keyAnimationForDifferentButtonsButDifferentNow());
      if (!Activated) {
         ConenctsTheSoundButtonAndBox.GetComponent<MeshRenderer>().material = Color[1];
         Activated = true;
         if (ModuleSolved) {
            return;
         }
         StartCoroutine(Listen());
      }
      else {
         ConenctsTheSoundButtonAndBox.GetComponent<MeshRenderer>().material = Color[0];
         Activated = false;
         if (ModuleSolved) {
            return;
         }
         StopCoroutine(Listen());
      }
   }

   #endregion

   #region Sounds

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
         yield return new WaitForSecondsRealtime(1f);
      }
      NotActivatedLol:
      yield return null;
   }

   #endregion

   #region Animations

   IEnumerator ShutOff (string Input) {
      Animating = true;
      switch (Input) {
         case "Strike":
            for (int i = 7; i < 42; i++) {
               LEDSegments[i].GetComponent<MeshRenderer>().material = ColorLED[1];
            }
            for (int x = 0; x < 10; x++) {
               for (int j = 7; j < LEDSegments.Length; j++) {
                  if (LEDSegments[j].gameObject.activeSelf) {
                     LEDSegments[j].gameObject.SetActive(false);
                  }
               }
               yield return new WaitForSecondsRealtime(.2f);
               for (int i = 0; i < ErrorLED.Length; i++) {
                  LEDSegments[ErrorLED[i]].gameObject.SetActive(true);
               }
               yield return new WaitForSecondsRealtime(.2f);
            }
            for (int i = 7; i < 42; i++) {
               LEDSegments[i].GetComponent<MeshRenderer>().material = ColorLED[0];
            }
            goto case "Clear";
         case "Clear":
            for (int j = 7; j < LEDSegments.Length; j++) {
               if (LEDSegments[j].gameObject.activeSelf) {
                  LEDSegments[j].gameObject.SetActive(false);
                  yield return new WaitForSecondsRealtime(.01f);
               }
            }
            Temp = "";
            Answer = 0;
            break;
         case "Solve":
            if (Activated) {
               ConenctsTheSoundButtonAndBox.GetComponent<MeshRenderer>().material = Color[0];
               Activated = false;
               StopCoroutine(Listen());
               StartCoroutine(ShutOff("Solve"));
            }
            for (int i = 7; i < 42; i++) {
               LEDSegments[i].GetComponent<MeshRenderer>().material = ColorLED[2];
            }
            for (int x = 0; x < 10; x++) {
               for (int j = 7; j < LEDSegments.Length; j++) {
                  if (LEDSegments[j].gameObject.activeSelf) {
                     LEDSegments[j].gameObject.SetActive(false);
                  }
               }
               yield return new WaitForSecondsRealtime(.2f);
               for (int i = 0; i < GreatLED.Length; i++) {
                  LEDSegments[GreatLED[i]].gameObject.SetActive(true);
               }
               yield return new WaitForSecondsRealtime(.2f);
            }
            break;
      }
      Animating = false;
      Presses = 0;
   }

   private IEnumerator keyAnimation (int HiKavin) {
      Keypad[HiKavin].AddInteractionPunch(0.125f);
      Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
      for (int i = 0; i < 5; i++) {
         Keypad[HiKavin].transform.localPosition += new Vector3(0, -0.15F, 0);
         yield return new WaitForSeconds(0.005F);
      }
      for (int i = 0; i < 5; i++) {
         Keypad[HiKavin].transform.localPosition += new Vector3(0, +0.15F, 0);
         yield return new WaitForSeconds(0.005F);
      }
   }

   private IEnumerator keyAnimationForDifferentButtons (int HiKavin) {
      PitchSelector[HiKavin].AddInteractionPunch(0.125f);
      Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
      for (int i = 0; i < 5; i++) {
         PitchSelector[HiKavin].transform.localPosition += new Vector3(0, -0.15F, 0);
         yield return new WaitForSeconds(0.005F);
      }
      for (int i = 0; i < 5; i++) {
         PitchSelector[HiKavin].transform.localPosition += new Vector3(0, +0.15F, 0);
         yield return new WaitForSeconds(0.005F);
      }
   }

   private IEnumerator keyAnimationForDifferentButtonsButDifferentNow () {
      SoundStarter.AddInteractionPunch(0.125f);
      Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
      for (int i = 0; i < 5; i++) {
         SoundStarter.transform.localPosition += new Vector3(0, -0.0005F, 0);
         yield return new WaitForSeconds(0.005F);
      }
      for (int i = 0; i < 5; i++) {
         SoundStarter.transform.localPosition += new Vector3(0, +0.0005F, 0);
         yield return new WaitForSeconds(0.005F);
      }
   }

   bool[] ShowingSegments (int Input) {
      bool[] Output = new bool[7];
      switch (Input) {    //tm tl    tr     mm     bl    br    bm
         case 0: Output = new bool[] { true, true, true, false, true, true, true }; break;
         case 1: Output = new bool[] { false, false, true, false, false, true, false }; break;
         case 2: Output = new bool[] { true, false, true, true, true, false, true }; break;
         case 3: Output = new bool[] { true, false, true, true, false, true, true }; break;
         case 4: Output = new bool[] { false, true, true, true, false, true, false }; break;
         case 5: Output = new bool[] { true, true, false, true, false, true, true }; break;
         case 6: Output = new bool[] { true, true, false, true, true, true, true }; break;
         case 7: Output = new bool[] { true, false, true, false, false, true, false }; break;
         case 8: Output = new bool[] { true, true, true, true, true, true, true }; break;
         case 9: Output = new bool[] { true, true, true, true, false, true, true }; break;
      }
      return Output;
   }

   /*private IEnumerator Flip () {
      JukeboxActive = true;
      Debug.LogFormat("[Double Pitch #{0}] Welcome to the Shitty Beats Jukebox! Listen to Shitty Beats today! https://www.youtube.com/playlist?list=PL6giE1a_sXZxLMIpgOvrprJqx26XipcEz", moduleId);
      jukeboxMode.Initiate();
      float delta = 0;
      const float duration = 1.5f;
      while (delta < duration) {
         delta += Time.deltaTime;
         modTF.localRotation = Quaternion.Euler(Easing.OutSine(delta, 0, 180, duration), 0, 0);
         yield return null;
      }
   }*/

   #endregion

   #region Twitch Plays

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
         Keypad[10].OnInteract();
         while (Animating)
            yield return null;
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
      while (Animating)
         yield return null;
      for (int i = 0; i < 5; i++) {
         Keypad[int.Parse(NumericalAnswer.ToString("00000")[i].ToString())].OnInteract();
         yield return new WaitForSeconds(.1f);
      }
      Keypad[11].OnInteract();
   }
}
#endregion
