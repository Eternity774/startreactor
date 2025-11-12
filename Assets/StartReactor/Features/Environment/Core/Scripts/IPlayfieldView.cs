using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace StartReactor.Features.Environment
{
    public interface IPlayfieldView
    {
        event Action<int> OnButtonClicked;
        
        Transform GridContainer { get; }

        void InitializeButtons(List<PlayfieldButton> buttons);
        void SetButtonColor(int buttonIndex, Color color);
        void SetAllButtonsColor(Color color);
        void ResetAllButtonsToDefaultColor();
        void ShowButtonFeedback(int buttonIndex, Color feedbackColor, float duration);
        void ClearGrid();
    }
}

