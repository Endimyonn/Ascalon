#if GODOT
using Godot;
using System;

public partial class AscalonGodot : Node
{
	//singleton
	public static AscalonGodot instance;

	//core
	private static Ascalon ascalonInstance;


	[ExportSubgroup("Main Config Settings")]
	[Export] public bool autoLoadMainConfig = true;
    [Export] public string mainConfigName = "config";
	[Export] public bool autoSaveOnQuit = true;



	public override void _Ready()
	{
		if (instance != null)
		{
			GD.PrintErr("AscalonGodot was initialized twice. This is not allowed.");
			this.QueueFree();
			return;
		}
		instance = this;

		//initialize Ascalon
		ascalonInstance = new Ascalon();
		ascalonInstance.netModule = new AscalonEmptyNet();
		ascalonInstance.Initialize();

		//main config
		if (autoLoadMainConfig == true)
		{
			AscalonConfigTools.ReadConfigGodot(mainConfigName);
		}
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
				AscalonConfigTools.WriteConfigGodot("config");
			}
		}
    }
}
#endif
