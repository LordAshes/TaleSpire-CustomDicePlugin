using BepInEx;
using BepInEx.Configuration;

using UnityEngine;

using System.Collections.Generic;
using System.Linq;

namespace LordAshes
{
    [BepInPlugin(Guid, "Custom Dice Plug-In", Version)]
    [BepInDependency(LordAshes.FileAccessPlugin.Guid)]
    public class CustomDicePlugin : BaseUnityPlugin
    {
        // Plugin info
        public const string Guid = "org.lordashes.plugins.customdice";
        public const string Version = "1.0.0.0";

        // Configuration
        private Dictionary<string, DieConfig> diceConfig { get; set; } = new Dictionary<string, DieConfig>();
        private ConfigEntry<KeyboardShortcut>[] setStyleTriggers { get; set; } = new ConfigEntry<KeyboardShortcut>[10];

        // Content directory
        private static string dir = UnityEngine.Application.dataPath.Substring(0, UnityEngine.Application.dataPath.LastIndexOf("/")) + "/TaleSpire_CustomData/";

        // Asset bundle
        private static AssetBundle[] assetBundles = new AssetBundle[10];

        // Transformed Dice
        private static List<int> customDice = new List<int>();

        // Active style
        private int style = 1;

        /// <summary>
        /// Function for initializing plugin
        /// This function is called once by TaleSpire
        /// </summary>
        void Awake()
        {
            UnityEngine.Debug.Log("Lord Ashes Custom Dice Plugin Active.");

            if (FileAccessPlugin.File.Exists("dice01")) { assetBundles[0] = FileAccessPlugin.AssetBundle.Load("dice01"); } else { assetBundles[0] = null; Debug.LogError("Could not locate asset 'dice01' in TaleSpire_CustomData/Minis or a plugin CustomData/Minis folder"); }
            if (FileAccessPlugin.File.Exists("dice02")) { assetBundles[1] = FileAccessPlugin.AssetBundle.Load("dice02"); } else { assetBundles[1] = null; }
            if (FileAccessPlugin.File.Exists("dice03")) { assetBundles[2] = FileAccessPlugin.AssetBundle.Load("dice03"); } else { assetBundles[2] = null; }
            if (FileAccessPlugin.File.Exists("dice04")) { assetBundles[3] = FileAccessPlugin.AssetBundle.Load("dice04"); } else { assetBundles[3] = null; }
            if (FileAccessPlugin.File.Exists("dice05")) { assetBundles[4] = FileAccessPlugin.AssetBundle.Load("dice05"); } else { assetBundles[4] = null; }
            if (FileAccessPlugin.File.Exists("dice06")) { assetBundles[5] = FileAccessPlugin.AssetBundle.Load("dice06"); } else { assetBundles[5] = null; }
            if (FileAccessPlugin.File.Exists("dice07")) { assetBundles[6] = FileAccessPlugin.AssetBundle.Load("dice07"); } else { assetBundles[6] = null; }
            if (FileAccessPlugin.File.Exists("dice08")) { assetBundles[7] = FileAccessPlugin.AssetBundle.Load("dice08"); } else { assetBundles[7] = null; }
            if (FileAccessPlugin.File.Exists("dice09")) { assetBundles[8] = FileAccessPlugin.AssetBundle.Load("dice09"); } else { assetBundles[8] = null; }
            if (FileAccessPlugin.File.Exists("dice10")) { assetBundles[9] = FileAccessPlugin.AssetBundle.Load("dice10"); } else { assetBundles[9] = null; }

            setStyleTriggers[0] = Config.Bind("Hotkeys", "Set Style 01", new KeyboardShortcut(KeyCode.Keypad1, KeyCode.LeftControl));
            setStyleTriggers[1] = Config.Bind("Hotkeys", "Set Style 02", new KeyboardShortcut(KeyCode.Keypad2, KeyCode.LeftControl));
            setStyleTriggers[2] = Config.Bind("Hotkeys", "Set Style 03", new KeyboardShortcut(KeyCode.Keypad3, KeyCode.LeftControl));
            setStyleTriggers[3] = Config.Bind("Hotkeys", "Set Style 04", new KeyboardShortcut(KeyCode.Keypad4, KeyCode.LeftControl));
            setStyleTriggers[4] = Config.Bind("Hotkeys", "Set Style 05", new KeyboardShortcut(KeyCode.Keypad5, KeyCode.LeftControl));
            setStyleTriggers[5] = Config.Bind("Hotkeys", "Set Style 06", new KeyboardShortcut(KeyCode.Keypad6, KeyCode.LeftControl));
            setStyleTriggers[6] = Config.Bind("Hotkeys", "Set Style 07", new KeyboardShortcut(KeyCode.Keypad7, KeyCode.LeftControl));
            setStyleTriggers[7] = Config.Bind("Hotkeys", "Set Style 08", new KeyboardShortcut(KeyCode.Keypad8, KeyCode.LeftControl));
            setStyleTriggers[8] = Config.Bind("Hotkeys", "Set Style 09", new KeyboardShortcut(KeyCode.Keypad9, KeyCode.LeftControl));
            setStyleTriggers[9] = Config.Bind("Hotkeys", "Set Style 10", new KeyboardShortcut(KeyCode.Keypad0, KeyCode.LeftControl));

            diceConfig.Add("D4", new DieConfig()
            {
                position = Config.Bind("D4", "Position", new Vector3(0.05000000074505806f, 0.07000000029802323f, -0.05999999865889549f)).Value,
                angles = Config.Bind("D4", "Rotations", new Vector3(342.3999938964844f, 8.699999809265137f, 328.6000061035156f)).Value,
                scale = Config.Bind("D4", "Scale", 0.4f).Value,
                labelShift = Config.Bind("D4", "Label Shift", 10000.1f).Value
            });

            diceConfig.Add("D6", new DieConfig()
            {
                position = Config.Bind("D6", "Position", new Vector3(0, 0, 0)).Value,
                angles = Config.Bind("D6", "Rotations", new Vector3(0, 0, 0)).Value,
                scale = Config.Bind("D6", "Scale", 0.35f).Value,
                labelShift = Config.Bind("D6", "Label Shift", 1.05f).Value
            });

            diceConfig.Add("D8", new DieConfig()
            {
                position = Config.Bind("D8", "Position", new Vector3(0, 0, 0)).Value,
                angles = Config.Bind("D8", "Rotations", new Vector3(0, 0, 0)).Value,
                scale = Config.Bind("D8", "Scale", 0.55f).Value,
                labelShift = Config.Bind("D8", "Label Shift", 1.05f).Value
            });

            diceConfig.Add("D10", new DieConfig()
            {
                position = Config.Bind("D10", "Position", new Vector3(0.029999999329447748f, 0.10000000149011612f, 0.0f)).Value,
                angles = Config.Bind("D10", "Rotations", new Vector3(3.0f, 357.6000061035156f, 337.1000061035156f)).Value,
                scale = Config.Bind("D10", "Scale", 0.6f).Value,
                labelShift = Config.Bind("D10", "Label Shift", 1.1f).Value
            });

            diceConfig.Add("D100", new DieConfig()
            {
                position = Config.Bind("D100", "Position", new Vector3(0.029999999329447748f, 0.10000000149011612f, 0.0f)).Value,
                angles = Config.Bind("D100", "Rotations", new Vector3(3.0f, 357.6000061035156f, 337.1000061035156f)).Value,
                scale = Config.Bind("D100", "Scale", 0.6f).Value,
                labelShift = Config.Bind("D100", "Label Shift", 1.1f).Value
            });

            diceConfig.Add("D12", new DieConfig()
            {
                position = Config.Bind("D12", "Position", new Vector3(0, 0, 0)).Value,
                angles = Config.Bind("D12", "Rotations", new Vector3(359.3999938964844f, 22.0f, 2.0f)).Value,
                scale = Config.Bind("D12", "Scale", 0.5f).Value,
                labelShift = Config.Bind("D12", "Label Shift", 1.1f).Value
            });

            diceConfig.Add("D20", new DieConfig()
            {
                position = Config.Bind("D20", "Position", new Vector3(0, 0, 0)).Value,
                angles = Config.Bind("D20", "Rotations", new Vector3(0, 0, 0)).Value,
                scale = Config.Bind("D20", "Scale", 0.9f).Value,
                labelShift = Config.Bind("D20", "Label Shift", 1.05f).Value
            });
        }

        /// <summary>
        /// Function for determining if view mode has been toggled and, if so, activating or deactivating Character View mode.
        /// This function is called periodically by TaleSpire.
        /// </summary>
        void Update()
        {
            foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>())
            {
                if (go.name.StartsWith("numbered1D") && go.name.EndsWith("(Clone)") && !customDice.Contains(go.GetInstanceID()))
                {
                    string sides = go.name.Substring("numbered1D".Length);
                    sides = sides.Substring(0, sides.IndexOf("(Clone)"));
                    Debug.Log("New Die With " + sides + " Sides Found");
                    // Request customization
                    customDice.Add(go.GetInstanceID());
                    CustomizeDie(go, int.Parse(sides), style);
                }
            }

            for(int s=0; s<10; s++)
            {
                if(StrictKeyCheck(setStyleTriggers[s].Value))
                {
                    if (assetBundles[s] != null)
                    {
                        // Set style
                        style = s + 1;
                        SystemMessage.DisplayInfoText("Grabbing Style " + style + " Dice Tray...");
                    }
                    else
                    {
                        // Ignore
                        SystemMessage.DisplayInfoText("Forgot To Bring Style " + style + "Dice Tray...\r\n(Requires dice" + (s + 1).ToString("d2") + " assetBundle in TaleSpire_CustomData/Misc)");
                    }
                }
            }
        }

        public void CustomizeDie(GameObject die, int sides, int style)
        {
            // Move sides numbers out
            float moveFactor = diceConfig["D" + sides].labelShift * 0.01f; // Hide sides
            foreach (Transform side in die.transform.Children())
            {
                // Ensure it is a sides object
                if (side.name.Contains("Mark"))
                {
                    // Multiply current local position to move it out
                    side.localPosition = new Vector3(side.localPosition.x * moveFactor, side.localPosition.y * moveFactor, side.localPosition.z * moveFactor);
                }
            }
            // Create shell
            GameObject shell = GameObject.Instantiate(assetBundles[style-1].LoadAsset<GameObject>("d" + sides));
            // Apply the parent transforms
            shell.transform.position = die.transform.position;
            shell.transform.rotation = die.transform.rotation;
            shell.transform.SetParent(die.transform);
            // Apply the dice specific transforms
            Debug.Log("Applying Shell Specific Transformations (Rot " + diceConfig["D" + sides].angles + ", Scale " + diceConfig["D" + sides].scale + ", Pos " + diceConfig["D" + sides].position + ")");
            shell.transform.localRotation = Quaternion.Euler(diceConfig["D" + sides].angles);
            shell.transform.localPosition = diceConfig["D" + sides].position;
            shell.transform.localScale = new Vector3(diceConfig["D" + sides].scale, diceConfig["D" + sides].scale, diceConfig["D" + sides].scale);
        }

        /// <summary>
        /// Method to properly evaluate shortcut keys. 
        /// </summary>
        /// <param name="check"></param>
        /// <returns></returns>
        public bool StrictKeyCheck(KeyboardShortcut check)
        {
            if (!check.IsUp()) { return false; }
            foreach (KeyCode modifier in new KeyCode[] { KeyCode.LeftAlt, KeyCode.RightAlt, KeyCode.LeftControl, KeyCode.RightControl, KeyCode.LeftShift, KeyCode.RightShift })
            {
                if (Input.GetKey(modifier) != check.Modifiers.Contains(modifier)) { return false; }
            }
            return true;
        }

        /// <summary>
        /// Class for holding dice transforms
        /// </summary>
        public class DieConfig
        {
            public Vector3 angles { get; set; }
            public float labelShift { get; set; }
            public Vector3 position { get; set; }
            public float scale { get; set; }
        }
    }
}
