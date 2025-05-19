using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class SkipButtonManager : MonoBehaviour
{
    public GameObject skipButton;
    public VideoPlayer videoPlayer;
    public string nextSceneName = "MainScene";

    private bool canSkip = false;

    private void Start()
    {
        skipButton.SetActive(false);
        Invoke(nameof(EnableSkipping), 10f); //mozna skipowac po 10s

        if (videoPlayer != null)
            videoPlayer.loopPointReached += OnVideoFinished;
        else
            Debug.LogWarning("Brakuje VideoPlayera w SkipButtonManager!");
    }

    void EnableSkipping()
    {
        canSkip = true;
        skipButton.SetActive(true);
    }

    private void Update()
    {
        if (canSkip && Input.GetKeyDown(KeyCode.Space))
        {
            SkipVideo();
        }
    }

    public void OnSkipPressed()
    {
        SkipVideo();
    }

    private void SkipVideo()
    {
        SceneManager.LoadScene(nextSceneName);
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
