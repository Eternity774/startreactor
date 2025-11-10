using System;
using System.Collections.Generic;
using UnityEngine;

namespace StartReactor.Features.Environment
{
    /// <summary>
    /// Interface for playfield view - handles visual representation of the gameplay grid.
    /// </summary>
    public interface IPlayfieldView
    {
        event Action<int> OnButtonClicked;
        
        void InitializeButtons(List<PlayfieldButton> buttons);
        void SetButtonHighlight(int buttonIndex, bool highlight, float intensity);
        void SetButtonColor(int buttonIndex, Color color);
        void SetButtonInteractable(int buttonIndex, bool interactable);
        void SetAllButtonsInteractable(bool interactable);
        void ShowButtonFeedback(int buttonIndex, Color feedbackColor, float duration);
        void ClearGrid();
        Transform GridContainer { get; }
    }
}

