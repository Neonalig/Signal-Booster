using System;
using System.Timers;

namespace SignalBooster.Models; 

// In essence, in Dead Signal, code resets are on a rolling timer; we expect it to be around 4-5 minutes, but we don't know initially.
// When you spot a code reset, you can force the timer to restart, and it will update the duration (i.e. now 4.5m) and it starts again immediately.
// After a few resets we should slowly hone in exactly on the duration of the timer, meaning we're both accurate, and no longer need to restart.

/// <summary> A rolling timer with adjustable duration. </summary>
/// <remarks> The simple implementation is able to adjust its duration based on the observed interval between ticks, however only on the next tick;
///           this means if it adjusts to an interval shorter than the real interval, there is no way to increase it as the next tick occurs too soon and resets the timer. </remarks>
public sealed class SimpleRollingTimer {
    readonly Timer _Timer;
    double         _Duration, _LastTick;
    bool           _Started;
    int            _ResetCount;

    /// <summary> Creates a new <see cref="SimpleRollingTimer"/>. </summary>
    public SimpleRollingTimer() {
        _Timer         =  new();
        _Timer.Elapsed += OnTick;
    }

    /// <summary> Event signature for the <see cref="Tick"/> event. </summary>
    /// <param name="Timer"> The timer that raised the event. </param>
    public delegate void TickHandler( SimpleRollingTimer Timer );

    /// <summary> Raised when the timer ticks. </summary>
    public event TickHandler? Tick;

    /// <summary> Starts the timer with the specified duration. </summary>
    /// <param name="Duration"> The duration in milliseconds. </param>
    public void Start( double Duration ) {
        if (!_Started || _Duration != Duration) {
            _Duration       = Duration;
            _Timer.Interval = Duration;
            _LastTick       = GetCurrentTime();
            _Timer.Start();
            _Started    = true;
            _ResetCount = 0;
        }
    }

    /// <summary> Stops the timer. </summary>
    public void Stop() {
        _Timer.Stop();
        _Started = false;
    }

    /// <summary> Resets the timer and adjusts the duration based on the time elapsed since the last tick. </summary>
    public void Reset() {
        if (!_Started) { return; }

        double Now     = GetCurrentTime();
        double Elapsed = Now - _LastTick;
        _LastTick = Now;

        // Adjust the duration based on the observed interval
        _Duration       = (_Duration * _ResetCount + Elapsed) / (_ResetCount + 1);
        _Timer.Interval = _Duration;
        _ResetCount++;
        _Timer.Start();
    }

    void OnTick( object? Sender, ElapsedEventArgs E ) => Tick?.Invoke(this);

    static double GetCurrentTime() => DateTime.Now.TimeOfDay.TotalMilliseconds;
    
    // Example usage:
    // RollingTimer Timer = new();
    // Timer.Tick += ( Timer ) => Console.WriteLine( $"Timer ticked after {Timer.Duration}ms" );
    // Timer.Start( 1000 ); // Initial guessed duration
}

/// <summary> A rolling timer with adjustable duration. </summary>
/// <remarks> The advanced implementation is able to adjust its duration based on the observed interval between ticks, including increasing as necessary. </remarks>
public sealed class RollingTimer {
    readonly Timer _Timer;
    double         _LastTick, _StartTime = -1;

    /// <summary> Gets the current duration of the timer, in milliseconds. </summary>
    public double Duration { get; private set; }

    /// <summary> Gets the number of times the timer has been reset. </summary>
    public int ResetCount { get; private set; }

    /// <summary> Gets whether the timer is currently running. </summary>
    public bool IsRunning { get; private set; }

    /// <summary> Gets the time elapsed since the last tick, in milliseconds. </summary>
    public double Elapsed => Duration >= 0 || _StartTime >= 0 ? GetCurrentTime() - (_StartTime >= 0 ? _StartTime : _LastTick) : 0;
    
    /// <summary> Gets the time remaining until the next tick, in milliseconds. </summary>
    public double Remaining => Duration >= 0 ? Duration - Elapsed : 0;

    /// <summary> Creates a new <see cref="RollingTimer"/>. </summary>
    public RollingTimer() {
        _Timer         =  new();
        _Timer.Elapsed += OnTick;
        Duration       =  -1;
    }

    /// <summary> Event signature for the <see cref="Tick"/> event. </summary>
    /// <param name="Timer"> The timer that raised the event. </param>
    public delegate void TickHandler( RollingTimer Timer );

    /// <summary> Raised when the timer ticks. </summary>
    public event TickHandler? Tick;

    /// <summary> Starts the timer with the specified duration. </summary>
    /// <param name="Duration"> The duration in milliseconds. </param>
    public void Start( double Duration = -1 ) {
        if (Duration > 0) {
            this.Duration = Duration;
        }

        if (this.Duration > 0) {
            _Timer.Interval = this.Duration;
            _LastTick       = GetCurrentTime();
            _Timer.Start();
            IsRunning = true;
            Console.WriteLine("Started");
        } else {
            // If duration is not set, just mark as running and wait for Reset call
            _StartTime = GetCurrentTime();
            IsRunning  = true;
            Console.WriteLine("No duration set, waiting for Reset call");
        }

        ResetCount = 0;
    }

    /// <summary> Stops the timer. </summary>
    public void Stop() {
        _Timer.Stop();
        IsRunning = false;
        Console.WriteLine("Stopped");
    }

    /// <summary> Resets the timer and adjusts the duration based on the time elapsed since the last tick. </summary>
    public void Reset() {
        if (!IsRunning) {
            Console.WriteLine("Not running, ignoring Reset call");
            return;
        }

        double Now = GetCurrentTime();
        double ElapsedSinceLastTick;
        if (_StartTime >= 0) {
            ElapsedSinceLastTick = Now - _StartTime;
            _StartTime           = -1; // Reset start time
        } else {
            ElapsedSinceLastTick = Now - _LastTick;
        }

        if (Duration <= 0) {
            Duration = ElapsedSinceLastTick;
            Console.WriteLine($"Duration was unknown, set to {Duration}ms");
        } else {
            double RemainingUntilNextTick = Duration - ElapsedSinceLastTick;

            // Adjust the duration based on proximity to last or next tick
            if (ElapsedSinceLastTick < RemainingUntilNextTick) {
                Duration = (Duration * ResetCount + Duration + ElapsedSinceLastTick) / (ResetCount + 1);
                Console.WriteLine($"Adjusted duration to {Duration}ms based on proximity to last tick");
            } else {
                Duration = (Duration * ResetCount + ElapsedSinceLastTick) / (ResetCount + 1);
                Console.WriteLine($"Adjusted duration to {Duration}ms based on proximity to next tick");
            }
        }

        _LastTick       = Now;
        _Timer.Interval = Duration;
        _Timer.Start();
        ResetCount++;
    }

    void OnTick( object? Sender, ElapsedEventArgs E ) {
        _LastTick = GetCurrentTime();
        Tick?.Invoke(this);
    }

    static double GetCurrentTime() => DateTime.Now.TimeOfDay.TotalMilliseconds;

    #region Overrides of Object

    /// <inheritdoc />
    public override string ToString() => $"{{{nameof(Duration)}: {Duration}, {nameof(ResetCount)}: {ResetCount}, {nameof(IsRunning)}: {IsRunning}, {nameof(Elapsed)}: {Elapsed}}}";

    #endregion

    /// <summary> Clears and stops the timer, including resetting all duration data. </summary>
    public void Clear() {
        Duration   = -1;
        _StartTime = -1;
        Stop();
        ResetCount = 0;
        Console.WriteLine("Cleared");
    }

    public void Rollover() {
        // If we haven't yet started, start
        // If we've started, but are currently paused, resume
        // If we've started, and are currently running, reset
        if (Duration <= 0 && !IsRunning) {
            Start();
        } else if (!IsRunning) {
            Start();
        } else {
            Reset();
        }
    }
    
    public enum RolloverAction {
        Start,
        Resume,
        Reset
    }

    public RolloverAction GetRolloverAction() {
        if (Duration <= 0 && !IsRunning) {
            return RolloverAction.Start;
        }
        if (!IsRunning) {
            return RolloverAction.Resume;
        }
        return RolloverAction.Reset;
    }
    
    public void Cancel() {
        // If we are started and running, stop
        // If we are started and paused, or not started, clear
        if (IsRunning) {
            Stop();
        } else {
            Clear();
        }
    }
    
    public enum CancelAction {
        Stop,
        Clear
    }
    
    public CancelAction GetCancelAction() {
        if (IsRunning) {
            return CancelAction.Stop;
        }
        return CancelAction.Clear;
    }
}