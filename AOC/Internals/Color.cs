using System;

namespace AdventOfCode.Internals;

/// <summary>
/// Represents an RGB color (0..1 per channel, but can be constructed from 0..255 as well).
/// </summary>
public struct Color
{
    public float R, G, B; // 0..1
    public Color(float r, float g, float b) { R = r; G = g; B = b; }
    public Color(byte r, byte g, byte b) { R = r / 255f; G = g / 255f; B = b / 255f; }
    public static Color From255(byte r, byte g, byte b) => new Color(r, g, b);
    public static Color From01(float r, float g, float b) => new Color(r, g, b);
    public (byte, byte, byte) To255() => ((byte)(R * 255), (byte)(G * 255), (byte)(B * 255));
} 