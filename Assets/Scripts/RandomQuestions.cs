using System.Collections.Generic;
using UnityEngine;

public class RandomQuestions : MonoBehaviour
{
    [SerializeField] private TileBox TileBox1;
    [SerializeField] private List<TileData> questions1Box;
    [SerializeField] private List<TileData> _cachedQuestions1Box;

    //[SerializeField] private TileBox TileBox2;
    //[SerializeField] private List<TileData> questions2Box;
    //private List<TileData> _cachedQuestions2Box;

    //[SerializeField] private TileBox TileBox3;
    //[SerializeField] private List<TileData> questions3Box;
    //private List<TileData> _cachedQuestions3Box;

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
            //case 2:
            //    randNum = Random.Range(0, _cachedQuestions2Box.Count);
            //    returnedQuestion = _cachedQuestions2Box[randNum];
            //    _cachedQuestions2Box.RemoveAt(randNum);
            //    return returnedQuestion;
            //case 3:
            //    randNum = Random.Range(0, _cachedQuestions3Box.Count);
            //    returnedQuestion = _cachedQuestions3Box[randNum];
            //    _cachedQuestions3Box.RemoveAt(randNum);
            //    return returnedQuestion;
            default: return null;
        }
    }

    public void GenerateQuestion()
    {
        GameObject[] tiles1 = TileBox1.Tiles;
        //GameObject[] tiles2 = TileBox2.Tiles;
        //GameObject[] tiles3 = TileBox3.Tiles;

        foreach (var tile in tiles1)
        {
            tile.GetComponent<TileQuestion>().QuestionData = GenerateRandomQuestion(1);
        }
        //foreach (var tile in tiles2)
        //{
        //    tile.GetComponent<TileQuestion>().QuestionData = GenerateRandomQuestion(2);
        //}
        //foreach (var tile in tiles3)
        //{
        //    tile.GetComponent<TileQuestion>().QuestionData = GenerateRandomQuestion(3);
        //}
    }

    public void RestoreLists()
    {
        _cachedQuestions1Box = questions1Box;
        //_cachedQuestions2Box = questions2Box;
        //_cachedQuestions3Box = questions3Box;
    }


}
