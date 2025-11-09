using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Text;
using Mono.Data.Sqlite;
using System.Data;

public class DatabaseDebugger : MonoBehaviour
{
    [Header("Настройки дебага")]
    public bool runOnStart = true;
    public bool testAllConnectionStrings = true;

    private StringBuilder log = new StringBuilder();
    private string logPath;

    void Start()
    {
        if (runOnStart)
            StartCoroutine(RunFullDiagnostic());
    }

    [ContextMenu("Запустить диагностику")]
    public void RunDiagnosticManual()
    {
        StartCoroutine(RunFullDiagnostic());
    }

    IEnumerator RunFullDiagnostic()
    {
        log.Clear();
        logPath = Path.Combine(Application.persistentDataPath, "database_diagnostic.txt");

        AddLog("=== ПОЛНАЯ ДИАГНОСТИКА БАЗЫ ДАННЫХ ===");
        AddLog($"Время: {DateTime.Now}");
        AddLog($"Путь к логу: {logPath}");

        yield return StartCoroutine(CheckSystemInfo());
        yield return StartCoroutine(CheckUnityEnvironment());
        yield return StartCoroutine(CheckSQLiteAssemblies());
        yield return StartCoroutine(CheckDatabaseFile());

        if (testAllConnectionStrings)
            yield return StartCoroutine(TestAllConnectionStrings());

        yield return StartCoroutine(TestSpecificIssues());

        SaveLogToFile();
        DisplayResults();

        yield return null;
    }

    IEnumerator CheckSystemInfo()
    {
        AddLog("\n--- СИСТЕМНАЯ ИНФОРМАЦИЯ ---");

        try
        {
            var os = Environment.OSVersion;
            AddLog($"ОС: {os.VersionString}");
            AddLog($"Платформа: {os.Platform}");
            AddLog($"Версия: {os.Version.Major}.{os.Version.Minor}.{os.Version.Build}");
            AddLog($"Service Pack: {os.ServicePack}");

            // Определяем Windows версию
            if (os.Platform == PlatformID.Win32NT)
            {
                if (os.Version.Major == 10)
                {
                    if (os.Version.Build >= 22000)
                        AddLog("Обнаружена: Windows 11");
                    else
                        AddLog("Обнаружена: Windows 10");
                }
            }

            AddLog($"Компьютер: {Environment.MachineName}");
            AddLog($"Пользователь: {Environment.UserName}");
            AddLog($".NET Version: {Environment.Version}");
            AddLog($"Системная папка: {Environment.SystemDirectory}");
            AddLog($"Кол-во процессоров: {Environment.ProcessorCount}");
            AddLog($"64-bit система: {Environment.Is64BitOperatingSystem}");
            AddLog($"64-bit процесс: {Environment.Is64BitProcess}");
        }
        catch (Exception ex)
        {
            AddLog($"Ошибка системной информации: {ex.Message}");
        }

        yield return null;
    }

    IEnumerator CheckUnityEnvironment()
    {
        AddLog("\n--- СРЕДА UNITY ---");

        AddLog($"Unity версия: {Application.unityVersion}");
        AddLog($"Платформа: {Application.platform}");
        AddLog($"Версия приложения: {Application.version}");
        AddLog($"Data Path: {Application.dataPath}");
        AddLog($"Persistent Data Path: {Application.persistentDataPath}");
        AddLog($"Streaming Assets Path: {Application.streamingAssetsPath}");
        AddLog($"Текущая директория: {Directory.GetCurrentDirectory()}");

        // Проверяем права записи
        try
        {
            string testFile = Path.Combine(Application.persistentDataPath, "test_write.txt");
            File.WriteAllText(testFile, "test");
            File.Delete(testFile);
            AddLog("Права записи: ✅ Есть");
        }
        catch (Exception ex)
        {
            AddLog($"Права записи: ❌ Нет - {ex.Message}");
        }

        yield return null;
    }

    IEnumerator CheckSQLiteAssemblies()
    {
        AddLog("\n--- ПРОВЕРКА SQLite СБОРОК ---");

        string[] assembliesToCheck = {
            "Mono.Data.Sqlite, Version=4.0.0.0, Culture=neutral, PublicKeyToken=0738eb9f132ed756",
            "Mono.Data.Sqlite, Culture=neutral, PublicKeyToken=0738eb9f132ed756",
            "Mono.Data.Sqlite",
            "System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
            "System.Data"
        };

        foreach (string assemblyName in assembliesToCheck)
        {
            try
            {
                var type = Type.GetType(assemblyName);
                AddLog($"{assemblyName}: {(type != null ? "✅ Найдена" : "❌ Не найдена")}");
            }
            catch (Exception ex)
            {
                AddLog($"{assemblyName}: ❌ Ошибка - {ex.Message}");
            }
        }

        // Проверяем конкретные типы
        string[] typesToCheck = {
            "Mono.Data.Sqlite.SqliteConnection",
            "Mono.Data.Sqlite.SqliteCommand",
            "Mono.Data.Sqlite.SqliteDataReader",
            "System.Data.IDbConnection",
            "System.Data.IDbCommand"
        };

        foreach (string typeName in typesToCheck)
        {
            string fullTypeName = $"{typeName}, Mono.Data.Sqlite";
            try
            {
                var type = Type.GetType(fullTypeName);
                if (type != null)
                {
                    // Пробуем создать экземпляр
                    try
                    {
                        var instance = Activator.CreateInstance(type);
                        AddLog($"{typeName}: ✅ Доступен (можно создать)");
                    }
                    catch
                    {
                        AddLog($"{typeName}: ✅ Найден (но нельзя создать)");
                    }
                }
                else
                {
                    AddLog($"{typeName}: ❌ Не найден");
                }
            }
            catch (Exception ex)
            {
                AddLog($"{typeName}: ❌ Ошибка - {ex.Message}");
            }
        }

        yield return null;
    }

    IEnumerator CheckDatabaseFile()
    {
        AddLog("\n--- ПРОВЕРКА ФАЙЛА БАЗЫ ДАННЫХ ---");

        string dbPath = Path.Combine(Application.streamingAssetsPath, "QuestionDatabase.db");
        AddLog($"Ожидаемый путь: {dbPath}");
        AddLog($"Файл существует: {File.Exists(dbPath)}");

        if (File.Exists(dbPath))
        {
            try
            {
                FileInfo info = new FileInfo(dbPath);
                AddLog($"Размер: {info.Length} байт");
                AddLog($"Создан: {info.CreationTime}");
                AddLog($"Изменен: {info.LastWriteTime}");

                // Проверяем возможность чтения
                byte[] data = File.ReadAllBytes(dbPath);
                AddLog($"Можно прочитать: ✅ {data.Length} байт");

                // Проверяем сигнатуру SQLite
                if (data.Length >= 16)
                {
                    string signature = BitConverter.ToString(data, 0, 16);
                    AddLog($"Сигнатура файла: {signature}");

                    // SQLite signature: "SQLite format 3"
                    if (Encoding.ASCII.GetString(data, 0, 15) == "SQLite format 3")
                        AddLog("Формат файла: ✅ SQLite 3");
                    else
                        AddLog("Формат файла: ❌ Не SQLite 3");
                }
            }
            catch (Exception ex)
            {
                AddLog($"Ошибка чтения файла: {ex.Message}");
            }
        }
        else
        {
            AddLog("Содержимое StreamingAssets:");
            try
            {
                string[] files = Directory.GetFiles(Application.streamingAssetsPath);
                foreach (string file in files)
                {
                    AddLog($"  - {Path.GetFileName(file)}");
                }
            }
            catch (Exception ex)
            {
                AddLog($"Ошибка чтения папки: {ex.Message}");
            }
        }

        yield return null;
    }

    IEnumerator TestAllConnectionStrings()
    {
        AddLog("\n--- ТЕСТ ПОДКЛЮЧЕНИЙ ---");

        string dbPath = Path.Combine(Application.streamingAssetsPath, "QuestionDatabase.db");

        if (!File.Exists(dbPath))
        {
            AddLog("❌ Файл БД не существует, пропускаем тесты подключения");
            yield break;
        }

        string[] connectionStrings = {
            $"Data Source={dbPath}",
            $"Data Source={dbPath};Version=3",
            $"Data Source={dbPath};Version=3;New=True",
            $"Data Source={dbPath};Cache=Shared",
            $"Data Source={dbPath};Cache=Private",
            $"URI=file:{dbPath}",
            $"URI=file:{dbPath}?cache=shared",
            $"{dbPath}"
        };

        bool anySuccess = false;

        foreach (string connString in connectionStrings)
        {
            AddLog($"\nТестируем: {connString}");

            try
            {
                using (IDbConnection dbcon = new SqliteConnection(connString))
                {
                    dbcon.Open();
                    AddLog("  ✅ Подключение успешно!");

                    // Тестируем запросы
                    try
                    {
                        using (IDbCommand cmd = dbcon.CreateCommand())
                        {
                            cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table'";
                            using (IDataReader reader = cmd.ExecuteReader())
                            {
                                int count = 0;
                                while (reader.Read()) count++;
                                AddLog($"  ✅ Таблиц в БД: {count}");
                            }
                        }

                        using (IDbCommand cmd = dbcon.CreateCommand())
                        {
                            cmd.CommandText = "SELECT COUNT(*) FROM Questions";
                            int questionCount = Convert.ToInt32(cmd.ExecuteScalar());
                            AddLog($"  ✅ Вопросов в БД: {questionCount}");
                        }
                    }
                    catch (Exception ex)
                    {
                        AddLog($"  ⚠️ Запросы не работают: {ex.Message}");
                    }

                    dbcon.Close();
                    anySuccess = true;
                }
            }
            catch (Exception ex)
            {
                AddLog($"  ❌ Ошибка: {ex.GetType().Name}");
                AddLog($"     Сообщение: {ex.Message}");

                if (ex.InnerException != null)
                {
                    AddLog($"     Внутренняя ошибка: {ex.InnerException.Message}");
                }
            }

            yield return new WaitForSeconds(0.1f); // Небольшая пауза
        }

        AddLog(anySuccess ? "\n✅ ХОРОШИЕ НОВОСТИ: Есть рабочие подключения!" :
                            "\n❌ ПЛОХИЕ НОВОСТИ: Ни одно подключение не работает");
    }

    IEnumerator TestSpecificIssues()
    {
        AddLog("\n--- ДЕТЕКТОР ПРОБЛЕМ ---");

        // Проблема 1: Отсутствие нативных библиотек
        AddLog("Проверка нативных библиотек...");
        try
        {
            string tempPath = Path.GetTempFileName();
            string testConnString = $"Data Source={tempPath}";

            using (var conn = new SqliteConnection(testConnString))
            {
                conn.Open();
                conn.Close();
            }
            File.Delete(tempPath);
            AddLog("Нативные библиотеки: ✅ Доступны");
        }
        catch (DllNotFoundException)
        {
            AddLog("Нативные библиотеки: ❌ SQLite3.dll не найдена");
        }
        catch (Exception ex) when (ex.Message.Contains("DLL"))
        {
            AddLog("Нативные библиотеки: ❌ Проблема с DLL");
        }
        catch (Exception ex)
        {
            AddLog($"Нативные библиотеки: ⚠️ Другая ошибка - {ex.Message}");
        }

        yield return null;
    }

    void AddLog(string message)
    {
        log.AppendLine(message);
        Debug.Log(message);
    }

    void SaveLogToFile()
    {
        try
        {
            File.WriteAllText(logPath, log.ToString());
            Debug.Log($"Полный лог сохранен: {logPath}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Не удалось сохранить лог: {ex.Message}");
        }
    }

    void DisplayResults()
    {
        Debug.Log("\n" + "=".PadRight(50, '='));
        Debug.Log("ДИАГНОСТИКА ЗАВЕРШЕНА");
        Debug.Log("Проверьте консоль и файл лога для деталей");
        Debug.Log("=".PadRight(50, '='));

        // Показываем кнопку в GUI для удобства
#if UNITY_EDITOR
        UnityEditor.EditorUtility.DisplayDialog(
            "Диагностика завершена",
            $"Проверьте консоль Unity и файл:\n{logPath}",
            "OK");
#endif
    }

    [ContextMenu("Показать лог файл")]
    void ShowLogFile()
    {
        if (File.Exists(logPath))
        {
            Application.OpenURL($"file://{logPath}");
        }
        else
        {
            Debug.Log("Лог файл еще не создан");
        }
    }
}