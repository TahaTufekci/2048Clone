using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region Canvas Groups
    [SerializeField] private CanvasGroup losePanel;
    [SerializeField] private CanvasGroup winPanel;
    #endregion
    
    [SerializeField] private Image mainMask;

    public void FadeInLosePanel()
    {
        losePanel.gameObject.SetActive(true);
        losePanel.gameObject.transform.DOScale(Vector3.one, 0.5f).From(Vector3.zero);
    }

    public void FadeInWinPanel()
    {
        winPanel.gameObject.SetActive(true);
        winPanel.gameObject.transform.DOScale(Vector3.one, 0.5f).From(Vector3.zero);
        
    }

    public void SetMaskState(Image mask, bool isActive, Action onClickAction = null)
    {
        if (isActive)
        {
            SetMaskClickAction(mask, onClickAction);
            mask.gameObject.SetActive(true);
        }
        else
        {
            mask.gameObject.SetActive(false);
        }
    }
    private void SetMaskClickAction(Image mask, Action action)
    {
        EventTrigger trigger = mask.GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        trigger.triggers.Clear();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((eventData) => { action?.Invoke(); });
        trigger.triggers.Add(entry);
    }
    
    public void ControlPanels(GameState gameState)
    {
        if (gameState.HasFlag(GameState.Lose))
        {
            mainMask.DOFade(0.5f, 0.5f).SetDelay(0.8f).From(0f);
            SetMaskState(mainMask, true);
            Sequence sequence = DOTween.Sequence();
            sequence.PrependInterval(0.8f).OnStepComplete(() => FadeInLosePanel());
        }
        else if (gameState.HasFlag(GameState.Win))
        {
            mainMask.DOFade(0.5f, 0.5f).SetDelay(0.8f).From(0f);
            SetMaskState(mainMask, true);
            Sequence sequence = DOTween.Sequence();
            sequence.PrependInterval(0.8f).OnStepComplete(() => FadeInWinPanel());

        }
    }
    
    private void OnEnable()
    {
        GameManager.OnGameStateChanged += ControlPanels;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= ControlPanels;
    }
}
