using UnityEngine;

public class TopDownCamera : MonoBehaviour
{
    public Transform[] players;
    public Vector3 offset;
    public float minZoom = 5f;
    public float maxZoom = 15f;
    public float zoomLerpSpeed = 5f;
    public float rotationLerpSpeed = 5f;
    public float movementLerpSpeed = 5f;
    public float maxZoomSpeed = 5f;
    public float fieldOfView = 60f;

    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
        cam.aspect = (float)Screen.width / (float)Screen.height;
    }

private void LateUpdate()
{
    // Calculate the bounds of the camera's view
    Vector3 minPos = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
    Vector3 maxPos = new Vector3(float.MinValue, float.MinValue, float.MinValue);

    foreach (Transform player in players)
    {
        if (player.position.x < minPos.x) minPos.x = player.position.x;
        if (player.position.y < minPos.y) minPos.y = player.position.y;
        if (player.position.z < minPos.z) minPos.z = player.position.z;
        if (player.position.x > maxPos.x) maxPos.x = player.position.x;
        if (player.position.y > maxPos.y) maxPos.y = player.position.y;
        if (player.position.z > maxPos.z) maxPos.z = player.position.z;
    }

    // Determine the distance between the players in the horizontal and vertical directions
    float distanceX = maxPos.x - minPos.x;
    float distanceY = maxPos.y - minPos.y;

    // Calculate the desired camera position and zoom level
    Vector3 cameraPos = (minPos + maxPos) / 2f + offset;
    float zoomLevelX = Mathf.Lerp(maxZoom, minZoom, distanceX / (maxZoom - minZoom));
    float zoomLevelY = Mathf.Lerp(maxZoom, minZoom, distanceY / (maxZoom - minZoom));
    float zoomLevel = Mathf.Max(zoomLevelX, zoomLevelY);

    // Limit the zooming out of the camera
    float maxDistanceX = (maxZoom - minZoom) / Mathf.Tan(cam.fieldOfView * Mathf.Deg2Rad / 2f);
    float maxDistanceY = (maxZoom - minZoom) / (Mathf.Tan(cam.fieldOfView * Mathf.Deg2Rad / 2f) * cam.aspect);
    float maxDistanceTotal = Mathf.Max(maxDistanceX, maxDistanceY);
    float cameraDistanceX = distanceX / Mathf.Tan(cam.fieldOfView * Mathf.Deg2Rad / 2f);
    float cameraDistanceY = distanceY / Mathf.Tan(cam.fieldOfView * Mathf.Deg2Rad / 2f) / cam.aspect;
    float cameraDistance = Mathf.Max(cameraDistanceX, cameraDistanceY);
    float cameraDistanceNeeded = Mathf.Min(cameraDistance, maxDistanceTotal);
    float cameraDistanceLerp = Mathf.Clamp01(zoomLerpSpeed * Time.deltaTime / Mathf.Abs(cam.transform.position.z - cameraDistanceNeeded));
    cam.transform.position = Vector3.Lerp(cam.transform.position, cameraPos - cam.transform.forward * cameraDistanceNeeded, cameraDistanceLerp);

    // Adjust the camera's field of view to ensure all players are visible
    float targetZoom = Mathf.Lerp(cam.fieldOfView, fieldOfView, zoomLerpSpeed * Time.deltaTime / zoomLevel);
    float maxZoomChange = maxZoomSpeed * Time.deltaTime;
    cam.fieldOfView = Mathf.Clamp(targetZoom, cam.fieldOfView - maxZoomChange, cam.fieldOfView + maxZoomChange);

    cam.aspect = (float)Screen.width / (float)Screen.height;

    // Smoothly rotate the camera to look at the center point of the players
    Vector3 centerPos = (minPos + maxPos) / 2f;
    Quaternion targetRotation = Quaternion.LookRotation(centerPos - transform.position, Vector3.up);
    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationLerpSpeed * Time.deltaTime);
    


    // Smoothly move the camera position to avoid sudden jumps when the players collide
    Vector3 targetPos = (minPos + maxPos) / 2f + offset;
    float maxDistance = Mathf.Max(Vector3.Distance(transform.position, targetPos), 0.01f);
    float movementLerp = Mathf.Clamp01(movementLerpSpeed * Time.deltaTime / maxDistance);
    transform.position = Vector3.Lerp(transform.position, targetPos, movementLerp);
}

    }


