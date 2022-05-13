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
using UnityEngine;
using FronkonGames.GameWork.Core;

namespace FronkonGames.GameWork.Modules.SceneModule
{
  /// <summary>
  /// .
  /// </summary>
  public sealed class TestModuleLogic : IInitializable,
                                        IUpdatable
  {
    private SceneModule sceneModule;

    /// <summary>
    /// Should be updated?
    /// </summary>
    /// <value>True/false.</value>
    public bool ShouldUpdate { get; set; } = true;

    /// <summary>
    /// Is it initialized?
    /// </summary>
    /// <value>Value</value>
    public bool Initialized { get; set; }

    /// <summary>
    /// When initialize.
    /// </summary>
    public void OnInitialize()
    {
      sceneModule = GameTest.Instance.GetModule<SceneModule>();
    }

    /// <summary>
    /// At the end of initialization.
    /// Called in the first Update frame.
    /// </summary>
    public void OnInitialized() { }

    /// <summary>
    /// When deinitialize.
    /// </summary>
    public void OnDeinitialize() { }

    /// <summary>
    /// Update event.
    /// </summary>
    public void OnUpdate()
    {
      if (sceneModule != null && sceneModule.IsLoading == false)
      {
        int currentSceneBUildIndex = sceneModule.CurrentSceneBuildIndex;

        if (currentSceneBUildIndex != 1 && Input.GetKeyUp(KeyCode.Alpha1) == true)
          sceneModule.Load("Menu");
        else if (currentSceneBUildIndex != 2 && Input.GetKeyUp(KeyCode.Alpha2) == true)
          sceneModule.Load("Level1");
        else if (currentSceneBUildIndex != 3 && Input.GetKeyUp(KeyCode.Alpha3) == true)
          sceneModule.Load("Level2");
      }
    }

    /// <summary>
    /// FixedUpdate event.
    /// </summary>
    public void OnFixedUpdate() { }

    /// <summary>
    /// LateUpdate event.
    /// </summary>
    public void OnLateUpdate() { }
  }
}
