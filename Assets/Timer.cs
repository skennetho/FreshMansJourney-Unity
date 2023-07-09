using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    public UnityEvent OnTimeEnd = new();
    public UnityEvent<float, float> OnTimeUpdate = new();

    [SerializeField] private float _maxSeconds = 30;

    private float _currentSecond = 600; //임의의 큰수

    private bool _isPaused = false;

    public float CurrentSec => _currentSecond;
    public float MaxSec => _maxSeconds;

    // Update is called once per frame
    void Update()
    {
        if(_isPaused) { return; }

        _currentSecond -= Time.deltaTime;
        OnTimeUpdate.Invoke(_currentSecond, _maxSeconds);

        if(_currentSecond <= 0)
        {
            Pause();
            OnTimeEnd.Invoke();
        }
    }

    public void Reset(float maxSeconds)
    {
        _maxSeconds = maxSeconds;
        Reset();
    }

    public void Reset()
    {
        _currentSecond = _maxSeconds;
        OnTimeUpdate.Invoke(_currentSecond, _maxSeconds);

        Resume();
    }

    public void Pause()
    {
        _isPaused = true;
    }

    public void Resume()
    {
        _isPaused = false;
    }
}
