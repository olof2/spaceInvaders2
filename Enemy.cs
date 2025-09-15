using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace spaceInvaders1._1
{
    internal class Enemy
{
    public Texture2D texture;
    public Vector2 position;
    public Vector2 velocity;
    public int windowHeight;
    public int windowWidth;
    public int lives;

    public Enemy(Texture2D texture, Vector2 position, Vector2 velocity, int windowHeight, int windowWidth)
    {
        this.texture = texture;
        this.position = position;
        this.velocity = velocity;
        this.windowHeight = windowHeight;
        this.windowWidth = windowWidth;
    }

    public Vector2 Update()
    {
        position.Y += velocity.Y;
        return position;
    }

    public void Draw(SpriteBatch sb)
    {
        sb.Draw(texture, position, Microsoft.Xna.Framework.Color.White);
    }
}
}
