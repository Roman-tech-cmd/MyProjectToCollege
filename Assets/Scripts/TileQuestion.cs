using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileQuestion : MonoBehaviour,IPointerClickHandler
{

    [SerializeField] private int wrongAttempts;
    public int WrongAttempts
    {
        get { return wrongAttempts; }
        set { wrongAttempts = value; }
    }
    
    [SerializeField] private TileData questionData;
    private int idQuestion;
    public int IdQuestion => idQuestion;
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
    public bool IsSelected;
    public bool IsSolved;

    public static event Action OnQuestionSolved; // Событие при решении вопроса

    public void OnPointerClick(PointerEventData eventData)
    {
        print("Tile Selected");
        tileManager.SetSelectTile(eventData.pointerClick.GetComponent<TileQuestion>());
    }

    // Можно вызывать это событие при решении
    public void NotifySolved()
    {
        OnQuestionSolved?.Invoke();
    }


    [SerializeField] private TileManager tileManager;


    public void Inicialize()
    {
        tileManager = GameObject.FindGameObjectWithTag("TileManager").GetComponent<TileManager>();
        question = questionData.question;
        imageQuestion = questionData.imageQuestion;
        answerOptions = questionData.answerOptions;
        hints = questionData.hints;
        correctButtonId = questionData.correctIdButton;
        idQuestion = questionData.idQuestion;
        hintsImage = questionData.hintImage;
    }
    private TextMeshProUGUI _numQuestion;
    public TextMeshProUGUI NumQuestion => _numQuestion;

    void Start()
    {
        Inicialize();
        _numQuestion = GetComponentInChildren<TextMeshProUGUI>();
        _numQuestion.text = idQuestion.ToString();
    }

}
