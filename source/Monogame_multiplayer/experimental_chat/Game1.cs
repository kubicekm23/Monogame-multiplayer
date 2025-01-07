using System;
using experimental_chat;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace _01_Monogame_multiplayer_game;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Server _server;
    private Client _client;

    private bool _localServer = false;   // podle toho jestli tento počítač bude server
    private string _serverPassword = "HesloHeslo";
    private string _serverIP = "127.0.0.1";

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        if (_localServer)
        {
            Window.Title = "Chat server";
        }
        else {Window.Title = "Chat client";}
        
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;
        
        _graphics.ApplyChanges();
    }

    protected override void Initialize()
    {
        try
        {
            if (_localServer) 
                _server = new Server(_serverPassword);
            else 
                _client = new Client(_serverPassword, _serverIP);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to initialize network: {e.Message}");
            Exit();
        }
        
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            Exit();
            if (_localServer) _server.StopServer();
            else _client.StopClient();
        }

        try
        {
            if (_localServer)
                _server?.Update();
            else
                _client?.Update();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Network error: {e.Message}");
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add your drawing code here

        base.Draw(gameTime);
    }
    
    protected override void UnloadContent()
    {
        if (_localServer) 
            _server?.StopServer();
        else 
            _client?.StopClient();
            
        base.UnloadContent();
    }
}