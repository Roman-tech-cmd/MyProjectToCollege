using UnityEngine;

[CreateAssetMenu(fileName = "NewQuizQuestion", menuName = "Quiz/QuizQuestion", order = 1)]
public class TileData : ScriptableObject
{

    [Tooltip("Номер вопроса")]
    public int idQuestion;

    [Tooltip("Изображение вопроса")]
    public Sprite imageQuestion;

    [Header("Основные данные")]
    [Tooltip("Текст вопроса")]
    [TextArea]
    public string question;

    [Header("Варианты ответов")]
    [Tooltip("Массив с вариантами ответов (включая правильный)")]
    public string[] answerOptions;
    
    [Tooltip("Правильный ответ")]
    public int correctIdButton;

    [Header("Подсказки")]
    [Tooltip("Массив с подсказками к вопросу")]
    public string hints;
    public Sprite hintImage;
}
