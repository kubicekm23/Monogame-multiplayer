using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _01_Monogame_multiplayer_game;


public class PlayerLocal
{
    private Vector2 _position;
    public Vector2 Position => _position;
    
    private Vector2 _velocity;
    private Vector2 _acceleration;
    private Vector2 _friction;
    
    private Texture2D _texture;

    public PlayerLocal(Vector2 position, GraphicsDevice graphicsDevice)
    {
        _position = position;
        _velocity = Vector2.Zero;
        _acceleration = new Vector2(0.1f, 0.1f);
        _friction = new Vector2(0.02f, 0.02f);
        
        Color[] colors = new Color[35 * 35];

        for (int i = 0; i < 35; i++)
        {
            for (int j = 0; j < 35; j++)
            {
                colors[i + j * 35] = Color.White;
            }
        }

        _texture.SetData(colors);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_texture, _position, Color.White);
    }

    public void Move(Vector2 direction)
    {
        _velocity += _acceleration*direction;

        if (_velocity.X > 0) { _velocity.X -= _friction.X; }
        else if (_velocity.X < 0) { _velocity.X += _friction.X; }
        if (_velocity.Y > 0) { _velocity.Y -= _friction.Y; }
        else if (_velocity.Y < 0) { _velocity.Y += _friction.Y; }
        
        _position += _velocity;
    }
}