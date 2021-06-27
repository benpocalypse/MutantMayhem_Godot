using Godot;
using System;

public class CutsceneIntro : Node2D
{
	private Generic2dGame game;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		game = (Generic2dGame)GetNode("/root/Generic2dGame");
	}
	
	private void _on_AnimationPlayer_animation_finished(String anim_name)
	{
		game.GotoScene(Generic2dGame.Scenes.Level1);
	}

  // Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		if (Input.IsActionJustPressed("ui_accept")	|| 
			Input.IsActionJustPressed("ui_left")	||
			Input.IsActionJustPressed("ui_right")	|| 
			Input.IsActionJustPressed("ui_select")	|| 
			Input.IsActionJustPressed("ui_cancel")	||
			Input.IsActionJustPressed("ui_touch"))
		{
			game.GotoScene(Generic2dGame.Scenes.Level1);
		}
	}
	
	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventKey eventKey)
		{
			game.GotoScene(Generic2dGame.Scenes.Level1);
		}
	}
}
