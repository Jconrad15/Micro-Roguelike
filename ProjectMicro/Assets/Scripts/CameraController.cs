using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private readonly int zOffset = -10;
    private readonly float panSpeed = 7f;
    private readonly float zoomSpeed = 20f;
    private Vector3 destinationLocation;

    private readonly float minSize = 5f;
    private readonly float maxSize = 100f;

    private Vector3 initialPosition = new Vector3(50, 50, -10);

    private void OnEnable()
    {
        PlayerInstantiation.RegisterOnPlayerCreated(OnPlayerCreated);

        PlayerController playerController =
            FindObjectOfType<PlayerController>();
        playerController.RegisterOnPlayerMove(OnPlayerMove);

        FindObjectOfType<LocationGenerator>()
            .RegisterOnLocationCreated(SnapToPlayer);
        FindObjectOfType<WorldGenerator>()
            .RegisterOnWorldCreated(SnapToPlayer);

        transform.position = initialPosition;
    }

    private void Update()
    {
        // Zoom
        float scroll = Input.mouseScrollDelta.y;
        if (scroll != 0)
        {
            float currentSize = Camera.main.orthographicSize;
            currentSize -= scroll * zoomSpeed * currentSize * Time.deltaTime;

            if (currentSize < minSize)
            {
                currentSize = minSize;
            }
            else if (currentSize > maxSize)
            {
                currentSize = maxSize;
            }

            Camera.main.orthographicSize = currentSize;
        }
    }

    private void OnPlayerCreated(Player p)
    {
        StartCoroutine(MoveCamera(p.X, p.Y));
    }

    private void OnPlayerMove(int x, int y)
    {
        StopAllCoroutines();
        StartCoroutine(MoveCamera(x, y));
    }

    private void SnapToPlayer()
    {
        StopAllCoroutines();
        transform.position = destinationLocation;
    }

    private IEnumerator MoveCamera(int x, int y)
    {
        Vector3 currentLocation = transform.position;
        destinationLocation = new Vector3(x, y, zOffset);

        float distance =
            Vector3.Distance(destinationLocation, currentLocation);

        while (distance > 0.001f)
        {
            float step = panSpeed * Time.deltaTime;
            currentLocation = Vector3.MoveTowards(
                currentLocation, destinationLocation, step);

            transform.position = currentLocation;
            yield return new WaitForEndOfFrame();
        }

        transform.position = destinationLocation;
    }
}
