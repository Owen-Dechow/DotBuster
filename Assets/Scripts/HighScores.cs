using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[System.Serializable]
public class HighScores
{
    public string[] names;
    public int[] scores;

    public HighScores()
    {
        names = new string[10];
        scores = new int[10];
    }

    public static float GetHighScore()
    {
        HighScores scores = SaveSystem.GetHighScores();
        return scores.scores[0];
    }

    public static int GetRanked(int score)
    {
        HighScores scores = SaveSystem.GetHighScores();

        int rank = 0;
        for (int i = 0; i < scores.scores.Length; i++)
        {
            if (score > scores.scores[i]) { rank = i + 1; break; }
        }
        return rank;
    }

    public static void SubmitHighScore(int score, string name)
    {
        int rank = GetRanked(score) - 1;
        HighScores highScores = SaveSystem.GetHighScores();

        List<string> nameList = new(highScores.names);
        List<int> scoreList = new(highScores.scores);
        nameList.Insert(rank, name);
        scoreList.Insert(rank, score);
        nameList.RemoveAt(nameList.Count - 1);
        scoreList.RemoveAt(scoreList.Count - 1);
        highScores.names = nameList.ToArray();
        highScores.scores = scoreList.ToArray();

        SaveSystem.SaveHighScores(highScores);
    }
}

public static class SaveSystem
{
    public static readonly string path = Application.persistentDataPath + "owen_dechow_game_highscore_data_2_23.supercoolgamestuff";

    public static void SaveHighScores(HighScores highScores)
    {
        FileStream stream = new(path, FileMode.Create);
        {
            BinaryFormatter formatter = new();
            formatter.Serialize(stream, highScores);
        }
        stream.Close();
    }

    public static HighScores GetHighScores()
    {
        HighScores highScores;

        if (File.Exists(path))
        {
            FileStream stream = new(path, FileMode.Open);
            {
                BinaryFormatter formatter = new();
                highScores = (HighScores)formatter.Deserialize(stream);
            }
            stream.Close();
        }
        else
        {
            highScores = new HighScores();
        }


        return highScores;
    }
}
