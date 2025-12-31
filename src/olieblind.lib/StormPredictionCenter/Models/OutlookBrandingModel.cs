using System.Drawing;

namespace olieblind.lib.StormPredictionCenter.Models;

public class OutlookBrandingModel(string category, Point position, string fontName, float fontSize, int offset)
{
    public string Category { get; } = category;
    public Point Position { get; } = position;
    public string FontName { get; } = fontName;
    public float FontSize { get; } = fontSize;
    public int Offset { get; } = offset;
}