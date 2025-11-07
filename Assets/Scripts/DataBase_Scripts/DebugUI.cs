using UnityEngine;
using TMPro;
using System.Collections;

public class DebugUI : MonoBehaviour
{
    public TileData tileData;
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI[] answerTexts;

    void Start()
    {
        StartCoroutine(DebugCoroutine());
    }

    IEnumerator DebugCoroutine()
    {
        Debug.Log("=== НАЧАЛО ДИАГНОСТИКИ ===");

        // Ждем немного
        yield return new WaitForSeconds(1f);

        // Проверяем ссылки
        Debug.Log($"TileData: {tileData != null}");
        Debug.Log($"QuestionText: {questionText != null}");
        Debug.Log($"AnswerTexts: {answerTexts != null}");
        if (answerTexts != null)
        {
            for (int i = 0; i < answerTexts.Length; i++)
            {
                Debug.Log($"AnswerText[{i}]: {answerTexts[i] != null}");
            }
        }

        // Проверяем данные
        if (tileData != null)
        {
            Debug.Log($"UseDatabase: {tileData.useDatabase}");
            Debug.Log($"Question: '{tileData.question}'");
            Debug.Log($"AnswerOptions: {tileData.answerOptions != null}");
            if (tileData.answerOptions != null)
            {
                Debug.Log($"AnswerOptions Length: {tileData.answerOptions.Length}");
                for (int i = 0; i < tileData.answerOptions.Length; i++)
                {
                    Debug.Log($"Answer[{i}]: '{tileData.answerOptions[i]}'");
                }
            }
        }

        // Пробуем обновить UI вручную
        UpdateUI();

        Debug.Log("=== КОНЕЦ ДИАГНОСТИКИ ===");
    }

    void UpdateUI()
    {
        if (tileData == null || questionText == null) return;

        // Обновляем вопрос
        questionText.text = string.IsNullOrEmpty(tileData.question) ? "ВОПРОС НЕ ЗАГРУЖЕН" : tileData.question;
        Debug.Log($"UI Question Text set to: '{questionText.text}'");

        // Обновляем ответы
        if (answerTexts != null && tileData.answerOptions != null)
        {
            for (int i = 0; i < answerTexts.Length; i++)
            {
                if (answerTexts[i] != null)
                {
                    if (i < tileData.answerOptions.Length)
                    {
                        answerTexts[i].text = tileData.answerOptions[i];
                        answerTexts[i].gameObject.SetActive(true);
                        Debug.Log($"UI Answer[{i}] set to: '{answerTexts[i].text}'");
                    }
                    else
                    {
                        answerTexts[i].gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    // Для теста - обновление по кнопке
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("=== ПРИНУДИТЕЛЬНОЕ ОБНОВЛЕНИЕ ===");
            UpdateUI();
        }
    }
}