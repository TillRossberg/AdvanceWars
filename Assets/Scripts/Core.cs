using UnityEngine;
using UnityEngine.SceneManagement;

public class Core : Singleton<Core>
{
    [SerializeField] private Model _model;
    public static Model Model
    {
        get { return Instance._model; }
    }
    [SerializeField] private View _view;
    public static View View
    {
        get { return Instance._view; }
    }
    [SerializeField] private Controller _controller;
    public static Controller Controller
    {
        get { return Instance._controller; }
    }
    [SerializeField] private Manager_Audio _audioManager;
    public static Manager_Audio AudioManager
    {
        get { return Instance._audioManager; }
    }

    private void OnEnable()
    {
        //DontDestroyOnLoad(this.gameObject);
        Model.Init();
        View.Init();
        Controller.Init();
        AudioManager.Init();
    }
   
    private void Start()
    {
        Controller.StartGame();
        Controller.CameraController.Init();
    }
    public void Restart()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }
    public void Quit()
    {
        Application.Quit();
    }
}