using Godot;
using System;

public class Control : Godot.Control
{
	Button[,] buttons; 
	int boardSize = 10;
	int bSize = 40;
	int sSize = 20;
	public override void _Ready()
	{
		Connect("Claimed", this, nameof(_on_claimed));
		
		int count = 2*boardSize + 1;
		int i_size, j_size;
		int i_pos = 0, j_pos = 0;
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

	public void _on_claimed(int i, int j)
	{
	}
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
