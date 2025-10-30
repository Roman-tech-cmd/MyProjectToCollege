using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;


public class TileManager : MonoBehaviour
{
    [SerializeField] private GameObject blockMenu;
    public GameObject BlockMenu => blockMenu;
    [SerializeField] private GameObject blockMenu2;
    public GameObject BlockMenu2 => blockMenu2;
    [SerializeField] private TileQuestion[] _tiles;
    public TileQuestion[] Tiles
    {
        get { return _tiles; }
        set
        {
            _tiles = value;
        }
    }
    private TileQuestion selectedTiles;
    public TileQuestion SelectedTiles{
        get { return selectedTiles; }
        set
        {
            if (selectedTiles == value)
            {
                if (selectedTiles != null)
                {
                    selectedTiles.transform.DOScale(1f, 0.2f);
                }
                selectedTiles = null;
                return;
            }

            if (selectedTiles != null)
            {
                selectedTiles.transform.DOScale(1f, 0.2f);

            }

            selectedTiles = value;
            if (selectedTiles != null)
            {
                selectedTiles.transform.DOScale(0.9f, 0.2f);
                print(selectedTiles.IdQuestion);
            }
        }       
    }
    [SerializeField] private TextMeshProUGUI textQuestion;
    public TextMeshProUGUI TextQuestion => textQuestion;
    [SerializeField] private TextMeshProUGUI numQuestion;
    public TextMeshProUGUI NumQuestion => numQuestion;
    [SerializeField] private GameObject[] buttons;
    [SerializeField] private TextMeshProUGUI hintsTextUI;
    [SerializeField] private Image hintsImage;

    [SerializeField] private GameObject hintMenu;
    public GameObject HintMenu
    {
        get
        {
            return hintMenu; 
        }
        set
        {
            hintMenu = value;
        }
    }
    [SerializeField] private GameObject textMenu;
    public GameObject TextMenu
    {
        get
        {
            return textMenu; 
        }
        set
        {
            textMenu = value;
        }
    }

    [SerializeField] private GameObject TileBox1;
    [SerializeField] private Button buttonTileBox1;
    [SerializeField] private bool isTileBox1Solved;
    public bool IsTileBox1Solved
    {
        get
        {
            return isTileBox1Solved; 
        }
        set
        {
            isTileBox1Solved = value;
        }
    }

    [SerializeField] private GameObject TileBox2;
    [SerializeField] private Button buttonTileBox2;
    [SerializeField] private bool isTileBox2Solved;
    public bool IsTileBox2Solved{
        get
        {
            return isTileBox2Solved; 
        }
        set
        {
            isTileBox2Solved = value;
        }
    }

    [SerializeField] private GameObject TileBox3;
    [SerializeField] private Button buttonTileBox3;
    [SerializeField] private bool isTileBox3Solved;
    public bool IsTileBox3Solved{
        get
        {
            return isTileBox3Solved; 
        }
        set
        {
            isTileBox3Solved = value;
        }
    }

    private GameObject activeTileBox;
    public GameObject ActiveTileBox
    {
        get { return activeTileBox; }
        set
        {
            if (activeTileBox == value)
            {
                if (activeTileBox != null)
                {
                    return;
                }
            }

            if (activeTileBox != null)
            {
                activeTileBox.GetComponent<TileBox>().SetTileUnselected();
            }

            activeTileBox = value;
        }
        
        
    }
    [SerializeField] private Button activeTileBoxButton;
    public Button ActiveTileBoxButton
    {
        get { return activeTileBoxButton; }
        set
        {
            if (activeTileBoxButton != null)
            {
                activeTileBoxButton.image.color = Color.white;
                activeTileBoxButton.transform.DOScale(1f, 0.2f);
            }
            activeTileBoxButton = value;
            if (activeTileBoxButton != null)
            {
                activeTileBoxButton.image.color = Color.springGreen;
                activeTileBoxButton.transform.DOScale(0.9f, 0.2f);
            }
        }
    }

    [SerializeField] private GameObject borderUI;
    private Image img;

    public void RecheckTiles()
    {
        foreach (TileQuestion tile in _tiles)
        {
            tile.IsSelected = false;
        }
    }

    public void CheckSelectedTile()
    {
        foreach (TileQuestion tile in _tiles)
        {
            if (tile.IsSelected)
            {
                SelectedTiles = tile;
                numQuestion.text = tile.IdQuestion.ToString()+".";
            }
        }
    }

    private void Start()
    {
        img = GameObject.FindGameObjectWithTag("HintsImage").GetComponent<Image>();
    }

    void Update()
    {
        if (SelectedTiles != null)
        {
            numQuestion.text = SelectedTiles.IdQuestion.ToString()+".";
            if (SelectedTiles.IsSolved)
            {
                blockMenu.SetActive(true);
                foreach (GameObject button in buttons)
                {
                    button.GetComponent<Button>().interactable = false;
                    if (button.GetComponent<ButtonTakerQuestion>().idButton == SelectedTiles.CorrectButtonId - 1)
                    {
                        button.GetComponent<Image>().color = Color.green;
                    }
                }
            }
        }

        if (activeTileBox == null)
        {
            ClearInfo();
            TileBox1.SetActive(false);
            TileBox2.SetActive(false);
            TileBox3.SetActive(false);
            borderUI.SetActive(false);
        }
        else
        {
            borderUI.SetActive(true);
        }
        if (SelectedTiles != null)
        {
            foreach (GameObject button in buttons)
            {
                button.SetActive(true);
            }
            textMenu.transform.DOLocalMoveY(-370, 0.5f);
        }
        else
        {

            //textMenu.transform.DOLocalMoveY(-770, 0.5f);
            ClearInfo();
        }
        

        //if (isTileBox1Solved == false)
        //{
        //    buttonTileBox2.interactable = false;
        //}
        //else
        //{
        //    buttonTileBox2.interactable = true;
        //}


        //if (isTileBox2Solved == false)
        //{
        //    buttonTileBox3.interactable = false;
        //}
        //else
        //{
        //    buttonTileBox3.interactable = true;
        //}        

    }

    public void ClearInfo()
    {
        textQuestion.text = "";
        hintsTextUI.text = "";
        textMenu.transform.DOLocalMoveY(-770, 0.5f);
        hintMenu.transform.DOLocalMoveX(1230, 0.5f);
        numQuestion.text = "";
        foreach (GameObject button in buttons)
        {
            button.SetActive(false);
        }
        selectedTiles = null;
    }
    


    public void SetTileBox(int NumBox)
    {        
        switch (NumBox)
        {   
            case 1:
                
                TileBox1.SetActive(true);

                TileBox2.SetActive(false);

                TileBox3.SetActive(false);

                ActiveTileBox = TileBox1;
                ActiveTileBoxButton = buttonTileBox1;
                ClearInfo();
                break;
            case 2: 
                //if (isTileBox1Solved)
                //{
                    TileBox1.SetActive(false);

                    TileBox2.SetActive(true);

                    TileBox3.SetActive(false);

                    ActiveTileBox = TileBox2;
                    ClearInfo();
                    selectedTiles = null;
                    ActiveTileBoxButton = buttonTileBox2;
                ClearInfo();
                //}     
                break;
            case 3:
                //if (isTileBox1Solved && isTileBox2Solved)
                //{
                    TileBox1.SetActive(false);

                    TileBox2.SetActive(false);

                    TileBox3.SetActive(true);

                    ActiveTileBox = TileBox3;
                   
                    selectedTiles = null;
                    ActiveTileBoxButton = buttonTileBox3;
                    ClearInfo();
                //}
                break;
        }
    }




    public void SetSelectTile(TileQuestion tile)
    {
        RecheckTiles();
        SelectedTiles = tile;
        tile.IsSelected = true;

        tile.WrongAttempts = 0;

        if (selectedTiles != null && textQuestion != null)
        {
            textQuestion.text = selectedTiles.Question;
        }

        for (int i = 0; i < buttons.Length; i++)
        {
            if (selectedTiles == null) return;
            if (i < selectedTiles.AnswerOptions.Length)
            {
                buttons[i].SetActive(true);
                TextMeshProUGUI buttonText = buttons[i].GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.text = selectedTiles.AnswerOptions[i];
                }

                ButtonTakerQuestion buttonHandler = buttons[i].GetComponent<ButtonTakerQuestion>();
                if (buttonHandler != null)
                {
                    buttonHandler.idButton = i;
                    buttonHandler.tileManager = this;
                }
            }
            else
            {
                buttons[i].SetActive(false);
            }
        }
        foreach (GameObject button in buttons)
        {
            button.GetComponent<Button>().interactable = true;
            button.GetComponent<Image>().color = Color.white;
        }

        hintMenu.transform.DOLocalMoveX(1230, 0.5f);
        hintsTextUI.text = "";

        if (selectedTiles.IsSolved) blockMenu.SetActive(true);
        else blockMenu.SetActive(false);
    }

    public void CheckAnswer(int selectedAnswerIndex)
    {
        if (selectedAnswerIndex == selectedTiles.CorrectButtonId - 1)
        {
            print("Правильно!");
            selectedTiles.IsSolved = true;
            blockMenu.SetActive(true);
            blockMenu2.SetActive(true);
            FlipCardColors(selectedTiles.GetComponent<RectTransform>(), selectedTiles.GetComponent<Image>(), Color.white, Color.green);

            PlayExpressiveSuccessAnimation(buttons[selectedAnswerIndex]);
            switch (activeTileBox.GetComponent<TileBox>().IdBox)
            {
                case 1:
                    if (activeTileBox.GetComponent<TileBox>().CheckBoxToSolve())
                    {
                        isTileBox1Solved = true;
                        HintMenu.transform.DOLocalMoveX(1230, 0.5f);
                    }
                    break;
                case 2:
                    if (activeTileBox.GetComponent<TileBox>().CheckBoxToSolve())
                    {
                        isTileBox2Solved = true;
                        HintMenu.transform.DOLocalMoveX(1230, 0.5f);
                    }
                    break;
                case 3:
                    if (activeTileBox.GetComponent<TileBox>().CheckBoxToSolve())
                    {
                        isTileBox3Solved = true;
                        HintMenu.transform.DOLocalMoveX(1230, 0.5f);
                    }
                    break;
            }
            selectedTiles.WrongAttempts = 0;
        }
        else
        {
            print("Неправильно!");
            blockMenu.SetActive(true);
            blockMenu2.SetActive(true);
            PlayExpressiveDenyAnimation(buttons[selectedAnswerIndex]);

            selectedTiles.WrongAttempts++;

            if (selectedTiles.WrongAttempts >= 2)
            {
                hintMenu.transform.DOLocalMoveX(850, 0.5f);
                hintsTextUI.text = SelectedTiles.Hints;

                if (activeTileBoxButton == buttonTileBox1)
                {
                    hintsTextUI.gameObject.SetActive(false);
                    TileQuestion tile = selectedTiles.GetComponent<TileQuestion>();
                    if (tile.HintsImage != null)
                    {
                        hintsImage.gameObject.SetActive(true);
                        img.sprite = selectedTiles.GetComponent<TileQuestion>().HintsImage;
                    }
                }
                else
                {
                    hintsImage.gameObject.SetActive(false);
                    hintsTextUI.gameObject.SetActive(true);
                    hintsTextUI.text = SelectedTiles.Hints;
                }
            }
        }
    }
    private void PlayExpressiveDenyAnimation(GameObject target)
    {
        RectTransform rectTransform = target.GetComponent<RectTransform>();
        Image buttonImage = target.GetComponent<Image>();
        Color originalColor = buttonImage.color;
        Vector2 originalPos = rectTransform.anchoredPosition;

        Sequence denySequence = DOTween.Sequence().OnComplete(() =>
        {
            blockMenu.SetActive(false);
            blockMenu2.SetActive(false);
        });

        // 1. Быстрое движение влево-вправо с изменением цвета
        denySequence.Append(rectTransform.DOAnchorPosX(originalPos.x - 40f, 0.12f).SetEase(Ease.OutCubic));
        denySequence.Join(buttonImage.DOColor(Color.red, 0.08f));
        denySequence.Join(rectTransform.DOScale(1.1f, 0.08f));

        denySequence.Append(rectTransform.DOAnchorPosX(originalPos.x + 35f, 0.11f).SetEase(Ease.OutCubic));
        denySequence.Append(rectTransform.DOAnchorPosX(originalPos.x - 25f, 0.10f).SetEase(Ease.OutCubic));
        denySequence.Append(rectTransform.DOAnchorPosX(originalPos.x + 15f, 0.09f).SetEase(Ease.OutCubic));

        // 2. Возврат к нормальному состоянию
        denySequence.Append(rectTransform.DOAnchorPosX(originalPos.x, 0.15f).SetEase(Ease.OutBack));
        denySequence.Join(buttonImage.DOColor(originalColor, 0.15f));
        denySequence.Join(rectTransform.DOScale(1f, 0.15f));
    }
    public void PlayExpressiveSuccessAnimation(GameObject target)
    {
        RectTransform rectTransform = target.GetComponent<RectTransform>();
        Image buttonImage = target.GetComponent<Image>();
        Color originalColor = buttonImage.color;
        Vector2 originalPos = rectTransform.anchoredPosition;

        Sequence seq = DOTween.Sequence().OnComplete(() =>
        {
            blockMenu.SetActive(false);
            blockMenu2.SetActive(false);
        });

        // Прыжок вверх с зеленым цветом
        seq.Append(rectTransform.DOAnchorPosY(originalPos.y + 40f, 0.25f).SetEase(Ease.OutCubic));
        seq.Join(rectTransform.DOScale(1.2f, 0.25f));
        if (buttonImage != null)
            seq.Join(buttonImage.DOColor(new Color(0.3f, 1f, 0.3f), 0.25f));

        // Приземление с bounce
        seq.Append(rectTransform.DOAnchorPosY(originalPos.y, 0.35f).SetEase(Ease.OutBounce));
        seq.Join(rectTransform.DOScale(1f, 0.35f));
    }


    public void FlipCardColors(RectTransform cardRect, Image cardImage, Color fromColor, Color toColor)
    {
        Sequence flipSequence = DOTween.Sequence();

        // Первая половина - сжатие и смена цвета
        flipSequence.Append(cardRect.DOScaleX(0f, 0.2f).SetEase(Ease.InBack)).OnComplete(() =>
        {
            cardImage.sprite = cardImage.GetComponent<TileQuestion>().ImageQuestion;
            cardRect.DOScaleX(0.9f, 0.2f).SetEase(Ease.OutBack);
            cardImage.GetComponent<TileQuestion>().NumQuestion.text = "";
        }); ;
    }
    
    


}
