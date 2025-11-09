using System.Collections.Generic;
using UnityEngine;

public class RandomQuestions : MonoBehaviour
{
    [SerializeField] private TileBox TileBox1;
    [SerializeField] private List<TileData> questions1Box;
    [SerializeField] private List<TileData> _cachedQuestions1Box;

    void Start()
    {
        RestoreLists();
        GenerateQuestion();
    }
    public TileData GenerateRandomQuestion(int numBox)
    {
        int randNum;
        TileData returnedQuestion;
        switch (numBox)
        {
            case 1:
                randNum = Random.Range(0, _cachedQuestions1Box.Count);
                returnedQuestion = _cachedQuestions1Box[randNum];
                _cachedQuestions1Box.RemoveAt(randNum);
                return returnedQuestion;
            default: return null;
        }
    }

    public void GenerateQuestion()
    {
        GameObject[] tiles1 = TileBox1.Tiles;

        foreach (var tile in tiles1)
        {
            tile.GetComponent<TileQuestion>().QuestionData = GenerateRandomQuestion(1);
        }
    }

    public void RestoreLists()
    {
        _cachedQuestions1Box = questions1Box;
    }
}
