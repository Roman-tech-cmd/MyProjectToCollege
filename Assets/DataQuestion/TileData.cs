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
            availableQuestionIdsByType.Add(1, new List<int>()); // Группа 1
            availableQuestionIdsByType.Add(2, new List<int>()); // Группа 2
        }

        // Загружаем вопрос указанного типа при старте
        LoadQuestionOfSpecifiedType();
    }

    // Метод для загрузки вопроса указанного в инспекторе типа
    public void LoadQuestionOfSpecifiedType()
    {
        if (!isDataLoaded)
        {
            StartCoroutine(LoadRandomQuestionByType(questionType));
        }
    }

    // Метод для загрузки случайного вопроса по типу
    public IEnumerator LoadRandomQuestionByType(int type)
    {
        if (isDataLoaded) yield break;

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
                        // Используем поле QuestionType в таблице Questions для фильтрации
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
        // Загружаем текст вопроса и тип
        using (IDbCommand cmd = dbcon.CreateCommand())
        {
            cmd.CommandText = $"SELECT QuestionText, QuestionType FROM Questions WHERE Id = {questionId}";
            using (IDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    question = reader.GetString(0);
                    questionType = reader.GetInt32(1); // Загружаем тип из базы
                    idQuestion = questionId;
                }
            }
        }

        // Загружаем ответы для этого вопроса из ОБЩЕЙ таблицы Answers
        List<string> answers = new List<string>();
        string correctAnswerText = "";

        using (IDbCommand cmd = dbcon.CreateCommand())
        {
            // Используем общую таблицу Answers для всех типов вопросов
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
        Debug.Log($"Всего ответов: {answerOptions.Length}");
    }

    // Метод для принудительной перезагрузки нового случайного вопроса (того же типа)
    public void ReloadRandomQuestion()
    {
        isDataLoaded = false;
        StartCoroutine(LoadRandomQuestionByType(questionType));
    }

    // Метод для смены типа вопроса и загрузки нового
    public void ChangeQuestionTypeAndReload(int newType)
    {
        questionType = newType;
        isDataLoaded = false;
        StartCoroutine(LoadRandomQuestionByType(newType));
    }

    // Метод для проверки правильности ответа
    public bool CheckAnswer(string selectedAnswer)
    {
        return selectedAnswer == correctAnswer;
    }

    // Метод для получения индекса правильного ответа
    public int GetCorrectAnswerIndex()
    {
        for (int i = 0; i < answerOptions.Length; i++)
        {
            if (answerOptions[i] == correctAnswer)
            {
                return i;
            }
        }
        return -1;
    }

    // Метод для проверки загружены ли данные
    public bool IsDataLoaded()
    {
        return isDataLoaded;
    }

    // Метод для получения типа вопроса
    public int GetQuestionType()
    {
        return questionType;
    }

    // Метод для установки типа вопроса (можно вызывать из инспектора)
    public void SetQuestionType(int type)
    {
        if (type == 1 || type == 2)
        {
            questionType = type;
        }
        else
        {
            Debug.LogWarning("Тип вопроса должен быть 1 или 2");
        }
    }

    // Метод для получения количества оставшихся вопросов по типу
    public static int GetRemainingQuestionsCountByType(int type)
    {
        if (availableQuestionIdsByType.ContainsKey(type))
        {
            return availableQuestionIdsByType[type].Count;
        }
        return 0;
    }

    // Метод для получения общего количества оставшихся вопросов
    public static int GetTotalRemainingQuestionsCount()
    {
        int total = 0;
        foreach (var list in availableQuestionIdsByType.Values)
        {
            total += list.Count;
        }
        return total;
    }

    // Метод для сброса системы (при начале новой игры)
    public static void ResetQuestionSystem()
    {
        foreach (var list in availableQuestionIdsByType.Values)
        {
            list.Clear();
        }
    }
}

