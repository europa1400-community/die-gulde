using System;
using GuldeLib.Timing;
using Sirenix.OdinInspector;

namespace GuldeLib.Society
{
    public class PersonComponent : SerializedMonoBehaviour
    {
        public int Age { get; set; }

        public event EventHandler<AgeChangedEventArgs> AgeChanged;

        public event EventHandler<InitializedEventArgs> Initialized;

        void Start()
        {
            Locator.Time.YearTicked += OnYearTicked;

            Initialized?.Invoke(this, new InitializedEventArgs(Age));
        }

        void OnYearTicked(object sender, TimeComponent.TimeEventArgs e)
        {
            Age += 1;
            AgeChanged?.Invoke(this, new AgeChangedEventArgs(Age));
        }

        public class AgeChangedEventArgs : EventArgs
        {
            public int Value { get; }

            public AgeChangedEventArgs(int value) => Value = value;
        }

        public class InitializedEventArgs : EventArgs
        {
            public int Age { get; }

            public InitializedEventArgs(int age) =>
                Age = age;
        }
    }
}