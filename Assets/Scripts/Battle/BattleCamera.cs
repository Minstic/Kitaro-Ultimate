using UnityEngine;

public class BattleCamera : MonoBehaviour
{
	private Vector3 dragOrigin;
	public float dragSpeed = 2f;
	public float zoomSpeed = 5f;
	public float minZoom = 4f;
	public float maxZoom = 6f;

	public float minX = -3.5f; // Límite izquierdo
	public float maxX = 4.2f;  // Límite derecho

	void Update()
	{
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
		{
			HandleTouchControls();
		}
		else
		{
			HandleDragging();
			HandleZoom();
		}
	}

	void HandleDragging()
	{
		if (Input.GetMouseButtonDown(1))
		{
			dragOrigin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		}

		if (Input.GetMouseButton(1))
		{
			Vector3 difference = dragOrigin - Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector3 newPosition = transform.position + new Vector3(difference.x, 0, 0);
			newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
			transform.position = newPosition;
		}
	}

	void HandleZoom()
	{
		float scroll = Input.GetAxis("Mouse ScrollWheel");
		Camera.main.orthographicSize -= scroll * zoomSpeed;
		Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, minZoom, maxZoom);
	}

	void HandleTouchControls()
	{
		if (Input.touchCount == 1) // Un dedo: arrastre
		{
			Touch touch = Input.GetTouch(0);

			if (touch.phase == TouchPhase.Began)
			{
				dragOrigin = Camera.main.ScreenToWorldPoint(touch.position);
			}
			else if (touch.phase == TouchPhase.Moved)
			{
				Vector3 difference = dragOrigin - Camera.main.ScreenToWorldPoint(touch.position);
				Vector3 newPosition = transform.position + new Vector3(difference.x, 0, 0);
				newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
				transform.position = newPosition;
			}
		}
		else if (Input.touchCount == 2) // Dos dedos: zoom
		{
			Touch touch0 = Input.GetTouch(0);
			Touch touch1 = Input.GetTouch(1);

			Vector2 touch0Prev = touch0.position - touch0.deltaPosition;
			Vector2 touch1Prev = touch1.position - touch1.deltaPosition;

			float prevMagnitude = (touch0Prev - touch1Prev).magnitude;
			float currentMagnitude = (touch0.position - touch1.position).magnitude;

			float difference = currentMagnitude - prevMagnitude;

			Camera.main.orthographicSize -= difference * zoomSpeed * 0.01f;
			Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, minZoom, maxZoom);
		}
	}
}
