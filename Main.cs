using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class Main : Control
{
	MiniMax miniMax;
	[Signal]
	public delegate void ContinuePlayer();
	[Signal]
	public delegate void ContinueBot();
	Button[,] buttons; 
	int boardSize = 5;
	int searchingDepth = 3;
	int margin = 20;
	int bSize = 40;
	int sSize = 20;
	int count;
	int[,] StateOfGame;
	int active_player = -1;
	bool changePlayer = true;
	// EdgeButton[,] validMoves;
	List<(int,int)> validMoves;
	// List<(int,int)> allBoxes;
	bool reset = false;
	int playerScore = 0, botScore = 0;
	int PlayerScore 
	{
		get { return playerScore;} 
		set { playerScore = value; GetNode<Button>("PlayerScore").Text = playerScore.ToString();}
		
	}
	int BotScore
	{
		get { return botScore;} 
		set { botScore = value; GetNode<Button>("BotScore").Text = botScore.ToString();}
		
	}
	int Score
	{
		get { return PlayerScore - BotScore;} // Player - maximizing, Bot - minimizing
	}
	public override void _Ready()
	{
		prepare_game();
		play();
	}

	private void prepare_game()
	{
		/*  
			StateOfGame:
			0-4 - BoxButton - of how many claimed lines a box has
			-1 - free edge 
			-10 - DotButtons and claimed EdgeButtons
		*/
		PlayerScore = 0;
		BotScore = 0;
		active_player = 1;
		changePlayer = false;
		
		count = 2*boardSize + 1;
		Button b = new Button();
		buttons = new Button[count, count];
		StateOfGame = new int[count,count];
		validMoves = new List<(int, int)>(2*(boardSize+1)*(boardSize+1));
		miniMax = new MiniMax(count);

		int i_size, j_size;
		int i_pos = margin, j_pos = margin;
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
					StateOfGame[i,j]=-10;
				}
				else if (i_size == sSize ^ j_size == sSize)
				{
					int o = j%2==0 ? 0 : 1; //horizontal : vertical
					b = new EdgeButton(i,j, o);
					validMoves.Add((i,j));
					StateOfGame[i,j]=-1;
				}
				else if (i_size == bSize && j_size == bSize)
				{	
					b = new BoxButton(i,j);
					StateOfGame[i,j]=0;
				}

				buttons[i,j] = b;
				b.RectPosition = new Vector2(j_pos, i_pos);
				b.RectSize = new Vector2(j_size,i_size);
				j_pos += j_size;
				b.Show();
				AddChild(b);
			}
			i_pos += i_size;
			
		}
	}

	private void clear_game()
	{
		foreach( var b in buttons) b.QueueFree();
		buttons = null;
		validMoves = null;
	}
	public void _on_Reset_pressed()
	{
		reset_game();
	}
	public void reset_game()
	{
		reset =  true;
		EmitSignal(nameof(ContinuePlayer));
		clear_game();
		prepare_game();
		reset = false;
		play();

	}
	public void _on_3_pressed()
	{
		boardSize = 3;
		searchingDepth = 5;
		reset_game();
	}
	public void _on_5_pressed()
	{
		boardSize = 5;
		searchingDepth = 3;
		reset_game();
	}
	public void _on_10_pressed()
	{
		boardSize = 10;
		searchingDepth = 2;
		reset_game();
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
		BoxButton tmp;
		if(o==0) //horizontal
		{
			if(j>0) 
			{
				tmp = (BoxButton)buttons[i, j-1];
				tmp.EdgeClaimed(ap);
				StateOfGame[i,j-1]++;
			}
			if(j<count-1) 
			{
				tmp = (BoxButton)buttons[i, j+1];
				tmp.EdgeClaimed(ap);
				StateOfGame[i,j+1]++;
			}
		}
		else
		{
			if(i>0) 
			{
				tmp = (BoxButton)buttons[i-1, j];
				tmp.EdgeClaimed(ap);
				StateOfGame[i-1,j]++;
			}
			if(i<count-1) 
			{
				tmp = (BoxButton)buttons[i+1, j];
				tmp.EdgeClaimed(ap);
				StateOfGame[i+1,j]++;
			} 
		}
		validMoves.Remove((i,j));
	}
	public void _on_filled(int i, int j, int ap)
	{
		changePlayer = false;
		if(ap==1) PlayerScore++;
		else if(ap == -1) BotScore++;
	}
	async void play()
	{
		while(validMoves.Count>0 && !reset)
		{
			if(changePlayer) active_player = -active_player;
			else changePlayer = true;
			foreach (var c in GetChildren()) 
				if(c is EdgeButton) ((Control)c).MouseFilter = MouseFilterEnum.Ignore;
				
			if(active_player == 1)
			{
				//player stuff
				foreach (var c in GetChildren()) 
				if(c is Control) ((Control)c).MouseFilter = MouseFilterEnum.Pass;
				await ToSignal(this,"ContinuePlayer");
			}
			else if(active_player == -1)
			{
				// AI stuff
				AI_turn();
			}
		}
	}

	public async void AI_turn()
	{
		EdgeButton choice;
		(int, int) move;
		miniMax.Move(StateOfGame,validMoves,1,Score,true,out move);
		var (i,j) = move;
		if (i<0 || j<0) return;
		choice = (EdgeButton)buttons[i,j];
		choice.Claim();
		await ToSignal(this,"ContinueBot");
	}

}
