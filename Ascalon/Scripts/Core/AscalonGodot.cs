#if GODOT
using Godot;
using System;

public partial class AscalonGodot : Node
{
	//singleton
	public static AscalonGodot instance;

	//core
	private static Ascalon ascalonInstance;


	[Export] public bool autoSaveOnQuit = true;
    [Export] public string mainConfigName = "config";
	[Export] public bool loadConfigGodotStyle = true;



	public override void _Ready()
	{
		if (instance != null)
		{
			GD.PrintErr("AscalonGodot was initialized twice. This is not allowed.");
			this.QueueFree();
			return;
		}
		instance = this;

		ascalonInstance = new Ascalon();

		ascalonInstance.netModule = new AscalonEmptyNet();

		ascalonInstance.mainConfigName = this.mainConfigName;
		ascalonInstance.loadConfigUnityStyle = false;
		ascalonInstance.loadConfigGodotStyle = this.loadConfigGodotStyle;

		ascalonInstance.Initialize();

		GD.Print("Ascalon initialized");
	}

	public override void _Process(double delta)
	{
		if (ascalonInstance.uiModule != null)
		{
			ascalonInstance.uiModule.Update();
		}

		ascalonInstance.netModule.Update();
	}

    public override void _Notification(int what)
    {
        if (what == NotificationWMCloseRequest)
		{
			//handle autosave
			if (autoSaveOnQuit == true)
			{
				if (ascalonInstance.loadConfigGodotStyle == true)
				{
					AscalonConfigTools.WriteConfigGodot("config");
				}
				else
				{
					AscalonConfigTools.WriteConfig("config");
				}
			}
		}
    }
}
#endif
