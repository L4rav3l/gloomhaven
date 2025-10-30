using System;
using System.Collections.Generic;
using TiledSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace Gloomhaven;

public class Upstair : IScene
{
    private GraphicsDevice graphicsDevice;
    private SceneManager sceneManager;
    private ContentManager contentManager;
    
    private TmxMap map;

    private Texture2D playerTexture;
    private Texture2D Ghost1;
    private Texture2D Ghost2;
    private Texture2D TileSet;
    private Texture2D codePaper;

    private Song GhostSong;

    private SpriteFont pixelfont;

    private Vector2 Ghost;

    private bool route;
    private bool code;
    private bool proof = false;

    private double GhostSongCooldown;
    
    private string[] Task = new string[4];

    private Player player;
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

    public Upstair(GraphicsDevice graphicsDevice, SceneManager sceneManager, ContentManager contentManager)
    {
        this.graphicsDevice = graphicsDevice;
        this.sceneManager = sceneManager;
        this.contentManager = contentManager;

        Ghost = new Vector2(830, 1247);
        route = false;

        codePaper = new Texture2D(graphicsDevice, 1, 1);
        codePaper.SetData(new [] {Color.White});
    }

    public void LoadContent()
    {
        player = new Player(new Vector2(1279, 1340));
        camera = new Camera2D(graphicsDevice.Viewport);

        map = new TmxMap("Content/upstair.tmx");

        playerTexture = contentManager.Load<Texture2D>("player");
        Ghost1 = contentManager.Load<Texture2D>("ghost_1");
        Ghost2 = contentManager.Load<Texture2D>("ghost_2");
        GhostSong = contentManager.Load<Song>("ghost-in-near");
        TileSet = contentManager.Load<Texture2D>("tilesmap");

        solidTiles = LoadCollisionObjects("Content/upstair.tmx");

        pixelfont = contentManager.Load<SpriteFont>("pixelfont");

        Task[0] = "Object: Find evidence 0/3";
        Task[1] = "Object: Find evidence 1/3";
        Task[2] = "Object: Find evidence 2/3";
        Task[3] = "Object: Leave the mansion.";
    }

    public void Update(GameTime gameTime)
    {
        if(GameData.PlayerVector != null)
        {
            player.Position = GameData.PlayerVector.Value;
            GameData.PlayerVector = null;
        }

        player.Update(gameTime, solidTiles, camera);
        camera.Follow(player.Position, new Vector2(map.Width * 64, map.Height * 64));

        double elapsed = gameTime.ElapsedGameTime.TotalSeconds * 1000;
        Vector2 GhostPosition = camera.WorldToScreen(Ghost);

        if(GhostSongCooldown >= 0)
        {
            GhostSongCooldown -= elapsed;
        } else if(Vector2.Distance(GhostPosition, player.screenPos) <= 320)
        {
            GhostSongCooldown = 10000;
            MediaPlayer.Play(GhostSong);
            MediaPlayer.IsRepeating = false;
        }

        if(Ghost.Y == 1247 && route == false)
        {
            route = true;
        } else if(route == false)
        {
            Ghost.Y++;
        }

        if(Ghost.Y == 804 && route == true)
        {
            route = false;
        } else if(route == true)
        {
            Ghost.Y--;
        }

        if(Vector2.Distance(player.screenPos, GhostPosition) <= 48)
        {
            sceneManager.ChangeScene("ghost");
        }

        KeyboardState state = Keyboard.GetState();

        if(state.IsKeyDown(Keys.E))
        {
            if(Vector2.Distance(player.screenPos, camera.WorldToScreen(new Vector2(992, 729))) <= 64 && proof == false)
            {
                GameData.Task++;
                proof = true;
            }

            if(Vector2.Distance(player.screenPos, camera.WorldToScreen(new Vector2(1501, 539))) <= 64)
            {
                code = true;
                GameData.Move = false;
            }
        }

        if(state.IsKeyDown(Keys.Escape))
        {
            if(Vector2.Distance(player.screenPos, camera.WorldToScreen(new Vector2(1501, 539))) <= 64)
            {
                code = false;
                GameData.Move = true;
            }
        }

        if(Vector2.Distance(player.screenPos, camera.WorldToScreen(new Vector2(1280, 1460))) <= 100)
        {
            sceneManager.ChangeScene("house-floor-1");
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        int mapWidth = map.Width;
        int mapHeight = map.Height;

        int Width = graphicsDevice.Viewport.Width;
        int Height = graphicsDevice.Viewport.Height;

        graphicsDevice.Clear(Color.Black);

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                int i = y * mapWidth + x;
                if (i >= map.Layers[0].Tiles.Count) continue;

                var tile = map.Layers[0].Tiles[i];
                if (tile.Gid == 0) continue;

                int tilesPerRow = TileSet.Width / map.TileWidth;
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

                spriteBatch.Draw(TileSet, screenPosition, source, Color.White * 0.3f);
            }
        }

        Vector2 GhostPosition = camera.WorldToScreen(Ghost);

        player.Draw(spriteBatch, playerTexture, camera);

        if(route == false)
        {
            spriteBatch.Draw(Ghost1, GhostPosition, null, Color.White * 0.15f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.5f);
        } else {
            spriteBatch.Draw(Ghost2, GhostPosition, null, Color.White * 0.15f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.5f);
        }

        Vector2 ObjectM = pixelfont.MeasureString(Task[GameData.Task]);
        Vector2 Object = new Vector2((int)((Width / 2) - ((ObjectM.X / 2) * 0.5)),(int)(50 - ((ObjectM.Y / 2) * 0.5)));

        spriteBatch.DrawString(pixelfont, Task[GameData.Task], Object, Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0.7f);

        if(code == true)
        {
            spriteBatch.Draw(codePaper, new Rectangle(Width / 2 - 250, Height / 2 - 150, 500, 300), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.6f);
            
            Vector2 CodeM = pixelfont.MeasureString("Code: 6036");
            spriteBatch.DrawString(pixelfont, "Code: 6036", new Vector2((Width / 2) - (CodeM.X / 2), (Height / 2 - (CodeM.Y / 2))), Color.Black,0f, Vector2.Zero, 1f, SpriteEffects.None, 0.7f);
        }
    }
}