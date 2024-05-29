# Ascalon
Ascalon is a modular, extensible tool providing a system which can create and interact with ConCommands and ConVars, and provide a Debug Console experience similar to those found in AAA games.

Ascalon is designed with support for most C# environments, and has special support for Unity and Godot built in.

## Quick start
Ascalon can be quickly set up by directly importing it into a Unity/Godot/C# project. Examples of how to define and use ConCommands and ConVars can be found in `Scripts/Stock Commands/BaseCommands.cs`.

To call a ConCommand or ConVar, run `Ascalon.Call("<command> <args>")`.
For example,
- `Ascalon.Call("con_echo \"Hello World!\"")`
- `Ascalon.Call("con_verbose true")`

### Starting Ascalon
Ascalon must be loaded in. The method of doing so varies by project type.
If using Unity, it is done by adding an object with the AscalonUnity script.
If using Godot, the "Ascalon Runner" scene should be added to Autoload.
Otherwise, an instance may be instantiated and configured directly through whatever means you prefer.

### Frontend
For Unity and Godot, Ascalon ships with a UI frontend implementation ready to go.
If using Unity, the DebugConsoleCanvas prefab includes the frontend as well as a ready-to-go instance of AscalonUnity.
If using Godot, the "Ascalon Console UI" scene includes the frontend, and should be added to Autoload.

## Features
- ConCommands
- ConVars
- Command processor
- Save/load ConVars as config files
- Flags for access control
- Networking support
- RCon