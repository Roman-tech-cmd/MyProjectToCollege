using System.Collections.Generic;
using UnityEngine;

public class RandomPosButton : MonoBehaviour
{
    [SerializeField] private List<GameObject> pointsButtons; // Точки-позиции
    [SerializeField] private List<GameObject> answerButtons;  // Кнопки с ответами

    private List<GameObject> availablePoints; // Копия точек (будем удалять при назначении)
    private List<GameObject> availableButtons; // Копия кнопок

    void Start()
    {
        RandomPos();
    }

    public void RandomPos()
    {
        // Копируем списки
        availablePoints = new List<GameObject>(pointsButtons);
        availableButtons = new List<GameObject>(answerButtons);

        // Проверка на количество
        if (availableButtons.Count != availablePoints.Count)
        {
            Debug.LogError($"[RandomPosButton] Количество кнопок ({availableButtons.Count}) не совпадает с количеством точек ({availablePoints.Count})");
            return;
        }

        // Перемещаем каждую кнопку в случайную точку
        foreach (GameObject button in availableButtons)
        {
            int pointIndex = Random.Range(0, availablePoints.Count);
            GameObject targetPoint = availablePoints[pointIndex];

            button.transform.SetParent(targetPoint.transform);
            button.transform.localPosition = Vector3.zero;
            button.transform.localRotation = Quaternion.identity;

            // Убираем использованную точку
            availablePoints.RemoveAt(pointIndex);
        }
    }
}
