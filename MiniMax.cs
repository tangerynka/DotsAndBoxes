using Godot;
using System.Collections.Generic;
/*
    0-4 - BoxButton - of how many claimed lines a box has
    -1 - free edge 
    null - DotButtons and claimed EdgeButtons
*/
public class MiniMax
{
    int gamesize;
    int[,] StateOfGame;
    List<(int,int)> ValidMoves;
    public MiniMax(int gamesize)
    {
        this.gamesize = gamesize;
        StateOfGame = new int[gamesize,gamesize];
		// validMoves = new List<(int, int)>(2*(boardSize+1)*(boardSize+1));

    }
    public void Move(int[,] SoG, List<(int,int)> validMoves, int depth, int score)
    {
        ValidMoves = new List<(int, int)>(validMoves);
        int i=0, j=0;
        validMoves.Remove((i,j));
        Move(SoG, validMoves, ++depth, score);
    }
}
