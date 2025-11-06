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

    [SerializeField] private InfoFromDataBase infoFromDataBase;


    void Start()
    {
        Questions = new List<string>();
        StartCoroutine(LoadDatabase());
    }

    IEnumerator LoadDatabase()
    {
        string dbPath = Path.Combine(Application.streamingAssetsPath, "QuestionDatabase.db");

#if UNITY_ANDROID || UNITY_WEBGL
        // Для мобильных платформ и WebGL
        using (var www = UnityEngine.Networking.UnityWebRequest.Get(dbPath))
        {
            yield return www.SendWebRequest();
            if (www.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                string tempPath = Path.Combine(Application.persistentDataPath, "temp.db");
                File.WriteAllBytes(tempPath, www.downloadHandler.data);
                LoadQuestions(tempPath);
                File.Delete(tempPath);
            }
        }
#else
        // Для Windows/Mac и редактора
        if (File.Exists(dbPath))
        {
            LoadQuestions(dbPath);
        }
        else
        {
            Debug.LogError("База данных не найдена");
            textQ.text = "База данных не найдена";
        }
#endif

        yield return null; // Добавляем возврат значения для всех путей
    }

    void LoadQuestions(string path)
    {
        try
        {
            string connectionString = "URI=file:" + path;
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
                            infoFromDataBase.QuestionsNationalCulture.Add("Вопрос " + id);
                        }
                    }
                }
                using (IDbCommand cmd = dbcon.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM Questions";
                    using (IDataReader reader = cmd.ExecuteReader())
                    {
                        Questions.Clear();
                        while (reader.Read())
                        {
                            string id = reader[0].ToString();
                            infoFromDataBase.QuestionsNationalCulture.Add("Вопрос " + id);
                        }
                    }
                }
            }
            textQ.text = $"Загружено вопросов: {Questions.Count}";
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Ошибка: {ex.Message}");
            textQ.text = "Ошибка загрузки базы";
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && Questions.Count > 0)
        {
            int r = Random.Range(0, Questions.Count);
            textQ.SetText(Questions[r]);
        }
    }
}