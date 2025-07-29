using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Animation_Cam : MonoBehaviour
{
    public float dragSpeed = 2f;
    public float zoomSpeed = 5f;
    public float minZoom = 2f;
    public float maxZoom = 10f;

    private Vector3 dragOrigin;
    private Vector3 OgPoint;
    private float OgZoom;

    void Start()
    {
        OgPoint = transform.position;
        OgZoom = Camera.main.orthographicSize;
    }

    void Update()
    {
        if (!IsPointerOverUI())
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
            transform.position += difference;
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
        if (Input.touchCount > 0 && IsPointerOverUI()) return; // Evita mover la cámara si el toque está sobre UI

        if (Input.touchCount == 1) // Un solo dedo -> Arrastrar
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                dragOrigin = Camera.main.ScreenToWorldPoint(touch.position);
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                Vector3 difference = dragOrigin - Camera.main.ScreenToWorldPoint(touch.position);
                transform.position += difference;
            }
        }
        else if (Input.touchCount == 2) // Dos dedos -> Zoom
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

    public void ResetPosition()
    {
        transform.position = OgPoint;
        Camera.main.orthographicSize = OgZoom;
    }

    private bool IsPointerOverUI()
    {
        if (EventSystem.current == null)
            return false;

        if (Application.isMobilePlatform)
        {
            foreach (Touch touch in Input.touches)
            {
                PointerEventData eventData = new PointerEventData(EventSystem.current)
                {
                    position = touch.position
                };
                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(eventData, results);
                if (results.Count > 0)
                    return true;
            }
        }
        else
        {
            return EventSystem.current.IsPointerOverGameObject();
        }
        return false;
    }
}
