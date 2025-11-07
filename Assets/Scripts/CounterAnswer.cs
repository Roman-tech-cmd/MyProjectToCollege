using UnityEngine;
using TMPro;
using System;

public class CounterAnswer : MonoBehaviour
{
    [SerializeField] private int countIncorrectAnswer;
    [SerializeField] private TextMeshProUGUI textIncorrectAnswer;
    [SerializeField] private int countCorrectAnswer;


    private void Start()
    {
        countIncorrectAnswer = 0;
        TileManager.CounterIncorrectAnswer += IncIncorrectAnswer;
        TileManager.CounterСorrectAnswer += IncCorrectAnswer;
    }

    public void IncIncorrectAnswer()
    {
        countIncorrectAnswer++;
        //textIncorrectAnswer.SetText($"Количество допущенных ошибок: {countIncorrectAnswer.ToString()}");
    }
    
    public void IncCorrectAnswer()
    {
        countCorrectAnswer++;
        textIncorrectAnswer.SetText($"Количество правильных ответов: {countCorrectAnswer.ToString()} из 27");
    }
}
