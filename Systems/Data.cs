using Microsoft.Xna.Framework;

namespace Gloomhaven;

public static class GameData
{
    public static int Task { get; set; }
    public static bool Move = true;
    public static bool End = false;
    public static Vector2? PlayerVector;
}
