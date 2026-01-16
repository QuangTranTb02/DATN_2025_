using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum WeatherState
{
    Clear,
    Rain,
    HeavyRain,
    RainAndThunder
}
public class WeatherManager : TimeAgent
{
    [Range(0f,1f)][SerializeField] float chanceToChangeWeather = 0.02f;
    WeatherState currentweatherState = WeatherState.Clear;
    [SerializeField] ParticleSystem rainObject;
    [SerializeField] ParticleSystem heavyRainObject;
    [SerializeField] ParticleSystem rainAndThunderObject;

    private void Start()
    {
        Init();
        OnTimeTick += RandomWeatherChangeCheck;
        UpdateWeather();
    }
    public void RandomWeatherChangeCheck(DayTimeController dayTimeController)
    {
        if(Random.value <= chanceToChangeWeather)
        {
            RandonWeatherChange();
        }
    }

    private void RandonWeatherChange()
    {
        WeatherState newWeatherState = (WeatherState)Random.Range(0, Enum.GetNames(typeof(WeatherState)).Length);
        ChageWeather(newWeatherState);
    }

    private void ChageWeather(WeatherState newWeatherState)
    {
        currentweatherState = newWeatherState;
        UpdateWeather();
        Debug.Log(currentweatherState);
    }
    private void UpdateWeather()
    {
        switch (currentweatherState)
        {
            case WeatherState.Clear:
                rainObject.gameObject.SetActive(false);
                heavyRainObject.gameObject.SetActive(false);
                rainAndThunderObject.gameObject.SetActive(false);
                break;
            case WeatherState.Rain:
                rainObject.gameObject.SetActive(true);
                heavyRainObject.gameObject.SetActive(false);
                rainAndThunderObject.gameObject.SetActive(false);
                break;
            case WeatherState.HeavyRain:
                rainObject.gameObject.SetActive(false);
                heavyRainObject.gameObject.SetActive(true);
                rainAndThunderObject.gameObject.SetActive(false);
                break;
            case WeatherState.RainAndThunder:
                rainObject.gameObject.SetActive(false);
                heavyRainObject.gameObject.SetActive(false);
                rainAndThunderObject.gameObject.SetActive(true);
                break;
        }
    }
}
