using Godot;
using System;
using System.Collections.Generic;


public class Main : Control
{
	[Signal]
	public delegate void ContinuePlayer();
	[Signal]
	public delegate void ContinueBot();
	Button[,] buttons; 
	int boardSize = 10;
	int bSize = 40;
	int sSize = 20;
	int count;
	int active_player = -1;
	bool changePlayer = true;
	List<EdgeButton> validMoves;
	public override void _Ready()
	{
		Connect("ContinuePlayer", this, "conP");
		Connect("ContinueBot", this, "conB");
		count = 2*boardSize + 1;
		int i_size, j_size;
		int i_pos = 20, j_pos = 20;
		Button b = new Button();
		buttons = new Button[count, count];
		validMoves = new List<EdgeButton>((boardSize+1)*(boardSize+1));
		for(int i = 0; i<count; i++)
		{
			if(i%2 == 0) i_size = sSize;
			else i_size = bSize;
			j_pos = 20;
			for(int j = 0; j<count; j++)
			{
				if(j%2 == 0) j_size = sSize;
				else j_size = bSize;
				if (i_size == sSize && j_size == sSize)
				{
					b = new DotButton(i,j);
				}
				else if (i_size == sSize ^ j_size == sSize)
				{
					int o = j%2==0 ? 0 : 1; //horizontal : vertical
					b = new EdgeButton(i,j,o);
					validMoves.Add((EdgeButton)b);
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
		play();
	}

	public void conP()
	{

	}
	public void conB()
	{

	}
	public void _on_claimed_player(int i, int j, int o)
	{
		handle_button(i, j, o);
		EmitSignal(nameof(ContinuePlayer));
	}
	public void _on_claimed_bot(int i, int j, int o)
	{
		handle_button(i, j, o);
		EmitSignal(nameof(ContinueBot));

	}
	public void handle_button(int i, int j, int o)
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
		buttons[i,j].Text = "C";
		buttons[i,j] = null;
		validMoves.Remove((EdgeButton)buttons[i,j]);
		Console.WriteLine(validMoves.Count);
	}
	public void _on_filled()
	{
		changePlayer = false;
	}
	public void _not_filled()
	{
		changePlayer = true;
	}
	public override void _Process(float delta)
	{
		
	}

	async void play()
	{
		while(true)
		{
			if(changePlayer) active_player = -active_player;

			foreach (Control c in GetChildren()) c.MouseFilter = MouseFilterEnum.Ignore;
			
			if (validMoves.Count == 0)
			{
				//GameOver
			}

			Console.WriteLine(active_player);
			if(active_player == 1)
			{
				//player stuff
				foreach (Control c in GetChildren()) c.MouseFilter = MouseFilterEnum.Pass;
				await ToSignal(this,"ContinuePlayer");
			}
			else if(active_player == -1)
			{
				//AI stuff
				AI_turn();
			}
			// Disconnect("Continue", this, "con");
		}
	}

	public async void AI_turn()
	{
		EdgeButton choice;
		int c = validMoves.Count;
		Random r = new Random();
		choice = validMoves[r.Next(c)];
		choice.Claim();
		await ToSignal(this,"ContinueBot");

	}

}
