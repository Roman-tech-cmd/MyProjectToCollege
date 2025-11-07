using Mono.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using TMPro;
using UnityEngine;

public class TakerInfoFromBase : MonoBehaviour
{
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI[] answerTexts;

    private List<QuestionData> allQuestions = new List<QuestionData>();
    private QuestionData currentQuestion;

    void Start()
    {
        LoadAllQuestionsFromDatabase();
    }

    public void LoadAllQuestionsFromDatabase()
    {
        StartCoroutine(LoadQuestionsCoroutine());
    }

    IEnumerator LoadQuestionsCoroutine()
    {
        string dbPath = Path.Combine(Application.streamingAssetsPath, "QuestionDatabase.db");

#if UNITY_ANDROID || UNITY_WEBGL
        using (var www = UnityEngine.Networking.UnityWebRequest.Get(dbPath))
        {
            yield return www.SendWebRequest();
            if (www.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                string tempPath = Path.Combine(Application.persistentDataPath, "temp.db");
                File.WriteAllBytes(tempPath, www.downloadHandler.data);
                ParseQuestionsFromDatabase(tempPath);
                File.Delete(tempPath);
            }
        }
#else
        if (File.Exists(dbPath))
        {
            ParseQuestionsFromDatabase(dbPath);
        }
#endif

        yield return null;
    }

    void ParseQuestionsFromDatabase(string dbPath)
    {
        try
        {
            allQuestions.Clear();

            string connectionString = "URI=file:" + dbPath;
            using (IDbConnection dbcon = new SqliteConnection(connectionString))
            {
                dbcon.Open();

                Dictionary<int, QuestionData> questionsDict = new Dictionary<int, QuestionData>();

                // Загружаем все вопросы
                using (IDbCommand cmd = dbcon.CreateCommand())
                {
                    cmd.CommandText = "SELECT ID, QuestionText FROM Questions";
                    using (IDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int questionId = reader.GetInt32(0);
                            string questionText = reader.GetString(1);

                            questionsDict[questionId] = new QuestionData
                            {
                                Id = questionId,
                                QuestionText = questionText
                            };
                        }
                    }
                }

                // Загружаем все ответы
                using (IDbCommand cmd = dbcon.CreateCommand())
                {
                    cmd.CommandText = "SELECT QuestionId, AnswerText, IsCorrect FROM Answers ORDER BY QuestionId";
                    using (IDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int questionId = reader.GetInt32(0);
                            string answerText = reader.GetString(1);
                            bool isCorrect = reader.GetBoolean(2);

                            if (questionsDict.ContainsKey(questionId))
                            {
                                questionsDict[questionId].Answers.Add(new AnswerData
                                {
                                    AnswerText = answerText,
                                    IsCorrect = isCorrect
                                });
                            }
                        }
                    }
                }

                // Проверяем что у каждого вопроса ровно 4 ответа (1 правильный, 3 неправильных)
                foreach (var question in questionsDict.Values)
                {
                    int correctCount = 0;
                    int wrongCount = 0;

                    foreach (var answer in question.Answers)
                    {
                        if (answer.IsCorrect) correctCount++;
                        else wrongCount++;
                    }

                    // Добавляем только вопросы с правильной структурой
                    if (correctCount == 1 && wrongCount == 3 && question.Answers.Count == 4)
                    {
                        allQuestions.Add(question);
                    }
                    else
                    {
                        Debug.LogWarning($"Вопрос {question.Id} имеет неправильную структуру ответов. Правильных: {correctCount}, Неправильных: {wrongCount}");
                    }
                }

                Debug.Log($"Загружено вопросов с 4 ответами (1 верный, 3 неверных): {allQuestions.Count}");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Ошибка загрузки базы: {ex.Message}");
        }
    }

    // Метод для получения случайного вопроса
    public QuestionData GetRandomQuestion()
    {
        if (allQuestions.Count == 0)
        {
            Debug.LogWarning("Нет загруженных вопросов");
            return null;
        }

        int randomIndex = Random.Range(0, allQuestions.Count);
        return allQuestions[randomIndex];
    }

    // Метод для показа случайного вопроса
    public void ShowRandomQuestion()
    {
        currentQuestion = GetRandomQuestion();

        if (currentQuestion != null)
        {
            DisplayQuestion(currentQuestion);
        }
    }

    // Метод для отображения вопроса и 4 ответов
    public void DisplayQuestion(QuestionData question)
    {
        // Показываем текст вопроса
        questionText.text = question.QuestionText;

        // Перемешиваем ответы
        List<AnswerData> shuffledAnswers = ShuffleAnswers(new List<AnswerData>(question.Answers));

        // Показываем ровно 4 ответа
        for (int i = 0; i < answerTexts.Length; i++)
        {
            if (i < shuffledAnswers.Count)
            {
                answerTexts[i].text = shuffledAnswers[i].AnswerText;
                answerTexts[i].transform.parent.gameObject.SetActive(true);
            }
            else
            {
                answerTexts[i].transform.parent.gameObject.SetActive(false);
            }
        }
    }

    // Перемешивание ответов
    List<AnswerData> ShuffleAnswers(List<AnswerData> answers)
    {
        for (int i = 0; i < answers.Count; i++)
        {
            AnswerData temp = answers[i];
            int randomIndex = Random.Range(i, answers.Count);
            answers[i] = answers[randomIndex];
            answers[randomIndex] = temp;
        }
        return answers;
    }

    private void Update()
    {
        // Показ случайного вопроса по нажатию Space
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShowRandomQuestion();
        }
    }
}