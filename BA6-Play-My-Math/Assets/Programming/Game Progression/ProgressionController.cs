using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Programming.GameProgression
{
    public class ProgressionController : MonoBehaviour
    {
        private void Start()
        {
            DontDestroyOnLoad(this);
        }

        public void WinRound()
        {
            //TODO: open rewardUI (or switch scene)
        }

        public void LooseRound()
        {
            //TODO: open menu to start new Round
        }

        public void StartNewRound()
        {
            //TODO: close the rewardUI (or switch scene)
        }
    }
}
