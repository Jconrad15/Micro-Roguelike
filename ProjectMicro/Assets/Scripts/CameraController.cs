using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private readonly int zOffset = -10;
    private readonly float standardSpeed = 6f;
    private Vector3 destinationLocation;

    private void OnEnable()
    {
        PlayerInstantiation.RegisterOnPlayerCreated(OnPlayerCreated);

        PlayerController playerController = FindObjectOfType<PlayerController>();
        playerController.RegisterOnPlayerMove(OnPlayerMove);

        FindObjectOfType<LocationGenerator>().RegisterOnLocationCreated(SnapToPlayer);
        FindObjectOfType<WorldGenerator>().RegisterOnWorldCreated(SnapToPlayer);
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

        while (Vector3.Distance(destinationLocation, currentLocation) > 0.001f)
        {
            float step = standardSpeed * Time.deltaTime;
            currentLocation = Vector3.MoveTowards(
                currentLocation, destinationLocation, step);

            transform.position = currentLocation;
            yield return new WaitForEndOfFrame();
        }

        transform.position = destinationLocation;
    }
}
