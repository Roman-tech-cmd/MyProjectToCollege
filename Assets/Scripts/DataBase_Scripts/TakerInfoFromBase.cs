using Mono.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using TMPro;
using UnityEngine;

public class TestDatabase : MonoBehaviour
{
    public List<string> Questions;
    public TextMeshProUGUI textQ;
    private string dbPath;

    void Start()
    {
        Questions = new List<string>();
        InitializeDatabase();
    }

    void InitializeDatabase()
    {
        textQ.text = "Загрузка базы данных...";

        // Всегда используем БД из StreamingAssets
        dbPath = Path.Combine(Application.streamingAssetsPath, "QuestionDatabase.db");

        Debug.Log($"Путь к БД: {dbPath}");

        // Загружаем вопросы из базы
        LoadQuestionsFromDatabase();
    }

    void LoadQuestionsFromDatabase()
    {
        try
        {
            // Для разных платформ разный способ доступа к файлу
#if UNITY_ANDROID || UNITY_WEBGL
            StartCoroutine(LoadDatabaseForMobile());
#else
            // Для Windows/Mac и редактора - прямой доступ к файлу
            if (File.Exists(dbPath))
            {
                LoadQuestionsDirect();
            }
            else
            {
                Debug.LogError($"БД не найдена: {dbPath}");
                textQ.text = "База данных не найдена";
            }
#endif
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Ошибка загрузки БД: {ex.Message}");
            textQ.text = $"Ошибка: {ex.Message}";
        }
    }

    IEnumerator LoadDatabaseForMobile()
    {
        // Для Android и WebGL загружаем через UnityWebRequest
        using (var www = UnityEngine.Networking.UnityWebRequest.Get(dbPath))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                // Создаем временный файл
                string tempPath = Path.Combine(Application.persistentDataPath, "temp.db");
                File.WriteAllBytes(tempPath, www.downloadHandler.data);

                // Загружаем из временного файла
                LoadQuestionsFromFile(tempPath);

                // Удаляем временный файл
                File.Delete(tempPath);
            }
            else
            {
                Debug.LogError($"Ошибка загрузки: {www.error}");
                textQ.text = $"Ошибка: {www.error}";
            }
        }
    }

    void LoadQuestionsDirect()
    {
        // Прямая загрузка для Windows/Mac/Editor
        string connectionString = "URI=file:" + dbPath;
        using (IDbConnection dbcon = new SqliteConnection(connectionString))
        {
            dbcon.Open();

            using (IDbCommand cmd = dbcon.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Questions";
                using (IDataReader reader = cmd.ExecuteReader())
                {
                    Questions.Clear();

                    while (reader.Read())
                    {
                        string id = reader[0].ToString();
                        Questions.Add("Вопрос " + id);
                    }
                }
            }
        }

        textQ.text = $"Загружено вопросов: {Questions.Count}";
        Debug.Log($"Успешно загружено: {Questions.Count} вопросов");
    }

    void LoadQuestionsFromFile(string filePath)
    {
        // Загрузка из конкретного файла
        string connectionString = "URI=file:" + filePath;
        using (IDbConnection dbcon = new SqliteConnection(connectionString))
        {
            dbcon.Open();

            using (IDbCommand cmd = dbcon.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Questions";
                using (IDataReader reader = cmd.ExecuteReader())
                {
                    Questions.Clear();

                    while (reader.Read())
                    {
                        string id = reader[0].ToString();
                        Questions.Add("Вопрос " + id);
                    }
                }
            }
        }

        textQ.text = $"Загружено вопросов: {Questions.Count}";
        Debug.Log($"Успешно загружено: {Questions.Count} вопросов");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && Questions != null && Questions.Count > 0)
        {
            int r = Random.Range(0, Questions.Count);
            textQ.SetText(Questions[r]);
        }
    }
}