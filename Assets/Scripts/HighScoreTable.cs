using UnityEngine;

public class HighScoreTable : MonoBehaviour
{
    [SerializeField] GameObject HighScorePanel;
    [SerializeField] Transform scrollTransform;

    private void Start()
    {
        HighScores highScores = SaveSystem.GetHighScores();
        for (int i = 0; i < highScores.scores.Length; i++)
        {
            if (highScores.scores[i] == 0) continue;
            GameObject go = Instantiate(HighScorePanel, scrollTransform);
            go.transform.Find("Score").GetComponent<TMPro.TextMeshProUGUI>().text =
                $"#{i + 1} {highScores.names[i]}: {highScores.scores[i]}";
        }
    }
}
