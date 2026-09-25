using System.Diagnostics;

namespace SynTools.Probe.Core;

public sealed record ProcessRequest(string Executable, IReadOnlyList<string> Arguments, string? WorkingDirectory = null, TimeSpan? Timeout = null);
public sealed record ProcessResult(int ExitCode, string StandardOutput, string StandardError, bool Cancelled, TimeSpan Duration);

public sealed class ProcessRunner
{
    public async Task<ProcessResult> RunAsync(ProcessRequest request, CancellationToken cancellationToken = default)
    {
        var info = new ProcessStartInfo(request.Executable)
        {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
            WorkingDirectory = request.WorkingDirectory ?? Environment.CurrentDirectory
        };
        foreach (var argument in request.Arguments) info.ArgumentList.Add(argument);
        using var process = new Process { StartInfo = info };
        var watch = Stopwatch.StartNew();
        if (!process.Start()) throw new InvalidOperationException("No se pudo iniciar el proceso.");
        var stdout = process.StandardOutput.ReadToEndAsync();
        var stderr = process.StandardError.ReadToEndAsync();
        using var timeout = request.Timeout is { } value ? new CancellationTokenSource(value) : null;
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeout?.Token ?? CancellationToken.None);
        var cancelled = false;
        try
        {
            await process.WaitForExitAsync(linked.Token);
        }
        catch (OperationCanceledException)
        {
            cancelled = true;
            if (!process.HasExited) process.Kill(entireProcessTree: true);
            await process.WaitForExitAsync(CancellationToken.None);
        }
        var result = new ProcessResult(process.ExitCode, await stdout, await stderr, cancelled, watch.Elapsed);
        if (cancelled) throw new ProcessTreeCancelledException(result);
        return result;
    }
}

public sealed class ProcessTreeCancelledException(ProcessResult result) : OperationCanceledException("Proceso y árbol cancelados.")
{
    public ProcessResult Result { get; } = result;
}
