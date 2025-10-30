using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Gloomhaven;

public class End : IScene
{
    private GraphicsDevice graphicsDevice;
    private SceneManager sceneManager;
    private ContentManager contentManager;

    private SpriteFont _pixelfont;

    public End(GraphicsDevice graphicsDevice, SceneManager sceneManager, ContentManager contentManager)
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
            sceneManager.ChangeScene("menu");
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        graphicsDevice.Clear(Color.Gray);

        int width = graphicsDevice.Viewport.Width;
        int height = graphicsDevice.Viewport.Height;

        Vector2 StoryM = _pixelfont.MeasureString("You have successfully collected the materials from the mansion.");
        Vector2 Story2M = _pixelfont.MeasureString("You wrote the article that won you the award.");
        Vector2 InstructionM = _pixelfont.MeasureString("Press the key E to continue.");

        Vector2 Story = new Vector2((width / 2) - (StoryM.X / 2), (height / 4) - (StoryM.Y / 2));
        Vector2 Story2 = new Vector2((width / 2) - (Story2M.X / 2), (height / 4) - (Story2M.Y / 2) + 50);
        Vector2 Instruction = new Vector2((width / 2) - (InstructionM.X / 2), (height / 4) - (InstructionM.Y / 2) + 100);

        spriteBatch.DrawString(_pixelfont, "You have successfully collected the materials from the mansion.", Story, Color.Orange);
        spriteBatch.DrawString(_pixelfont, "You wrote the article that won you the award.", Story2, Color.Orange);
        spriteBatch.DrawString(_pixelfont, "Press the key E to continue.", Instruction, Color.Orange);
    }
}