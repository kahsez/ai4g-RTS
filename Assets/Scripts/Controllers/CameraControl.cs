using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    ///////////////////////////////////////////////////
    //////////////////// ATTRIBUTES ///////////////////
    ///////////////////////////////////////////////////

    [SerializeField] private float _minDistance;

    private float _maxDistance;
    
    private float _zoomStep = 0.5f;

    [SerializeField] private float _dragSpeed = 1f;

    [SerializeField] private float _width;

    [SerializeField] private float _height;
	 	
    private Vector3 _dragOrigin;
    private Vector3 _dragMove;
    private float _dist;

    private Camera _cam;

    ///////////////////////////////////////////////////
    ///////////////////// METHODS /////////////////////
    ///////////////////////////////////////////////////

    private void Start()
    {
        _cam = GetComponent<Camera>();
        _dist = transform.position.z;
        _maxDistance = CalculateMaxSize();
        _cam.orthographicSize = _maxDistance;
        _zoomStep = (_maxDistance - _minDistance) / 6f;
    }


    // Update is called once per frame
    private void Update()
    {
        // ZOOM
        Zoom();
        // DRAG
        Drag(); 
        // LIMIT MOVEMENT
        SnapToLimits();
    }

    private void Zoom()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            // Zoom in
            if (_cam.orthographicSize > _minDistance)
                ZoomOrthoCamera(_cam.ScreenToWorldPoint(Input.mousePosition), _zoomStep);
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            // Zoom out
            if (_cam.orthographicSize < _maxDistance)
                ZoomOrthoCamera(_cam.ScreenToWorldPoint(Input.mousePosition), -_zoomStep);
        }
    }

    private void Drag()
    {
        if (Input.GetMouseButtonDown(2))
        {
            _dragOrigin = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, _dist);
            Cursor.visible = false;
        }
        else if (Input.GetMouseButton(2))
        {
            _dragMove = new Vector3(Input.mousePosition.x - _dragOrigin.x, Input.mousePosition.y - _dragOrigin.y, _dist);
            _dragOrigin = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _dist);
            transform.position = new Vector3(transform.position.x - _dragMove.x * Time.deltaTime * _dragSpeed, transform.position.y - _dragMove.y * Time.deltaTime * _dragSpeed, _dist);
        }
        else if (Input.GetMouseButtonUp (2)) {
            Cursor.visible = true;
        }
    }

    private void SnapToLimits()
    {
        float camHeight = _cam.orthographicSize * 2f;
        float camWidth = camHeight * ((float) Screen.width / (float) Screen.height);

        float posX = Mathf.Clamp(transform.position.x, camWidth / 2f, _width - camWidth / 2f);
        float posY = Mathf.Clamp(transform.position.y, camHeight / 2f, _height - camHeight / 2f);
        transform.position = new Vector3(posX, posY, _dist);
    }
	
    private void ZoomOrthoCamera(Vector3 zoomTowards, float amount)
    {
        // Calculate how much we will have to move towards the zoomTowards position
        float multiplier = (1.0f / _cam.orthographicSize * amount);
 
        // Move camera
        transform.position += (zoomTowards - transform.position) * multiplier; 
 
        // Zoom camera
        _cam.orthographicSize -= amount;
 
        // Limit zoom
        _cam.orthographicSize = Mathf.Clamp(_cam.orthographicSize, _minDistance, _maxDistance);
    }

    private float CalculateMaxSize()
    {
        float proportion = (float)_cam.pixelWidth / (float)_cam.pixelHeight;
        float maxWidth = _width;
        float maxHeight = maxWidth / proportion;

        return maxHeight / 2f;
    }
}
