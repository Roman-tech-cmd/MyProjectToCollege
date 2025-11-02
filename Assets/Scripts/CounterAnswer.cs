using UnityEngine;
using TMPro;
using System;

public class CounterAnswer : MonoBehaviour
{
    [SerializeField] private int countIncorrectAnswer;
    [SerializeField] private TextMeshProUGUI textIncorrectAnswer;


    private void Start()
    {
        countIncorrectAnswer = 0;
        TileManager.CounterIncorrectAnswer += IncIncorrectAnswer;
    }

    public void IncIncorrectAnswer()
    {
        countIncorrectAnswer++;
        textIncorrectAnswer.SetText($"Количество допущенных ошибок: {countIncorrectAnswer.ToString()}");
    }
}
