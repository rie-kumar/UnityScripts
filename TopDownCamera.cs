using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Indicates which mouse button is being pressed.
/// </summary>
public enum MouseButton{
    Primary,
    Secondary,
    Middle
}

public class TopDownCamera : MonoBehaviour
{
    /// <summary>
    /// Determines if the camera is allowed to rotate using mouse.
    /// </summary>
    public bool allowMouseRotation = true;

    /// <summary>
    /// Determines if the camera is allowed to zoom in/out on the <see cref="player"/> object
    /// </summary>
    public bool allowMouseZoom = true;

    /// <summary>
    /// Determines if the mouse zoom is inverted.
    /// </summary>
    public bool mouseZoomInverse = false;

    /// <summary>
    /// The speed of rotation.
    /// </summary>
    [Range(0.1f,10.0f)]
    public float turnSpeed = 4.0f;

    /// <summary>
    /// The speed of zoom.
    /// </summary>
    [Range(0.1f, 10.0f)]
    public float zoomSpeed = 4.0f;

    /// <summary>
    /// The distance above the specified <see cref="player"/>. (y-axis)
    /// </summary>
    public float distanceAboveTarget = 10.0f;

    /// <summary>
    /// The distance from the specified <see cref="player"/>. (z-axis)
    /// </summary>
    public float distanceFromTarget = 10.0f;
    
    /// <summary>
    /// The minimum distance the camera is allowed to be above the specified <see cref="player"/> object.
    /// </summary>
    public float minDistanceAboveTarget = 10.0f;

    /// <summary>
    /// The maximum distance the camera is allowed to be above the specified <see cref="player"/> object.
    /// </summary>
    public float maxDistanceAboveTarget = 40.0f;

    /// <summary>
    /// The <see cref="player"/> that the camera will follow and rotate around.
    /// </summary>
    public Transform player;

    /// <summary>
    /// The offset for the camera from the <see cref="player"/> object.
    /// </summary>
    private Vector3 offset;

    /// <summary>
    /// Determines which mouse key uses rotates the player.
    /// </summary>
    public MouseButton rotationKey = MouseButton.Secondary;

    void Start()
    {
        //Sets the default offset.
        offset = new Vector3(player.position.x, player.position.y + distanceAboveTarget, player.position.z + distanceFromTarget);

        //Checks to see if the camera allows rotation.
        if (!allowMouseRotation)
            turnSpeed = 0.0f;

        //Checks to see if the camera allows zoom.
        if (!allowMouseZoom)
            zoomSpeed = 0.0f;

        //Checks to see if the camera's zoom is in inverse.
        if (mouseZoomInverse)
            zoomSpeed *= -1;
    }

    void LateUpdate()
    {
        var rotation = Input.GetAxis("Mouse X") * turnSpeed;

        //Checks if the correct mouse key is pressed.
        if (!Input.GetMouseButton((int)rotationKey))
        {
            //If not then we set the rotation to nothing. 
            rotation = 0.0f;
        }

        offset = offset + new Vector3(0, CheckCameraBoundry());

        //Grabs the new offset using hte old offset and the new rotation
        offset = Quaternion.AngleAxis(rotation, Vector3.up) * offset;

        //Set the new position for the camera.
        transform.position = player.position + offset;

        //Looks at target object 
        transform.LookAt(player.position);
    }

    /// <summary>
    /// Checks if the camera is within the boundries of <see cref="minDistanceAboveTarget"/> and <see cref="maxDistanceAboveTarget"/>.
    /// </summary>
    /// <returns>
    /// 0, if the camera is outside the boundry or does not allow zoom
    /// zoomed amount, if the camera is within boundries.
    /// </returns>
    float CheckCameraBoundry()
    {
        //Zooms the camera.
        var zoom = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;

        //Checks to see if zoom is enabled or the user has zoomed.
        if (zoom == 0)
            return 0;

        //Checks to see if the zoom is within camera boundries.
        if (zoom + offset.y < minDistanceAboveTarget)
            return 0;
        else if (zoom + offset.y > maxDistanceAboveTarget)
            return 0;

        return zoom;
    }

    /// <summary>
    /// Validates the inspector's serializable values. 
    /// </summary>
    void OnValidate()
    {
        distanceAboveTarget = Mathf.Clamp(distanceAboveTarget, 0, float.MaxValue);
        distanceFromTarget = Mathf.Clamp(distanceFromTarget, 0, float.MaxValue);
        minDistanceAboveTarget = Mathf.Clamp(minDistanceAboveTarget, 0, float.MaxValue);
        maxDistanceAboveTarget = Mathf.Clamp(maxDistanceAboveTarget, 0, float.MaxValue);
    }
}
