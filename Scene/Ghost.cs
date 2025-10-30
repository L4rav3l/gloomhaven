using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Gloomhaven;

public class Ghost : IScene
{
    private GraphicsDevice graphicsDevice;
    private SceneManager sceneManager;
    private ContentManager contentManager;

    private Texture2D GhostTexture;
    private Song Jumpscare;

    private double Countdown;
    private bool Played = false;

    public Ghost(GraphicsDevice graphicsDevice, SceneManager sceneManager, ContentManager contentManager)
    {
        this.graphicsDevice = graphicsDevice;
        this.sceneManager = sceneManager;
        this.contentManager = contentManager;

        Countdown = 4000;
    }

    public void LoadContent()
    {
        GhostTexture = contentManager.Load<Texture2D>("ghost_1");
        Jumpscare = contentManager.Load<Song>("jumpscare");
    }

    public void Update(GameTime gameTime)
    {
        double elapsed = gameTime.ElapsedGameTime.TotalSeconds * 1000;

        if(Countdown >= 0)
        {
            Countdown -= elapsed;
        }

        if(Countdown <= 0)
        {
            sceneManager.ChangeScene("menu");
        }

        if(Played == false)
        {
            MediaPlayer.Play(Jumpscare);
            MediaPlayer.IsRepeating = false;

            Played = true;
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        int Width = graphicsDevice.Viewport.Width;
        int Height = graphicsDevice.Viewport.Height;

        graphicsDevice.Clear(Color.Black);

        Vector2 GhostPos = new Vector2((Width / 2) - 440, (Height / 2) - 480);

        spriteBatch.Draw(GhostTexture, GhostPos, null, Color.White, 0f, Vector2.Zero, 15f, SpriteEffects.None, 0.1f);
    }
}