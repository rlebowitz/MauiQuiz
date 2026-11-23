using CommunityToolkit.Maui;
namespace MauiQuizChoice.Controls;

public partial class CircularProgressView : GraphicsView, IDrawable
{
    private double _animatedProgress;

    [BindableProperty(PropertyChangedMethodName = nameof(OnProgressChanged))]
    public partial double Progress { get; set; }

    [BindableProperty(PropertyChangedMethodName = nameof(OnTextChanged))]
    public partial string Text { get; set; } = string.Empty;
    public CircularProgressView()
    {
        Drawable = this;
        WidthRequest = 48;
        HeightRequest = 48;
    }

    private static void OnProgressChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is CircularProgressView view && newValue is double targetProgress)
        {
            view.AnimateProgress(targetProgress);
        }
    }

    private static void OnTextChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is CircularProgressView view)
        {
            view.Invalidate();
        }
    }

    private void AnimateProgress(double targetProgress)
    {
        this.AbortAnimation("ProgressRingAnimation");

        double startProgress = _animatedProgress;
        var animation = new Animation(
            callback: v =>
            {
                _animatedProgress = v;
                Invalidate();
            },
            start: startProgress,
            end: targetProgress,
            easing: Easing.CubicOut);

        animation.Commit(this, "ProgressRingAnimation", rate: 16, length: 350);
    }
    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.SaveState();

        float strokeWidth = 3.5f;
        float radius = (Math.Min(dirtyRect.Width, dirtyRect.Height) - strokeWidth) / 2f;
        float centerX = dirtyRect.Center.X;
        float centerY = dirtyRect.Center.Y;

        // 1. Background Track Circle (Translucent White)
        canvas.StrokeColor = Colors.White.WithAlpha(0.3f);
        canvas.StrokeSize = strokeWidth;
        canvas.DrawCircle(centerX, centerY, radius);

        // 2. Active Progress Arc (Solid White, starting at 12 o'clock / 90 degrees)
        float clampedProgress = (float)Math.Clamp(Progress, 0.0, 1.0);
        if (clampedProgress > 0)
        {
            canvas.StrokeColor = Colors.White;
            canvas.StrokeSize = strokeWidth;
            canvas.StrokeLineCap = LineCap.Round;

            float startAngle = 90f;
            float sweepAngle = 360f * clampedProgress;
            // In MAUI DrawArc, angles move counter-clockwise from 0° (3 o'clock),
            // so subtracting sweepAngle draws clockwise from 90° (12 o'clock).
            float endAngle = startAngle - sweepAngle;

            float x = centerX - radius;
            float y = centerY - radius;
            float diameter = radius * 2;

            canvas.DrawArc(x, y, diameter, diameter, startAngle, endAngle, clockwise: true, closed: false);
        }

        // 3. Centered Fraction Text (e.g., "1/5")
        if (!string.IsNullOrEmpty(Text))
        {
            canvas.FontColor = Colors.White;
            canvas.FontSize = 13;
            canvas.Font = Microsoft.Maui.Graphics.Font.DefaultBold;
            canvas.DrawString(Text, dirtyRect, HorizontalAlignment.Center, VerticalAlignment.Center);
        }

        canvas.RestoreState();
    }
}