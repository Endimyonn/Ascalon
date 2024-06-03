# Ascalon
Ascalon is a modular, extensible tool providing a system which can create and interact with console commands and variables, creating a Debug Console experience similar to those found in AAA games.

Ascalon is designed for most C# environments, and has special support for Unity and Godot built in.

### Features
- Console commands
- Console Variables ("ConVars")
- Command processing
- Saving/loading ConVars as config files
- Flags applicable to commands/ConVars for access control and metadata
- Networked gameplay support
- Remote control over network

## Installation
Ascalon can be installed by directly importing it into a Unity/Godot/C# project. Ascalon must be initialized at runtime. The method of doing so varies by project type. For the Unity and Godot environments, modules are provided to automatically initialize the core, and to create a UI frontend.

Under Unity, nothing extra needs to be done beyond importing it into the project. The "runner" (responsible for core initialization) and UI are loaded during startup automatically, and their prefabs can be found under `Modules/Unity/Resources/` if the startup configuration needs to be adjusted. Unity 2019.1 or newer is required.

Under Godot, the runner and UI frontend should be added to the project's Autoload configuration. They can be found under `Modules/Godot/Scenes/`. The `Feed Entry` scene should not be added. Godot 4 or newer is recommended. 3.x may work, has not been tested.

Otherwise, the Ascalon core must be initialized and configured manually. This involves creating a new `Ascalon`, configuring its module fields, and then calling the `Initialize()` method on it. See the built-in Unity/Godot runners for examples (in files `AscalonUnity.cs` and `AscalonGodot.cs`) of how to go about initialization.

## Quick start
Examples of how to define and use ConCommands and ConVars can be found in `Scripts/Stock Commands/BaseCommands.cs`.

To call a ConCommand or ConVar, run `Ascalon.Call("<command> <args>")`.  
For example,
- `Ascalon.Call("con_echo \"Hello World!\"")`
- `Ascalon.Call("con_verbose true")`
- `Ascalon.Call("test_array 3.5|1|9.81")`

To retrieve or manipulate the data held by a ConVar, run `Ascalon.GetConVar(<name>)` or `Ascalon.SetConVar(name, data)`.
For example,
- `this.moveSpeed = (float)Ascalon.GetConVar("player_speed")`
- `Ascalon.SetConVar("cam_sensitivity", sensitivitySlider.value)`

### Networking
Ascalon supports networking in two ways: a networking module and RCon support.

The networking module is resposible for enabling commands and ConVars to be sent from client to server (or client to client). Due to the high number of networking libraries out there, it is impossible for me as a solo developer to support all of them. Currently, Mirror Networking support is included. To implement support for a library, a class deriving from `AscalonNetModule` should be created and hooked up to the Ascalon core during its initialization.

RCon (remote control) support comes in the form of an included client (`AscalonRConModule`) connected to the core, and a simple server that runs alongside Ascalon. The client encodes a call and a password and sends it to the server.

ConFlags, which are assigned when defining a command or ConVar, can (among other things) determine whether a command/ConVar is network-eligible, how it is used in a networked environment, and whether it may be used over RCon.

## Contributing
If you find a bug, or have an improvement you'd like to see, please open an issue.