using System.Collections.Generic;
using UnityEngine;

public class RandomQuestions : MonoBehaviour
{
    [SerializeField] private List<TileData> questions1Box;
    private List<TileData> _cachedQuestions1Box;
    [SerializeField] private List<TileData> questions2Box;
    private List<TileData> _cachedQuestions2Box;
    [SerializeField] private List<TileData> questions3Box;
    private List<TileData> _cachedQuestions3Box;

    void Start()
    {
        _cachedQuestions1Box = questions1Box;
        _cachedQuestions2Box = questions2Box;
        _cachedQuestions3Box = questions3Box;
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
            case 2:
                randNum = Random.Range(0, _cachedQuestions1Box.Count);
                returnedQuestion = _cachedQuestions1Box[randNum];
                _cachedQuestions1Box.RemoveAt(randNum);
                return returnedQuestion;
            case 3:
                randNum = Random.Range(0, _cachedQuestions1Box.Count);
                returnedQuestion = _cachedQuestions1Box[randNum];
                _cachedQuestions1Box.RemoveAt(randNum);
                return returnedQuestion;
                default: return null;
        }
    }


}
