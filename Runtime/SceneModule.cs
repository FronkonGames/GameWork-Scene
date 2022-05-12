////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Martin Bustos @FronkonGames <fronkongames@gmail.com>
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of
// the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using FronkonGames.GameWork.Core;
using FronkonGames.GameWork.Foundation;

namespace FronkonGames.GameWork.Modules.SceneModule
{
  /// <summary>
  /// .
  /// </summary>
  [CreateAssetMenu(fileName = "SceneModule", menuName = "Game:Work/Modules/Scene Module", order = 0)]
  public sealed class SceneModule : ScriptableModule,
                                    IInitializable,
                                    IBeforeSceneLoad
  {
    /// <summary>
    /// Is it initialized?
    /// </summary>
    /// <value>Value</value>
    public bool Initialized { get; set; }

    /// <summary>
    /// Is loading?
    /// </summary>
    public bool IsLoading { get { return isLoading; } }

    /// <summary>
    /// 
    /// </summary>
    /// <value></value>
    public int CurrentSceneBuildIndex { get { return currentSceneBuildIndex; } }

    [SerializeField, Tooltip("Scene to start (>0).")]
    private int startSceneIndex = 1;

    [SerializeField, Tooltip("Background load priority.")]
    private ThreadPriority backgroundLoadingPriority = ThreadPriority.High;

    [SerializeField, Tooltip("Async operations priority.")]
    private int asyncOpPriority = 100;

    [SerializeField, Tooltip("Unload unsued assets?")]
    private bool unloadUnusedAssets = true;

    /// <summary>
    /// Before the additive load.
    /// </summary>
    public static Action OnBeforeLoad;

    /// <summary>
    /// Loading progress (0 .. 1).
    /// </summary>
    public static Action<float> OnProgress;

    /// <summary>
    /// At the end of the scene loading.
    /// </summary>
    public static Action OnAfterLoad;

    [NonSerialized]
    private bool isLoading;

    [NonSerialized]
    private int currentSceneBuildIndex = -1;

    [NonSerialized]
    private ThreadPriority defaultThreadPriority;

    [NonSerialized]
    private Camera cameraMain;

    /// <summary>
    /// When initialize.
    /// </summary>
    public async void OnInitialize()
    {
      defaultThreadPriority = Application.backgroundLoadingPriority;
      cameraMain = GameObject.FindObjectOfType<Camera>();

      if (SceneManager.GetActiveScene().buildIndex == 0)
      {
        if (startSceneIndex > 0)
          await Load(startSceneIndex);
      }
      else
        Log.Error("This module must exist only on scene 0.");
    }

    /// <summary>
    /// At the end of initialization.
    /// Called in the first Update frame.
    /// </summary>
    public void OnInitialized()
    {
      Log.Info($"Scene module initialized: {SceneManager.sceneCountInBuildSettings} scenes found");
    }

    /// <summary>
    /// When deinitialize.
    /// </summary>
    public void OnDeinitialize()
    {
      Application.backgroundLoadingPriority = defaultThreadPriority;
    }

    /// <summary>
    /// Before scene is loaded.
    /// </summary>
    public void OnBeforeSceneLoad()
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    public async void Load(string sceneName)
    {
      int sceneBuildIndex = -1;
      for (int i = 0; i < SceneManager.sceneCountInBuildSettings; ++i)
      {
        string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
        if (string.IsNullOrEmpty(scenePath) == false && scenePath.Length > 6)
        {
          string sceneBuildName = scenePath.Substring(0, scenePath.Length - 6).Substring(scenePath.LastIndexOf('/') + 1);

          if (String.Compare(sceneName, sceneBuildName, StringComparison.OrdinalIgnoreCase) == 0)
          {
            sceneBuildIndex = i;

            break;
          }
        }
      }

      if (sceneBuildIndex > 0)
        await Load(sceneBuildIndex);
      else
        Log.Error($"Scene '{sceneName}' not found");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sceneBuildIndex"></param>
    /// <returns></returns>
    public async Task Load(int sceneBuildIndex)
    {
      if (IsLoading == false)
      {
        if (sceneBuildIndex != currentSceneBuildIndex && sceneBuildIndex < SceneManager.sceneCountInBuildSettings)
        {
          isLoading = true;

          OnBeforeLoad?.Invoke();

          Application.backgroundLoadingPriority = backgroundLoadingPriority;

          // @TODO: Fade in.

          if (currentSceneBuildIndex != -1)
          {
            if (cameraMain != null)
              cameraMain.gameObject.SetActive(true);

            await UnloadAsync();
          }

          await LoadAsync(sceneBuildIndex);

          Application.backgroundLoadingPriority = defaultThreadPriority;

          isLoading = false;

          OnAfterLoad?.Invoke();
        }
        else
          Log.Error($"Scene {sceneBuildIndex} is already loaded or not found");
      }
      else
        Log.Error("Scene loading in progress");
    }

    private async Task LoadAsync(int sceneBuildIndex)
    {
      AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneBuildIndex, LoadSceneMode.Additive);
      asyncOp.allowSceneActivation = false;
      asyncOp.priority = asyncOpPriority;

      // @HACK: The final 10% is to wakeup the main thread.
      while (asyncOp.progress < 0.9f)
      {
        // [0, 0.9] > [0, 1].
        float progress = Mathf.Clamp01(asyncOp.progress / 0.9f);

        // @TODO: Progressbar.

        OnProgress?.Invoke(progress);
        
        await Task.Delay(10);
      }

      asyncOp.allowSceneActivation = true;

      if (cameraMain != null)
        cameraMain.gameObject.SetActive(false);

      // @HACK: The activation of the scene takes a frame.
      //yield return waitForFixedUpdate;
      //SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(sceneBuildIndex));

      currentSceneBuildIndex = sceneBuildIndex;
    }

    private async Task UnloadAsync()
    {
      if (currentSceneBuildIndex != -1)
      {
        AsyncOperation asyncOp = SceneManager.UnloadSceneAsync(currentSceneBuildIndex);
        asyncOp.allowSceneActivation = false;
        asyncOp.priority = asyncOpPriority;

        // @HACK: The final 10% is to wakeup the main thread.
        while (asyncOp.allowSceneActivation == true ? asyncOp.isDone == false : asyncOp.progress < 0.9f)
          await Task.Delay(10);

        asyncOp.allowSceneActivation = true;

        if (unloadUnusedAssets == true)
          Resources.UnloadUnusedAssets();

        currentSceneBuildIndex = -1;
      }
      else
        Log.Warning("No scene loaded");
    }
  }
}
