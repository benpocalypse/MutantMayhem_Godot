using Godot;
using System;

public class CreditsScreen : Node2D
{
	public override void _Ready()
	{
		
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }

	private void _on_BackButton_pressed()
	{
		var game = (Generic2dGame)GetNode("/root/Generic2dGame");
		game.GotoScene(Generic2dGame.Scenes.Titlescreen);
	}
}
