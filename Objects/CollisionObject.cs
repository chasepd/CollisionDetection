using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollisionDetection.Objects
{
    internal class CollisionObject
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public Texture2D Texture;
        public Vector2 Scale = Vector2.One;
        public float Rotation = 0f;
        public Sprite Sprite;
        public RectangleF Bounds => Sprite.GetBoundingRectangle(Position, Rotation, Scale);
    }
}
