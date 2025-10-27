using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace Gloomhaven;

public class Garden : IScene
{
    private GraphicsDevice graphicsDevice;
    private SceneManager sceneManager;
    private ContentManager contentManager;

    public Garden(GraphicsDevice graphicsDevice, SceneManager sceneManager, ContentManager contentManager)
    {
        this.graphicsDevice = graphicsDevice;
        this.sceneManager = sceneManager;
        this.contentManager = contentManager;
    }

    public void LoadContent()
    {

    }

    public void Update(GameTime gameTime)
    {

    }

    public void Draw(SpriteBatch spriteBatch)
    {
        
    }
}