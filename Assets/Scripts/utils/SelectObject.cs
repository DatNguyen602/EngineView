using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectObject : MonoBehaviour
{
    [SerializeField] private GameObject _uiPosition, _uiRote, _uiZoom, _uiNameObject;
    [SerializeField] private CheckBox _checkbox;
    [SerializeField] private GameObject _itemList, _content;
    private GameObject selectedObject;
    private bool isDragging = false;
    private Ray ray;
    private RaycastHit hit;
    private List<Outline> _listObject;
    private List<Vector3> _startPos = new List<Vector3>(), _startRote = new List<Vector3>();

    private void Start()
    {
        _checkbox.GetComponent<Button>().onClick.AddListener(() =>
         {
             _checkbox.isCheck = !_checkbox.isCheck;
             this.ChangeActiveObject(_checkbox.isCheck);
         });
        _listObject = GameObject.FindObjectsOfType<Outline>().ToList();
        foreach (Outline i in _listObject)
        {
            try
            {
                _startPos.Add(i.transform.localPosition);
                _startRote.Add(i.transform.eulerAngles);
            }
            catch { }
        }
        RenderList();
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (!isDragging && !EventSystem.current.IsPointerOverGameObject() && !Input.GetKey(KeyCode.LeftShift))
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    sellect(hit.collider.gameObject);
                }
            }

            if (selectedObject != null)
            {
                mathVector3(_uiPosition, selectedObject.transform.localPosition);
                mathVector3(_uiRote, selectedObject.transform.eulerAngles);
                _uiNameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = selectedObject.name;
                isDragging = true;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging && selectedObject != null && selectedObject.tag != "ground" && (!EventSystem.current.IsPointerOverGameObject()))
        {
            Vector3 vectorMove = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0);
            if (Input.GetKey(KeyCode.R))
            {
                selectedObject.transform.Rotate(CameraController.instance.transform.up, -vectorMove.x * CameraController.instance._speetRote * Time.deltaTime * 100, Space.World);
                selectedObject.transform.Rotate(CameraController.instance.transform.right, vectorMove.y * CameraController.instance._speetRote * Time.deltaTime * 100, Space.World);
                mathVector3(_uiRote, selectedObject.transform.eulerAngles);
            }
            else
            {
                Vector3 camRight = CameraController.instance._cameraMain.transform.right.normalized;
                Vector3 camUp = CameraController.instance._cameraMain.transform.up.normalized;
                Vector3 cameraMotionExtend = vectorMove.x * camRight + vectorMove.y * camUp;
                float rateZoom = 0.1f;
                try
                {
                    rateZoom = CameraController.instance._cameraMain.GetComponentInChildren<Camera>().fieldOfView / 150;
                }
                catch { }
                selectedObject.transform.Translate(cameraMotionExtend * CameraController.instance._speetMove * Time.deltaTime * 8 * rateZoom, Space.World);
            }
        }

        _uiZoom.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = CameraController.instance._cameraMain.GetComponentInChildren<Camera>().fieldOfView.ToString();
        _uiZoom.transform.GetChild(2).GetComponent<Slider>().value = CameraController.instance._cameraMain.GetComponentInChildren<Camera>().fieldOfView;
    }

    public static void mathVector3(GameObject ob, Vector3 vt)
    {
        ob.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = vt.x.ToString();
        ob.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = vt.y.ToString();
        ob.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = vt.z.ToString();
    }

    public void ChangeActiveObject(bool state)
    {
        if(selectedObject != null && selectedObject.tag != "ground")
        {
            selectedObject.SetActive(state);
        }
    }

    public void RenderList()
    {
        for (int i = 0; i < _content.transform.childCount; i++)
        {
            Destroy(_content.transform.GetChild(i).gameObject);
        }
        foreach(Outline i in _listObject)
        {
            GameObject temp = Instantiate(_itemList);
            temp.transform.parent = _content.transform;
            temp.GetComponentInChildren<TextMeshProUGUI>().text = i.gameObject.name;
            temp.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (selectedObject != null && i.gameObject.Equals(selectedObject))
                {
                    zoomObject(selectedObject);
                }
                sellect(i.gameObject);
            });
            ObjectView obv = temp.GetComponent<ObjectView>();
            obv.index = _listObject.IndexOf(i);
            obv.nameObject = i.name;
            obv.target = i.gameObject;
        }
    }

    public void sellect(GameObject g)
    {
        GameObject uiChoose;
        if (selectedObject != null && selectedObject.tag != "ground")
        {
            uiChoose = _content.transform.GetChild(_listObject.IndexOf(selectedObject.GetComponent<Outline>())).gameObject;
            uiChoose.GetComponentInChildren<Image>().color = new Color32(60, 0, 0, 130);
            selectedObject.GetComponent<Outline>().enabled = false;
        }
        selectedObject = g;
        if (selectedObject != null && selectedObject.tag != "ground")
        {
            selectedObject.GetComponent<Outline>().enabled = true;
            uiChoose = _content.transform.GetChild(_listObject.IndexOf(selectedObject.GetComponent<Outline>())).gameObject;
            uiChoose.GetComponentInChildren<Image>().color = new Color32(255, 0, 0, 130);
        }
        _checkbox.isCheck = selectedObject.activeSelf;
    }

    void zoomObject(GameObject go)
    {
        Bounds bounds = go.GetComponent<Renderer>().bounds;
        Vector3 scaledSize = bounds.size;
        float maxSize = Mathf.Max(scaledSize.x, scaledSize.y, scaledSize.z);
        CameraController.instance._cameraMain.transform.position = bounds.center;

        float fov = 2 * Mathf.Atan((Mathf.Abs(maxSize) * 1.1f / 2) / Mathf.Abs(CameraController.instance._cameraMain.transform.GetChild(0).transform.localPosition.z)) * (180 / Mathf.PI);
        CameraController.instance._cameraMain.GetComponentInChildren<Camera>().fieldOfView = fov % 60;
    }

    public void resetAll()
    {
        for(int i = 0; i < _listObject.Count; i++)
        {
            try
            {
                _listObject[i].transform.localPosition = _startPos[i];
                _listObject[i].transform.eulerAngles = _startRote[i];
            }
            catch { }
        }
    }
}
