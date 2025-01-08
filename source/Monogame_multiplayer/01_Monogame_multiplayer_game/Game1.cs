using System;
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

    private NetworkSettings _networkSettings;
    
    // players
    private PlayerLocal _playerLocal;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        
        _networkSettings = new NetworkSettings("127.0.0.1", 9050, false, "Heslo");  // default hodnoty

        if (_networkSettings.IsServer) { Window.Title = "Chat server"; }
        else {Window.Title = "Chat client";}
        
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;
        
        _graphics.ApplyChanges();
    }

    protected override void Initialize()
    {
        try { if (_networkSettings.IsServer) _server = new Server(_networkSettings.Password, _networkSettings.Port);else _client = new Client(_networkSettings.Password, _networkSettings.HostIP); }
        catch (Exception e) { Console.WriteLine($"Failed to initialize network: {e.Message}"); Exit(); }
        
        _playerLocal = new PlayerLocal(new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2), GraphicsDevice);
        
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        var KeyboardState = Keyboard.GetState();
        Vector2 direction = Vector2.Zero;
        
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape)) { Exit(); }
        
        try
        {
            if (_networkSettings.IsServer) _server?.Update();
            else _client?.Update();
        }
        catch (Exception e) { Console.WriteLine($"Network error: {e.Message}"); }

        if (KeyboardState.IsKeyDown(Keys.W)) direction.Y = -1;
        if (KeyboardState.IsKeyDown(Keys.S)) direction.Y = 1;
        if (KeyboardState.IsKeyDown(Keys.A)) direction.X = -1;
        if (KeyboardState.IsKeyDown(Keys.D)) direction.X = 1;
        
        _playerLocal.Move(direction);
        
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();
        _playerLocal.Draw(_spriteBatch);
        _spriteBatch.End();
        
        base.Draw(gameTime);
    }
    
    protected override void UnloadContent()
    {
        if (_networkSettings.IsServer) _server?.StopServer();
        else _client?.StopClient();
            
        base.UnloadContent();
    }
}