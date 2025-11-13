using UnityEngine;
using TMPro;
using System;

public class CounterAnswer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textÑorrectAnswer;
    [SerializeField] private int countCorrectAnswer;


    private void Start()
    {
        countCorrectAnswer = 0;
        TileManager.CounterÑorrectAnswer += IncCorrectAnswer;
    }
    
    public void IncCorrectAnswer()
    {
        countCorrectAnswer++;
        textÑorrectAnswer.SetText($"Êîëè÷åñòâî âåğíûõ îòâåòîâ {countCorrectAnswer.ToString()} / 27");
    }
}
