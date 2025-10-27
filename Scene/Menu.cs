using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace Gloomhaven;

public class Menu : IScene
{
    private GraphicsDevice graphicsDevice;
    private SceneManager sceneManager;
    private ContentManager contentManager;

    private SpriteFont _pixelfont;
    
    private bool _Blinking = false;

    private int _Selected = 0;
    private float _KeyCooldown = 300;
    private float _BlinkCooldown = 300;

    public Menu(GraphicsDevice graphicsDevice, SceneManager sceneManager, ContentManager contentManager)
    {
        this.graphicsDevice = graphicsDevice;
        this.sceneManager = sceneManager;
        this.contentManager = contentManager;
    }

    public void LoadContent()
    {
        _pixelfont = contentManager.Load<SpriteFont>("pixelfont");
    }

    public void Update(GameTime gameTime)
    {
        float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds * 1000;
        KeyboardState state = Keyboard.GetState();

        if(_KeyCooldown >= 0)
        {
            _KeyCooldown -= elapsed;
        }

        if(_BlinkCooldown >= 0)
        {
            _BlinkCooldown -= elapsed;
        }

        if(_Blinking == true && _BlinkCooldown <= 0)
        {
            _Blinking = false;
            _BlinkCooldown = 300;
        }

        if(_Blinking == false && _BlinkCooldown <= 0)
        {
            _Blinking = true;
            _BlinkCooldown = 300;
        }

        if(_KeyCooldown <= 0)
        {
            if(state.IsKeyDown(Keys.Up))
            {
                if(_Selected == 0)
                {
                    _Selected = 1;
                    _KeyCooldown = 300;
                } else {
                    _Selected = 0;
                    _KeyCooldown = 300;
                }
            }

            if(state.IsKeyDown(Keys.Down))
            {
                if(_Selected == 0)
                {
                    _Selected = 1;
                    _KeyCooldown = 300;
                } else {
                    _Selected = 0;
                    _KeyCooldown = 300;
                }
            }

            if(state.IsKeyDown(Keys.Enter))
            {
                if(_Selected == 0)
                {

                } else {
                    new Game1().Exit();
                }
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        graphicsDevice.Clear(Color.Gray);

        int width = graphicsDevice.Viewport.Width;
        int height = graphicsDevice.Viewport.Height;

        Vector2 LabelM = _pixelfont.MeasureString("Gloomhaven");
        Vector2 Label = new Vector2((width / 2) - (LabelM.X / 2), (height / 4) - (LabelM.Y / 2));

        spriteBatch.DrawString(_pixelfont, "Gloomhaven", Label, Color.Orange);

        string playText = _Selected == 0 && _Blinking ? "> Play <" : "  Play  ";
        Vector2 PlayM = _pixelfont.MeasureString(playText);
        Vector2 Play = new Vector2((width / 2) - (PlayM.X / 2), (height / 4) - (PlayM.Y / 2) + 100);

        spriteBatch.DrawString(_pixelfont, playText, Play, Color.Orange);

        string quitText = _Selected == 1 && _Blinking ? "> Quit <" : "  Quit  ";
        Vector2 QuitM = _pixelfont.MeasureString(quitText);
        Vector2 Quit = new Vector2((width / 2) - (QuitM.X / 2), (height / 4) - (QuitM.Y / 2) + 150);

        spriteBatch.DrawString(_pixelfont, quitText, Quit, Color.Orange);
    }
}