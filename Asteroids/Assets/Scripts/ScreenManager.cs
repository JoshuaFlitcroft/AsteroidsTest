using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Screen helper - check when things are off screen.
/// </summary>
public class ScreenManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance.
    /// </summary>
    private static ScreenManager instance;
    /// <summary>
    /// Singleton Accessor.
    /// </summary>
    /// <returns>Instance of class</returns>
    public static ScreenManager GetInstance()
    {
        return instance;
    }

    /// <summary>
    /// Main camera in the scene - used to find edges and check when off screen.
    /// </summary>
    [Tooltip("Main camera in the scene - game view")]
    [SerializeField]
    private Camera mainCamera;

    /// <summary>
    /// Left edge of the screen.
    /// </summary>
    private float leftEdge;
    /// <summary>
    /// Right edge of the screen.
    /// </summary>
    private float rightEdge;
    /// <summary>
    /// Top edge of the screen.
    /// </summary>
    private float topEdge;
    /// <summary>
    /// Bottom edge of the screen.
    /// </summary>
    private float bottomEdge;

    /// <summary>
    /// Called when GameObject is woken.
    /// </summary>
    private void Awake()
    {
        instance = this;

        //JF: First calculate our bottom edge points.
        Vector3 screenPoint = Vector3.zero;
        Vector3 worldPoint = mainCamera.ScreenToWorldPoint(screenPoint);
        leftEdge = worldPoint.x;
        bottomEdge = worldPoint.y;
        //JF: Then calculate our top right points.
        screenPoint.x = Screen.width;
        screenPoint.y = Screen.height;
        worldPoint = mainCamera.ScreenToWorldPoint(screenPoint);
        rightEdge = worldPoint.x;
        topEdge = worldPoint.y;
    }

    /// <summary>
    /// Get the left edge of the screen.
    /// </summary>
    /// <returns>Left screen edge</returns>
    public float LeftEdge()
    {
        return leftEdge;
    }
    /// <summary>
    /// Get the right edge of the screen.
    /// </summary>
    /// <returns>Right screen edge</returns>
    public float RightEdge()
    {
        return rightEdge;
    }
    /// <summary>
    /// Get the top edge of the screen.
    /// </summary>
    /// <returns>Top screen edge</returns>
    public float TopEdge()
    {
        return topEdge;
    }
    /// <summary>
    /// Get the bottom edge of the screen.
    /// </summary>
    /// <returns>Bottom screen edge</returns>
    public float BottomEdge()
    {
        return bottomEdge;
    }

    /// <summary>
    /// Checks whether a position is off the screen.
    /// </summary>
    /// <param name="position">Position to check</param>
    /// <param name="buffer">Amount of buffer to add to screen edges</param>
    /// <returns>true if off screen, false if not</returns>
    public bool IsOffScreen(Vector3 position, float buffer)
    {
        return IsOffLeftEdge(position, buffer) || IsOffRightEdge(position, buffer) || IsOffTopEdge(position, buffer) || IsOffBottomEdge(position, buffer);
    }
    /// <summary>
    /// Checks whether a position is off the left edge of the screen.
    /// </summary>
    /// <param name="position">Position to check</param>
    /// <param name="buffer">Amount of buffer to add to the edge</param>
    /// <returns>true if off edge, false if not</returns>
    public bool IsOffLeftEdge(Vector3 position, float buffer)
    {
        return position.x < leftEdge - buffer;
    }
    /// <summary>
    /// Checks whether a position is off the right edge of the screen.
    /// </summary>
    /// <param name="position">Position to check</param>
    /// <param name="buffer">Amount of buffer to add to the edge</param>
    /// <returns>true if off edge, false if not</returns>
    public bool IsOffRightEdge(Vector3 position, float buffer)
    {
        return position.x > rightEdge + buffer;
    }
    /// <summary>
    /// Checks whether a position is off the top edge of the screen.
    /// </summary>
    /// <param name="position">Position to check</param>
    /// <param name="buffer">Amount of buffer to add to the edge</param>
    /// <returns>true if off edge, false if not</returns>
    public bool IsOffTopEdge(Vector3 position, float buffer)
    {
        return position.y > topEdge + buffer;
    }
    /// <summary>
    /// Checks whether a position is off the bottom edge of the screen.
    /// </summary>
    /// <param name="position">Position to check</param>
    /// <param name="buffer">Amount of buffer to add to the edge</param>
    /// <returns>true if off edge, false if not</returns>
    public bool IsOffBottomEdge(Vector3 position, float buffer)
    {
        return position.y < bottomEdge - buffer;
    }

    /// <summary>
    /// Checks whether a position needs to be teleported (off edge) and returns teleported position if so, old position if not.
    /// </summary>
    /// <param name="position">Position to check</param>
    /// <param name="buffer">Amount of buffer to add to the edges</param>
    /// <returns>Teleported position if off edge, original position if not</returns>
    public Vector3 TryTeleportToOtherSide(Vector3 position, float buffer)
    {
        if (IsOffLeftEdge(position, buffer))
        {
            position.x = rightEdge + buffer;
        }
        else if (IsOffRightEdge(position, buffer))
        {
            position.x = leftEdge - buffer;
        }

        if (IsOffTopEdge(position, buffer))
        {
            position.y = bottomEdge - buffer;
        }
        else if (IsOffBottomEdge(position, buffer))
        {
            position.y = topEdge + buffer;
        }

        //JF: Make sure we return position regardless of whether it was off the edge or not.
        return position;
    }
}
