using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseCanvas : MonoBehaviour
{
    [SerializeField] private Button _resumeBtn;
    [SerializeField] private Button _backToMainBtn;

    private GamePlayManager _gamePlayManager;

    private void Start()
    {
        ReferenceHolder.Request<GamePlayManager>(Initialize);
    }   

    private void Initialize(GamePlayManager gamePlayManager)
    {
        _gamePlayManager = gamePlayManager;
        _resumeBtn.onClick.AddListener(Resume);
        _backToMainBtn.onClick.AddListener(BackToMainMenu);
    }

    private void BackToMainMenu()
    {
        ClosePauseCanvas();
    }

    private void ClosePauseCanvas()
    {
        gameObject.SetActive(false);
    }

    private void Resume()
    {
        ClosePauseCanvas();
        _gamePlayManager.ResumeGame();
    }
}
