# Ascalon
Ascalon is a modular, extensible tool providing a system which can create and interact with ConCommands and ConVars, and provide a Debug Console experience similar to those found in AAA games.

Ascalon is designed for Unity, but can be run with minimal changes in any C# environment supporting reflections.

## Quick start
Ascalon can be quickly set up by adding it as a submodule to a project, or directly importing it into a Unity/C# project. Examples of how to define and use ConCommands and ConVars can be found in `Scripts/StockCommands/BaseCommands.cs`.

Ascalon must be loaded in by adding an object with the AscalonUnity script if using Unity (otherwise, an instance may be instantiated and configured directly). The DebugConsoleCanvas prefab includes a UI console as well as a ready-to-go instance of AscalonUnity.
It is recommended that you load Ascalon in a separate scene before the main game loads, to avoid potential duplication issues. An automatic scene transition script is included for this purpose.

To call a ConCommand or ConVar, run `Ascalon.Call("<command> <args>")`.

## Features
- ConCommands
- ConVars
- Command processor
- Save/load ConVars as config files
- Flags for access control
- Networking support

## Planned features
- Networking support
    - Default implementations for popular libraries
        - Mirror ✔
        - Photon Bolt
        - Networking for GameObjects
