using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    [SerializeField, Range(0, 25)]
    public float _speetRote = 5f;

    [SerializeField, Range(0, 50)]
    public float _speetMove = 5f;

    [SerializeField, Range(0, 10)]
    public float _zoomSpeed = 2f;

    [SerializeField] public GameObject _cameraMain;
    [SerializeField] private Vector2 _rangeZoom;
    public static CameraController instance;

    void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    void FixedUpdate()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButton(1))
            {
                Rote();
            }
            else if (Input.GetMouseButton(2))
            {
                Move();
            }
            else if (Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                Zoom(Input.GetAxis("Mouse ScrollWheel"));
            }
        }
    }

    void Rote()
    {
        Vector3 vectorRote = new Vector3(Input.GetAxis("Mouse Y") * -1, Input.GetAxis("Mouse X"), 0);
        Vector3 vt = _cameraMain.transform.localRotation.eulerAngles;
        vt += vectorRote * _speetRote * Time.deltaTime * 20;
        _cameraMain.transform.rotation = Quaternion.Euler(vt);
    }

    void Move()
    {
        Vector3 vectorMove = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0);

        Vector3 camRight = _cameraMain.transform.right.normalized;
        Vector3 camUp = _cameraMain.transform.up.normalized;
        Vector3 cameraMotionExtend = vectorMove.x * camRight + vectorMove.y * camUp;
        float rateZoom = 0.1f;
        try
        {
            rateZoom = _cameraMain.GetComponentInChildren<Camera>().fieldOfView / 150;
        }
        catch { }
        _cameraMain.transform.Translate(cameraMotionExtend * _speetMove * Time.deltaTime * -10 * rateZoom, Space.World);
    }

    void Zoom(float z)
    {
        float newFieldView = _cameraMain.GetComponentInChildren<Camera>().fieldOfView - z * _speetMove * Time.deltaTime * 100;
        _cameraMain.GetComponentInChildren<Camera>().fieldOfView = Mathf.Clamp(newFieldView, _rangeZoom[0], _rangeZoom[1]);
    }
}
