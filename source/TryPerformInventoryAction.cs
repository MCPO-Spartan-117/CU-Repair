using System;
//using System.IO;
//using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
//using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
//using CUCoreLib.Data;
using CUCoreLib.Helpers;
//using CUCoreLib.Registries;
//using CUCoreLib.Saving;
using System.Collections.Generic;
//using UnityEngine.EventSystems;
//using System.Collections.ObjectModel;

namespace MCPO {
	class TryPerformInventoryAction {
		static ManualLogSource Logger = MCPO.Plugin.Logger;
		static bool repairfunct(Item dragItem, Item item) {
			if(Input.GetKey(CUCoreUtils.GetFriendlyKeyBind(CUCoreUtils.GetFriendlyKeyName(KeyCode.N)))) {
				if((bool)item && !(bool)item.battery && !item.TryGetComponent<WaterContainerItem>(out var throwaway) && item.condition < 1f) {
					foreach(Recipe recipe in Recipes.recipes) {
						if(recipe?.result != null && recipe.result.id == item.id) {
							List<RecipeItem> recitems = recipe.items;
							bool repair = false;
							string id = "";
							float liquse = 0f;
							float cond = 1f;
							foreach(RecipeItem part in recitems) {
								if(Plugin.liquidrepair && part?.isLiquid != null && part.isLiquid) {
									if(dragItem.TryGetComponent<WaterContainerItem>(out var liqcomp)) {
										foreach(LiquidStack liq in liqcomp.stack) {
											Liquids.Registry.TryGetValue(liq.liquidId, out var value);
											if(part?.quality != null) {
												CraftingQuality qualityThatMeetsCriteria = Item.GetQualityThatMeetsCriteria(part.quality.id, value.GetScaledQualities(liq.amount));
												if(qualityThatMeetsCriteria != null) {
													id = liq.liquidId;
													liquse = Mathf.Lerp(0f, liq.amount, part.quality.amount / qualityThatMeetsCriteria.amount);
													cond = Mathf.Clamp01(qualityThatMeetsCriteria.amount / part.quality.amount);
													repair = true;
													break;
												}
											}
										}
									}
								} else if(Plugin.idrepair && part?.specificId != null && part.specificId == dragItem.id) {
									cond = dragItem.condition;
									repair = true;
									break;
								} else if(Plugin.qualityrepair && part?.quality != null && Item.HasCommonQuality(part.quality.id, dragItem.Stats.qualities) != null) {
									cond = dragItem.condition;
									repair = true;
									break;
								}
							}

							if(repair) {
								float overshoot = item.condition + (Plugin.repairmult / recitems.Count);
								float liqovershoot = item.condition + ((Plugin.repairmult * cond) / recitems.Count);
								item.SetCondition(Mathf.Clamp01(liqovershoot));
								float temp = 1f;
								bool getcomp = dragItem.TryGetComponent<WaterContainerItem>(out var liqcomp);
								if(liquse > 0f && id != "") {
									if(getcomp) {
										if(liqovershoot > 1f) {
											temp = 1 - (liqovershoot - 1) / (Plugin.repairmult / recitems.Count);
										}
										liqcomp.Drain(liqcomp.CalculateDrainSingleLiquid(id, liquse * temp));
									}
								} else {
									if(getcomp) {
										if(liqcomp.CurrentTotal > 0f) {
											break;
										}
									}
//									var temp2 = Mathf.Clamp01(dragItem.condition - temp);
//									if(temp2 == 0f) {
//										UnityEngine.Object.Destroy(dragItem.gameObject);
//									} else {
//										dragItem.SetCondition(temp2);
//									}
									if(overshoot > 1f) {
										temp = 1 - (overshoot - 1) / (Plugin.repairmult / recitems.Count);
									}
									dragItem.SetCondition(Mathf.Clamp01(dragItem.condition - temp));
								}
								return true;
							}
						}
					}
				}
			}
			return false;
		}

		[HarmonyPatch(typeof(PlayerCamera), nameof(PlayerCamera.TryPerformInventoryAction))]
		static class Repairclass {
//				if (!hit.gameObject.TryGetComponent<InvButton>(out var component) || !component.Overlaps(uiCasts)) {
//					return false;
//				}
//				if (component.GetItem() == __instance.dragItem) {
//					return true;
//				}
//				Item item = component.GetItem();
//---------------------------------------------------------------
//				if(repairfunct(dragItem, item)) {
//					return true;
//				}
//---------------------------------------------------------------
//				if ((bool)item && (bool)item.battery) {
//					if (__instance.dragItem.Stats.HasTag("tool")) {
//						item.battery.UnloadBattery();

			public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator ILgen) {
				Logger.LogInfo("Patching NoContainerDecayPatch");
				var startidx = -1;
				var endidx = -1;
				var methodidx = -1;
				List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
				for (var i = 0; i < codes.Count; i++) {
					if (methodidx == -1) {
						if (codes[i].IsLdloc()) {
							startidx = i;
							Logger.LogDebug("Found start " + i);
						} else if (codes[i].Calls(AccessTools.Method(typeof(InvButton), nameof(InvButton.GetItem)))) {
							methodidx = i;
							Logger.LogDebug("Found method " + i);
						}
					} else {
						if (codes[i].IsStloc() && (i - methodidx == 1)) {
							endidx = i;
							Logger.LogDebug("Found end " + i);
							break;
						} else {
							methodidx = -1;
							Logger.LogDebug("Clearing methodidx " + i);
						}
					}
				}

				if (endidx != -1) {
					Label afterfunct = ILgen.DefineLabel();
					codes[endidx + 1].labels.Add(afterfunct);
					Type[] paramtypes = [typeof(Item), typeof(Item)];
					List<CodeInstruction> callfunct = [
						new CodeInstruction(OpCodes.Ldarg_0),
						CodeInstruction.LoadField(typeof(PlayerCamera), nameof(PlayerCamera.dragItem)),
						new CodeInstruction(OpCodes.Ldloc_1),
						CodeInstruction.Call(typeof(TryPerformInventoryAction), nameof(TryPerformInventoryAction.repairfunct), paramtypes),
						new CodeInstruction(OpCodes.Brfalse, afterfunct),
						new CodeInstruction(OpCodes.Ldc_I4_1),
						new CodeInstruction(OpCodes.Ret)
					];
					codes.InsertRange(endidx + 1, (IEnumerable<CodeInstruction>)callfunct);
				} else {
					Logger.LogError("Not patching");
				}

				return (IEnumerable<CodeInstruction>)codes;
			}
		}
	}
}
