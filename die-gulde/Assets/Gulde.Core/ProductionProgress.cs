using System;
using UnityEngine;

namespace Gulde.Core
{
    [Serializable]
    public class ProductionProgress
    {
        [SerializeField]
        public Recipe recipe;
        
        /// <summary>
        /// The progress of the production of the recipe in percent (0.0 - 1.0)
        /// </summary>
        [SerializeField]
        public float progress;
        
        public void Reset()
        {
            progress = 0.0f;
        }
    }
}