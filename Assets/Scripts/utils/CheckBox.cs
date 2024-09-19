using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CheckBox : MonoBehaviour
{
    private bool _state;
    public bool isCheck
    {
        get
        {
            return _state;
        }
        set
        {
            _state = value;
            _stateUI.SetActive(value);
        }
    }
    private GameObject _stateUI;

    private void Start()
    {
        _stateUI = transform.GetChild(1).gameObject;
    }
}
