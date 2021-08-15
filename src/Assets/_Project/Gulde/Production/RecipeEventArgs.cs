using System;

namespace Gulde.Production
{
    public class RecipeEventArgs : EventArgs
    {
        public RecipeEventArgs(Recipe recipe)
        {
            Recipe = recipe;
        }

        public Recipe Recipe { get; }
    }
}