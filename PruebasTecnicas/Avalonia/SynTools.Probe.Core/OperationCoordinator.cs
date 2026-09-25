namespace SynTools.Probe.Core;

public enum OperationState { Idle, Running, Cancelling, Succeeded, Failed, Cancelled }

public sealed record OperationSnapshot(OperationState State, string? Name, double? Progress, string? Error);

public sealed class OperationCoordinator
{
    private readonly object _gate = new();
    private OperationLease? _active;
    private OperationSnapshot _snapshot = new(OperationState.Idle, null, null, null);

    public OperationSnapshot Snapshot { get { lock (_gate) return _snapshot; } }

    public OperationLease Begin(string name, bool indeterminate = false)
    {
        lock (_gate)
        {
            if (_active is not null) throw new InvalidOperationException("Ya existe una operación pesada incompatible.");
            _snapshot = new(OperationState.Running, name, indeterminate ? null : 0, null);
            return _active = new OperationLease(this, name);
        }
    }

    internal void Report(OperationLease lease, double? progress)
    {
        lock (_gate)
        {
            EnsureOwner(lease);
            _snapshot = _snapshot with { Progress = progress is null ? null : Math.Clamp(progress.Value, 0, 1) };
        }
    }

    internal void RequestCancellation(OperationLease lease)
    {
        lock (_gate)
        {
            EnsureOwner(lease);
            if (_snapshot.State is OperationState.Running)
                _snapshot = _snapshot with { State = OperationState.Cancelling };
            lease.CancellationSource.Cancel();
        }
    }

    internal bool Complete(OperationLease lease, OperationState terminal, string? error)
    {
        lock (_gate)
        {
            if (!ReferenceEquals(_active, lease)) return false;
            if (terminal is not (OperationState.Succeeded or OperationState.Failed or OperationState.Cancelled))
                throw new ArgumentOutOfRangeException(nameof(terminal));
            _snapshot = new(terminal, lease.Name, terminal is OperationState.Succeeded ? 1 : _snapshot.Progress, error);
            _active = null;
            return true;
        }
    }

    private void EnsureOwner(OperationLease lease)
    {
        if (!ReferenceEquals(_active, lease)) throw new InvalidOperationException("La reserva ya no pertenece a esta operación.");
    }
}

public sealed class OperationLease : IDisposable
{
    private readonly OperationCoordinator _owner;
    private int _closed;
    internal CancellationTokenSource CancellationSource { get; } = new();
    internal string Name { get; }
    public CancellationToken CancellationToken => CancellationSource.Token;

    internal OperationLease(OperationCoordinator owner, string name) => (_owner, Name) = (owner, name);
    public void Report(double progress) => _owner.Report(this, progress);
    public void Indeterminate() => _owner.Report(this, null);
    public void Cancel() => _owner.RequestCancellation(this);
    public bool Succeed() => Close(OperationState.Succeeded, null);
    public bool Fail(string error) => Close(OperationState.Failed, error);
    public bool AcknowledgeCancellation() => Close(OperationState.Cancelled, null);
    private bool Close(OperationState terminal, string? error) => Interlocked.Exchange(ref _closed, 1) == 0 && _owner.Complete(this, terminal, error);
    public void Dispose() { if (Interlocked.Exchange(ref _closed, 1) == 0) _owner.Complete(this, OperationState.Cancelled, null); CancellationSource.Dispose(); }
}
