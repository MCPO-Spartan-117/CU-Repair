# Repair

A small BepInEx plugin for Casualties Unknown to modify `PlayerCamera::TryPerformInventoryAction` to add a function to repair items with recipes with its recipe items.

## Overview 
Options to repair with liquids, exact items or items that qualify and to change how much they repair,
How much is repaired depends on the durability or quantity that was used, over-repairing simply damages the repair item instead of destroying it.

## TODO
Exclude tag system, probably will need to be in a BepInEx config.
Balancing, obviously some recipes will be unbalanced, will be configurable but generally should honor rule it as modded items do exist and may not use proper tags.

## Build
1. (Optional) Mod source can be placed inside of the game directory, (game/dir/mod)
2. Open the project in Visual Studio, JetBrains Rider or use the dotnet SDK CLI. (Or any other IDE)
3. Build `ScavTemplate/Template.csproj` via Ctrl + Shift + B (`dotnet build`)
4. If auto-detection misses your setup, open the linked `vars.targets` file from the project and override `BaseGamePath`
