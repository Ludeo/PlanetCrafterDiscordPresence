using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using MijuTools;
using SpaceCraft;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TerraformationDetailsOverlay
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private static ManualLogSource logger;
        private readonly Harmony harmony = new(PluginInfo.PLUGIN_GUID);
        private static SceneState sceneState = SceneState.MainMenu;
        private static GameObject parent;
        private static GameObject textObject;

        private static ConfigEntry<int> top;
        private static ConfigEntry<int> right;
        private static ConfigEntry<int> width;
        private static ConfigEntry<int> height;
        private static ConfigEntry<int> fontSize;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(SavedDataHandler), "LoadSavedData")]
        public static void LoadSavedData()
        {
            sceneState = SceneState.InGame;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Intro), "ShowMainMenu")]
        public static void ShowMainMenu()
        {
            sceneState = SceneState.MainMenu;
            DestroyUI();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UiWindowPause), "OnFeedback")]
        public static void OnFeedback()
        {
            parent?.SetActive(false);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UiWindowPause), "OnOptions")]
        public static void OnOptions()
        {
            parent?.SetActive(false);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UiWindowPause), "OnBackToGame")]
        public static void OnBackToGame()
        {
            parent?.SetActive(true);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UiWindowPause), "OnClose")]
        public static void OnClosePause()
        {
            parent?.SetActive(true);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UiWindowOptions), "OnClose")]
        public static void OnCloseOptions()
        {
            parent?.SetActive(true);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UiWindowFeedback), "OnClose")]
        public static void OnCloseFeedback()
        {
            parent?.SetActive(true);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UiWindow), "OnOpen")]
        public static void OnOpen()
        {
            parent?.SetActive(false);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UiWindow), "OnClose")]
        public static void OnClose()
        {
            parent?.SetActive(true);
        }

        private void Awake()
        {
            logger = Logger;
            harmony.PatchAll(typeof(Plugin));

            top = Config.Bind("Position", "Top", 350, "Distance from the top");
            right = Config.Bind("Position", "Right", 100, "Distance from the right");
            width = Config.Bind("Position", "Width", 450, "The width of the textbox");
            height = Config.Bind("Position", "Height", 500, "The height of the textbox");
            fontSize = Config.Bind("Font", "Font Size", 25, "The size of the font");

            logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        private void Update()
        {
            if(sceneState == SceneState.InGame)
            {
                Init();
                UpdateUI();
            }
        }

        private void Init()
        {
            if(parent == null)
            {
                parent = new GameObject();
                Canvas canvas = parent.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;

                textObject = new();
                textObject.transform.parent = parent.transform;

                Text text = textObject.AddComponent<Text>();
                text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                text.text = "";
                text.fontSize = fontSize.Value;
                text.color = Color.white;

                RectTransform rectTransform = text.GetComponent<RectTransform>();
                rectTransform.localPosition = new Vector3(Screen.width/2 - right.Value, Screen.height/2 - top.Value, 0);
                rectTransform.sizeDelta = new Vector2(width.Value, height.Value);

                parent.SetActive(true);
            }
        }

        private void UpdateUI()
        {
            WorldUnitsHandler manager = Managers.GetManager<WorldUnitsHandler>();

            string oxygenText = $"Oxygen: {manager.GetUnit(DataConfig.WorldUnitType.Oxygen).GetValueString()} ";
            string heatText = $"Heat: {manager.GetUnit(DataConfig.WorldUnitType.Heat).GetValueString()} ";
            string pressureText = $"Pressure: {manager.GetUnit(DataConfig.WorldUnitType.Pressure).GetValueString()} ";
            string biomassText = $"Biomass: {manager.GetUnit(DataConfig.WorldUnitType.Biomass).GetValueString()} ";

            textObject.GetComponent<Text>().text = $"{oxygenText}\n{heatText}\n{pressureText}\n{biomassText}";
        }

        private static void DestroyUI()
        {
            Destroy(textObject);
            Destroy(parent);
            parent = null;
            textObject = null;
        }

        private void OnDestroy()
        {
            DestroyUI();
            harmony.UnpatchSelf();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(LiveDevTools), "ToggleUi")]
        static void ToggleUi(List<GameObject> ___handObjectsToHide)
        {
            bool active = !___handObjectsToHide[0].activeSelf;
            parent?.SetActive(active);
        }
    }
}
