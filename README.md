<p align="center"><img src="Documentation/Animation.gif" /></p>
<p align="center"><b>Async Scene Loading Management</b></p>

<br>
<p align="center">
  <a style="text-decoration:none">
    <img src="https://img.shields.io/github/package-json/v/FronkonGames/GameWork-Scene-Module?style=flat-square" alt="version" />
  </a>  
  <a style="text-decoration:none">
    <img src="https://img.shields.io/github/license/FronkonGames/GameWork-Scene-Module?style=flat-square" alt="license" />
  </a>
  <a style="text-decoration:none">
    <img src="https://img.shields.io/github/languages/top/FronkonGames/GameWork-Scene-Module?style=flat-square" alt="top language" />
  </a>
</p>

<p align="center"><b>⚠️Still In Early Development ⚠️<b/></p>
## 🔧 Requisites

- Unity 2020.3 or higher.
- [Game:Work Core](https://github.com/FronkonGames/GameWork-Core).
- [Game:Work Foundation](https://github.com/FronkonGames/GameWork-Foundation).
- Test Framework 1.1.31 or higher.

## 🚀 Installation

### Editing your 'manifest.json'

- Open the manifest.json file of your Unity project.
- In the section "dependencies" add:

```
{
  ...
  "dependencies":
  {
    ...
    "FronkonGames.GameWork.Modules.Scene": "git+https://github.com/FronkonGames/GameWork-Scene.git",
    "FronkonGames.GameWork.Core": "git+https://github.com/FronkonGames/GameWork-Core.git",
    "FronkonGames.GameWork.Foundation": "git+https://github.com/FronkonGames/GameWork-Foundation.git"
  }
  ...
}
```

## 📜 License

Code released under [MIT License](https://github.com/FronkonGames/GameWork-Scene-Module/blob/main/LICENSE).