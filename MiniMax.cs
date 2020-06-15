using Godot;
using System.Collections.Generic;
using System.Linq;
using System;
/*
    StateOfGame
    0-4 - BoxButton - of how many claimed lines a box has
    -1 - free edge
    -10 - DotButtons and claimed EdgeButtons
*/
public class MiniMax
{
    int gamesize;
    List<(int,int)> BestMoves;
    List<(int,int)> GoodMoves;
    List<(int,int)> WorstMoves;
    List<(int,int)> Moves;
    Random r = new Random();

    public MiniMax(int gamesize)
    {
        this.gamesize = gamesize;
    }
    public int Move(int[,] SoG, List<(int,int)> validMoves, int depth, int score, bool minimazing, out (int,int) mv) // Bot - minimizing score
    {
        int[,] StateOfGame = (int[,])SoG.Clone();
        mv = (-1,-1);
        if(validMoves.Count == 0 || depth <=0)
        {
            return score;
        }
        BestMoves = new List<(int, int)>(gamesize*gamesize);
        GoodMoves = new List<(int, int)>(gamesize*gamesize);
        WorstMoves = new List<(int, int)>(gamesize*gamesize);
        Moves = new List<(int, int)>(gamesize*gamesize);
        List<(int,int)> tmpL;
        int Score = score;
        int tmpScore;
        int inc = minimazing? -1 : 1;
        (int,int) m;
        for (int i = 0; i < gamesize; i++)
        {
            for (int j = 0; j < gamesize; j++)
            {
                int tmp = StateOfGame[i,j];
                if(tmp==0 || tmp ==1)
                {
                    GoodMoves.Add((i-1,j));
                    GoodMoves.Add((i+1,j));
                    GoodMoves.Add((i,j-1));
                    GoodMoves.Add((i,j+1));
                }
                else if(tmp==2)
                {
                    WorstMoves.Add((i-1,j));
                    WorstMoves.Add((i+1,j));
                    WorstMoves.Add((i,j-1));
                    WorstMoves.Add((i,j+1));
                }
                else if(tmp==3)
                {
                    BestMoves.Add((i-1,j));
                    BestMoves.Add((i+1,j));
                    BestMoves.Add((i,j-1));
                    BestMoves.Add((i,j+1));
                }
            }
        }
        GoodMoves = GoodMoves.Intersect(validMoves).ToList();
        WorstMoves = WorstMoves.Intersect(validMoves).ToList();
        BestMoves = BestMoves.Intersect(validMoves).ToList();
        GoodMoves = GoodMoves.Except(WorstMoves).ToList();
        GoodMoves = GoodMoves.Except(BestMoves).ToList();
        
        foreach (var (i,j) in BestMoves)
        {
            StateOfGame = (int[,])SoG.Clone();
            tmpL = new List<(int, int)>(validMoves);
            tmpL.Remove((i,j));
            StateOfGame[i,j]= -1;
            if(i<gamesize-1) if(StateOfGame[i+1,j]>=0) StateOfGame[i+1,j]+=1;
            if(i>0) if(StateOfGame[i-1,j]>=0) StateOfGame[i-1,j]+=1;
            if(j>0) if(StateOfGame[i,j-1]>=0) StateOfGame[i,j-1]+=1;
            if(j< gamesize-1) if(StateOfGame[i,j+1]>=0) StateOfGame[i,j+1]+=1;
            tmpScore = Move(StateOfGame, tmpL, depth-1, score+inc, minimazing, out m);
            if((minimazing && tmpScore <= Score) || (!minimazing && tmpScore >= Score))
            {
                mv = (i,j);
                Score = tmpScore;
            }
        }

        if (mv==(-1,-1))
        {
            foreach (var (i,j) in GoodMoves)
            {
                StateOfGame = (int[,])SoG.Clone();
                tmpL = new List<(int, int)>(validMoves);
                tmpL.Remove((i,j));
                StateOfGame[i,j]= -1;
                if(i<gamesize-1) if(StateOfGame[i+1,j]>=0) StateOfGame[i+1,j]+=1;
                if(i>0) if(StateOfGame[i-1,j]>=0) StateOfGame[i-1,j]+=1;
                if(j>0) if(StateOfGame[i,j-1]>=0) StateOfGame[i,j-1]+=1;
                if(j< gamesize-1) if(StateOfGame[i,j+1]>=0) StateOfGame[i,j+1]+=1;
                tmpScore = Move(StateOfGame, tmpL, depth-1, score, !minimazing, out m);
                if((minimazing && tmpScore <= Score) || (!minimazing && tmpScore >= Score))
                {
                    if (tmpScore == Score) Moves.Add((i,j));
                    else
                    {
                        Moves.Clear();
                        Moves.Add((i,j));
                        Score = tmpScore;
                    }
                }
            }

            if(Moves.Count>0)
            {
                mv = Moves[r.Next(Moves.Count)];
            }

        }
        

        if(mv==(-1,-1))
        {
            foreach (var (i,j) in WorstMoves)
            {
                StateOfGame = (int[,])SoG.Clone();
                tmpL = new List<(int, int)>(validMoves);
                tmpL.Remove((i,j));
                StateOfGame[i,j]= -1;
                if(i<gamesize-1) if(StateOfGame[i+1,j]>=0) StateOfGame[i+1,j]+=1;
                if(i>0) if(StateOfGame[i-1,j]>=0) StateOfGame[i-1,j]+=1;
                if(j>0) if(StateOfGame[i,j-1]>=0) StateOfGame[i,j-1]+=1;
                if(j< gamesize-1) if(StateOfGame[i,j+1]>0) StateOfGame[i,j+1]+=1;
                tmpScore = Move(StateOfGame, tmpL, depth-1, score, !minimazing, out m);
                Moves.Add((i,j));
                Score = tmpScore;
            }
            mv = Moves[r.Next(Moves.Count)];

        }
        return Score;
    }
}
