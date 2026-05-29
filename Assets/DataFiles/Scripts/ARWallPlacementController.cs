using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARWallPlacementController : MonoBehaviour
{
    [Header("AR References")]
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private ARPlaneManager planeManager;
    [SerializeField] private ARAnchorManager anchorManager;

    [Header("Placement")]
    [SerializeField] private GameObject objectToPlace;
    [SerializeField] private float wallOffset = 0.015f;
    [SerializeField] private bool hidePlanesAfterPlacement = true;

    private static readonly List<ARRaycastHit> Hits = new();

    private GameObject spawnedObject;
    private ARAnchor currentAnchor;

    private void Update()
    {
        if (!TryGetTapPosition(out Vector2 tapPosition))
            return;

        TryPlaceObjectOnWall(tapPosition);
    }

    private bool TryGetTapPosition(out Vector2 tapPosition)
    {
        tapPosition = default;

        if (Touchscreen.current != null)
        {
            var touch = Touchscreen.current.primaryTouch;

            if (touch.press.wasPressedThisFrame)
            {
                tapPosition = touch.position.ReadValue();
                return true;
            }
        }

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            tapPosition = Mouse.current.position.ReadValue();
            return true;
        }

        return false;
    }

private void TryPlaceObjectOnWall(Vector2 screenPosition)
{
    bool hasHit = raycastManager.Raycast(
        screenPosition,
        Hits,
        TrackableType.PlaneWithinPolygon
    );

    if (!hasHit)
        return;

    ARRaycastHit hit = Hits[0];
    ARPlane hitPlane = planeManager.GetPlane(hit.trackableId);

    if (hitPlane == null)
        return;

    if (hitPlane.alignment != PlaneAlignment.Vertical)
    {
        Debug.Log("Hit plane is not vertical. Ignoring.");
        return;
    }

    Pose hitPose = hit.pose;

    // AR vertical plane normal usually comes from the plane pose's local up direction.
    Vector3 wallNormal = hitPose.rotation * Vector3.up;

    // Make sure the quad faces the camera, not inside the wall.
    Camera cam = Camera.main;
    Vector3 directionToCamera = cam.transform.position - hitPose.position;

    if (Vector3.Dot(wallNormal, directionToCamera) < 0f)
    {
        wallNormal = -wallNormal;
    }

    // Place slightly in front of the wall to avoid flickering/z-fighting.
    Vector3 spawnPosition = hitPose.position + wallNormal * wallOffset;

    // Make it behave like a wall-mounted screen:
    // forward = wall normal, up = world up.
Quaternion screenRotation = Quaternion.LookRotation(-wallNormal, Vector3.up);
    if (spawnedObject != null)
        Destroy(spawnedObject);

    if (currentAnchor != null)
        Destroy(currentAnchor.gameObject);

    currentAnchor = anchorManager.AttachAnchor(hitPlane, hitPose);

    spawnedObject = Instantiate(objectToPlace, spawnPosition, screenRotation);

    if (currentAnchor != null)
    {
        spawnedObject.transform.SetParent(currentAnchor.transform, true);
    }

    if (hidePlanesAfterPlacement)
    {
        HideDetectedPlanes();
    }
}

    private void HideDetectedPlanes()
    {
        foreach (ARPlane plane in planeManager.trackables)
        {
            plane.gameObject.SetActive(false);
        }

        planeManager.enabled = false;
    }
}