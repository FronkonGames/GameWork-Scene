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
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using FronkonGames.GameWork.Core;
using FronkonGames.GameWork.Foundation;

namespace FronkonGames.GameWork.Modules.SceneModule
{
  /// <summary>
  /// .
  /// </summary>
  public sealed class SceneModule : MonoBehaviourModule,
                                    IInitializable
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
    public string Title
    {
      get { return tittleText != null ? tittleText?.text : string.Empty; }
      set
      {
        if (tittleText != null)
          tittleText.text = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <value></value>
    public string Tooltip
    {
      get { return tooltipText != null ? tooltipText?.text : string.Empty; }
      set
      {
        if (tooltipText != null)
          tooltipText.text = value;
      }
    }

    /// <summary>
    /// Current scene build index.
    /// </summary>
    /// <value></value>
    public int CurrentSceneBuildIndex { get { return sceneBuildIndex; } }

    [SerializeField, SceneBuildIndex(1, "Scene to start (>0).")]
    private int startSceneIndex = 1;

    [SerializeField, Enum((int)ThreadPriority.High, "Background load priority.")]
    private ThreadPriority backgroundLoadingPriority = ThreadPriority.High;

    [SerializeField, Slider(0, 100, 100, "Async operations priority.")]
    private int asyncOpPriority = 100;

    [SerializeField, Bool("Unload unsued assets?")]
    private bool unloadUnusedAssets = true;

    [SerializeField, Slider(0.0f, 2.0f, 0.25f, "Fade in/out time in seconds.")]
    private float fadeTime = 0.25f;

    [SerializeField, CanvasGroup("Loading canvas.")]
    private CanvasGroup canvasGroup = null;

    [SerializeField, Int(100, "Canvas sort order.")]
    private int canvasSortOrder = 100;

    [SerializeField, Color(0.0f, 0.0f, 0.0f, 1.0f, "Background color.")]
    private Color backgroundColor = Color.black;

    [SerializeField, Image("Loading image.")]
    private Image backgroundImage = null;

    [SerializeField, Text("Title.")]
    private Text tittleText = null;

    [SerializeField, Text("Tooltip text.")]
    private Text tooltipText = null;

    [SerializeField, Image("Progress background image.")]
    private Image progressBackgroundImage = null;

    [SerializeField, Image("Progress image.")]
    private Image progressForegroundImage = null;

    [SerializeField, Text("Progress text.")]
    private Text progressText = null;

    [SerializeField, String("Resources path.")]
    private string backgroundImagePath;

    [SerializeField, Float(0.0f, 10.0f, 0.0f, "Wait extra time after the scene load.")]
    private float waitExtraTime = 0.0f;

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

    private bool isLoading;

    private int sceneBuildIndex = -1;

    private ThreadPriority defaultThreadPriority;

    private Camera cameraMain;

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public string GetLoadingTranslated()
    {
      string loading = "Loading";
      switch (Application.systemLanguage)
      {
        case SystemLanguage.Afrikaans:      loading = "Laai"; break;
        case SystemLanguage.Arabic:         loading = "جار التحميل"; break;
        case SystemLanguage.Basque:         loading = "Lanean"; break;
        case SystemLanguage.Belarusian:     loading = "Загрузка"; break;
        case SystemLanguage.Bulgarian:      loading = "Зареждане"; break;
        case SystemLanguage.Catalan:        loading = "Carregant"; break;
        case SystemLanguage.Chinese:        loading = "装载"; break;
        case SystemLanguage.Czech:          loading = "Načítání"; break;
        case SystemLanguage.Danish:         loading = "Indlæser"; break;
        case SystemLanguage.Dutch:          loading = "Het laden"; break;
        case SystemLanguage.English:        loading = "Loading"; break;
        case SystemLanguage.Estonian:       loading = "Laadimine"; break;
        // case SystemLanguage.Faroese: lo  adingString = ""; break;  ????
        case SystemLanguage.Finnish:        loading = "Ladataan"; break;
        case SystemLanguage.French:         loading = "Chargement"; break;
        case SystemLanguage.German:         loading = "Wird geladen"; break;
        case SystemLanguage.Greek:          loading = "Φόρτωση"; break;
        case SystemLanguage.Hebrew:         loading = "טעינה"; break;
        case SystemLanguage.Hungarian:      loading = "Betöltés"; break;
        case SystemLanguage.Icelandic:      loading = "Hleðsla"; break;
        case SystemLanguage.Indonesian:     loading = "Pemuatan"; break;
        case SystemLanguage.Italian:        loading = "Caricamento in corso"; break;
        case SystemLanguage.Japanese:       loading = "ロード中"; break;
        case SystemLanguage.Korean:         loading = "로드 중"; break;
        case SystemLanguage.Latvian:        loading = "Notiek ielāde"; break;
        case SystemLanguage.Lithuanian:     loading = "Įkeliama"; break;
        case SystemLanguage.Norwegian:      loading = "Lasting"; break;
        case SystemLanguage.Polish:         loading = "Ładuję"; break;
        case SystemLanguage.Portuguese:     loading = "Carregando"; break;
        case SystemLanguage.Romanian:       loading = "Se incarca"; break;
        case SystemLanguage.Russian:        loading = "Загрузка"; break;
        case SystemLanguage.SerboCroatian:  loading = "Лоадинг"; break;
        case SystemLanguage.Slovak:         loading = "Načítava"; break;
        case SystemLanguage.Slovenian:      loading = "Nalaganje"; break;
        case SystemLanguage.Spanish:        loading = "Cargando"; break;
        case SystemLanguage.Swedish:        loading = "Läser in"; break;
        case SystemLanguage.Thai:           loading = "กำลังโหลด"; break;
        case SystemLanguage.Turkish:        loading = "Yükleniyor"; break;
        case SystemLanguage.Ukrainian:      loading = "Завантаження"; break;
        case SystemLanguage.Vietnamese:     loading = "Tải"; break;
      }

      return loading;
    }

    /// <summary>
    /// When initialize.
    /// </summary>
    public void OnInitialize()
    {
      defaultThreadPriority = Application.backgroundLoadingPriority;
      cameraMain = GameObject.FindObjectOfType<Camera>();
      cameraMain.backgroundColor = backgroundColor;

      if (SceneManager.GetActiveScene().buildIndex == 0)
      {
        if (startSceneIndex > 0)
          Load(startSceneIndex);
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
    }

    /// <summary>
    /// When deinitialize.
    /// </summary>
    public void OnDeinitialize()
    {
      Application.backgroundLoadingPriority = defaultThreadPriority;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    public void Load(string sceneName)
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
        Load(sceneBuildIndex);
      else
        Log.Error($"Scene '{sceneName}' not found");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sceneBuildIndex"></param>
    /// <returns></returns>
    public async void Load(int sceneBuildIndex) => await LoadAsync(sceneBuildIndex);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sceneBuildIndex"></param>
    /// <returns></returns>
    private async Task LoadAsync(int sceneBuildIndex)
    {
      if (IsLoading == false)
      {
        if (sceneBuildIndex != this.sceneBuildIndex && sceneBuildIndex < SceneManager.sceneCountInBuildSettings)
        {
          isLoading = true;

          OnBeforeLoad?.Invoke();

          Application.backgroundLoadingPriority = backgroundLoadingPriority;

          await LoadUI();

          await Fade(0.0f, 1.0f, fadeTime, (fade) => canvasGroup.alpha = fade);

          if (tittleText != null)
            await Fade(0.0f, 1.0f, fadeTime * 0.5f, (fade) => tittleText.color = tittleText.color.SetA(fade), fadeTime);
          
          if (tooltipText != null)
            await Fade(0.0f, 1.0f, fadeTime * 0.5f, (fade) => tooltipText.color = tooltipText.color.SetA(fade), fadeTime);

          if (progressBackgroundImage != null)
            await Fade(0.0f, 1.0f, fadeTime * 0.5f, (fade) => progressBackgroundImage.color = progressBackgroundImage.color.SetA(fade), fadeTime);

          cameraMain?.gameObject.SetActive(true);

          if (this.sceneBuildIndex != -1)
            await UnloadAsync();

          AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneBuildIndex, LoadSceneMode.Additive);
          asyncOp.allowSceneActivation = false;
          asyncOp.priority = asyncOpPriority;

#if FAKE_PROGRESS
          // @HACK: The final 10% is to wakeup the main thread.
          while (asyncOp.progress < 0.9f)
            await Awaiters.NextUpdate();

          float loadingTime = Rand.Range(1.0f, 2.0f);
          float time = 0.0f;
          while (time <= loadingTime)
          {
            float progress = time / loadingTime;

            if (progressForegroundImage != null)
              progressForegroundImage.fillAmount = progress;

            OnProgress?.Invoke(progress);

            time += Time.unscaledDeltaTime;

            await Awaiters.NextUpdate();
          }
#else
          // @HACK: The final 10% is to wakeup the main thread.
          while (asyncOp.progress < 0.9f)
          {
            // [0, 0.9] > [0, 1].
            float progress = Mathf.Clamp01(asyncOp.progress / 0.9f);

            if (progressForegroundImage != null)
              progressForegroundImage.fillAmount = progress;

            OnProgress?.Invoke(progress);

            await Awaiters.Seconds(0.01f);
          }
#endif

          cameraMain?.gameObject.SetActive(false);

          await Awaiters.NextFixedUpdate();

          asyncOp.allowSceneActivation = true;

          await Awaiters.NextFixedUpdate();

          this.sceneBuildIndex = sceneBuildIndex;
          SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(sceneBuildIndex));

          if (waitExtraTime > 0.0f)
            await Awaiters.Seconds(waitExtraTime);

          await Fade(1.0f, 0.0f, fadeTime, (fade) => canvasGroup.alpha = fade);

          UnloadUI();

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

    private async Task UnloadAsync()
    {
      if (sceneBuildIndex != -1)
      {
        AsyncOperation asyncOp = SceneManager.UnloadSceneAsync(sceneBuildIndex);
        asyncOp.allowSceneActivation = false;
        asyncOp.priority = asyncOpPriority;

        // @HACK: The final 10% is to wakeup the main thread.
        while (asyncOp.allowSceneActivation == true ? asyncOp.isDone == false : asyncOp.progress < 0.9f)
          await Task.Delay(10);

        asyncOp.allowSceneActivation = true;

        if (unloadUnusedAssets == true)
          Resources.UnloadUnusedAssets();

        sceneBuildIndex = -1;
      }
      else
        Log.Warning("No scene loaded");
    }

    private async Task LoadUI()
    {
      if (canvasGroup != null)
      {
        if (string.IsNullOrEmpty(backgroundImagePath) == false)
        {
          ResourceRequest request = Resources.LoadAsync<Sprite>(backgroundImagePath);
          while (request.isDone == false)
            await Task.Delay(10);

          if (request.asset != null)
          {
            backgroundImage.sprite = request.asset as Sprite;
            backgroundImage.color = Color.white;
          }
          else
            Log.Error($"Texture '{backgroundImagePath}' not found");
        }

        canvasGroup.gameObject.SetActive(true);
        canvasGroup.alpha = 0.0f;

        Canvas canvas = canvasGroup.gameObject.GetComponent<Canvas>();
        if (canvas != null)
          canvas.sortingOrder = canvasSortOrder;

        if (tittleText != null)
          tittleText.color = tittleText.color.SetA(0.0f);

        if (tooltipText != null)
          tooltipText.color = tooltipText.color.SetA(0.0f);

        if (progressBackgroundImage != null)
          progressBackgroundImage.color = progressBackgroundImage.color.SetA(0.0f);

        if (progressForegroundImage != null)
          progressForegroundImage.fillAmount = 0.0f;
      }
    }

    private async Task Fade(float start, float end, float duration, Action<float> fade, float delay = 0.0f)
    {
      fade.Invoke(start);

      if (delay > 0.0f)
        await Awaiters.Seconds(delay);

      if (duration > 0.0f)
      {
        float time = 0.0f;
        while (time < duration)
        {
          time += Time.unscaledDeltaTime;

          fade.Invoke(Mathf.Lerp(start, end, time / duration));

          await Awaiters.NextUpdate();
        }
      }

      fade.Invoke(end);
    }

    private void UnloadUI()
    {
      if (canvasGroup != null)
      {
        if (canvasGroup != null)
          canvasGroup.gameObject.SetActive(false);

        if (backgroundImage != null)
        {
          Resources.UnloadAsset(backgroundImage.sprite);
          backgroundImage.sprite = null;
        }
      }
    }
  }
}
