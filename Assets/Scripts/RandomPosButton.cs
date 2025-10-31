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
        // Создаём копии списков, чтобы безопасно удалять
        availablePoints = new List<GameObject>(pointsButtons);
        availableButtons = new List<GameObject>(answerButtons);

        // Проверка на количество
        if (availableButtons.Count != availablePoints.Count)
        {
            Debug.LogError($"[RandomPosButton] Количество кнопок ({availableButtons.Count}) не совпадает с количеством точек ({availablePoints.Count})");
            return;
        }

        // Перемешиваем кнопки по точкам
        while (availableButtons.Count > 0)
        {
            // Берём случайную кнопку
            int buttonIndex = Random.Range(0, availableButtons.Count);
            GameObject selectedButton = availableButtons[buttonIndex];

            // Берём случайную точку
            int pointIndex = Random.Range(0, availablePoints.Count);
            GameObject targetPoint = availablePoints[pointIndex];

            // Перемещаем кнопку к точке
            selectedButton.transform.SetParent(targetPoint.transform);
            selectedButton.transform.localPosition = Vector3.zero;
            selectedButton.transform.localRotation = Quaternion.identity;

            // Удаляем из списков, чтобы не назначить дважды
            availableButtons.RemoveAt(buttonIndex);
            availablePoints.RemoveAt(pointIndex);
        }
    }
}
