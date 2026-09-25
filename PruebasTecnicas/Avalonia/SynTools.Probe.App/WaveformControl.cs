using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;

namespace SynTools.Probe.App;

public sealed class WaveformControl : Control
{
    private readonly double[] _points = Enumerable.Range(0, 20_000).Select(i => Math.Sin(i / 21d) * (0.55 + 0.45 * Math.Sin(i / 997d))).ToArray();
    private readonly DispatcherTimer _timer;
    private double _playhead;
    public double Zoom { get; set; } = 1;
    public double Offset { get; set; }

    public WaveformControl()
    {
        MinHeight = 260;
        _timer = new DispatcherTimer(TimeSpan.FromMilliseconds(16), DispatcherPriority.Render, (_, _) => { _playhead = (_playhead + 0.0025) % 1; InvalidateVisual(); });
        _timer.Start();
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        var bounds = Bounds;
        context.FillRectangle(Brushes.Black, bounds);
        if (bounds.Width <= 1 || bounds.Height <= 1) return;
        var visible = Math.Max(2, (int)(_points.Length / Zoom));
        var start = Math.Clamp((int)((_points.Length - visible) * Offset), 0, _points.Length - visible);
        var step = Math.Max(1, visible / Math.Max(1, (int)bounds.Width));
        var geometry = new StreamGeometry();
        using (var builder = geometry.Open())
        {
            var first = true;
            for (var index = 0; index < visible; index += step)
            {
                var point = new Point(index / (double)visible * bounds.Width, bounds.Height * (0.5 - _points[start + index] * 0.42));
                if (first) { builder.BeginFigure(point, false); first = false; } else builder.LineTo(point);
            }
        }
        context.DrawGeometry(null, new Pen(Brushes.DeepSkyBlue, 1), geometry);
        var x = _playhead * bounds.Width;
        context.DrawLine(new Pen(Brushes.OrangeRed, 2), new Point(x, 0), new Point(x, bounds.Height));
    }
}
