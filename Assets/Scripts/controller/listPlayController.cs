using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;

public class listPlayController : MonoBehaviour
{
    [SerializeField] private Transform _content, _timeControl;
    [SerializeField] private GameObject _itemPrefab, _btnPlay, _uiAfterPlay;
    [SerializeField] private RawImage _mainVideoPlay;
    private List<string> _listPlay = new List<string>();
    [SerializeField] private VideoPlayer _playVideo;
    private string _mainPath = Path.Combine(Application.streamingAssetsPath, "Video");
    [Serializable]
    public enum StateVideo
    {
        play,
        stop,
        paulse,
        next,
        pre
    }
    private int _curVideo;
    private bool _isEndVideo;
    [SerializeField] private const int rateFrame = 1;

    void Start()
    {
        string[] mp4Files = Directory.GetFiles(_mainPath, "*.mp4");
        foreach (string file in mp4Files)
        {
            _listPlay.Add(Path.GetFileName(file));
        }
        renderUI();
        _mainVideoPlay.gameObject.SetActive(false);
        _mainVideoPlay.transform.parent.gameObject.SetActive(false);
        _curVideo = -1;
        _timeControl.GetChild(1).transform.GetChild(0).GetComponent<Slider>().onValueChanged.AddListener(delegate
        {
            _playVideo.time = (_timeControl.GetChild(1).transform.GetChild(0).GetComponent<Slider>().value * _playVideo.length) / rateFrame;
            Debug.Log(_timeControl.GetChild(1).transform.GetChild(0).GetComponent<Slider>().value);
        });
        _isEndVideo = false;
    }

    void Update()
    {
        try
        {
            _timeControl.GetChild(1).GetComponent<Slider>().value = (float) ((_playVideo.time / _playVideo.length) * rateFrame);
            _timeControl.GetChild(0).GetComponent<TextMeshProUGUI>().text = (_playVideo.time / 60).ToString("F0") + ":" + (_playVideo.time % 60).ToString("F0") + " / " + (_playVideo.length / 60).ToString("F0") + ":" + (_playVideo.length % 60).ToString("F0");
        }
        catch { }
        if (Mathf.Abs((float)_playVideo.length - (float)_playVideo.time) < 0.5f && (float)_playVideo.length > 0)
        {
            _uiAfterPlay.SetActive(true);
        }
        else
        {
            _uiAfterPlay.SetActive(false);
        }
    }

    void onClick(string str)
    {
        Debug.Log("CLicked ...!");
        if(_listPlay.IndexOf(str) != _curVideo) StartCoroutine(LoadAndPlayVideo(str));
        else
        {
            _playVideo.Play();
        }
    }

    void renderUI()
    {
        for (int i = 0; i < _content.childCount; i++)
        {
            Destroy(_content.GetChild(i).gameObject);
        }
        foreach (string i in _listPlay)
        {
            GameObject temp = Instantiate(_itemPrefab);
            temp.transform.parent = _content;
            temp.GetComponentInChildren<TextMeshProUGUI>().text = i;
            temp.GetComponent<Button>().onClick.AddListener(() => {
                onClick(i);
                _curVideo = _listPlay.IndexOf(i);
            });
        }
    }

    public void PlayVideo()
    {
        _playVideo.Play();
    }

    public void PauseVideo()
    {
        _playVideo.Pause();
    }

    public void NextVideo()
    {
        _playVideo.time += 5.0f;
    }

    public void PreviousVideo()
    {
        _playVideo.time -= 5.0f;
    }

    public void StopVideo()
    {
        _playVideo.Stop();
        _mainVideoPlay.gameObject.SetActive(false);
        _mainVideoPlay.transform.parent.gameObject.SetActive(false);
    }

    public void reLoadVideo()
    {
        _playVideo.time = 0;
        _playVideo.Play();
    }

    public void videoNext()
    {
        _curVideo += 1;
        if(_curVideo >= _listPlay.Count) _curVideo = 0;
        StartCoroutine(LoadAndPlayVideo(_listPlay[_curVideo]));
    }

    private IEnumerator LoadAndPlayVideo(string videoName)
    {
        string videoPath = System.IO.Path.Combine(_mainPath, videoName);
        UnityWebRequest request = UnityWebRequest.Get(videoPath);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            _mainVideoPlay.gameObject.SetActive(true);
            _mainVideoPlay.transform.parent.gameObject.SetActive(true);
            _playVideo.url = videoPath;
            _playVideo.Prepare();
            _playVideo.Play();
            _btnPlay.SetActive(true);
            _uiAfterPlay.SetActive(false);
        }
    }
}
