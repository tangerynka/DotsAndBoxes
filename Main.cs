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
	int margin = 20;
	int bSize = 40;
	int sSize = 20;
	int count;
	int active_player = -1;
	bool changePlayer = true;
	// EdgeButton[,] validMoves;
	List<(int,int)> validMoves;
	int playerScore = 0;
	int botScore = 0;
	public override void _Ready()
	{
		// Connect("ContinuePlayer", this, "conP");
		// Connect("ContinueBot", this, "conB");
		count = 2*boardSize + 1;
		int i_size, j_size;
		int i_pos = margin, j_pos = margin;
		Button b = new Button();
		buttons = new Button[count, count];
		validMoves = new List<(int, int)>(2*(boardSize+1)*(boardSize+1));
		for(int i = 0; i<count; i++)
		{
			if(i%2 == 0) i_size = sSize;
			else i_size = bSize;
			j_pos = margin;
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
					b = new EdgeButton(i,j, o);
					// b.Text = validMoves.Count.ToString();
					validMoves.Add((i,j));
					Console.WriteLine(i.ToString() +" " +j.ToString() +" "+ validMoves.Count);
					// Console.WriteLine(i.ToString() +" " +j.ToString() +" "+ (validMoves.Count-1).ToString());
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

	public void _on_claimed_player(int i, int j, int o)
	{
		handle_button(i, j, o, 1);
		EmitSignal(nameof(ContinuePlayer));
	}
	public void _on_claimed_bot(int i, int j, int o)
	{
		handle_button(i, j, o, -1);
		EmitSignal(nameof(ContinueBot));

	}
	public void handle_button(int i, int j, int o, int ap)
	{
		// int new_j, new_i;
		BoxButton tmp;
		if(o==0) //horizontal
		{
			// new_i = Mathf.Clamp((i-1)/2,0,boardSize);
			// new_j = Mathf.Clamp((j+1)/2,0,boardSize+1);
			if(j>0) 
			{
				tmp = (BoxButton)buttons[i, j-1];
				tmp.EdgeClaimed(ap);
			}
			if(j<count-1) 
			{
				tmp = (BoxButton)buttons[i, j+1];
				tmp.EdgeClaimed(ap);
			}
		}
		else
		{
			// new_i = Mathf.Clamp(i/2,0,boardSize);
			// new_j = Mathf.Clamp(j/2,0,boardSize+1);
			if(i>0) 
			{
				tmp = (BoxButton)buttons[i-1, j];
				tmp.EdgeClaimed(ap);
			}
			if(i<count-1) 
			{
				tmp = (BoxButton)buttons[i+1, j];
				tmp.EdgeClaimed(ap);
			} 
		}
		// validMoves.RemoveAt(((EdgeButton)buttons[i,j]).index);
		// buttons[i,j].Text = "C";
		buttons[i,j] = null;
		validMoves.Remove((i,j));
		Console.WriteLine(validMoves.Count);
		// Console.WriteLine(i.ToString() +" " +j.ToString()+" "+ new_j.ToString() +" "+ (i*(boardSize+ i%2) + new_j ).ToString());
		// Console.WriteLine();
		// validMoves.RemoveAt(j+i/2)
		// Console.WriteLine(validMoves.Count);
	}
	public void _on_filled()
	{
		changePlayer = false;
	}
	public void _not_filled()
	{
		// changePlayer = true;
	}
	public override void _Process(float delta)
	{
		
	}

	async void play()
	{
		while(validMoves.Count>0)
		{
			if(changePlayer) active_player = -active_player;
			else changePlayer = true;

			foreach (var c in GetChildren()) 
				if(c is Control) ((Control)c).MouseFilter = MouseFilterEnum.Ignore;
			
			// if (validMoves.Count == 0)
			// {
			// 	//GameOver
			// }

			if(active_player == 1)
			{
				//player stuff
				foreach (var c in GetChildren()) 
				if(c is Control) ((Control)c).MouseFilter = MouseFilterEnum.Pass;
				await ToSignal(this,"ContinuePlayer");
			}
			else if(active_player == -1)
			{
				//AI stuff
				AI_turn();
			}
		}
	}

	public async void AI_turn()
	{
		EdgeButton choice;
		int c = validMoves.Count;
		Random r = new Random();
		int tmp = r.Next(c);
		// Console.WriteLine(tmp);
		var (i,j) = validMoves[tmp];
		choice = (EdgeButton)buttons[i, j];
		choice.Claim();
		// validMoves.Remove(choice);
		await ToSignal(this,"ContinueBot");

	}

}
