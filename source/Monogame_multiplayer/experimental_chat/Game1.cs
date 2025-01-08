using System;
using experimental_chat;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace experimental_chat;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Server _server;
    private Client _client;

    private bool _localServer = true;   // podle toho jestli tento počítač bude server
    private string _serverPassword = "HesloHeslo";
    private string _serverIP = "127.0.0.1";
    
    private textInputBox _inputBox;
    private SpriteFont _font;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        if (_localServer) { Window.Title = "Chat server"; }
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

        _font = Content.Load<SpriteFont>("font");
        
        _inputBox = new textInputBox(new Vector2(200, 400), GraphicsDevice, 600, 200, _font);
    }

    protected override void Update(GameTime gameTime)
    {
        var KeyboardState = Keyboard.GetState();
        
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            Exit();
            if (_localServer) _server.StopServer();
            else _client.StopClient();
        }

        // Test zpráva
        if (KeyboardState.IsKeyDown(Keys.Space))
        {
            if (_localServer)
                _server?.BroadcastMessage("Hello from server!");
            else
                _client?.SendMessage("Hello from client!");
        }
        
        CheckHeldCharacters(KeyboardState);
        
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

        _spriteBatch.Begin();
        
        _inputBox.Draw(_spriteBatch);
        
        _spriteBatch.End();

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

    private void CheckHeldCharacters(KeyboardState keyboardState)
    {
        if (keyboardState.IsKeyDown(Keys.Enter)) { SendMessage(_inputBox.GetText()); }
        else {
            if (keyboardState.IsKeyDown(Keys.Back)) { _inputBox.DelLetter(); }
            if (keyboardState.IsKeyDown(Keys.Space)) { _inputBox.UpdateText(" "); }
            if (keyboardState.IsKeyDown(Keys.Q)) { _inputBox.UpdateText("q"); }
            if (keyboardState.IsKeyDown(Keys.W)) { _inputBox.UpdateText("w"); }
            if (keyboardState.IsKeyDown(Keys.E)) { _inputBox.UpdateText("e"); }
            if (keyboardState.IsKeyDown(Keys.R)) { _inputBox.UpdateText("r"); }
            if (keyboardState.IsKeyDown(Keys.T)) { _inputBox.UpdateText("t"); }
            if (keyboardState.IsKeyDown(Keys.Y)) { _inputBox.UpdateText("y"); }
            if (keyboardState.IsKeyDown(Keys.U)) { _inputBox.UpdateText("u"); }
            if (keyboardState.IsKeyDown(Keys.I)) { _inputBox.UpdateText("i"); }
            if (keyboardState.IsKeyDown(Keys.O)) { _inputBox.UpdateText("o"); }
            if (keyboardState.IsKeyDown(Keys.P)) { _inputBox.UpdateText("p"); }
            if (keyboardState.IsKeyDown(Keys.A)) { _inputBox.UpdateText("a"); }
            if (keyboardState.IsKeyDown(Keys.S)) { _inputBox.UpdateText("s"); }
            if (keyboardState.IsKeyDown(Keys.D)) { _inputBox.UpdateText("d"); }
            if (keyboardState.IsKeyDown(Keys.F)) { _inputBox.UpdateText("f"); }
            if (keyboardState.IsKeyDown(Keys.G)) { _inputBox.UpdateText("g"); }
            if (keyboardState.IsKeyDown(Keys.H)) { _inputBox.UpdateText("h"); }
            if (keyboardState.IsKeyDown(Keys.J)) { _inputBox.UpdateText("j"); }
            if (keyboardState.IsKeyDown(Keys.K)) { _inputBox.UpdateText("k"); }
            if (keyboardState.IsKeyDown(Keys.L)) { _inputBox.UpdateText("l"); }
            if (keyboardState.IsKeyDown(Keys.Z)) { _inputBox.UpdateText("z"); }
            if (keyboardState.IsKeyDown(Keys.X)) { _inputBox.UpdateText("x"); }
            if (keyboardState.IsKeyDown(Keys.C)) { _inputBox.UpdateText("c"); }
            if (keyboardState.IsKeyDown(Keys.V)) { _inputBox.UpdateText("v"); }
            if (keyboardState.IsKeyDown(Keys.B)) { _inputBox.UpdateText("b"); }
            if (keyboardState.IsKeyDown(Keys.N)) { _inputBox.UpdateText("n"); }
            if (keyboardState.IsKeyDown(Keys.M)) { _inputBox.UpdateText("m"); }
        }
    }

    private void SendMessage(string message)
    {
        if (_localServer)
        {
            _server.BroadcastMessage(message);
        }
        else { _client.SendMessage(message); }
    }
}