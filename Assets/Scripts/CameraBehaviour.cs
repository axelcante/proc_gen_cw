using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    [Header("Camera Settings")]
    public Camera m_Camera;
    //public float m_cameraMoveSpeed = 10f;
    public float m_cameraZoomSpeed = 10f;
    public float m_minZoom = 20f;
    public float m_maxZoom = 50f;
    public float m_xClamp = 100f;
    public float m_yClamp = 80f;

    private Transform m_DefaultPosition;
    private float m_DefaultZoom;
    //private float m_zoomFactor;

    // Start is called before the first frame update
    void Start()
    {
        m_DefaultPosition = transform;
        m_DefaultZoom = m_Camera.orthographicSize;
    }

    // Update is called once per frame
    void Update()
    {
        // The camera move speed needs to scale depending on current zoom level,
        // i.e., normal speed on max zoom, but faster speed on min zoom.
        // I actually found that setting a move speed = to zoom level worked perfectly!

        // Move camera left and right
        if (Input.GetKey(KeyCode.A)) {
            transform.position += Vector3.left * m_Camera.orthographicSize * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D)) {
            transform.position += Vector3.right * m_Camera.orthographicSize * Time.deltaTime;
        }

        // Move camera up and down
        if (Input.GetKey(KeyCode.W)) {
            transform.position += Vector3.up * m_Camera.orthographicSize * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S)) {
            transform.position += Vector3.down * m_Camera.orthographicSize * Time.deltaTime;
        }

        // Zoom in and out with mouse scroll
        float zoomAmount = Input.GetAxis("Mouse ScrollWheel") * -m_cameraZoomSpeed;
        float zoom = Mathf.Clamp(m_Camera.orthographicSize + zoomAmount, m_minZoom, m_maxZoom);
        m_Camera.orthographicSize = zoom;

        // Clamp camera position on x and y axis
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, -m_xClamp, m_xClamp),
            Mathf.Clamp(transform.position.y, -m_yClamp, m_yClamp),
            transform.position.z
        );

        // Reset camera
        if (Input.GetKeyDown(KeyCode.R)) {
            Debug.Log("Resetting");
            transform.position = m_DefaultPosition.position;
            m_Camera.orthographicSize = m_DefaultZoom;
        }
    }
}
