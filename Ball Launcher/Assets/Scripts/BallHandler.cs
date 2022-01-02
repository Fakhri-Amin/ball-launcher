using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BallHandler : MonoBehaviour
{
    [SerializeField] GameObject ballPrefab;
    [SerializeField] Rigidbody2D pivot;
    [SerializeField] float delayDuration;
    [SerializeField] float respawnDelay;

    Rigidbody2D currentBallRigidbody;
    SpringJoint2D currentSpringJoint;

    private Camera mainCamera;
    private bool isDragging;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        SpawnNewBall();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentBallRigidbody == null) return;

        if (!Touchscreen.current.primaryTouch.press.isPressed)
        {
            if (isDragging)
            {
                LaunchBall();
            }

            isDragging = false;
            return;
        }

        isDragging = true;
        Vector2 touchPos = Touchscreen.current.primaryTouch.position.ReadValue();
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(touchPos);

        currentBallRigidbody.isKinematic = true;
        currentBallRigidbody.position = worldPos;
    }

    void SpawnNewBall()
    {
        GameObject ballInstance = Instantiate(ballPrefab, pivot.position, Quaternion.identity);
        currentBallRigidbody = ballInstance.GetComponent<Rigidbody2D>();
        currentSpringJoint = ballInstance.GetComponent<SpringJoint2D>();
        currentSpringJoint.connectedBody = pivot;
    }

    private void LaunchBall()
    {
        currentBallRigidbody.isKinematic = false;
        currentBallRigidbody = null;

        Invoke(nameof(DetachBall), delayDuration);
    }

    void DetachBall()
    {
        currentSpringJoint.enabled = false;
        currentSpringJoint = null;

        Invoke(nameof(RespawnBall), respawnDelay);
    }

    void RespawnBall()
    {
        SpawnNewBall();
    }
}
