# Repair

A small BepInEx plugin for Casualties Unknown to modify `PlayerCamera::TryPerformInventoryAction` to add a function to repair items with recipes with its recipe items.

## Overview 
Options to repair with liquids, exact items or items that qualify and to change how much they repair,\
How much is repaired depends on the durability or quantity that was used, over-repairing simply damages the repair item instead of destroying it.\
Optionally uses [YamlDotNet](https://www.nuget.org/packages/YamlDotNet) via [BepInEx-YamlDotNet](https://github.com/MCPO-Spartan-117/BepInEx-YamlDotNet) for advanced configuration.

## TODO
Balancing, obviously some recipes will be unbalanced, will be configurable but generally should honor rule it as modded items do exist and may not use proper tags.

## Build
1. (Optional) Mod source can be placed inside of the game directory, (game/dir/mod)
2. (YAML Support) Install 'BepInEx-YamlDotNet' first to enable support for YAML.
3. Open the project in Visual Studio, JetBrains Rider or use the dotnet SDK CLI. (Or any other IDE)
4. Build `ScavTemplate/Template.csproj` via Ctrl + Shift + B (`dotnet build`)
5. If auto-detection misses your setup, open the linked `vars.targets` file from the project and override `BaseGamePath`
