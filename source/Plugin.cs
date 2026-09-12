//using System;
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
#if YAML
using YamlDotNet.Serialization;
using System.IO;
using System.Reflection;
#endif

namespace MCPO {
	[BepInPlugin(ModGUID, ModName, ModVersion)]
	[BepInDependency("net.cucorelib", BepInDependency.DependencyFlags.HardDependency)]
	[BepInDependency("YamlDotNet", BepInDependency.DependencyFlags.SoftDependency)]
	public class Plugin : BaseUnityPlugin {
		public const string ModName = "Repair"; // To change .dll name, change the name in vars.targets
		public const string ModGUID = "LGPLv3.MCPO." + ModName;
		public const string ModVersion = "1.1.0";

		internal static new ManualLogSource Logger;
		private readonly Harmony _harmony = new(ModGUID);
		public static Plugin Instance { get; private set; } = null!;
		const int configversion = 1;
		static string confdirpath = BepInEx.Paths.ConfigPath + "/Repair";
		static string confpath = confdirpath + "/V" + configversion + ".yaml";
		internal static bool liquidrepair = true;
		internal static bool liquidquarepair = true;
		internal static bool idrepair = true;
		internal static bool qualityrepair = true;
		internal static float repairmult = 1f;
		public static YAMLConf conf;
		#if YAML
		static bool yaml = false;
		static bool init = false;
		#endif

		public void Awake() {
			Logger = base.Logger;
			Instance = this;

			#if YAML
			try {
				Assembly.Load("YamlDotNet");
				try {
					Assembly.Load("BepInEx-YamlDotNet");
					yaml = true;
				} catch {
					Logger.LogWarning("BepInEx-YamlDotNet was either not found or malformed, using default config");
				}
			} catch {
				Logger.LogWarning("YamlDotNet was either not found or malformed, using default config");
			}

			if(yaml) {
				if(!Directory.Exists(confdirpath)) {
					Directory.CreateDirectory(confdirpath);
					yamlinit();
				} else if(!File.Exists(confpath)) {
					yamlinit();
				} else {
					init = true;
				}

				ModOptionsRegistry.Register(ModOptionDefinition.Bool(ModGUID + ".configreload",
				"Reload config",
				"Toggle if you edited the config at runtime",
				Setting.SettingCategory.Game,
				true, value => {
					yamldeser();
				}));
			} else {
				conf = new();
			}
			#else
			Logger.LogWarning("Compiled without YAML support, advanced configuration disabled");
			conf = new();
			#endif

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

		#if YAML
		void yamlinit() {
			conf = new();
			string textyaml = new SerializerBuilder().WithNewLine("\n").Build().Serialize(conf);
			textyaml = "#Empty is vaild(non-standard), [] is a empty array/dictionary, '- ' on the same indention is a entry in a array/dictionary\n" + textyaml;
			File.AppendAllText(confpath, textyaml);
		}

		void yamldeser() {
			if(init) {
				try {
					conf = new DeserializerBuilder().IgnoreUnmatchedProperties().WithNodeDeserializer(new RemoveNull()).Build().Deserialize<YAMLConf>(File.ReadAllText(confpath));
				} catch {
					Logger.LogError("Config malformed, using defaults");
					conf = new();
				}
			} else {
				init = true;
			}
		}
		#endif
	}
}
