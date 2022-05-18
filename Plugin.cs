using BepInEx;
using BepInEx.Logging;
using DiscordGameSDKWrapper;
using HarmonyLib;
using MijuTools;
using SpaceCraft;
using System;

namespace DiscordPresence
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private static Discord discord = new(976371269756940358, (ulong)CreateFlags.Default);
        private readonly Harmony harmony = new(PluginInfo.PLUGIN_GUID);
        private static ManualLogSource logger;
        private static SceneState sceneState = SceneState.MainMenu;
        private static long sceneStartTimeStamp = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
        private static long lastUpdatedTimeStap = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();

        private void Awake()
        {
            logger = Logger;
            harmony.PatchAll(typeof(Plugin));
            logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        private void Update()
        {
            if(new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds() - lastUpdatedTimeStap > 15)
            {
                UpdatePresence();
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Intro), "ShowMainMenu")]
        public static void ShowMainMenu()
        {
            sceneState = SceneState.MainMenu;
            sceneStartTimeStamp = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
            UpdatePresence();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Intro), "ShowOptionsMenu")]
        public static void ShowOptionsMenu()
        {
            sceneState = SceneState.Options;
            sceneStartTimeStamp = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
            UpdatePresence();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Intro), "ShowSaveFilesList")]
        public static void ShowSaveFilesList()
        {
            sceneState = SceneState.WorldSelector;
            sceneStartTimeStamp = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
            UpdatePresence();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(SaveFilesSelector), "SelectedSaveFile")]
        public static void SelectedSaveFile()
        {
            sceneState = SceneState.InGame;
            sceneStartTimeStamp = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
            UpdatePresence();
        }

        private static void UpdatePresence()
        {
            lastUpdatedTimeStap = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
            discord.RunCallbacks();
            ActivityManager activityManager = discord.GetActivityManager();

            Activity activity = new()
            {
                Timestamps = new ActivityTimestamps()
                {
                    Start = sceneStartTimeStamp,
                },
                Assets = new()
                {
                    LargeImage = "image_large",
                }
            };

            switch (sceneState)
            {
                case SceneState.Options:
                    activity.State = "In Options";
                    break;

                case SceneState.InGame:
                    WorldUnitsHandler manager = Managers.GetManager<WorldUnitsHandler>();
                    activity.State = manager.GetUnit(DataConfig.WorldUnitType.Terraformation).GetValueString();
                    activity.Details = "Playing on " + Managers.GetManager<SavedDataHandler>().saveFileName;

                    string text = string.Empty;
                    string oxygenText = $"Oxygen: {manager.GetUnit(DataConfig.WorldUnitType.Oxygen).GetValueString()} ";
                    string heatText = $"Heat: {manager.GetUnit(DataConfig.WorldUnitType.Heat).GetValueString()} ";
                    string pressureText = $"Pressure: {manager.GetUnit(DataConfig.WorldUnitType.Pressure).GetValueString()} ";
                    string biomassText = $"Biomass: {manager.GetUnit(DataConfig.WorldUnitType.Biomass).GetValueString()} ";
                    text += oxygenText + heatText + pressureText + biomassText;

                    activity.Assets.LargeText = text;
                    activity.Assets.SmallImage = "image_small";
                    activity.Assets.SmallText = Readable.GetTerraformStageName(Managers.GetManager<TerraformStagesHandler>().GetCurrentGlobalStage());
                    break;

                case SceneState.MainMenu:
                    activity.State = "In Main Menu";
                    break;

                case SceneState.WorldSelector:
                    activity.State = "Selecting World";
                    break;
            }

            activityManager.UpdateActivity(activity, (result) =>
            {
                if (result == Result.Ok)
                {
                    logger.LogInfo("Discord status set to " + activity.State);
                }
                else
                {
                    logger.LogError("Discord status failed!");
                }
            });

            discord.RunCallbacks();
        }

        private void OnDestroy()
        {
            discord.Dispose();
            harmony.UnpatchSelf();
        }
    }
}
