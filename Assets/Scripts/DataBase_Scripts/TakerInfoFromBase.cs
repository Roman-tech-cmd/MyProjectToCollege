using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System.Collections.Generic;
using TMPro;
using System.IO;
using System.Collections;

public class TestDatabase : MonoBehaviour
{
    public List<string> Questions;
    public TextMeshProUGUI textQ;
    private string dbPath;

    void Start()
    {
        Questions = new List<string>();
        StartCoroutine(InitializeDatabase());
    }

    IEnumerator InitializeDatabase()
    {
        textQ.text = "Загрузка базы данных...";

        // Путь для БД - рядом с exe файлом для простоты доступа
        dbPath = Path.Combine(Directory.GetCurrentDirectory(), "QuestionDatabase.db");

        Debug.Log($"Путь к БД: {dbPath}");
        Debug.Log($"Текущая директория: {Directory.GetCurrentDirectory()}");

        // Проверяем существует ли БД рядом с exe
        if (!File.Exists(dbPath))
        {
            Debug.Log("БД не найдена рядом с exe, ищем в StreamingAssets...");
            yield return StartCoroutine(CopyDatabaseFromStreamingAssets());
        }
        else
        {
            Debug.Log("БД найдена рядом с exe файлом");
            LoadQuestionsFromDatabase();
        }
    }

    IEnumerator CopyDatabaseFromStreamingAssets()
    {
        // Правильный путь к StreamingAssets в билде
        string sourcePath = Path.Combine(Application.streamingAssetsPath, "QuestionDatabase.db");

        Debug.Log($"Ищем БД в: {sourcePath}");

#if UNITY_ANDROID || UNITY_WEBGL
        // Для Android и WebGL
        using (var www = UnityEngine.Networking.UnityWebRequest.Get(sourcePath))
        {
            yield return www.SendWebRequest();
            
            if (www.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                File.WriteAllBytes(dbPath, www.downloadHandler.data);
                Debug.Log("БД скопирована из StreamingAssets!");
                LoadQuestionsFromDatabase();
            }
            else
            {
                Debug.LogError($"Ошибка копирования БД: {www.error}");
                textQ.text = $"Ошибка: {www.error}";
            }
        }
#else
        // Для Windows/Mac
        if (File.Exists(sourcePath))
        {
            try
            {
                File.Copy(sourcePath, dbPath, true);
                Debug.Log("БД скопирована из StreamingAssets!");
                LoadQuestionsFromDatabase();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Ошибка копирования: {ex.Message}");
                textQ.text = $"Ошибка копирования: {ex.Message}";
            }
        }
        else
        {
            Debug.LogError($"БД не найдена в StreamingAssets: {sourcePath}");

            // Покажем все файлы в StreamingAssets для отладки
            string streamingAssetsDir = Application.streamingAssetsPath;
            if (Directory.Exists(streamingAssetsDir))
            {
                string[] files = Directory.GetFiles(streamingAssetsDir);
                Debug.Log("Файлы в StreamingAssets:");
                foreach (string file in files)
                {
                    Debug.Log($" - {file}");
                }
            }

            textQ.text = $"БД не найдена по пути: {sourcePath}";
        }
        yield return null;
#endif
    }

    void LoadQuestionsFromDatabase()
    {
        try
        {
            if (!File.Exists(dbPath))
            {
                Debug.LogError($"Файл БД не существует: {dbPath}");
                textQ.text = "Файл базы данных не найден";
                return;
            }

            Debug.Log($"Подключаемся к БД: {dbPath}");

            // Open connection
            string connectionString = "URI=file:" + dbPath;
            IDbConnection dbcon = new SqliteConnection(connectionString);
            dbcon.Open();

            // Read and print all values in table
            IDbCommand cmnd_read = dbcon.CreateCommand();
            IDataReader reader;
            string query = "SELECT * FROM my_table";
            cmnd_read.CommandText = query;
            reader = cmnd_read.ExecuteReader();

            Questions.Clear();

            int count = 0;
            while (reader.Read())
            {
                string id = reader[0].ToString();
                string val = reader[1].ToString();

                Debug.Log($"id: {id}, val: {val}");
                Questions.Add("Вопрос " + id + ": " + val);
                count++;
            }

            textQ.text = $"База загружена! Вопросов: {Questions.Count}";
            Debug.Log($"Успешно загружено вопросов: {Questions.Count}");

            reader.Close();
            dbcon.Close();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Ошибка загрузки БД: {ex.Message}");
            textQ.text = $"Ошибка загрузки: {ex.Message}";
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && Questions != null && Questions.Count > 0)
        {
            int r = Random.Range(0, Questions.Count);
            textQ.SetText(Questions[r]);
        }
    }

    // Метод для отладки - покажет информацию о путях
    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 800, 20), $"DB Path: {dbPath}");
        GUI.Label(new Rect(10, 30, 800, 20), $"Current Dir: {Directory.GetCurrentDirectory()}");
        GUI.Label(new Rect(10, 50, 800, 20), $"StreamingAssets: {Application.streamingAssetsPath}");
        GUI.Label(new Rect(10, 70, 800, 20), $"DB Exists: {File.Exists(dbPath)}");
    }
}