# Ascalon
Ascalon is a modular, extensible tool providing a system which can create and interact with ConCommands and ConVars, and provide a Debug Console experience similar to those found in AAA games.

Ascalon is designed for Unity, but can be run with minimal changes in any C# environment supporting reflections.

## Quick start
Ascalon can be quickly set up by adding it as a submodule to a project, or directly importing it into a Unity/C# project. Examples of how to define and use ConCommands and ConVars can be found in `Scripts/StockCommands/BaseCommands.cs`.

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
