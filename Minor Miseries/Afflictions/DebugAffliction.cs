using AfflictionComponent.Components;
using AfflictionComponent.Enums;
using AfflictionComponent.Interfaces;

namespace Minor_Miseries.Afflictions
{
    internal class DebugAff
    {
        public class DebugAffliction : CustomAffliction, IDuration, IRemedies, IInstance
        {
            private static readonly string[] Icons =
            {
                "ico_injury_bloodLoss",
                "ico_injury_burdened",
                "ico_injury_burn1",
                "ico_injury_frostbite",
                "ico_injury_gorged",
                "ico_injury_headache",
                "ico_injury_laceration",
                "ico_injury_majorBruising",
                "ico_injury_minorBruising",
                "ico_injury_pain",
                "ico_injury_sprainedAnkle",
                "ico_injury_sprainedWrist",
                "ico_injury_suffocation",
                "ico_injury_chemicalPoisoning",
                "ico_injury_hypothermia",
                "ico_injury_Insomnia",
                "ico_injury_scurvy",
                "ico_injury_SourStomach",
                "ico_injury_PoorCirculation",
                "ico_injury_WeakJoints",
                "ico_injury_BrokenBody",
                "ico_injury_cabinFever",
                "ico_injury_dysentery",
                "ico_injury_foodPoisoning",
                "ico_injury_intestinalParasites",
                "ico_injury_eventEntity1", // this is anxiety
                "ico_injury_eventEntity2", // this is fear
                "ico_injury_dehydration",
                "ico_injury_UnsettledSleep", // this is hauntedmind
                "ico_injury_infectionRisk",
                "ico_injury_WeakConstitution",
                "ico_injury_improvedRest",
                "ico_injury_DiminishedState",
                "ico_injury_burnElectrical",
                "ico_injury_warmingUp",
                "ico_injury_fatigueReduced",
                "ico_injury_diabetes",
                "ico_injury_infectedWound",
                "ico_injury_nourished",
                "ico_injury_SevereLacerations",
                "ico_injury_unburdened",
                "ico_injury_InsomniaRisk",
                "ico_injury_brokenrib",
                "ico_event_toxicFog",
                "ico_sprainProtection_buff",
                "ico_Protection_buff",
                "ico_buff_energy",
                "ico_buff_wellfed",
                "ico_healing_buff"
            };

            private static int IconIndex = 0;

            // =========================
            // Audio debug state
            // =========================
            private static readonly string AudioFilePath =
                Path.Combine(Environment.CurrentDirectory, "UserData", "MinorMiseries_AudioCandidates.txt");

            private static readonly Dictionary<string, uint> EventNameToId = new(StringComparer.OrdinalIgnoreCase);
            private static readonly List<(string Name, uint Id)> AudioCandidates = new();

            private static int AudioIndex = 0;
            private static bool AudioAutoPlay = false;
            private static bool AudioInitialized = false;

            public InstanceType Type { get; set; } = InstanceType.Single;

            public void OnFoundExistingInstance(CustomAffliction existingAffliction)
            {
                if (existingAffliction is DebugAffliction debug)
                {
                    debug.ResetAffliction(resetRemedies: false);
                    var now = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused();
                    debug.EndTime = now + debug.Duration;
                }
            }

            public float Duration { get; set; } = 43800f;
            public float EndTime { get; set; }

            public Tuple<string, int, int>[] RemedyItems { get; set; } = Array.Empty<Tuple<string, int, int>>();
            public Tuple<string, int, int>[] AltRemedyItems { get; set; } = Array.Empty<Tuple<string, int, int>>();

            public bool InstantHeal { get; set; } = true;

            public DebugAffliction(AfflictionBodyArea bodyArea)
                : base("Debug", "Debug", "Debug", null, Icons[IconIndex], bodyArea)
            {
                InitializeAudioDebug();
            }

            public void CureSymptoms()
            {
            }

            public void OnCure()
            {
            }

            public override void OnUpdate()
            {
                // -------- ICON DEBUG --------
                if (!IsShiftHeld() && Input.GetKeyDown(KeyCode.E))
                {
                    ChangeIcon(+1);
                }

                if (!IsShiftHeld() && Input.GetKeyDown(KeyCode.D))
                {
                    ChangeIcon(-1);
                }

                // -------- AUDIO DEBUG --------
                if (IsShiftHeld() && Input.GetKeyDown(KeyCode.D))
                {
                    AudioPrevious();
                }

                if (IsShiftHeld() && Input.GetKeyDown(KeyCode.E))
                {
                    AudioPlayCurrent();
                }

                if (IsShiftHeld() && Input.GetKeyDown(KeyCode.F))
                {
                    AudioNext();
                }

                if (IsShiftHeld() && Input.GetKeyDown(KeyCode.R))
                {
                    AudioReload();
                }

                if (IsShiftHeld() && Input.GetKeyDown(KeyCode.X))
                {
                    AudioAutoPlay = !AudioAutoPlay;
                    HUDMessage.AddMessage($"Audio autoplay: {(AudioAutoPlay ? "ON" : "OFF")}");
                    MelonLogger.Msg($"[DebugAff] Audio autoplay: {AudioAutoPlay}");
                }
            }

            private void ChangeIcon(int dir)
            {
                IconIndex += dir;

                if (IconIndex >= Icons.Length)
                    IconIndex = 0;

                if (IconIndex < 0)
                    IconIndex = Icons.Length - 1;

                Cure();
                new DebugAffliction(AfflictionBodyArea.Head).Start();

                HUDMessage.AddMessage($"Debug icon: {Icons[IconIndex]}");
            }

            private static bool IsShiftHeld()
            {
                return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            }

            // =========================
            // Audio helpers
            // =========================

            private static void InitializeAudioDebug()
            {
                if (AudioInitialized)
                    return;

                BuildEventDictionary();
                EnsureAudioFileExists();
                AudioReload();

                AudioInitialized = true;
            }

            private static void BuildEventDictionary()
            {
                EventNameToId.Clear();

                try
                {
                    PropertyInfo[] properties = typeof(Il2CppAK.EVENTS).GetProperties(
                        BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

                    MelonLogger.Msg($"[DebugAff] Audio properties count = {properties.Length}");

                    int loaded = 0;

                    foreach (PropertyInfo prop in properties)
                    {
                        try
                        {
                            if (prop.PropertyType != typeof(uint))
                                continue;

                            if (!prop.CanRead)
                                continue;

                            if (!prop.Name.StartsWith("PLAY_", StringComparison.Ordinal))
                                continue;

                            object raw = prop.GetValue(null, null);
                            if (raw == null)
                                continue;

                            uint value = (uint)raw;
                            EventNameToId[prop.Name] = value;
                            loaded++;
                        }
                        catch (Exception exProp)
                        {
                            Core.Warn($"[DebugAff] Failed property {prop.Name}: {exProp.Message}");
                        }
                    }

                    Core.Log($"[DebugAff] Loaded audio events = {loaded}");
                }
                catch (Exception ex)
                {
                    Core.Error($"[DebugAff] Failed to build audio dictionary: {ex}");
                }
            }

            private static void EnsureAudioFileExists()
            {
                try
                {
                    string directory = Path.GetDirectoryName(AudioFilePath);
                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);

                    if (!File.Exists(AudioFilePath))
                    {
                        File.WriteAllLines(AudioFilePath, new[]
                        {
                            "# Shift + D = previous",
                            "# Shift + E = play current",
                            "# Shift + F = next",
                            "# Shift + R = reload txt",
                            "# Shift + X = toggle autoplay",
                            "",
                            "PLAY_EXERTIONPAINMAC",
                            "PLAY_PAINPULSE",
                            "PLAY_VOBREATHHIGHINTENSITYNOLOOP",
                            "PLAY_VOBREATHMEDIUMINTENSITYNOLOOP",
                            "PLAY_VOBREATHELOWINTENSITYNOLOOP",
                            "PLAY_VOCATCHBREATH"
                        });

                        MelonLogger.Msg($"[DebugAff] Created audio txt: {AudioFilePath}");
                    }
                }
                catch (Exception ex)
                {
                    Core.Error($"[DebugAff] Failed to create audio txt: {ex}");
                }
            }

            private static void AudioReload()
            {
                AudioCandidates.Clear();
                AudioIndex = 0;
                int invalidCount = 0;

                try
                {
                    if (!File.Exists(AudioFilePath))
                    {
                        HUDMessage.AddMessage("Audio txt not found");
                        return;
                    }

                    string[] lines = File.ReadAllLines(AudioFilePath);

                    foreach (string rawLine in lines)
                    {
                        string eventName = ExtractEventName(rawLine);

                        if (string.IsNullOrWhiteSpace(eventName))
                            continue;

                        if (EventNameToId.TryGetValue(eventName, out uint id))
                        {
                            AudioCandidates.Add((eventName, id));
                        }
                        else
                        {
                            invalidCount++;
                            Core.Warn($"[DebugAff] Unknown event name in txt: {eventName}");
                        }
                    }

                    Core.Log($"[DebugAff] Reloaded {AudioCandidates.Count} audio candidates");

                    if (invalidCount > 0)
                        HUDMessage.AddMessage($"Audio reload: {AudioCandidates.Count} valid / {invalidCount} invalid");
                    else
                        HUDMessage.AddMessage($"Audio reload: {AudioCandidates.Count} valid");

                    if (AudioCandidates.Count > 0)
                        AudioShowCurrent();
                    else
                        HUDMessage.AddMessage("No valid audio candidates");
                }
                catch (Exception ex)
                {
                    Core.Error($"[DebugAff] Failed to reload audio txt: {ex}");
                }
            }

            private static string ExtractEventName(string rawLine)
            {
                if (string.IsNullOrWhiteSpace(rawLine))
                    return null;

                string line = rawLine.Trim();

                if (line.StartsWith("#"))
                    return null;

                int commentIndex = line.IndexOf('#');
                if (commentIndex >= 0)
                    line = line.Substring(0, commentIndex).Trim();

                if (string.IsNullOrWhiteSpace(line))
                    return null;

                line = line.Trim().TrimEnd(',').Trim();

                if (line.StartsWith("\"") && line.EndsWith("\""))
                    line = line.Trim('"');

                int firstQuote = line.IndexOf('"');
                int secondQuote = line.IndexOf('"', firstQuote + 1);
                if (firstQuote >= 0 && secondQuote > firstQuote)
                    return line.Substring(firstQuote + 1, secondQuote - firstQuote - 1);

                return line;
            }

            private static void AudioPrevious()
            {
                if (AudioCandidates.Count == 0)
                {
                    HUDMessage.AddMessage("No audio candidates loaded");
                    return;
                }

                AudioIndex--;
                if (AudioIndex < 0)
                    AudioIndex = AudioCandidates.Count - 1;

                AudioShowCurrent();

                if (AudioAutoPlay)
                    AudioPlayCurrent();
            }

            private static void AudioNext()
            {
                if (AudioCandidates.Count == 0)
                {
                    HUDMessage.AddMessage("No audio candidates loaded");
                    return;
                }

                AudioIndex++;
                if (AudioIndex >= AudioCandidates.Count)
                    AudioIndex = 0;

                AudioShowCurrent();

                if (AudioAutoPlay)
                    AudioPlayCurrent();
            }

            private static void AudioPlayCurrent()
            {
                if (AudioCandidates.Count == 0)
                {
                    HUDMessage.AddMessage("No audio candidates loaded");
                    return;
                }

                var current = AudioCandidates[AudioIndex];

                try
                {
                    GameAudioManager.PlaySound(current.Id, GameManager.GetPlayerObject());
                    MelonLogger.Msg($"[DebugAff] Playing {current.Name} ({current.Id})");
                    HUDMessage.AddMessage($"Play: {current.Name}");
                }
                catch (Exception ex)
                {
                    Core.Error($"[DebugAff] Failed to play {current.Name}: {ex}");
                }
            }

            private static void AudioShowCurrent()
            {
                if (AudioCandidates.Count == 0)
                    return;

                var current = AudioCandidates[AudioIndex];
                string text = $"Audio [{AudioIndex + 1}/{AudioCandidates.Count}] {current.Name}";

                MelonLogger.Msg($"[DebugAff] {text}");
                HUDMessage.AddMessage(text);
            }
        }
    }
}