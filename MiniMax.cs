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
    // List<(int,int)> ValidMoves;
    List<(int,int)> BestMoves;
    List<(int,int)> GoodMoves;
    List<(int,int)> WorstMoves;
    public MiniMax(int gamesize)
    {
        this.gamesize = gamesize;
        // StateOfGame = new int[gamesize,gamesize];
		// validMoves = new List<(int, int)>(2*(boardSize+1)*(boardSize+1));

    }
    public int Move(int[,] SoG, List<(int,int)> validMoves, int depth, int score, bool minimazing, out (int,int) mv) // Bot - minimizing score
    {
        int[,] StateOfGame = (int[,])SoG.Clone();
        // for (int i = 0; i < gamesize; i++)
        // {
        //     for (int j = 0; j < gamesize; j++)
        //     {
        //         Console.Write(string.Format("{0} ", StateOfGame[i, j]));
        //     }
        //     Console.WriteLine();
        // }
        // Console.WriteLine();
        if(validMoves.Count == 0 || depth <=0) 
        {
            mv = (-1,-1);
            return score;
        }
        // int i=0, j=0;
        mv = validMoves[0];
        BestMoves = new List<(int, int)>(gamesize*gamesize);
        GoodMoves = new List<(int, int)>(gamesize*gamesize);
        WorstMoves = new List<(int, int)>(gamesize*gamesize);
        List<(int,int)> tmpL;
        int Score = score;
        int tmpScore;
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
                    // GoodMoves.ForEach(item => Console.WriteLine(item));
                    // Console.WriteLine(GoodMoves);
                }
                else if(tmp==2)
                {
                    WorstMoves.Add((i-1,j));
                    WorstMoves.Add((i+1,j));
                    WorstMoves.Add((i,j-1));
                    WorstMoves.Add((i,j+1));
                    // Console.WriteLine(WorstMoves);
                } 
                else if(tmp==3) 
                {
                    BestMoves.Add((i-1,j));
                    BestMoves.Add((i+1,j));
                    BestMoves.Add((i,j-1));
                    BestMoves.Add((i,j+1));
                    // Console.WriteLine(BestMoves);
                }
            }
        }
        GoodMoves = GoodMoves.Intersect(validMoves).ToList();
        WorstMoves = WorstMoves.Intersect(validMoves).ToList();
        BestMoves = BestMoves.Intersect(validMoves).ToList();
        GoodMoves = GoodMoves.Except(WorstMoves).ToList();
        // GoodMoves = GoodMoves.Where(x=> !WorstMoves.Contains(x).ToList());
        // validMoves.Remove((i,j));
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
            tmpScore = Move(StateOfGame, tmpL, depth-1, score, minimazing, out m);
            if((minimazing && tmpScore <= Score) || (!minimazing && tmpScore >= Score))
            {
                // Console.WriteLine("B"+ depth.ToString()+" "+ score+ " " + Score);
                Score = tmpScore;
                mv = (i,j);
            }
        }
        if(Score == score)
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
                if((minimazing && tmpScore < Score) || (!minimazing && tmpScore > Score))
                {
                    // Console.WriteLine("G"+ depth.ToString()+" "+ score+ " " + Score);
                    Score = tmpScore;
                    mv = (i,j);
                }
            }

        }
        if(Score == score)
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
                if((minimazing && tmpScore < Score) || (!minimazing && tmpScore > Score))
                {
                    // Console.WriteLine("W "+ depth.ToString()+" "+ score+ " " + Score);
                    Score = tmpScore;
                    mv = (i,j);
                }
            }
        }
        return Score;
    }
}
