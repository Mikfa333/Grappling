using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grappling : MonoBehaviour
{
    private LineRenderer lr;
    private Vector3 GrapplePoint;
    public LayerMask WhatIsGrappable;
    public Transform GunTip, Camera, Player;
    private float MaxDistance = 500f;
    private SpringJoint Joint;
    public GameObject GrapplingPoint;
    private bool CanGrapple;

    void Awake ()
    {
        lr = GetComponent<LineRenderer>();
    }

    void Update()
    {
        
        DetectGrapplingPoint();
        
        if (Input.GetMouseButtonDown(0))
        {
            StartGrapple();
        }
        else if(Input.GetMouseButtonUp(0))
        {
            StopGrapple();
        }
    }

    void StartGrapple()
    {
        RaycastHit hit;
        if (Physics.SphereCast(Camera.position, 5f, Camera.forward, out hit, MaxDistance, WhatIsGrappable) && CanGrapple)
        {
            GrapplePoint = hit.point;
            Joint = Player.gameObject.AddComponent<SpringJoint>();
            Joint.autoConfigureConnectedAnchor = false;
            Joint.connectedAnchor = GrapplePoint;

            float DistanceFromPoint = Vector3.Distance(Player.position, GrapplePoint);

            Joint.maxDistance = DistanceFromPoint * 0.8f;
            Joint.minDistance = DistanceFromPoint * 0.25f;

            Joint.spring = 4.5f;
            Joint.damper = 7f;
            Joint.massScale = 4.5f;

            lr.positionCount = 2;
            CurrentGrapplePosition = GunTip.position;

            CanGrapple = false;
        }
    }

    void LateUpdate()
    {
        DrawRope();
    }

    void StopGrapple()
    {
        lr.positionCount = 0;
        Destroy(Joint);
    }

    private Vector3 CurrentGrapplePosition;

    void DrawRope()
    {
        if (!Joint) return;

        CurrentGrapplePosition = Vector3.Lerp(CurrentGrapplePosition, GrapplePoint, Time.deltaTime * 8f);

        lr.SetPosition(0, GunTip.position);
        lr.SetPosition(1, GrapplePoint);

    }

    public bool IsGrappling()
    {
        return Joint != null;
    }

    public Vector3 GetGrapplePoint()
    {
        return GrapplePoint;
    }

    void DetectGrapplingPoint()
    {
        RaycastHit Detect;
        if (Physics.SphereCast(Camera.position, 5f, Camera.forward, out Detect, MaxDistance, WhatIsGrappable))
        {
            GrapplePoint = Detect.point;
            Instantiate(GrapplingPoint, GrapplePoint, Quaternion.identity);
            CanGrapple = true;
        }

    }

}