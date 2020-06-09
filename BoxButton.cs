using Godot;
using System;

public class BoxButton : Button
{
    int i, j;
    int claimed_edges = 0;

    public override void _Ready()
    {
        Disabled = true;
        Theme = (Theme)GD.Load("res://boxButton.tres");
    }
    public BoxButton(){}
    public BoxButton(int i, int j)
    {
        this.i = i;
        this.j = j;
        Text = i.ToString() + " " + j.ToString();
    }

    internal void EdgeClaimed()
    {
        claimed_edges++;
    }
    public override void _Process(float delta)
    {
        if(claimed_edges == 4) Theme = (Theme)GD.Load("res://playerButton.tres");
    }
}
