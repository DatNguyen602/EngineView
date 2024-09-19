using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading;
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

    private void Start()
    {
        _checkbox.GetComponent<Button>().onClick.AddListener(() =>
         {
             _checkbox.isCheck = !_checkbox.isCheck;
             this.ChangeActiveObject(_checkbox.isCheck);
         });
        _listObject = new List<Outline> (GameObject.FindObjectsOfType<Outline>());
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

                selectedObject.transform.Translate(cameraMotionExtend * CameraController.instance._speetMove * Time.deltaTime * 8, Space.World);
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
}
