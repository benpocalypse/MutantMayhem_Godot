using Godot;
using System;

public class Generic2dGame : Node
{
	private enum GameState
	{
		NotInGame,
		Playing,
		Paused
	};

	public enum Scenes
	{
		Unknown,
		Titlescreen,
		CharacterSelectScreen,
		CreditsScreen,
		Level1,
		Level2,
		Level3
	};

	public const int ScreenWidth = 1280;
	public const int ScreenHeight = 720;

	public Node CurrentSceneFile { get; set; }
	private Scenes CurrentScene = Scenes.Unknown;
	private GameState CurrentGameState = GameState.NotInGame;

	public override void _Ready()
	{
		Viewport root = GetTree().GetRoot();
		CurrentSceneFile = root.GetChild(root.GetChildCount() - 1);

		Scenes CurrentScene = Scenes.Unknown;
		Scenes NextScene = Scenes.Titlescreen;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{

	}

	private void ProcessSceneExit(Scenes _scene)
	{
		switch (_scene)
		{
			case Scenes.Unknown:
				break;

			case Scenes.Titlescreen:
				break;

			case Scenes.CharacterSelectScreen:
				break;

			case Scenes.CreditsScreen:
				break;

			case Scenes.Level1:
				break;

			case Scenes.Level2:
				break;

			case Scenes.Level3:
				break;
		}
	}

	private void ProcessSceneEntry(Scenes _scene)
	{
		switch (_scene)
		{
			case Scenes.Unknown:
				break;

			case Scenes.Titlescreen:
				break;

			case Scenes.CharacterSelectScreen:
				break;

			case Scenes.CreditsScreen:
				break;

			case Scenes.Level1:
				break;

			case Scenes.Level2:
				break;

			case Scenes.Level3:
				break;
		}
	}

	private void ProcessScene(Scenes _scene)
	{
		switch (_scene)
		{
			case Scenes.Unknown:
				break;

			case Scenes.Titlescreen:
				break;

			case Scenes.CharacterSelectScreen:
				break;

			case Scenes.CreditsScreen:
				break;

			case Scenes.Level1:
				break;

			case Scenes.Level2:
				break;

			case Scenes.Level3:
				break;
		}
	}

	public void GotoScene(Scenes nextScene)
	{
		// This function will usually be called from a signal callback,
		// or some other function from the current scene.
		// Deleting the current scene at this point is
		// a bad idea, because it may still be executing code.
		// This will result in a crash or unexpected behavior.

		// The solution is to defer the load to a later time, when
		// we can be sure that no code from the current scene is running:

		if (nextScene != CurrentScene)
		{
			ProcessSceneExit(CurrentScene);
			ProcessSceneEntry(nextScene);
			CurrentScene = nextScene;
		}

		ProcessScene(CurrentScene);

		CallDeferred(nameof(DeferredGotoScene), "res://Scenes/" + nextScene.ToString() + ".tscn");
	}

	public void DeferredGotoScene(string path)
	{
		// It is now safe to remove the current scene
		CurrentSceneFile.Free();

		// Load a new scene.
		var nextScene = (PackedScene)GD.Load(path);

		// Instance the new scene.
		CurrentSceneFile = nextScene.Instance();

		// Add it to the active scene, as child of root.
		GetTree().GetRoot().AddChild(CurrentSceneFile);

		// Optionally, to make it compatible with the SceneTree.change_scene() API.
		GetTree().SetCurrentScene(CurrentSceneFile);
	}


}
