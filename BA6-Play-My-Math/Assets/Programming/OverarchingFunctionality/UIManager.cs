using System;
using Programming.ExtensionMethods;
using UnityEngine;

namespace Programming.OverarchingFunctionality
{
    public class UIManager : MonoBehaviour
    {
        public UIManager Instance; 
        public UIInterface ActiveUI;

        private void Awake()
        {
            this.MakeSingleton(ref Instance);
        }

        public void SetActiveUI(UIInterface ui)
        {
            if (ActiveUI is not null)
            {
                ActiveUI.Disable();
            }

            ActiveUI = ui;
            ui.Enable();
        }
    }
}
