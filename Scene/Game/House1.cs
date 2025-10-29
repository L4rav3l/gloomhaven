using System;
using System.Collections.Generic;
using TiledSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace Gloomhaven;

public class House1 : IScene
{
    private GraphicsDevice graphicsDevice;
    private SceneManager sceneManager;
    private ContentManager contentManager;

    private TmxMap map;

    private Texture2D playerTexture;
    private Texture2D Ghost1;
    private Texture2D Ghost2;
    private Texture2D OutDoorTileSet;
    private Texture2D InDoorTileSet;
    private Texture2D Key;
    private Texture2D KeyPanel_Main;
    private Texture2D KeyPanel_Button;
    private Texture2D KeyPanel_Code;

    private Song GhostSong;

    private SpriteFont pixelfont;

    private Vector2 Ghost;
    private Vector2 Ghost2Pos;
    private Vector2[] KeysPos = new Vector2[12];

    private bool route;
    private bool route2;
    private bool KeyAvailable = true;
    private bool KeyInHand = false;
    private bool FloorDoor = false;
    private bool KeyPanel = false;
    private bool Error = false;
    private bool OfficeClosed = true;

    private double GhostSongCooldown;
    private double CodesCooldown;
    private int? Codes = null;

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

    public House1(GraphicsDevice graphicsDevice, SceneManager sceneManager, ContentManager contentManager)
    {
        this.graphicsDevice = graphicsDevice;
        this.sceneManager = sceneManager;
        this.contentManager = contentManager;

        Ghost = new Vector2(1555, 2351);
        route = false;

        Ghost2Pos = new Vector2(2638, 2893);
        route2 = true;

        KeyPanel_Main = new Texture2D(graphicsDevice, 1, 1);
        KeyPanel_Main.SetData(new[] { Color.Gray });

        KeyPanel_Button = new Texture2D(graphicsDevice, 1, 1);
        KeyPanel_Button.SetData(new[] { Color.White });

        KeyPanel_Code = new Texture2D(graphicsDevice, 1, 1);
        KeyPanel_Code.SetData(new[] {Color.White});
    }

    public void LoadContent()
    {
        player = new Player(new Vector2(1895, 3045));
        camera = new Camera2D(graphicsDevice.Viewport);

        map = new TmxMap("Content/floor.tmx");

        playerTexture = contentManager.Load<Texture2D>("player");
        Ghost1 = contentManager.Load<Texture2D>("ghost_1");
        Ghost2 = contentManager.Load<Texture2D>("ghost_2");
        GhostSong = contentManager.Load<Song>("ghost-in-near");
        OutDoorTileSet = contentManager.Load<Texture2D>("tilesmap");
        InDoorTileSet = contentManager.Load<Texture2D>("tilesmap-indoor");
        Key = contentManager.Load<Texture2D>("key");

        solidTiles = LoadCollisionObjects("Content/floor.tmx");

        pixelfont = contentManager.Load<SpriteFont>("pixelfont");
    }

    public void Update(GameTime gameTime)
    {
        player.Update(gameTime, solidTiles, camera);
        camera.Follow(player.Position, new Vector2(map.Width * 64, map.Height * 64));

        MouseState mouse = Mouse.GetState();

        if (mouse.LeftButton == ButtonState.Pressed)
        {
            if(KeyPanel == true) {

                Console.WriteLine($"X: {relativeX}, Y: {relativeY}");

                for (int i = 0; i < 12; i++)
                {
                    if (Vector2.Distance(KeysPos[i], new Vector2((float)mouse.X, (float)mouse.Y)) <= 50 && CodesCooldown <= 0)
                    {
                        if(i == 9)
                        {
                            Codes = Codes / 10;
                            Console.WriteLine("Törlés");
                        } else if(i == 10)
                        {
                            Codes = Codes * 10;
                            Console.WriteLine("nulla");
                            CodesCooldown = 500;
                        } else if(i == 11)
                        {
                            Console.WriteLine("Beküldés");
                            if(Codes == 6036)
                            {
                                OfficeClosed = false;
                            } else {
                                CodesCooldown = 2000;
                                Error = true;
                                Codes = null;
                            }
                        } else
                        {
                            if(Codes == null)
                            {
                                Codes = (i + 1);
                            } else if(Codes.ToString().Length < 4)
                            {
                                Codes = Codes * 10 + (i + 1);
                            }
                            CodesCooldown = 500;
                        }
                    }
                }
            }
        }

        double elapsed = gameTime.ElapsedGameTime.TotalSeconds * 1000;

        if(GhostSongCooldown >= 0)
        {
            GhostSongCooldown -= elapsed;
        }

        if(CodesCooldown >= 0)
        {
            CodesCooldown -= elapsed;
        } else {
            Error = false;
        }

        if(Ghost.X == 1555 && route == false)
        {
            route = true;
        } else if(route == false){
            Ghost.X++;
        }

        if(Ghost.X == 1154 && route == true)
        {
            route = false;
        } else if(route == true)
        {
            Ghost.X--;
        }

        if(Ghost2Pos.X == 2638 && route2 == false)
        {
            route2 = true;
        } else if(route2 == true)
        {
            Ghost2Pos.X++;
        }

        if(Ghost2Pos.X == 3442 && route2 == true)
        {
            route2 = false;
        } else if(route2 == false)
        {
            Ghost2Pos.X--;
        }

        Vector2 GhostPosition = camera.WorldToScreen(Ghost);
        Vector2 Ghost2Position = camera.WorldToScreen(Ghost2Pos);

        if(Vector2.Distance(player.screenPos, GhostPosition) <= 48)
        {
            sceneManager.ChangeScene("ghost");
        }

        if(Vector2.Distance(player.screenPos, GhostPosition) <= 320 && GhostSongCooldown <= 0)
        {
            MediaPlayer.Play(GhostSong);
            MediaPlayer.IsRepeating = false;
            GhostSongCooldown = 10000;
        }

        if(Vector2.Distance(player.screenPos, Ghost2Position) <= 48)
        {
            sceneManager.ChangeScene("ghost");
        }

        if(Vector2.Distance(player.screenPos, Ghost2Position) <= 320 && GhostSongCooldown <= 0)
        {
            MediaPlayer.Play(GhostSong);
            MediaPlayer.IsRepeating = false;
            GhostSongCooldown = 10000;
        }

        KeyboardState state = Keyboard.GetState();

        if(state.IsKeyDown(Keys.E))
        {
            Vector2 KeyPosition = camera.WorldToScreen(new Vector2(1183, 2207));

            if(Vector2.Distance(KeyPosition, player.screenPos) <= 32)
            {
                KeyAvailable = false;
                KeyInHand = true;
            }

            Vector2 FloorDoorPosition = camera.WorldToScreen(new Vector2(1856, 2206));
            
            if(Vector2.Distance(FloorDoorPosition, player.screenPos) <= 64)
            {
                if(FloorDoor == false)
                {
                    KeyInHand = false;
                    FloorDoor = true;
                } else {
                    
                }
            }

            Vector2 OfficeDoorPosition = camera.WorldToScreen(new Vector2(512, 2587));

            if(Vector2.Distance(OfficeDoorPosition, player.screenPos) <= 64 && KeyPanel == false)
            {
                KeyPanel = true;
                Console.WriteLine("kódpanel megnyitva");
            }
        }

            if(KeyPanel == true && state.IsKeyDown(Keys.Escape))
            {
                KeyPanel = false;
                Console.WriteLine("kódpanel bezárva");
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

                int tilesPerRow = OutDoorTileSet.Width / map.TileWidth;
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

                spriteBatch.Draw(OutDoorTileSet, screenPosition, source, Color.White * 0.3f);
            }
        }

        Vector2 GhostPosition = camera.WorldToScreen(Ghost);
        Vector2 Ghost2Position = camera.WorldToScreen(Ghost2Pos);
        Vector2 KeyPosition = camera.WorldToScreen(new Vector2(1183, 2207));
        player.Draw(spriteBatch, playerTexture, camera);
        
        if(route == false)
        {
            spriteBatch.Draw(Ghost1, GhostPosition, null, Color.White * 0.15f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.5f);
        } else {
            spriteBatch.Draw(Ghost2, GhostPosition, null, Color.White * 0.15f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.5f);
        }

        if(route2 == false)
        {
            spriteBatch.Draw(Ghost2, Ghost2Position, null, Color.White * 0.15f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.5f);
        } else {
            spriteBatch.Draw(Ghost1, Ghost2Position, null, Color.White * 0.15f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.5f);
        }

        if(KeyAvailable == true)
        {
            spriteBatch.Draw(Key, KeyPosition, null, Color.White * 0.3f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.4f);
        }

        if(KeyInHand == true)
        {
            spriteBatch.Draw(Key, new Vector2(player.screenPos.X + 45, player.screenPos.Y + 30), null, Color.White * 0.3f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.4f);
        }

        if(KeyPanel == true && OfficeClosed == true)
        {
            spriteBatch.Draw(KeyPanel_Main, new Rectangle(Width / 2 - 140, Height / 2 - 312, 380, 635), null, Color.Gray, 0f, Vector2.Zero, SpriteEffects.None, 0.5f);

            int num = 1;

            for(int y = 0; y < 4; y++)
            {
                for(int x = 0; x < 3; x++)
                {
                    spriteBatch.Draw(KeyPanel_Button, new Rectangle(((Width / 2 - 110) + (110 * x)), ((Height / 2 - 150) + (110 * y)), 100, 100), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.6f);
                
                    Vector2 textPos = new Vector2( (Width / 2 - 110) + (110 * x) + 35, (Height / 2 - 150) + (110 * y) + 25);
                    Vector2 buttonCenter = new Vector2((Width / 2 - 110) + (110 * x) + 50, (Height / 2 - 150) + (110 * y) + 50);
                    KeysPos[num-1] = buttonCenter;


                    if(num == 10)
                    {
                        spriteBatch.DrawString(pixelfont, "DEL", new Vector2(textPos.X - 20, textPos.Y), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.7f);
                    } else if(num == 11)
                    {
                        spriteBatch.DrawString(pixelfont, 0.ToString(), textPos, Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.7f);
                    } else if(num == 12)
                    {
                        spriteBatch.DrawString(pixelfont, "OK", new Vector2(textPos.X -10, textPos.Y), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.7f);
                    } else {
                        spriteBatch.DrawString(pixelfont, num.ToString(), textPos, Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.7f);
                    }
                    num++;
                }
            }

            spriteBatch.Draw(KeyPanel_Code, new Rectangle((Width / 2 - 110), (Height / 2 - 280), 320, 100), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.6f);
        
            if(Error)
            {
                spriteBatch.DrawString(pixelfont, "ERROR", new Vector2((Width / 2  - 110) + 90, (Height / 2 - 280) + 25), Color.Red, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.7f);
            } else { 
                spriteBatch.DrawString(pixelfont, Codes.ToString(), new Vector2((Width / 2  - 110) + 110, (Height / 2 - 280) + 25), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.7f);
            }
        }
    }
}