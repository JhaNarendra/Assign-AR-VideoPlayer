using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlacementReticle : MonoBehaviour
{
    [Header("AR References")]
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private ARPlaneManager planeManager;

    [Header("Reticle Visual")]
    [Tooltip("The GameObject representing the reticle (e.g. a ring or transparent quad).")]
    [SerializeField] private GameObject reticleVisual;
    [Tooltip("Offset distance from the wall to prevent z-fighting / clipping.")]
    [SerializeField] private float reticleOffset = 0.02f;

    [Header("Scanning Requirements")]
    [Tooltip("Minimum width of the scanned wall in meters to snap reticle.")]
    [SerializeField] private float minPlaneWidth = 0.5f;
    [Tooltip("Minimum height of the scanned wall in meters to snap reticle.")]
    [SerializeField] private float minPlaneHeight = 0.5f;

    private static readonly List<ARRaycastHit> Hits = new();
    private bool isPlaced = false;

    private void Start()
    {
        if (reticleVisual != null)
        {
            reticleVisual.SetActive(false);
        }
    }

    private void Update()
    {
        // If the video screen has already been placed, hide the reticle and stop raycasting
        if (isPlaced) return;

        UpdateReticlePose();
    }

    private void UpdateReticlePose()
    {
        // Shoot a raycast from the center of the screen
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        
        bool hasHit = raycastManager.Raycast(
            screenCenter, 
            Hits, 
            TrackableType.PlaneWithinPolygon
        );

        if (hasHit)
        {
            ARRaycastHit hit = Hits[0];
            ARPlane hitPlane = planeManager.GetPlane(hit.trackableId);

            // Only show the reticle if we are looking at a vertical plane that meets size requirements
            if (hitPlane != null && 
                hitPlane.alignment == PlaneAlignment.Vertical &&
                hitPlane.size.x >= minPlaneWidth && 
                hitPlane.size.y >= minPlaneHeight)
            {
                Pose hitPose = hit.pose;
                Vector3 wallNormal = hitPose.rotation * Vector3.up;

                // Make sure the normal points towards the camera
                Camera cam = Camera.main;
                Vector3 directionToCamera = cam.transform.position - hitPose.position;
                if (Vector3.Dot(wallNormal, directionToCamera) < 0f)
                {
                    wallNormal = -wallNormal;
                }

                // Position the reticle slightly in front of the wall to prevent clipping/z-fighting
                transform.position = hitPose.position + wallNormal * reticleOffset;

                // Rotate it to face flat against the wall
                transform.rotation = Quaternion.LookRotation(-wallNormal, Vector3.up);

                if (reticleVisual != null && !reticleVisual.activeSelf)
                {
                    reticleVisual.SetActive(true);
                }
                return;
            }
        }

        // Hide reticle if not looking at a valid wall
        if (reticleVisual != null && reticleVisual.activeSelf)
        {
            reticleVisual.SetActive(false);
        }
    }

    // Call this from your placement script when the screen is placed
    public void OnScreenPlaced()
    {
        isPlaced = true;
        if (reticleVisual != null)
        {
            reticleVisual.SetActive(false);
        }
    }
}
