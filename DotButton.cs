using Godot;
using System;

public class DotButton : Button
{
    int i, j;

    public override void _Ready()
    {
        Disabled = true;
        Theme = (Theme)GD.Load("res://dotButton.tres");
    }
    public DotButton(){}
    public DotButton(int i, int j)
    {
        this.i = i;
        this.j = j;
        Text = i.ToString() + " " + j.ToString();
    }
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
