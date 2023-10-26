using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Timers;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SignalBooster.Models;

namespace SignalBooster.ViewModels;

public class RollingTimerViewModel : ViewModelBase, IDisposable {
    public int ResetCount => _RollingTimer.ResetCount;

    public bool IsRunning => _RollingTimer.IsRunning;

    public double Elapsed   => _RollingTimer.Elapsed;
    public double Remaining => _RollingTimer.Remaining;
    public double Duration  => _RollingTimer.Duration;

    public double PercentRemaining => _RollingTimer.Duration <= 0 ? 0 : _RollingTimer.Remaining / _RollingTimer.Duration;

    public string ElapsedText   => TimeSpan.FromMilliseconds(Elapsed).ToString(@"mm\:ss\.fff");
    public string RemainingText => TimeSpan.FromMilliseconds(Remaining).ToString(@"mm\:ss\.fff");
    public string DurationText  => TimeSpan.FromMilliseconds(Duration).ToString(@"mm\:ss\.fff");

    public string PercentRemainingText {
        get {
            // Will just display remaining time, but like "1m 30s", "1m", "59s", "~now"
            double Remaining = _RollingTimer.Remaining;
            switch (Remaining) {
                case 0:      return string.Empty;
                case < 1000: return "~now";
            }

            TimeSpan      Rem = TimeSpan.FromMilliseconds(Remaining);
            StringBuilder SB  = new();

            static void Append( StringBuilder SB, int Value, string Unit ) {
                if (Value == 0) { return; }

                if (SB.Length > 0) { SB.Append(' '); }

                SB.Append(Value);
                SB.Append(Unit);
            }

            Append(SB, Rem.Hours, "h");
            Append(SB, Rem.Minutes, "m");
            Append(SB, Rem.Seconds, "s");
            return SB.ToString();
        }
    }

    public ICommand StartCommand { get; }
    public ICommand StopCommand  { get; }
    public ICommand ResetCommand { get; }
    public ICommand ClearCommand { get; }
    
    public ICommand StartOrResetCommand { get; }
    public ICommand StopOrClearCommand  { get; }
    
    public ICommand RolloverCommand { get; }
    public ICommand CancelCommand  { get; }
    
    public RollingTimer.RolloverAction RolloverAction => _RollingTimer.GetRolloverAction();
    public RollingTimer.CancelAction   CancelAction   => _RollingTimer.GetCancelAction();

    readonly RollingTimer _RollingTimer;
    readonly Timer        _RefreshTimer;
    
    public RollingTimerViewModel() {
        _RollingTimer      =  new();
        _RollingTimer.Tick += OnTick;

        StartCommand = new RelayCommand(Start);
        StopCommand  = new RelayCommand(Stop);
        ResetCommand = new RelayCommand(Reset);
        ClearCommand = new RelayCommand(Clear);
        
        StartOrResetCommand = new RelayCommand(StartOrReset);
        StopOrClearCommand  = new RelayCommand(StopOrClear);
        
        RolloverCommand = new RelayCommand(Rollover);
        CancelCommand   = new RelayCommand(Cancel);

        _RefreshTimer         =  new(10);
        _RefreshTimer.Elapsed += ( _, _ ) => { RefreshTimes(); };

        FullRefresh();
    }
    
    public void StartOrReset() {
        if (IsRunning) { Reset(); }
        else           { Start(); }
    }
    
    public void StopOrClear() {
        if (IsRunning) { Stop(); }
        else           { Clear(); }
    }

    void RefreshTimes() {
        OnPropertyChanged(nameof(Elapsed));
        OnPropertyChanged(nameof(ElapsedText));

        OnPropertyChanged(nameof(Remaining));
        OnPropertyChanged(nameof(RemainingText));

        OnPropertyChanged(nameof(Duration));
        OnPropertyChanged(nameof(DurationText));

        OnPropertyChanged(nameof(PercentRemaining));
        OnPropertyChanged(nameof(PercentRemainingText));
        // Console.WriteLine($"Percent remaining: {PercentRemaining} ({PercentRemainingText})");
    }

    void FullRefresh() {
        RefreshTimes();
        OnPropertyChanged(nameof(ResetCount));
        RefreshRunningState();
    }

    void RefreshRunningState() {
        OnPropertyChanged(nameof(IsRunning));
        OnPropertyChanged(nameof(RolloverAction));
        OnPropertyChanged(nameof(CancelAction));
    }
    
    public void Start() {
        Console.WriteLine($"Started {_RollingTimer}");

        _RollingTimer.Start(Duration);
        RefreshRunningState();

        _RefreshTimer.Start();
        RefreshTimes();
    }

    public void Stop() {
        Console.WriteLine($"Stopped {_RollingTimer}");

        _RollingTimer.Stop();
        RefreshRunningState();

        _RefreshTimer.Stop();
        RefreshTimes();
    }

    public void Reset() {
        Console.WriteLine($"Reset {_RollingTimer}");

        _RollingTimer.Reset();

        RefreshTimes();
        OnPropertyChanged(nameof(ResetCount));
        RefreshRunningState();
    }
    
    public void Clear() {
        Console.WriteLine($"Clear {_RollingTimer}");

        _RollingTimer.Clear();

        FullRefresh();
    }
    
    public void Rollover() {
        Console.WriteLine($"Rollover {_RollingTimer}");

        _RollingTimer.Rollover();
        if (!_RefreshTimer.Enabled) {
            _RefreshTimer.Start();
        }

        RefreshTimes();
        OnPropertyChanged(nameof(ResetCount));
        RefreshRunningState();
    }
    
    public void Cancel() {
        Console.WriteLine($"Cancel {_RollingTimer}");

        _RollingTimer.Cancel();
        if (_RefreshTimer.Enabled) {
            _RefreshTimer.Stop();
        }
        FullRefresh();
    }

    void OnTick( RollingTimer Timer ) {
        Console.WriteLine($"Tick {_RollingTimer}");

        RefreshTimes();
    }

    #region IDisposable

    void Dispose( bool Disposing ) {
        if (Disposing) {
            _RefreshTimer.Dispose();
        }
    }
    
    /// <inheritdoc />
    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    /// <inheritdoc />
    ~RollingTimerViewModel() {
        Dispose(false);
    }

    #endregion

}
