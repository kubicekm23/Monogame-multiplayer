using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace experimental_chat;

public class textInputBox
{
    private Texture2D _texture;
    private string _text;
    private Rectangle _rectangle;
    private SpriteFont _font;
    private Vector2 _position;
    
    public textInputBox(Vector2 position, GraphicsDevice graphicsDevice, int width, int height, SpriteFont font)
    {
        _texture = new Texture2D(graphicsDevice, 1, 1);
        _texture.SetData(new Color[] { Color.White });
        
        _font = font;
        _position = position;
        
        _rectangle = new Rectangle((int)position.X + 15, (int)position.Y + 15, width, height);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_texture, _rectangle, Color.White);
        spriteBatch.DrawString(_font, _text, _position, Color.Black);
    }

    public void UpdateText(string textToAdd)
    {
        _text += textToAdd;
    }

    public string GetText()
    {
        string text = _text;
        _text = string.Empty;
        
        return text;
    }
}