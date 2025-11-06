using UnityEngine;
using System.Collections.Generic;

public class InfoFromDataBase : MonoBehaviour
{
    [SerializeField] private List<string> questionsNationalCulture;
    public List<string> QuestionsNationalCulture
    {
        get { return questionsNationalCulture; }
        set { questionsNationalCulture = value; }
    }

    [SerializeField] private List<string> questionsMyCountry;
    public List<string> QuestionsMyCountry
    {
        get { return questionsMyCountry; }
        set { questionsMyCountry = value; }
    }

    private void Update()
    {
        //print(questionsNationalCulture.Count + " è " + questionsMyCountry.Count);
    }

}
