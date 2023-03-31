using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;

namespace CollisionDetection.Objects
{
    internal class CircularCollisionObject : CollisionObject
    {
        public override float Mass => Sprite.GetBoundingRectangle(Position, Rotation, Scale).Width / 2 * Sprite.GetBoundingRectangle(Position, Rotation, Scale).Height / 2 * MathHelper.Pi;
    }
}
