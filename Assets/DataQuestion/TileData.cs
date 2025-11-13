using Mono.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class TileData : MonoBehaviour
{
    [Tooltip("Номер вопроса")]
    public int idQuestion;

    [Header("Режим загрузки")]
    public bool useDatabase = true;

    [Header("Настройки типа вопроса")]
    public int questionType = 1;
    public Sprite imageQuestion;

    [Header("Основные данные")]
    [TextArea] public string question;

    [Header("Варианты ответов")]
    public string[] answerOptions;
    public string correctAnswer;

    [Header("Подсказки")]
    public string hints;
    public Sprite hintImage;

    private static Dictionary<int, List<int>> availableQuestionIdsByType = new Dictionary<int, List<int>>();
    private bool isDataLoaded = false;

    void Start()
    {
        if (availableQuestionIdsByType.Count == 0)
        {
            availableQuestionIdsByType.Add(1, new List<int>());
            availableQuestionIdsByType.Add(2, new List<int>());
        }

        if (useDatabase)
            LoadQuestionOfSpecifiedType();
        else
        {
            isDataLoaded = true;
            Debug.Log("Используется ручной ввод данных");
        }


    }

    void DebugListTables(string dbPath)
    {
        try
        {
            string connectionString = "URI=file:" + dbPath;
            using (IDbConnection dbcon = new SqliteConnection(connectionString))
            {
                dbcon.Open();

                using (IDbCommand cmd = dbcon.CreateCommand())
                {
                    // Запросим список всех таблиц
                    cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table'";
                    using (IDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Debug.Log("Таблица: " + reader.GetString(0));
                        }
                    }
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Ошибка при проверке таблиц: " + ex.Message);
        }
    }


    public void LoadQuestionOfSpecifiedType()
    {
        if (!isDataLoaded && useDatabase)
            StartCoroutine(LoadRandomQuestionByType(questionType));
    }

    IEnumerator LoadRandomQuestionByType(int type)
    {
        if (isDataLoaded || !useDatabase) yield break;

        questionType = type;

        string dbFileName = "QuestionDatabase.db";
        string persistentPath = Path.Combine(Application.persistentDataPath, dbFileName);

        // Если базы нет в persistentDataPath — копируем из StreamingAssets
        if (!File.Exists(persistentPath))
        {
            string sourcePath = Path.Combine(Application.streamingAssetsPath, dbFileName);

#if UNITY_ANDROID
            UnityWebRequest www = UnityWebRequest.Get(sourcePath);
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                File.WriteAllBytes(persistentPath, www.downloadHandler.data);
                Debug.Log("База скопирована в persistentDataPath: " + persistentPath);
            }
            else
            {
                Debug.LogError("Не удалось загрузить базу: " + www.error);
                yield break;
            }
#else
            File.Copy(sourcePath, persistentPath);
#endif
        }

        // Загружаем вопрос из базы
        LoadRandomQuestionFromDatabase(persistentPath, type);
        Debug.Log("Размер базы: " + new FileInfo(persistentPath).Length + " байт");
        DebugListTables(persistentPath);
    }

    void LoadRandomQuestionFromDatabase(string dbPath, int type)
    {
        try
        {
            string connectionString = "URI=file:" + dbPath;
            using (IDbConnection dbcon = new SqliteConnection(connectionString))
            {
                dbcon.Open();

                if (availableQuestionIdsByType[type].Count == 0)
                {
                    using (IDbCommand cmd = dbcon.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT Id FROM Questions WHERE QuestionType = {type}";
                        using (IDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                                availableQuestionIdsByType[type].Add(reader.GetInt32(0));
                        }
                    }
                    Debug.Log($"Загружено вопросов типа {type}: {availableQuestionIdsByType[type].Count}");
                }

                if (availableQuestionIdsByType[type].Count == 0)
                {
                    Debug.LogError($"Нет вопросов типа {type} в базе данных!");
                    return;
                }

                int randomIndex = Random.Range(0, availableQuestionIdsByType[type].Count);
                int randomQuestionId = availableQuestionIdsByType[type][randomIndex];
                availableQuestionIdsByType[type].RemoveAt(randomIndex);

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

                    if (!reader.IsDBNull(2))
                        hints = reader.GetString(2);
                }
            }
        }

        List<string> answers = new List<string>();
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
                    if (isCorrect) correctAnswer = answerText;
                }
            }
        }

        answerOptions = answers.ToArray();

        Debug.Log($"Вопрос {questionId}: {question}");
        Debug.Log($"Правильный ответ: {correctAnswer}");
        Debug.Log($"Подсказка: {hints}");
    }
}
