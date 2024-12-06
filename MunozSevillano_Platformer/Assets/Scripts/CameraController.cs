using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraController : MonoBehaviour
{
	public Transform target;
	public Tilemap tilemap;
	public float arrivalTime;
	public Camera followCamera;
	  
	private Vector3 currentVelocity;
	  
	private Vector2 viewportHalfSize;
	private float leftBoundary, rightBoundary, bottomBoundary, topBoundary;
	  
	public void Start()
	{
	tilemap.CompressBounds();
	CalculateBounds();
	}

	private void CalculateBounds()
	{
	viewportHalfSize = new Vector2(followCamera.aspect * followCamera.orthographicSize, followCamera.orthographicSize);
	leftBoundary = tilemap.transform.position.x + tilemap.cellBounds.min.x + viewportHalfSize.x;
	rightBoundary = tilemap.transform.position.x + tilemap.cellBounds.max.x - viewportHalfSize.x;
	bottomBoundary = tilemap.transform.position.y + tilemap.cellBounds.min.y + viewportHalfSize.y;
	rightBoundary = tilemap.transform.position.y + tilemap.cellBounds.max.y - viewportHalfSize.y;
	}

	public void LateUpdate()
	{
		Vector3 desiredPosition = target.position + new Vector3(target.position.x, target.position.y, transform.position.z);
		Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, arrivalTime);

		smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, leftBoundary, rightBoundary);
		smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, bottomBoundary, topBoundary);
	}
}