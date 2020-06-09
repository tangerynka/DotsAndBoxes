using Godot;
using System;



public class Main : Node
{
	// [Signal]
	// public delegate void Claimed(int i, int j);
	Button[,] buttons; 
	int boardSize = 10;
	int bSize = 40;
	int sSize = 20;
	int count;
	public override void _Ready()
	{
		
		count = 2*boardSize + 1;
		int i_size, j_size;
		int i_pos = 0, j_pos = 0;
		// Button bb ;
		Button b = new Button();
		buttons = new Button[count, count];
		for(int i = 0; i<count; i++)
		{
			if(i%2 == 0) i_size = sSize;
			else i_size = bSize;
			j_pos = 0;
			for(int j = 0; j<count; j++)
			{
				if(j%2 == 0) j_size = sSize;
				else j_size = bSize;
				// b.Flat = true;
				if (i_size == sSize && j_size == sSize)
				{
					b = new DotButton(i,j);
				}
				else if (i_size == sSize ^ j_size == sSize)
				{
					int o = j%2==0 ? 0 : 1; //horizontal : vertical
					b = new EdgeButton(i,j,o);
					// EmitSignal("Claimed", 1, 2);
					// Connect("Claimed", (EdgeButton)b, "_on_claimed");
				}
				else if (i_size == bSize && j_size == bSize)
				{	
					b = new BoxButton(i,j);
				}

				buttons[i,j] = b;
				b.RectPosition = new Vector2(i_pos, j_pos);
				b.RectSize = new Vector2(i_size,j_size);
				j_pos += j_size;
				b.Show();
				AddChild(b);
			}
			i_pos += i_size;
			
		}
	}

	public void _on_claimed(int i, int j, int o)
	{
		BoxButton tmp;
		if(o==0) //horizontal
		{
			if(j>0) 
			{
				tmp = (BoxButton)buttons[i, j-1];
				tmp.EdgeClaimed();
			}
			if(j<count-1) 
			{
				tmp = (BoxButton)buttons[i, j+1];
				tmp.EdgeClaimed();
			}
		}
		else
		{
			if(i>0) 
			{
				tmp = (BoxButton)buttons[i-1, j];
				tmp.EdgeClaimed();
			}
			if(i<count-1) 
			{
				tmp = (BoxButton)buttons[i+1, j];
				tmp.EdgeClaimed();
			} 
		}
		// buttons[0,0].Disabled = false;
		buttons[i,j].Text = "C";
	}
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
