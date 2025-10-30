using System;
using System.Collections.Generic;
using TiledSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace Gloomhaven;

public class Office : IScene
{
    private GraphicsDevice graphicsDevice;
    private SceneManager sceneManager;
    private ContentManager contentManager;

    private TmxMap map;
    
    private Texture2D playerTexture;
    private Texture2D Ghost1;
    private Texture2D Ghost2;
    private Texture2D TileSet;

    private Song GhostSong;

    private SpriteFont pixelfont;

    private Vector2 Ghost;

    private bool route;

    private string[] Task = new string[5];

    private Player player;
    private Camera2D camera;
    private List<Rectangle> solidTiles;

    public List<Rectangle> LoadCollisionObjects(string mapFilePath)
    {
        var map = new TmxMap(mapFilePath);
        var solidTiles = new List<Rectangle>();
        foreach(var objectGroup in map.ObjectGroups)
        {
            if(objectGroup.Name == "Collision")
            {
                foreach (var obj in objectGroup.Objects)
                {
                    if(obj.Width > 0 && obj.Height > 0)
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

        return solidTiles
    }

    public Office(GraphicsDevice graphicsDevice, SceneManager sceneManager, ContentManager contentManager)
    {
        this.graphicsDevice = graphicsDevice;
        this.sceneManager = sceneManager;
        this.contentManager = contentManager;

        Ghost = new Vector2(0, 0);
        route = false;
    }

    public void LoadContent()
    {
        player = new Player(new Vector2(1218, 1856));
        camera = new Camera2D(graphicsDevice.Viewport);

        map = new TmxMap("Content/office.tmx");

        playerTexture = contentManager.Load<Texture2D>("player");
        Ghost1 = contentManager.Load<Texture2D>("ghost_1");
        Ghost2 = ContentManager.Load<Texture2D>("ghost_2");
        GhostSong = contentManager.Load<Song>("ghost-in-near");
        TileSet = contentManager.Load<Texture2D>("tilesmap");

        solidTiles = LoadCollisionObjects("Content/office.tmx");

        pixelfont = contentManager.Load<SpriteFont>("pixelfont");

        Task[0] = "Find evidence 0/3";
        Task[1] = "Find evidence 1/3";
        Task[2] = "Find evidence 2/3";
        Task[3] = "Find evidence 3/3";
        Task[4] = "Leave the mansion.";
    }

    public void(GameTime gameTime)
    {
        player.Update(gameTime, solidTiles, camera);
        camera.Follow(player.Position, new Vector2(map.Width * 64, map.Height * 64))
    }
}