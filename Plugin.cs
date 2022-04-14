using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace SiteCleaner
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        static ManualLogSource logger;
        static ConfigEntry<KeyboardShortcut> cleanToolsKey;
        static ConfigEntry<KeyboardShortcut> cleanMaterialsKey;

        private void Awake()
        {
            cleanToolsKey = Config.Bind("General", "Clean Tools Key", new KeyboardShortcut(KeyCode.O), "Key to cleanup tools");
            cleanMaterialsKey = Config.Bind("General", "Clean Materials Key", new KeyboardShortcut(KeyCode.P), "Key to cleanup materials");

            // Plugin startup logic
            logger = Logger;
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded");
            Logger.LogInfo($"Patching...");
            Harmony.CreateAndPatchAll(typeof(Plugin));
            Logger.LogInfo($"Patched");
        }

        [HarmonyPatch(typeof(PlayerAttributes), "Update")]
        [HarmonyPostfix]
        static void Update_Postfix(GameObject ____camera)
        {
            if (cleanToolsKey.Value.IsDown())
            {
                ConstructionSite component2 = ____camera.GetComponent<WorldRefs>()._site.GetComponent<ConstructionSite>();
                component2.CollectAndMoveTools(1);
            }
            
            if (cleanMaterialsKey.Value.IsDown())
            {
                ConstructionSite component2 = ____camera.GetComponent<WorldRefs>()._site.GetComponent<ConstructionSite>();
                component2.CollectAndMoveLooseItems(1);
            }
        }
    }
}
