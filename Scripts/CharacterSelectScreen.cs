using Godot;
using System;

public class CharacterSelectScreen : Node2D
{
	private Generic2dGame game;

	public override void _Ready()
	{
		game = (Generic2dGame)GetNode("/root/Generic2dGame");
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }

	private void _on_StartGameButton_pressed()
	{
		game.GotoScene(Generic2dGame.Scenes.Level1);
	}
	
	private void _on_AudioStreamPlayer2D_finished()
	{
		// Replace with function body.
	}
}
