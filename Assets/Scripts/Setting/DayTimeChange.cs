using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Settings")]
    public Light directionalLight; // Ссылка на источник света (солнечный свет)
    public Gradient lightColor; // Градиент для цвета света
    public AnimationCurve lightIntensity; // Кривая для изменения интенсивности света
    public float dayDuration = 120f; // Длительность одного дня в секундах

    [Header("Skybox Settings")]
    public Material skyboxMaterial; // Материал неба
    public Gradient skyboxColor; // Градиент для цвета неба

    private float currentTime = 0f; // Текущее время дня

    void Start()
    {
        if (directionalLight == null)
        {
            Debug.LogError("Directional Light is not assigned!");
        }
        if (skyboxMaterial == null)
        {
            Debug.LogError("Skybox Material is not assigned!");
        }
    }

    void Update()
    {
        // Обновление времени
        currentTime += Time.deltaTime / dayDuration;
        if (currentTime > 1f)
        {
            currentTime = 0f; // Сбрасываем цикл дня
        }

        // Обновление света
        UpdateLighting();

        // Обновление Skybox
        UpdateSkybox();
    }

    private void UpdateLighting()
    {
        if (directionalLight != null)
        {
            // Поворачиваем свет
            directionalLight.transform.rotation = Quaternion.Euler((currentTime * 360f) - 90f, 170f, 0f);

            // Изменяем цвет света
            directionalLight.color = lightColor.Evaluate(currentTime);

            // Изменяем интенсивность света
            directionalLight.intensity = lightIntensity.Evaluate(currentTime);
        }
    }

    private void UpdateSkybox()
    {
        if (skyboxMaterial != null)
        {
            // Меняем цвет Skybox
            skyboxMaterial.SetColor("_Tint", skyboxColor.Evaluate(currentTime));
        }
    }
}
