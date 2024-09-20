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
    [SerializeField] private GameObject _itemPrefab;
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
    }

    void Update()
    {
        try
        {
            _timeControl.GetChild(1).GetComponent<Slider>().value = (float)_playVideo.time / (float)_playVideo.length;
            _timeControl.GetChild(0).GetComponent<TextMeshProUGUI>().text = (_playVideo.time / 60).ToString("F0") + ":" + (_playVideo.time % 60).ToString("F0") + " / " + (_playVideo.length / 60).ToString("F0") + ":" + (_playVideo.length % 60).ToString("F0");
        }
        catch { }
    }

    void onClick(string str)
    {
        Debug.Log("CLicked ...!");
        PlayVideo(str);
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

    void PlayVideo(string videoName)
    {
        string videoPath = System.IO.Path.Combine(_mainPath, videoName);
        _mainVideoPlay.gameObject.SetActive(true);
        _mainVideoPlay.transform.parent.gameObject.SetActive(true);
        _playVideo.url = videoPath;
        _playVideo.Prepare();
        _playVideo.Play();
    }
}
