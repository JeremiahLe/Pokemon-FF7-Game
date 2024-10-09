using System.ComponentModel;

[Description("Default orientation is facing left.")]
public enum UnitOrientation
{
    Left, Right, Center
}

internal static class UnitOrientationExtension
{
    public static bool GetUnitOrientation(UnitOrientation unitOrientation)
    {
        switch (unitOrientation)
        {
            case UnitOrientation.Left:
                return true;
            
            case UnitOrientation.Right:
                return false;
            
            case UnitOrientation.Center:
            default:
                return true;
        }
    }
}
