using System.Collections.Generic;
using UnityEngine;

public class QuestionData
{
    public int Id;
    public string QuestionText;
    public List<AnswerData> Answers = new List<AnswerData>();
}