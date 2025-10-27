using System.Collections.Generic;

namespace Gloomhaven;

public class SceneManager
{
    private Dictionary<string, IScene> scenes;
    private IScene CurrentScene;

    public SceneManager()
    {
        scenes = new();
    }

    public void AddScene(IScene scene, string name)
    {
        scene.LoadContent();

        scenes[name] = scene;
    }

    public void RemoveScene(string name)
    {
        scenes[name] = null;
    }

    public void ChangeScene(string name)
    {
        CurrentScene = scenes[name];
    }

    public IScene GetCurrentScene()
    {
        return CurrentScene;
    }
}