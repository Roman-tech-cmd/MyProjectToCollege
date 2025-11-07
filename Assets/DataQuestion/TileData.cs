using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;
using System.Data;
using System.IO;
using System.Collections.Generic;

public class TileData : MonoBehaviour
{

    [Tooltip("Номер вопроса")]
    public int idQuestion;

    [Space(10)]
    [Header("Режим загрузки")]
    [Tooltip("Если true - данные загружаются из БД, если false - вводятся вручную в инспекторе")]
    public bool useDatabase = true;
    [Space(10)]

    [Header("Настройки типа вопроса")]
    [Tooltip("Тип вопроса (1 или 2) - задается в инспекторе")]
    public int questionType = 1;

    [Tooltip("Изображение вопроса")]
    public Sprite imageQuestion;

    [Header("Основные данные")]
    [Tooltip("Текст вопроса")]
    [TextArea]
    public string question;

    [Header("Варианты ответов")]
    [Tooltip("Массив с вариантами ответов (включая правильный)")]
    public string[] answerOptions;

    [Tooltip("Правильный ответ текст")]
    public string correctAnswer;

    [Header("Подсказки")]
    [Tooltip("Массив с подсказками к вопросу")]
    public string hints;
    public Sprite hintImage;

    private static Dictionary<int, List<int>> availableQuestionIdsByType = new Dictionary<int, List<int>>();
    private bool isDataLoaded = false;



    public void Start()
    {
        // Инициализируем словарь для типов вопросов
        if (availableQuestionIdsByType.Count == 0)
        {
            availableQuestionIdsByType.Add(1, new List<int>());
            availableQuestionIdsByType.Add(2, new List<int>());
        }

        // Только если используем БД - загружаем данные
        if (useDatabase)
        {
            LoadQuestionOfSpecifiedType();
        }
        else
        {
            // В ручном режиме оставляем значения из инспектора как есть
            isDataLoaded = true;
            Debug.Log("Используется ручной ввод данных");
        }
    }

    // Метод для загрузки вопроса указанного в инспекторе типа
    public void LoadQuestionOfSpecifiedType()
    {
        if (!isDataLoaded && useDatabase)
        {
            StartCoroutine(LoadRandomQuestionByType(questionType));
        }
    }

    // Метод для загрузки случайного вопроса по типу
    public IEnumerator LoadRandomQuestionByType(int type)
    {
        if (isDataLoaded || !useDatabase) yield break;

        questionType = type;
        string dbPath = Path.Combine(Application.streamingAssetsPath, "QuestionDatabase.db");

#if UNITY_ANDROID || UNITY_WEBGL
        // Для мобильных платформ
        yield return StartCoroutine(LoadRandomQuestionMobile(dbPath, type));
#else
        // Для Windows/Mac
        if (File.Exists(dbPath))
        {
            LoadRandomQuestionFromDatabase(dbPath, type);
        }
        yield return null;
#endif
    }

    IEnumerator LoadRandomQuestionMobile(string dbPath, int type)
    {
        using (var www = UnityEngine.Networking.UnityWebRequest.Get(dbPath))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                string tempPath = Path.Combine(Application.persistentDataPath, "temp_question.db");
                File.WriteAllBytes(tempPath, www.downloadHandler.data);
                LoadRandomQuestionFromDatabase(tempPath, type);
                File.Delete(tempPath);
            }
        }
    }

    void LoadRandomQuestionFromDatabase(string dbPath, int type)
    {
        try
        {
            string connectionString = "URI=file:" + dbPath;
            using (IDbConnection dbcon = new SqliteConnection(connectionString))
            {
                dbcon.Open();

                // Получаем список всех ID вопросов для указанного типа
                if (availableQuestionIdsByType[type].Count == 0)
                {
                    using (IDbCommand cmd = dbcon.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT Id FROM Questions WHERE QuestionType = {type}";
                        using (IDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                availableQuestionIdsByType[type].Add(reader.GetInt32(0));
                            }
                        }
                    }

                    Debug.Log($"Загружено вопросов типа {type}: {availableQuestionIdsByType[type].Count}");
                }

                if (availableQuestionIdsByType[type].Count == 0)
                {
                    Debug.LogError($"Нет вопросов типа {type} в базе данных!");
                    return;
                }

                // Выбираем случайный ID вопроса из нужного типа
                int randomIndex = Random.Range(0, availableQuestionIdsByType[type].Count);
                int randomQuestionId = availableQuestionIdsByType[type][randomIndex];
                availableQuestionIdsByType[type].RemoveAt(randomIndex);

                // Загружаем данные выбранного вопроса
                LoadQuestionData(dbcon, randomQuestionId, type);

                isDataLoaded = true;
                Debug.Log($"Загружен вопрос ID: {randomQuestionId}, Тип: {type}");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Ошибка загрузки вопроса типа {type}: {ex.Message}");
        }
    }

    void LoadQuestionData(IDbConnection dbcon, int questionId, int type)
    {
        // Загружаем текст вопроса, тип и подсказку
        using (IDbCommand cmd = dbcon.CreateCommand())
        {
            cmd.CommandText = $"SELECT QuestionText, QuestionType, HintText FROM Questions WHERE Id = {questionId}";
            using (IDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    question = reader.GetString(0);
                    questionType = reader.GetInt32(1);
                    idQuestion = questionId;

                    // Загружаем подсказку если есть
                    if (!reader.IsDBNull(2))
                    {
                        hints = reader.GetString(2);
                    }
                }
            }
        }

        // Загружаем ответы для этого вопроса
        List<string> answers = new List<string>();
        string correctAnswerText = "";

        using (IDbCommand cmd = dbcon.CreateCommand())
        {
            cmd.CommandText = $"SELECT AnswerText, IsCorrect FROM Answers WHERE QuestionId = {questionId}";
            using (IDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    string answerText = reader.GetString(0);
                    bool isCorrect = reader.GetBoolean(1);

                    answers.Add(answerText);

                    if (isCorrect)
                    {
                        correctAnswer = answerText;
                        correctAnswerText = answerText;
                    }
                }
            }
        }

        // Проверяем что загружено 4 ответа
        if (answers.Count != 4)
        {
            Debug.LogWarning($"Вопрос {questionId} имеет {answers.Count} ответов вместо 4!");
        }

        // Записываем ответы в массив
        answerOptions = answers.ToArray();

        Debug.Log($"Загружен вопрос типа {type}: {question}");
        Debug.Log($"Правильный ответ: {correctAnswer}");
        Debug.Log($"Подсказка: {hints}");
        Debug.Log($"Всего ответов: {answerOptions.Length}");
    }

    // Метод для принудительной перезагрузки нового случайного вопроса
    public void ReloadRandomQuestion()
    {
        if (!useDatabase)
        {
            Debug.LogWarning("Режим перезагрузки недоступен при ручном вводе данных");
            return;
        }

        isDataLoaded = false;
        StartCoroutine(LoadRandomQuestionByType(questionType));
    }
}


