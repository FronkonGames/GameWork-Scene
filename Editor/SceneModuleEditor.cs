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
using UnityEditor;
using FronkonGames.GameWork.Foundation;

namespace FronkonGames.GameWork.Modules.SceneModule
{
  /// <summary>
  /// .
  /// </summary>
  [CustomEditor(typeof(SceneModule))]
  public sealed class SceneModuleEditor : Inspector
  {
    /// <summary>
    /// 
    /// </summary>
    protected override void InspectorGUI()
    {
      int scene = SceneField("startSceneIndex", "Start Scene");
      if (scene == 0)
        EditorGUILayout.HelpBox("The starting scene cannot be the first.", MessageType.Error);

      Header("Settings");

      EnumField("backgroundLoadingPriority", "Loading Priority");
      SliderField("asyncOpPriority", "Async Priority");
      BoolField("unloadUnusedAssets");

      Header("UI");

      SliderField("fadeTime");
      CanvasGroupField("canvasGroup");
      ColorField("backgroundColor");
      ImageField("backgroundUIColor");
      ImageField("backgroundImage");
      TextField("tittleText");
      TextField("tooltipText");
      ImageField("progressBackgroundImage");
      ImageField("progressForegroundImage");
      TextField("progressText");
      StringField("backgroundImagePath");
      FloatField("waitExtraTime");
    }
  }
}
