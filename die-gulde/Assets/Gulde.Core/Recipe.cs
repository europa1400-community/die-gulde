﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gulde.Core
{
    [CreateAssetMenu(fileName = "Recipe", menuName = "Recipe")]
    public class Recipe : ScriptableObject
    {
        [SerializeField]
        public string friendlyName;

        /// <summary>
        /// The resources consumed by the recipe
        /// </summary>
        [SerializeField]
        public List<Item> ingredients = new();

        /// <summary>
        /// The item produced the recipe
        /// </summary>
        [SerializeField]
        public Item product;

        /// <summary>
        /// The time it takes to produce the product in seconds
        /// </summary>
        [SerializeField]
        public float time;
        
        public Dictionary<Item, int> GetIngredientDictionary() => ingredients
            .GroupBy(e => e)
            .ToDictionary(e => e.Key, e => e.Count());
    }
}