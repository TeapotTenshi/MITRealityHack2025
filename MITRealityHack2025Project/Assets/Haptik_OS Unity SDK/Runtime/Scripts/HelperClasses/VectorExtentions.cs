using UnityEngine;

/// <summary>
/// Vecttor Extentions Class
/// 
/// This class provides a set of methods to extend the functionalicty of the Vector3 class of Unity.
/// </summary>
public static class VectorExtentions 
{

    /// <summary>
    /// This method returns the original vector with any of its axis changed to a specific value.
    /// </summary>
    /// <param name="original"> The original Vector3</param>
    /// <param name="x"> The X value of the vactro to be changed, if left empty it will retain the original X.</param>
    /// <param name="y"> The Y value of the vactro to be changed, if left empty it will retain the original Y.</param>
    /// <param name="z"> The Z value of the vactro to be changed, if left empty it will retain the original Z.</param>
    /// <returns></returns>
    public static Vector3 With(this Vector3 original, float? x = null, float? y = null, float? z = null) 
    {
        return new Vector3(x ?? original.x, y ?? original.y, z ?? original.z);
    }

    /// <summary>
    /// This method takes two Vector3 values and returns the direction from the source to the destination.
    /// </summary>
    /// <param name="source"> The source Vector3.</param>
    /// <param name="destination"> The destination  Vector3.</param>
    /// <returns></returns>
    public static Vector3 DirectionTo(this Vector3 source, Vector3 destination) 
    {
        return Vector3.Normalize(destination - source);
    }

    /// <summary>
    /// This method takes the Euler Angles of a rotation and returns them withy limit values on each axis.
    /// The maximum value is -180 degrees and the minimum 180 degrees.
    /// </summary>
    /// <param name="eulerRotation"> The original Euler Angles.</param>
    /// <returns> The corrected Euler Angles.</returns>
    public static Vector3 CorrectedEulers(this Vector3 eulerRotation)
    {
        Vector3 newEulers = new Vector3(eulerRotation.x, eulerRotation.y, eulerRotation.z);//eulerRotation;

        if (newEulers.x > 180)
        {
            newEulers.x -= 360;
        }

        if (newEulers.y > 180)
        {
            newEulers.y -= 360;
        }

        if (newEulers.z > 180)
        {
            newEulers.z -= 360;
        }

        return newEulers;
    }
}
