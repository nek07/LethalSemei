using UnityEngine;
using System.Collections.Generic;

public class CityGenerator : MonoBehaviour
{
    [SerializeField] private PanelHouseGenerator housePrefab;
    [SerializeField] private int rows = 3;
    [SerializeField] private int columns = 3;
    [SerializeField] private float margin = 10f;

    private void Start()
    {
        GenerateCityGrid();
    }

    private void GenerateCityGrid()
    {
        float currentZ = 0;

        for (int z = 0; z < rows; z++)
        {
            float currentX = 0;
            float maxRowDepth = 0;

            for (int x = 0; x < columns; x++)
            {
                Vector3 position = new Vector3(currentX, 0, currentZ);
                PanelHouseGenerator house = Instantiate(housePrefab, position, Quaternion.identity);

                // Сгенерировать дом
                house.StartGeneration();

                // Получить границы
                Bounds bounds = house.GetHouseBounds();

                float width = bounds.size.x;
                float depth = bounds.size.z;

                // Сдвигаем X позицию для следующего дома
                currentX += width + margin;

                // запоминаем максимальную глубину строки
                if (depth > maxRowDepth)
                    maxRowDepth = depth;
            }

            // Сдвигаем Z позицию для следующего ряда
            currentZ += maxRowDepth + margin;
        }
    }
}