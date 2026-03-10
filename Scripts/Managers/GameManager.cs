using UnityEngine.SceneManagement;
using System.Collections;
using DOUKH.Common.Enums;
using UnityEngine;

namespace DOUKH.Managers.Game
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public GameState CurrentState { get => _currentState; private set => _currentState = value; }

        [SerializeField] private GameState _currentState;
        [SerializeField] private bool _pauseOnFocusLost;
        [SerializeField] private bool _logging;
        //public event Action<float> _onLoadingChanged;
        //public event Action<GameState> _onStateChanged;

        public void LoadScene(string sceneName, bool async = false)
        {
            if (async) StartCoroutine(LoadSceneAsyncCoroutine(sceneName));
            else SceneManager.LoadScene(sceneName);
        }

        public void PauseGame()
        {
            if (CurrentState == GameState.Playing)
                SetGameState(GameState.Paused);
            else if (CurrentState == GameState.Paused)
                StartGame();
        }

        public void StartGame() => SetGameState(GameState.Playing);

        //public void GoToMainMenu() => SetGameState(GameState.MainMenu);

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public void ReloadCurrentScene(bool async = false) => LoadScene(SceneManager.GetActiveScene().name, async);

        private void Awake()
        {
            if (Instance == null) { Instance = this; DontDestroyOnLoad(transform.parent); }
            else { Destroy(gameObject); }
        }

        private void Start() => StartGame();

        //private void Start() => SetGameState(GameState.MainMenu);

        private void OnApplicationFocus(bool isFocused) { if (_pauseOnFocusLost && !isFocused && CurrentState == GameState.Playing) PauseGame(); }

        private void OnApplicationPause(bool isPaused) { if (isPaused && CurrentState == GameState.Playing) PauseGame(); }

        private IEnumerator LoadSceneAsyncCoroutine(string sceneName)
        {
            var asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            while (!asyncLoad.isDone)
            {
                //float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
                //_onLoadingChanged?.Invoke(progress);
                //Log($"Loading progress: {progress * 100}%");
                yield return null;
            }
            Log($"Scene {sceneName} loaded successfully");
        }

        private void SetGameState(GameState newState)
        {
            if (CurrentState == newState) return;

            CurrentState = newState;
            //_onStateChanged?.Invoke(newState);
            switch (newState)
            {
                case GameState.Playing:
                    SetTimeScale(1);
                    Log("Playing");
                    break;
                case GameState.Paused:
                    SetTimeScale(0);
                    Log("Pause");
                    break;
                //case GameState.MainMenu:
                //    SetTimeScale(1);
                //    LoadScene("MainMenu");
                //    break;
            }
        }

        private void SetTimeScale(float scale)
        {
            Time.timeScale = Mathf.Clamp(scale, 0f, 1f);
            Log($"Time scale set to: {Time.timeScale}");
        }

        private void Log(string message) { if (_logging) Debug.Log($"[GameManager] {message}"); }
    }
}