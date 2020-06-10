using Godot;
using System;
enum Orientation
{
    horizontal, vertical
}


public class EdgeButton : Button
{
    [Signal]
    public delegate void ClaimedPlayer(int i, int j, int o);
    [Signal]
    public delegate void ClaimedBot(int i, int j, int o);
    public int i, j;
    Theme t = (Theme)GD.Load("res://edgeButton.tres");
    Orientation orientation;
    // Called when the node enters the scene tree for the first time.
    public EdgeButton()
    {
        orientation = 0;
    }
    public EdgeButton(int i, int j, int orientation)
    {
        this.orientation = (Orientation)orientation;
        this.i = i;
        this.j = j;
        // Text = " " + i.ToString() + " " + j.ToString(); //orientation.ToString() + 
    }
    public override void _Ready()
    {
        Connect("ClaimedPlayer", GetParent(), "_on_claimed_player");
        Connect("ClaimedBot", GetParent(), "_on_claimed_bot");
		Connect("pressed", this, nameof(_on_Button_pressed));
        ToggleMode = true;
        Theme = t;
        
    }

    public void _on_Button_pressed()
    {
        // Text = "TOGGLED";
        Disabled = true;
        Theme = (Theme)GD.Load("res://playerButton.tres");
        EmitSignal(nameof(ClaimedPlayer),i,j, orientation);
        // GetTree().Root.GetNode<signalManager>("SignalManager").EmitSignal(nameof(Claimed),i,j);
    }
    public void Claim()
    {
        Disabled = true;
        Theme = (Theme)GD.Load("res://botButton.tres");
        EmitSignal(nameof(ClaimedBot),i,j, orientation);

    }
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
