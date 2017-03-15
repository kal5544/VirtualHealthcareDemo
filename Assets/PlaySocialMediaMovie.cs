using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class PlaySocialMediaMovie : MonoBehaviour {

    public RectTransform patientRect;
    public RawImage socialMediaImage;
    public AudioSource socialMediaAudioSource;
    public RawImage nationalGuardImage;
    public RawImage cdcImage;
    public RawImage facesImage;
    public RawImage landmarksImage;
    public RawImage heartbeatGraphImage;
    public RawImage heartbeatImage;
    public RawImage pointCloudImage;
    public AudioSource heartBeatSource;
    public Texture skeletonTexture;
    public Texture scanTexture;
    public Texture brainTexture;
    float imageTime = 0.0f;
    public int faceStage = 0;
    

    public Texture[] faceImages;
    public Texture[] heartbeatImages;
    public Texture[] landmarkImages;
    int faceImageIndex = 0;
    bool showFaceImages = false;

    public string ipAddress;
    public int port;
    public TcpListener listener;
    public TcpClient client;
    public NetworkStream nwStream;

    Texture2D streamedTex;
    byte[] pixelBytes;
    Color[] pixels;

	// Use this for initialization
	void Start ()
    {
        streamedTex = new Texture2D(512,512);
        listener = new TcpListener(IPAddress.Parse(ipAddress), port);
        listener.Start();
        
        client = listener.AcceptTcpClient();

        nwStream = client.GetStream();
        
        cdcImage.enabled = false;
        nationalGuardImage.enabled = false;
        socialMediaImage.enabled = false;
        pointCloudImage.enabled = false;
        facesImage.enabled = false;
        landmarksImage.enabled = false;
        heartbeatGraphImage.enabled = false;
        heartbeatImage.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (nwStream.DataAvailable)
        {
            pixelBytes = ProtoBuf.Serializer.DeserializeWithLengthPrefix<byte[]>(nwStream, ProtoBuf.PrefixStyle.Fixed32);
            streamedTex.LoadRawTextureData(pixelBytes);
            streamedTex.Apply();
            pointCloudImage.texture = streamedTex;
            Debug.Log("got texture");
        }
    }

    IEnumerator CenterDoctor()
    {
        float minCounter = 0;
        while (minCounter < 1.5f)
        {
            pointCloudImage.rectTransform.anchorMin = new Vector2(Mathf.Lerp(2f/3f, 1f/3f, minCounter/1.5f), Mathf.Lerp(2f/3f, 1f/3f, minCounter/1.5f));
            pointCloudImage.rectTransform.anchorMax = new Vector2(Mathf.Lerp(1f, 2f/3f, minCounter/1.5f), 1);
            //pointCloudImage.rectTransform.sizeDelta = new Vector2(Mathf.Lerp(-1 * Screen.width / 3, 0, minCounter / 1.5f), Mathf.Lerp(-1 * Screen.height / 3, -2*Screen.height/3, minCounter / 1.5f));
            minCounter += Time.deltaTime;
            yield return null;
        }
        pointCloudImage.rectTransform.anchorMin = new Vector2(1f/3f, 1f/3f);
        pointCloudImage.rectTransform.anchorMax = new Vector2(2f/3f, 1);
        cdcImage.enabled = true;
        nationalGuardImage.enabled = true;
    }

    IEnumerator MinimizePatient()
    {
        Debug.Log("coroutine started");
        Debug.Log(patientRect.sizeDelta.x);
        
        float minCounter = 0;
        while (minCounter < 1.5f)
        {
            patientRect.anchorMin = new Vector2(0, Mathf.Lerp(0, 2f/3f, minCounter/1.5f));
            patientRect.anchorMax = new Vector2(Mathf.Lerp(1f, 1f/3f, minCounter/1.5f), 1);
            patientRect.sizeDelta = Vector2.zero;
            //patientRect.sizeDelta = new Vector2(Mathf.Lerp(0, -2*Screen.width/3, minCounter / 1.5f), Mathf.Lerp(0, -2*Screen.height/3, minCounter / 1.5f));
            minCounter += Time.deltaTime;
            yield return null;
        }
        patientRect.anchorMin = new Vector2(0, 2f/3f);
        patientRect.anchorMax = new Vector2(1f/3f, 1);
        pointCloudImage.enabled = true;
        //((MovieTexture)pointCloudImage.texture).Play();

    }

    IEnumerator MaximizePatient()
    {

        float counter = 0;
        while (counter < 1.5f)
        {
            pointCloudImage.rectTransform.anchorMin = new Vector2(Mathf.Lerp(1f/3f, 2f/3f, counter/1.5f), Mathf.Lerp(1f/3f, 2f/3f, counter/1.5f));
            //pointCloudImage.rectTransform.sizeDelta = new Vector2(Mathf.Lerp(0, -1 * Screen.width / 3, counter / 1.5f), Mathf.Lerp(0, -2 * Screen.height / 3, counter / 1.5f));
            //patientRect.sizeDelta = new Vector2(Mathf.Lerp(-2 * Screen.width / 3, -1*Screen.width/3, counter / 1.5f), Mathf.Lerp(-2 * Screen.height / 3, -1*Screen.height/3, counter / 1.5f));
            patientRect.anchorMin = new Vector2(0, Mathf.Lerp(2f/3f, 1f/3f, counter/1.5f));
            patientRect.anchorMax = new Vector2(Mathf.Lerp(1f/3f, 2f/3f, counter/1.5f), 1);

            counter += Time.deltaTime;
            yield return null;
        }
        pointCloudImage.rectTransform.anchorMin = new Vector2(2f/3f, 2f/3f);
        patientRect.anchorMin = new Vector2(0, 1f/3f);
        patientRect.anchorMax = new Vector2(2f/3f, 1);


        showFaceImages = false;
        socialMediaImage.enabled = true;
        facesImage.enabled = false;
        landmarksImage.enabled = false;
        heartbeatGraphImage.enabled = false;
        heartbeatImage.enabled = false;
        socialMediaImage.texture = skeletonTexture;
        ((MovieTexture)socialMediaImage.texture).Play();

        yield return null;
    }

    public void ToggleConference()
    {
        patientRect.gameObject.SetActive(false);
        StartCoroutine(CenterDoctor());
    }

    public void ToggleBrain()
    {
        socialMediaImage.texture = brainTexture;
        ((MovieTexture)socialMediaImage.texture).Play();

    }

    public void ToggleScan()
    {
        socialMediaImage.texture = scanTexture;
        ((MovieTexture)socialMediaImage.texture).Play();
    }

    public void ToggleSkeleton()
    {
        ToggleHeartBeat();
        StartCoroutine(MaximizePatient());
        
    }

    public void ToggleHeartBeat()
    {
        landmarksImage.enabled = true;
        heartbeatGraphImage.enabled = true;
        if (!heartBeatSource.isPlaying)
            heartBeatSource.Play();
        else
            heartBeatSource.Pause();
    }

    public void TogglePointCloud()
    {
        StartCoroutine(MinimizePatient());
    }

    public void ToggleFacialImages()
    {
        ((MovieTexture)socialMediaImage.texture).Pause();
        socialMediaAudioSource.Pause();
        imageTime = Time.realtimeSinceStartup;
        if (faceStage == 0)
        {
            
            socialMediaImage.enabled = false;
            facesImage.enabled = true;
            showFaceImages = true;
        }

        if (faceStage == 1)
        {
            heartbeatImage.enabled = true;
            landmarksImage.enabled = true;
        }

        if(faceStage == 2)
        {
            heartbeatGraphImage.enabled = true;
        }
        faceStage++;
        if (faceStage > 2)
            faceStage = 0;
    }

    public void ToggleSocailMediaMovie()
    {
        if (!socialMediaImage.enabled)
            socialMediaImage.enabled = true;

        if (!((MovieTexture)socialMediaImage.texture).isPlaying)
        {
            ((MovieTexture)socialMediaImage.texture).Play();
            socialMediaAudioSource.Play();
        }
        else
        {
            ((MovieTexture)socialMediaImage.texture).Pause();
            socialMediaAudioSource.Pause();
        }
    }
}
