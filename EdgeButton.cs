using Godot;
using System;
enum Orientation
{
    horizontal, vertical
}


public class EdgeButton : Button
{
    [Signal]
    public delegate void Claimed(int i, int j, int o);
    int i, j;
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
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
        Text = " " + i.ToString() + " " + j.ToString(); //orientation.ToString() + 
    }
    public override void _Ready()
    {
        Connect("Claimed", GetParent(), "_on_claimed");
		Connect("pressed", this, nameof(_on_Button_pressed));
        ToggleMode = true;
        Theme = t;
        
    }

    public void _on_Button_pressed()
    {
        // Text = "TOGGLED";
        Disabled = true;
        Theme = (Theme)GD.Load("res://playerButton.tres");
        EmitSignal(nameof(Claimed),i,j, orientation);
        // GetTree().Root.GetNode<signalManager>("SignalManager").EmitSignal(nameof(Claimed),i,j);
    }
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
