using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Gloomhaven;

public class StartGame : IScene
{
    private GraphicsDevice graphicsDevice;
    private SceneManager sceneManager;
    private ContentManager contentManager;

    private SpriteFont _pixelfont;

    public StartGame(GraphicsDevice graphicsDevice, SceneManager sceneManager, ContentManager contentManager)
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
        KeyboardState state = Keyboard.GetState();

        if(state.IsKeyDown(Keys.E))
        {
            sceneManager.AddScene(new Garden(graphicsDevice, sceneManager, contentManager), "garden");
            sceneManager.AddScene(new House1(graphicsDevice, sceneManager, contentManager), "house-floor-1");
            sceneManager.AddScene(new Ghost(graphicsDevice, sceneManager, contentManager), "ghost");
            
            sceneManager.ChangeScene("house-floor-1");
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        graphicsDevice.Clear(Color.Gray);

        int width = graphicsDevice.Viewport.Width;
        int height = graphicsDevice.Viewport.Height;

        Vector2 StoryM = _pixelfont.MeasureString("You are a journalist investigating the decay of the Crowley Mansion.");
        Vector2 Story2M = _pixelfont.MeasureString("But be careful. The mansion is haunted, or so they say.");
        Vector2 InstructionM = _pixelfont.MeasureString("Press the key E to continue.");

        Vector2 Story = new Vector2((width / 2) - (StoryM.X / 2), (height / 4) - (StoryM.Y / 2));
        Vector2 Story2 = new Vector2((width / 2) - (Story2M.X / 2), (height / 4) - (Story2M.Y / 2) + 50);
        Vector2 Instruction = new Vector2((width / 2) - (InstructionM.X / 2), (height / 4) - (InstructionM.Y / 2) + 100);

        spriteBatch.DrawString(_pixelfont, "You are a journalist investigating the decay of the Crowley Mansion.", Story, Color.Orange);
        spriteBatch.DrawString(_pixelfont, "But be careful. The mansion is haunted, or so they say.", Story2, Color.Orange);
        spriteBatch.DrawString(_pixelfont, "Press the key E to continue.", Instruction, Color.Orange);
    }
}