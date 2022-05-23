﻿using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ModLoader
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private static ManualLogSource logger;
        private readonly Harmony harmony = new(PluginInfo.PLUGIN_GUID);
        private static GameObject canvasBase = null;
        private static GameObject canvasOptions = null;
        private static GameObject canvasSaveFilesSelector = null;
        private static GameObject canvasNewsletter = null;
        private static GameObject canvasModLoader = new("CanvasModLoader");

        private void Awake()
        {
            logger = Logger;
            harmony.PatchAll(typeof(Plugin));
            logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            // Getting all Canvas Objects
            GameObject canvasesObject = FindObjectFromInstanceID(59674) as GameObject;
            canvasBase = FindObjectFromInstanceID(59594) as GameObject;
            canvasOptions = FindObjectFromInstanceID(59672) as GameObject;
            canvasSaveFilesSelector = FindObjectFromInstanceID(59650) as GameObject;
            canvasNewsletter = FindObjectFromInstanceID(59688) as GameObject;

            // ----------------------------
            // Getting Button Image from Quit Button
            GameObject menuQuitButton = canvasBase.transform.GetChild(3).gameObject;
            Image menuQuitButtonImage = menuQuitButton.GetComponent<Image>();
            Sprite buttonSprite = menuQuitButtonImage.sprite;

            // ----------------------------
            // Getting font from Quit Button
            GameObject menuQuitButtonText = menuQuitButton.transform.GetChild(0).gameObject;
            TextMeshProUGUI menuQuitButtonTextMesh = menuQuitButtonText.GetComponent<TextMeshProUGUI>();
            TMP_FontAsset buttonFont = menuQuitButtonTextMesh.font;

            // ----------------------------
            // Button
            GameObject modLoaderObject = new("ButtonModLoader");
            modLoaderObject.transform.parent = canvasBase.transform;
            modLoaderObject.layer = LayerMask.NameToLayer("UI");

            Image modLoaderImage = modLoaderObject.AddComponent<Image>();
            modLoaderImage.sprite = buttonSprite;

            Button modLoaderButton = modLoaderObject.AddComponent<Button>();
            modLoaderButton.onClick.AddListener(() => { 
                canvasBase.SetActive(false);
                canvasOptions.SetActive(false);
                canvasSaveFilesSelector.SetActive(false);
                canvasNewsletter.SetActive(false);
                canvasModLoader.SetActive(true);
            });

            GameObject modLoaderText = new("Text (TMP)");
            modLoaderText.transform.parent = modLoaderObject.transform;
            modLoaderText.transform.localScale = new Vector3(1, 1, 1);
            modLoaderText.transform.localPosition = new Vector3(38, -12, 0);

            TextMeshProUGUI modLoaderTextMesh = modLoaderText.AddComponent<TextMeshProUGUI>();
            modLoaderTextMesh.text = "Mod Loader";
            modLoaderTextMesh.font = buttonFont;
            modLoaderTextMesh.fontSize = 16;

            RectTransform modLoaderObjectRectTransform = modLoaderObject.GetComponent<RectTransform>();
            modLoaderObjectRectTransform.position = new Vector3(960, 540, 0);
            modLoaderObjectRectTransform.localPosition = new Vector3(-550, -341.5f, 0);
            modLoaderObjectRectTransform.localScale = new Vector3(1.7992f, 1.7992f, 1.7992f);
            modLoaderObjectRectTransform.sizeDelta = new Vector2(160, 30);

            // ----------------------------
            // UI after Button press
            canvasModLoader.transform.parent = canvasesObject.transform;
            canvasModLoader.SetActive(false);

            // create panel like options menu
            // list plugins located in plugins folder
            // if plugin should get unloaded -> moved to unloadedplugins folder
            // show which plugins are loaded and which aren't with checkbox
            // if they are loaded (they are inside plugins folder) -> checkbox true
            // if they are unloaded (they are inside unloadedplugins folder) -> checkbox false
            // have a save button that moves every checked checkbox inside the plugins folder
            // every unchecked checkbox should get moved into unloaded plugins
            // somehow check if a plugin needs a dependency (maybe through a textfile or another way if it exists)
        }

        private void OnDestroy()
        {
            harmony.UnpatchSelf();
        }
    }
}
