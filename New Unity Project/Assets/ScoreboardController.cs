﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

public class ScoreboardController : MonoBehaviour {
	// Used for gaze raycasting.
	public Camera firstPersonCamera;
	// The fixed position in the real world for the scoreboard.
	private Anchor anchor;
	// The plane the scoreboard is positioned above/near.
	private DetectedPlane detectedPlane;
	// The scoreboard's offset from the plane in the Y axis.
	private float yOffset;
	private int score;
	// Use this for initialization
	void Start () {
		foreach (Renderer r in GetComponentsInChildren<Renderer>())
    {
        r.enabled = false;
    }
	}

	// Update is called once per frame
	void Update () {

		// The tracking state must be FrameTrackingState.Tracking
		// in order to access the Frame.
		if (Session.Status != SessionStatus.Tracking)
		{
		    return;
		}

		// If there is no plane, then return
		if (detectedPlane == null)
		{
		    return;
		}

		// Check for the plane being subsumed.
		// If the plane has been subsumed switch attachment to the subsuming plane.
		while (detectedPlane.SubsumedBy != null)
		{
		    detectedPlane = detectedPlane.SubsumedBy;
		}

		// Make the scoreboard face the viewer.
		transform.LookAt (firstPersonCamera.transform);

		// Move the position to stay consistent with the plane.
		transform.position = new Vector3(transform.position.x,
		            detectedPlane.CenterPose.position.y + yOffset, transform.position.z);
	}

	// in ScoreboardController.cs
	public void SetSelectedPlane(DetectedPlane detectedPlane)
	{
	    this.detectedPlane = detectedPlane;
	    CreateAnchor();
	}

	void CreateAnchor()
	{
    // Create the position of the anchor by raycasting a point towards
    // the top of the screen.
    Vector2 pos = new Vector2 (Screen.width * .5f, Screen.height * .90f);
    Ray ray = firstPersonCamera.ScreenPointToRay (pos);
    Vector3 anchorPosition = ray.GetPoint (5f);

    // Create the anchor at that point.
    if (anchor != null) {
      DestroyObject (anchor);
    }
    anchor = detectedPlane.CreateAnchor (
        new Pose (anchorPosition, Quaternion.identity));

    // Attach the scoreboard to the anchor.
    transform.position = anchorPosition;
    transform.SetParent (anchor.transform);

    // Record the y offset from the plane.
    yOffset = transform.position.y - detectedPlane.CenterPose.position.y;

    // Finally, enable the renderers.
    foreach (Renderer r in GetComponentsInChildren<Renderer>())
    {
        r.enabled = true;
    }
	}
}
