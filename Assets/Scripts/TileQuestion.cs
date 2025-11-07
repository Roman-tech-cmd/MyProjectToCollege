using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileQuestion : MonoBehaviour, IPointerClickHandler
{
    [Header("=== ОСНОВНЫЕ ДАННЫЕ ===")]
    [SerializeField] private int idQuestion;
    public int IdQuestion => idQuestion;

    [SerializeField] private TileData questionData;
    public TileData QuestionData
    {
        get { return questionData; }
        set { questionData = value; }
    }

    [SerializeField] private bool takeImageFromData;
    [SerializeField] private Sprite imageQuestionHandler;

    [Space(10)]
    private TileManager tileManager;

    [Header("=== СОСТОЯНИЕ ===")]
    private int wrongAttempts;
    public int WrongAttempts
    {
        get { return wrongAttempts; }
        set { wrongAttempts = value; }
    }

    private int tempWrongAttempts;
    public int TempWrongAttempts
    {
        get { return tempWrongAttempts; }
        set { tempWrongAttempts = value; }
    }

    [HideInInspector] public bool IsSelected;
    [HideInInspector] public bool IsSolved;

    [Header("=== ВИЗУАЛЬНЫЕ ЭЛЕМЕНТЫ ===")]
    private TextMeshProUGUI _numQuestion;
    public TextMeshProUGUI NumQuestion => _numQuestion;

    // --- ДАННЫЕ ВОПРОСА (не сериализованы, заполняются в Inicialize) ---
    private string question;
    public string Question => question;

    private Sprite imageQuestion;
    public Sprite ImageQuestion => imageQuestion;

    private int correctButtonId;
    public int CorrectButtonId => correctButtonId;

    private string[] answerOptions;
    public string[] AnswerOptions => answerOptions;

    private string hints;
    public string Hints => hints;

    private Sprite hintsImage;
    public Sprite HintsImage => hintsImage;

    // --- МЕТОДЫ ---


    private void Start()
    {
        _numQuestion = GetComponentInChildren<TextMeshProUGUI>();
        if (_numQuestion != null)
            _numQuestion.text = idQuestion.ToString();
    }
    void OnEnable()
    {
        Inicialize();
        TransformationStringAnswerToIdButton();
    }
    

    public void TransformationStringAnswerToIdButton()
    {
        for (int i = 0;i< questionData.answerOptions.Length;i++)
        {
            if (questionData.answerOptions[i] == questionData.correctAnswer)
            {
                correctButtonId = i+1;
            }
        }
    }

    public void Inicialize()
    {
        //questionData = GetComponent<TileData>();


        if (tileManager == null)
            tileManager = GameObject.FindGameObjectWithTag("TileManager").GetComponent<TileManager>();

        question = questionData.question;
        imageQuestion = takeImageFromData ? questionData.imageQuestion : imageQuestionHandler;
        answerOptions = questionData.answerOptions;
        hints = questionData.hints;
        hintsImage = questionData.hintImage;

        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        tileManager?.SetSelectTile(eventData.pointerClick.GetComponent<TileQuestion>());
    }
}
