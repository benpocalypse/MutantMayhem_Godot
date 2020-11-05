using Godot;
using System;

public class Titlescreen : Node
{
	private float scaleFactor = 1f;

	public override void _Ready()
	{
		
	}

	public override void _Process(float delta)
	{
		var foreground = (Sprite)GetNode("Foreground");
		scaleFactor += 0.1f;
		var factor = ((float)Math.Sin(((double)scaleFactor)));

		foreground.ApplyScale(new Vector2(1 + (factor/200), 1 + (factor/200)));
	}

	private void _on_StartGameButton_pressed()
	{
		var label = (Button)GetNode("StartGameButton");
		label.Text = "HELLO!";
		
		var game = (Generic2dGame)GetNode("/root/Generic2dGame");
		game.GotoScene(Generic2dGame.Scenes.Level1);
	}
	
	private void _on_CreditsButton_pressed()
	{
		var game = (Generic2dGame)GetNode("/root/Generic2dGame");
		game.GotoScene(Generic2dGame.Scenes.CreditsScreen);
	}
}
