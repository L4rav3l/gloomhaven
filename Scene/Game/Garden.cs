using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using TiledSharp;

namespace Gloomhaven;

public class Garden : IScene
{
    private GraphicsDevice graphicsDevice;
    private SceneManager sceneManager;
    private ContentManager contentManager;

    private TmxMap map;
    private SpriteFont pixelfont;
    private Texture2D tileset;
    private Texture2D playerTexture;
    private Texture2D key;
    private Texture2D end;
    private Player player;

    private bool KeyInHand = false;
    private bool Closed = false;
    private bool opened = false;

    private int progress = 0;
    private double ClosedCountdown = 0;

    private string[] Task = new string[3];

    private Camera2D camera;

    private List<Rectangle> solidTiles;

    public List<Rectangle> LoadCollisionObjects(string mapFilePath)
    {
        var map = new TmxMap(mapFilePath);
        var solidTiles = new List<Rectangle>();

        foreach (var objectGroup in map.ObjectGroups)
        {
            if (objectGroup.Name == "Collision")
            {
                foreach (var obj in objectGroup.Objects)
                {
                    if (obj.Width > 0 && obj.Height > 0)
                    {
                        var rect = new Rectangle(
                            (int)obj.X,
                            (int)obj.Y,
                            (int)obj.Width,
                            (int)obj.Height
                        );

                        solidTiles.Add(rect);
                    }
                }
            }
        }

        return solidTiles;
    }

    public Garden(GraphicsDevice graphicsDevice, SceneManager sceneManager, ContentManager contentManager)
    {
        this.graphicsDevice = graphicsDevice;
        this.sceneManager = sceneManager;
        this.contentManager = contentManager;

        camera = new Camera2D(graphicsDevice.Viewport);

        Task[0] = "Object: Search for the mansion key in the garden";
        Task[1] = "Object: Go in through the main door.";
        Task[2] = " ";

        end = new Texture2D(graphicsDevice, 1, 1);
        end.SetData(new[] { Color.Yellow }); 
    }

    public void LoadContent()
    {
        map = new TmxMap("Content/garden.tmx");
        solidTiles = LoadCollisionObjects("Content/garden.tmx");

        tileset = contentManager.Load<Texture2D>("tilesmap");
        key = contentManager.Load<Texture2D>("key");
        pixelfont = contentManager.Load<SpriteFont>("pixelfont");

        playerTexture = contentManager.Load<Texture2D>("player");
        player = new Player(new Vector2(814, 1167));
    }

    public void Update(GameTime gameTime)
    {
        player.Update(gameTime, solidTiles, camera);

        Vector2 worldSize = new Vector2(map.Width * map.TileWidth, map.Height * map.TileHeight);

        camera.Follow(player.Position, worldSize);

        KeyboardState state = Keyboard.GetState();

        double elapsed = gameTime.ElapsedGameTime.TotalSeconds * 1000;

        if(ClosedCountdown >= 0)
        {
            ClosedCountdown -= elapsed;
        } else {
            Closed = false;
        }

        if(state.IsKeyDown(Keys.E))
        {
            if(Vector2.Distance(player.screenPos, camera.WorldToScreen(new Vector2(1752, 1117))) <= 80 && KeyInHand == false)
            {
                KeyInHand = true;
                progress++;
            }
            
            if(Vector2.Distance(player.screenPos, camera.WorldToScreen(new Vector2(830, 633))) <= 70)
            {
                if(KeyInHand == true)
                {
                    sceneManager.ChangeScene("house-floor-1");
                    progress++;
                    KeyInHand = false;
                    opened = true;
                }  else if(opened == true)
                {
                    GameData.PlayerVector = new Vector2(1895, 3045);
                    sceneManager.ChangeScene("house-floor-1");
                } else {
                    Closed = true;
                    ClosedCountdown = 500;
                }
            }
        }

        if(Vector2.Distance(camera.WorldToScreen(new Vector2((int)(833 - 50), (int)(1181 - 50))), player.screenPos) <= 64 && GameData.End == true)
        {
            sceneManager.ChangeScene("end");
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {

        graphicsDevice.Clear(Color.Black);

        int mapWidth = map.Width;
        int mapHeight = map.Height;

        int Width = graphicsDevice.Viewport.Width;
        int Height = graphicsDevice.Viewport.Height;

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                int i = y * mapWidth + x;
                if (i >= map.Layers[0].Tiles.Count) continue;

                var tile = map.Layers[0].Tiles[i];
                if (tile.Gid == 0) continue;

                int tilesPerRow = tileset.Width / map.TileWidth;
                int tileIndex = tile.Gid - 1;

                int tileIndexX = tileIndex % tilesPerRow;
                int tileIndexY = tileIndex / tilesPerRow;

                Rectangle source = new Rectangle(
                    tileIndexX * map.TileWidth,
                    tileIndexY * map.TileHeight,
                    map.TileWidth,
                    map.TileHeight
                );

                Vector2 worldPosition = new Vector2(x * map.TileWidth, y * map.TileHeight);
                Vector2 screenPosition = camera.WorldToScreen(worldPosition);

                spriteBatch.Draw(tileset, screenPosition, source, Color.White * 0.3f);
            }
        }

        player.Draw(spriteBatch, playerTexture, camera);

        Vector2 ObjectM = pixelfont.MeasureString(Task[progress]);
        Vector2 Object = new Vector2((int)((Width / 2) - ((ObjectM.X / 2) * 0.5)),(int)(50 - ((ObjectM.Y / 2) * 0.5)));

        spriteBatch.DrawString(pixelfont, Task[progress], Object, Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0.3f);

        if(KeyInHand == true)
        {
            spriteBatch.Draw(key, new Vector2(player.screenPos.X + 45, player.screenPos.Y + 30), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.4f);
        }

        if(Closed == true)
        {
            Vector2 ClosedM = pixelfont.MeasureString("CLOSED");
            spriteBatch.DrawString(pixelfont, "CLOSED", new Vector2(Width / 2 - (ClosedM.X / 2), Height / 4 - (ClosedM.Y / 2)), Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0.5f);
        }

        if(GameData.End == true)
        {
            Vector2 rectanglepos = camera.WorldToScreen(new Vector2((int)(833 - 50), (int)(1181 - 50)));
            spriteBatch.Draw(end, new Rectangle((int)rectanglepos.X, (int)rectanglepos.Y, 100, 100), null, Color.Yellow, 0f, Vector2.Zero, SpriteEffects.None, 0.2f);
        }
    }
}

