using UnityEngine;
using UnityEngine.UI;

public class WebcamScript : MonoBehaviour
{
    public RawImage rawImage;
    private WebCamTexture webCamTexture;

    void Start()
    {
        webCamTexture = new WebCamTexture();


        rawImage.texture = webCamTexture;
        webCamTexture.Play();
    }

    private void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Space)))
        {
            toggleCamera(false);
        }
        else if ((Input.GetKeyDown(KeyCode.LeftAlt)))
        {
            toggleCamera(true);
        }
    }

    void OnDestroy()
    {
        if (webCamTexture != null)
        {
            webCamTexture.Stop();
        }
    }

    public void toggleCamera(bool state)
    {
        if (state)
        {
            webCamTexture.Play();
        }
        else
        {
            webCamTexture.Stop();
        }
    }
}
