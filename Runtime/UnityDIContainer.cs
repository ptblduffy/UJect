using System;
using UnityEngine;

namespace UJect
{
    public abstract class UnityDiContainer : MonoBehaviour
    {
        protected readonly DiContainer DiContainer = new DiContainer();

        protected abstract void Awake();

        private void OnDestroy()
        {
            DiContainer.Dispose();
        }
    }
}