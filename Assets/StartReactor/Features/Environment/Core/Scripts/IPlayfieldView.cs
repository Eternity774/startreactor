using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
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
        void SetButtonColor(int buttonIndex, Color color);
        void ShowButtonFeedback(int buttonIndex, Color feedbackColor, float duration);
        UniTask FlashAllButtons(Color color, float duration);
        void ClearGrid();
        Transform GridContainer { get; }
    }
}

