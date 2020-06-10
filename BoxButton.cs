using Godot;
using System;

public class BoxButton : Button
{
    [Signal]
    public delegate void Filled(int i, int j, int ap);
    [Signal]
    public delegate void NotFilled(int i, int j);
    int i, j;
    int claimed_edges = 0;

    public override void _Ready()
    {
        Connect("Filled", GetParent(), "_on_filled");
        Connect("NotFilled", GetParent(), "_not_filled");
        Disabled = true;
        Theme = (Theme)GD.Load("res://boxButton.tres");
    }
    public BoxButton(){}
    public BoxButton(int i, int j)
    {
        this.i = i;
        this.j = j;
        // Text = i.ToString() + " " + j.ToString();
    }

    internal void EdgeClaimed(int ap)
    {
        claimed_edges++;
        if(claimed_edges == 4) 
        {
            EmitSignal(nameof(Filled), i, j, ap);
            // Connect("Claimed", GetParent(), "_on_claimed");
            if(ap == 1) Theme = (Theme)GD.Load("res://playerButton.tres");
            else if(ap == -1) Theme = (Theme)GD.Load("res://botButton.tres");
        }
        // else EmitSignal(nameof(NotFilled));
    }
    public override void _Process(float delta)
    {
        
    }
}
