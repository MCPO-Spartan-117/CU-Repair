//using System;
//using System.IO;
//using System.Reflection;
//using System.Runtime.InteropServices;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
//using HarmonyLib.Public;
//using HarmonyLib.Tools;
//using MonoMod.RuntimeDetour;
using UnityEngine;
//using CUCoreLib.ContentReload;
using CUCoreLib.Data;
using CUCoreLib.Helpers;
using CUCoreLib.Registries;
//using CUCoreLib.Saving;
//using Newtonsoft.Json.Linq;
//using System.Collections.Generic;

namespace MCPO {
	[BepInPlugin(ModGUID, ModName, ModVersion)]
	[BepInDependency("net.cucorelib", BepInDependency.DependencyFlags.HardDependency)]
	public class Plugin : BaseUnityPlugin {
		public const string ModName = "Repair"; // To change .dll name, change the name in vars.targets
		public const string ModGUID = "LGPLv3.MCPO." + ModName;
		public const string ModVersion = "1.0.2";

		internal static new ManualLogSource Logger;
		private readonly Harmony _harmony = new(ModGUID);
		public static Plugin Instance { get; private set; } = null!;
		internal static bool liquidrepair = true;
		internal static bool liquidquarepair = true;
		internal static bool idrepair = true;
		internal static bool qualityrepair = true;
		internal static float repairmult = 1f;

		public void Awake() {
			Logger = base.Logger;
			Instance = this;

			CUCoreUtils.AllowKeybindRebind(CUCoreUtils.GetFriendlyKeyName(KeyCode.N), "Repair");

			ModOptionsRegistry.Register(ModOptionDefinition.Bool(ModGUID + ".liquidrepair",
			"Liquid repair",
			"Use exact liquids in the item's recipe to repair it",
			Setting.SettingCategory.Game,
			true, value => {
				liquidrepair = value;
			}));

			ModOptionsRegistry.Register(ModOptionDefinition.Bool(ModGUID + ".liquidquarepair",
			"Liquid quality repair",
			"Use qualifying liquids in the item's recipe to repair it",
			Setting.SettingCategory.Game,
			true, value => {
				liquidquarepair = value;
			}));

			ModOptionsRegistry.Register(ModOptionDefinition.Bool(ModGUID + ".idrepair",
			"Item repair",
			"Use exact items in the item's recipe to repair it",
			Setting.SettingCategory.Game,
			true, value => {
				idrepair = value;
			}));

			ModOptionsRegistry.Register(ModOptionDefinition.Bool(ModGUID + ".qualityrepair",
			"Quality repair",
			"Use qualifying items in the item's recipe to repair it",
			Setting.SettingCategory.Game,
			true, value => {
				qualityrepair = value;
			}));

			ModOptionsRegistry.Register(ModOptionDefinition.Float(ModGUID + ".repairmult",
			"Repair multipler",
			"Multiply the repair amount",
			Setting.SettingCategory.Game,
			1f, 0f, 5f, value => {
				repairmult = value;
			}));

			_harmony.PatchAll();
//			ContentReloadManager.EnableHotReload(ModGUID); // Required for hot reload, put any registry registrations in the RegisterReloadable method below
//			RegisterReloadable();
			Logger.LogInfo($"Plugin {ModName} is loaded!");
		}
	}
}
