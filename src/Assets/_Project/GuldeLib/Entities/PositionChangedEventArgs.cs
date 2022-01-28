using System;
using UnityEngine;

namespace GuldeLib.Entities
{
    public class PositionChangedEventArgs : EventArgs
    {
        public Vector2 Position { get; set; }

        public PositionChangedEventArgs(Vector2 position) => Position = position;
    }
}