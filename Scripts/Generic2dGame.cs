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
		//Level2,
		//Level3,
		Gameover
	};

	public const int ScreenWidth = 1280;
	public const int ScreenHeight = 720;

	public readonly Vector2 MoneyBagLocation = new Vector2(1200, 56);

	public int PlayerScore = 0;
	public int LeftArmDamage = 1;
	public int RightArmDamage = 1;

	public ulong Level1TimeLimit = 7200;

	public Node CurrentSceneFile { get; set; }

	public override void _Ready()
	{
		Viewport root = GetTree().Root;
		CurrentSceneFile = root.GetChild(root.GetChildCount() - 1);
	}

/*
	public override void _Process(float delta)
	{

	}
*/

	public void GotoScene(Scenes nextScene)
	{
		// This function will usually be called from a signal callback,
		// or some other function from the current scene.
		// Deleting the current scene at this point is
		// a bad idea, because it may still be executing code.
		// This will result in a crash or unexpected behavior.

		// The solution is to defer the load to a later time, when
		// we can be sure that no code from the current scene is running:
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
		GetTree().Root.AddChild(CurrentSceneFile);

		// Optionally, to make it compatible with the SceneTree.change_scene() API.
		GetTree().CurrentScene = CurrentSceneFile;
	}


}
