using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Setting
{
    public class TimeController : MonoBehaviour
    {
        [Header("Time Settings")] [SerializeField]
        private float timeMultiplier;

        [SerializeField] private float startHour;
        [SerializeField] private TextMeshProUGUI timeText;

        [Header("Sun and Moon Settings")] [SerializeField]
        private Light sunLight;

        [SerializeField] private float sunriseHour;
        [SerializeField] private float sunsetHour;
        [SerializeField] private Color dayAmbientLight;
        [SerializeField] private Color nightAmbientLight;
        [SerializeField] private AnimationCurve lightChangeCurve;
        [SerializeField] private float maxSunLightIntensity;
        [SerializeField] private Light moonLight;
        [SerializeField] private float maxMoonLightIntensity;

        [Header("Weather Settings")] [SerializeField]
        private ParticleSystem rainEffect;

        [SerializeField] private GameObject windZone;

        private DateTime currentTime;
        private TimeSpan sunriseTime;
        private TimeSpan sunsetTime;

        private bool isRaining = false;
        private bool isWindy = false;

        void Start()
        {
            currentTime = DateTime.Now.Date + TimeSpan.FromHours(startHour);
            sunriseTime = TimeSpan.FromHours(sunriseHour);
            sunsetTime = TimeSpan.FromHours(sunsetHour);
        }

        void Update()
        {
            UpdateTimeOfDay();
            RotateSun();
            UpdateLightSettings();
            HandleWeatherEffects();
        }

        private void UpdateTimeOfDay()
        {
            currentTime = currentTime.AddSeconds(Time.deltaTime * timeMultiplier);

            if (timeText != null)
            {
                timeText.text = currentTime.ToString("HH:mm");
            }
        }

        private void RotateSun()
        {
            float sunLightRotation;

            if (currentTime.TimeOfDay > sunriseTime && currentTime.TimeOfDay < sunsetTime)
            {
                TimeSpan sunriseToSunsetDuration = CalculateTimeDifference(sunriseTime, sunsetTime);
                TimeSpan timeSinceSunrise = CalculateTimeDifference(sunriseTime, currentTime.TimeOfDay);

                double percentage = timeSinceSunrise.TotalMinutes / sunriseToSunsetDuration.TotalMinutes;

                sunLightRotation = Mathf.Lerp(0, 180, (float)percentage);
            }
            else
            {
                TimeSpan sunsetToSunriseDuration = CalculateTimeDifference(sunsetTime, sunriseTime);
                TimeSpan timeSinceSunset = CalculateTimeDifference(sunsetTime, currentTime.TimeOfDay);

                double percentage = timeSinceSunset.TotalMinutes / sunsetToSunriseDuration.TotalMinutes;

                sunLightRotation = Mathf.Lerp(180, 360, (float)percentage);
            }

            sunLight.transform.rotation = Quaternion.AngleAxis(sunLightRotation, Vector3.right);
        }

        private void UpdateLightSettings()
        {
            float dotProduct = Vector3.Dot(sunLight.transform.forward, Vector3.down);
            sunLight.intensity = Mathf.Lerp(0, maxSunLightIntensity, lightChangeCurve.Evaluate(dotProduct));
            moonLight.intensity = Mathf.Lerp(maxMoonLightIntensity, 0, lightChangeCurve.Evaluate(dotProduct));
            RenderSettings.ambientLight =
                Color.Lerp(nightAmbientLight, dayAmbientLight, lightChangeCurve.Evaluate(dotProduct));
        }

       

        private void ToggleRain()
        {
            isRaining = !isRaining;
            Debug.Log(isRaining ? "Дождь начался" : "Дождь закончился");
        }

        private void ToggleWind()
        {
            isWindy = !isWindy;
            Debug.Log(isWindy ? "Ветер усилился" : "Ветер стих");
        }

        private TimeSpan CalculateTimeDifference(TimeSpan fromTime, TimeSpan toTime)
        {
            TimeSpan difference = toTime - fromTime;

            if (difference.TotalSeconds < 0)
            {
                difference += TimeSpan.FromHours(24);
            }

            return difference;
        }
        [SerializeField] private Transform player;  // Ссылка на игрока

        [SerializeField] private Vector3 rainOffset = new Vector3(0, 10, 0); // Смещение дождя над игроком

        private void HandleWeatherEffects()
        {
            if (Input.GetKeyDown(KeyCode.R)) ToggleRain();
            if (Input.GetKeyDown(KeyCode.W)) ToggleWind();

            // Дождь
            if (isRaining)
            {
                if (!rainEffect.isPlaying) rainEffect.Play();
        
                // Привязка дождя к позиции игрока
                if (player != null)
                {
                    rainEffect.transform.position = player.position + rainOffset;
                }
            }
            else if (rainEffect.isPlaying)
            {
                rainEffect.Stop();
            }

            // Ветер
            if (windZone != null)
            {
                windZone.SetActive(isWindy);
            }
        }
    }
    
}
