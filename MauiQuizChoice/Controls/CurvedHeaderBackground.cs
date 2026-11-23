using Microsoft.Maui.Controls.Shapes;

namespace MauiQuizChoice.Controls;

public class CurvedHeaderBackground : Border
{
    public CurvedHeaderBackground()
    {
        BackgroundColor = Color.FromArgb("#6B66F6");
        StrokeThickness = 0;
        Margin = new Thickness(-40, -50, -40, 0);
        StrokeShape = new RoundRectangle
        {
            CornerRadius = new CornerRadius(0, 0, 180, 180)
        };
    }
}