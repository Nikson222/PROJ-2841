using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts._Infrastructure;
using _Scripts._Infrastructure.Services;
using _Scripts.Game.Services;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ScoreDisplayer : MonoBehaviour
{
    [SerializeField] private Text _text;
    [SerializeField] private RectTransform bonusPopupPosition;
    
    private int lastScore = 0;
    
    private ScoreCounter _scoreCounter;
    private PopupTextService _popupTextService;
    
    [Inject]
    private void Construct(ScoreCounter scoreCounter, PopupTextService popupTextService)
    {
        _scoreCounter = scoreCounter;
        _popupTextService = popupTextService;
        
        _scoreCounter.OnScoreChanged += OnScoreChanged;
    }

    private void Start()
    {
        OnScoreChanged(_scoreCounter.Score);
    }

    private void OnScoreChanged(int score, bool isBonus = false)
    {
        if (isBonus)
        {
            ShowBonusPopup(score-lastScore);
        }

        lastScore = score;
        
        _text.text = score.ToString();
    }

    private void ShowBonusPopup(int score)
    {
        var popupText = "+" + score.ToString();

        _popupTextService.ShowPopupAs(popupText, bonusPopupPosition, Color.white, 0.3f, 0.3f);
    }

    private void OnDestroy()
    {
        _scoreCounter.OnScoreChanged -= OnScoreChanged;
    } 
}
