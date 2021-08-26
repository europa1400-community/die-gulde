using System;
using System.Collections;
using UnityEngine;

public class MonoRoutine
{
    Func<IEnumerator> Enumerator { get; }
    MonoBehaviour Behaviour { get; }
    Coroutine Coroutine { get; set; }

    WaitForCompletion waitForCompletion;
    public WaitForCompletion WaitForCompletion
    {
        get
        {
            if (waitForCompletion != null) return waitForCompletion;

            waitForCompletion = new WaitForCompletion(this);
            Start();

            return waitForCompletion;
        }
    }

    bool isRunning;
    public bool IsRunning
    {
        get => isRunning;
        private set
        {
            if (value) Started?.Invoke(Behaviour, EventArgs.Empty);
            if (!value) isPaused = false;
            isRunning = value;
        }
    }

    bool isPaused;
    public bool IsPaused
    {
        get => isPaused;
        set
        {
            if (value) Paused?.Invoke(Behaviour, EventArgs.Empty);
            else Unpaused?.Invoke(Behaviour, EventArgs.Empty);
            isPaused = value;
        }
    }

    public event EventHandler<MonoRoutineEventArgs> Stopped;
    public event EventHandler Paused;
    public event EventHandler Unpaused;
    public event EventHandler Started;

    public MonoRoutine(Func<IEnumerator> enumerator, MonoBehaviour behaviour = null)
    {
        Enumerator = enumerator;
        Behaviour = behaviour ? behaviour : MonoRoutineComponent.Instance;
    }

    public void Start()
    {
        if (IsRunning) return;
        IsRunning = true;
        Coroutine = Behaviour.StartCoroutine(Wrapper());
    }
    public void Stop()
    {
        if (!IsRunning) return;
        IsRunning = false;
        if (Coroutine != null) Behaviour.StopCoroutine(Coroutine);
        Stopped?.Invoke(Behaviour, new MonoRoutineEventArgs(true));
    }

    public void Restart()
    {
        Stop();
        Start();
    }

    public void Pause()
    {
        if (!IsRunning) return;
        IsPaused = true;
    }

    public void Unpause()
    {
        if (!IsRunning &! IsPaused) return;
        IsPaused = false;
    }

    public void TogglePause()
    {
        if (!IsRunning) return;
        IsPaused = !IsPaused;
    }

    IEnumerator Wrapper()
    {
        var enumerator = Enumerator?.Invoke();

        while (IsRunning)
        {
            if (IsPaused) yield return null;
            else
            {
                if (enumerator != null && enumerator.MoveNext())
                {
                    yield return enumerator.Current;
                }
                else
                {
                    IsRunning = false;
                    Stopped?.Invoke(Behaviour, new MonoRoutineEventArgs(false));
                    yield break;
                }
            }
        }
    }
}

public class MonoRoutine<TReturnType>
{
    Func<IEnumerator> Enumerator { get; }
    MonoBehaviour Behaviour { get; }
    Coroutine Coroutine { get; set; }

    public TReturnType ReturnValue { get; private set; }

    WaitForCompletion<TReturnType> waitForCompletion;
    public WaitForCompletion<TReturnType> WaitForCompletion
    {
        get
        {
            if (waitForCompletion != null) return waitForCompletion;

            waitForCompletion = new WaitForCompletion<TReturnType>(this);
            Start();

            return waitForCompletion;
        }
    }

    bool isRunning;
    public bool IsRunning
    {
        get => isRunning;
        private set
        {
            if (value) Started?.Invoke(Behaviour, EventArgs.Empty);
            if (!value) isPaused = false;
            isRunning = value;
        }
    }

    bool isPaused;
    public bool IsPaused
    {
        get => isPaused;
        set
        {
            if (value) Paused?.Invoke(Behaviour, EventArgs.Empty);
            else Unpaused?.Invoke(Behaviour, EventArgs.Empty);
            isPaused = value;
        }
    }

    public event EventHandler<MonoRoutineEventArgs<TReturnType>> Stopped;
    public event EventHandler Paused;
    public event EventHandler Unpaused;
    public event EventHandler Started;

    public MonoRoutine(Func<IEnumerator> enumerator, MonoBehaviour behaviour = null)
    {
        Enumerator = enumerator;
        Behaviour = behaviour ? behaviour : MonoRoutineComponent.Instance;
    }

    public void Start()
    {
        if (IsRunning) return;
        IsRunning = true;
        Coroutine = Behaviour.StartCoroutine(Wrapper());
    }

    public void Stop()
    {
        if (!IsRunning) return;
        IsRunning = false;
        if (Coroutine != null) Behaviour.StopCoroutine(Coroutine);
        Stopped?.Invoke(Behaviour, new MonoRoutineEventArgs<TReturnType>(true, default(TReturnType)));
    }

    public void Restart()
    {
        Stop();
        Start();
    }

    public void Pause()
    {
        if (!IsRunning) return;
        IsPaused = true;
    }

    public void Unpause()
    {
        if (!IsRunning &! IsPaused) return;
        IsPaused = false;
    }

    public void TogglePause()
    {
        if (!IsRunning) return;
        IsPaused = !IsPaused;
    }

    IEnumerator Wrapper()
    {
        var enumerator = Enumerator?.Invoke();

        while (IsRunning)
        {
            if (IsPaused) yield return null;
            else
            {
                if (enumerator != null && enumerator.MoveNext())
                {
                    var current = enumerator.Current;
                    yield return current;

                    if (current is TReturnType currentReturn) ReturnValue = currentReturn;
                }
                else
                {
                    IsRunning = false;
                    Stopped?.Invoke(Behaviour, new MonoRoutineEventArgs<TReturnType>(false, ReturnValue));
                    yield break;
                }
            }
        }
    }
}

public class MonoRoutineEventArgs : EventArgs
{
    public bool IsForced { get; }

    public MonoRoutineEventArgs(bool isForced)
    {
        IsForced = isForced;
    }
}

public class MonoRoutineEventArgs<TReturnType> : EventArgs
{
    public bool IsForced { get; }
    public TReturnType ReturnValue { get; }

    public MonoRoutineEventArgs(bool isForced, TReturnType returnValue)
    {
        IsForced = isForced;
        ReturnValue = returnValue;
    }
}

public class WaitForCompletion : CustomYieldInstruction
{
    MonoRoutine Routine { get; }

    public override bool keepWaiting => Routine.IsRunning;

    public WaitForCompletion(MonoRoutine routine)
    {
        Routine = routine;
    }
}

public class WaitForCompletion<TReturnType> : CustomYieldInstruction
{
    MonoRoutine<TReturnType> Routine { get; }

    public override bool keepWaiting => Routine.IsRunning;

    public WaitForCompletion(MonoRoutine<TReturnType> routine)
    {
        Routine = routine;
    }
}

public class MonoRoutineComponent : MonoBehaviour
{
    static MonoRoutineComponent instance;
    public static MonoRoutineComponent Instance
    {
        get
        {
            if (instance) return instance;

            var monoRoutineObject = new GameObject("routine");
            var monoRoutineComponent = monoRoutineObject.AddComponent<MonoRoutineComponent>();

            DontDestroyOnLoad(monoRoutineObject);
            instance = monoRoutineComponent;

            return instance;
        }
    }
}