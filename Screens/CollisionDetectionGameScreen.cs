using CollisionDetection.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Input;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollisionDetection.Screens
{
    internal class CollisionDetectionGameScreen : GameScreen
    {

        private SpriteBatch _spriteBatch;
        private List<CollisionObject> _collisionObjects;        
        private Tweener _tweener;
        private Dictionary<CollisionObject, List<CollisionObject>> _collisions;
        private readonly FastRandom _random;
      
        public int ScreenWidth => GraphicsDevice.Viewport.Width;
        public int ScreenHeight => GraphicsDevice.Viewport.Height;

        public CollisionDetectionGameScreen(Game game) :  base(game) 
        {
            _random = new FastRandom();
            _collisionObjects = new List<CollisionObject>();
            _tweener = new Tweener();
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void LoadContent()
        {
            base.LoadContent();
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            _spriteBatch.Dispose();
        }

        public override void Update(GameTime gameTime)
        {
            var keyboardState = KeyboardExtended.GetState();
            var elapsedSeconds = gameTime.GetElapsedSeconds();
            _collisions = new Dictionary<CollisionObject, List<CollisionObject>>();

            foreach(var collisionObject in _collisionObjects)
            {
                _collisions[collisionObject] = new List<CollisionObject>();
            }
            foreach (var collisionObject in _collisionObjects)
            {
                collisionObject.Position += collisionObject.Velocity * elapsedSeconds;
                ConstrainObject(collisionObject);
                HandleCollisions(collisionObject);
            }

            if (keyboardState.WasKeyJustDown(Keys.Escape))
            {
                Game.Exit();
            }
            else if (keyboardState.WasKeyJustDown(Keys.Space))
            {
                Texture2D texture = new Texture2D(GraphicsDevice, 1, 1);
                texture.SetData(new Color[] { Color.Red });
                if (_random.Next(0, 2) % 2 == 0)
                {
                    var newObj = new CircularCollisionObject
                    {
                        Position = new Vector2(_random.Next(0, ScreenWidth), _random.Next(0, ScreenHeight)),
                        Velocity = new Vector2(_random.Next(-300, 300), _random.Next(-300, 300)),
                        Sprite = new Sprite(texture),
                        Scale = new Vector2(_random.Next(25, 60), _random.Next(25, 60))
                    };
                    _collisionObjects.Add(newObj);
                }
                else
                {
                    var newObj = new RectangularCollisionObject
                    {
                        Position = new Vector2(_random.Next(0, ScreenWidth), _random.Next(0, ScreenHeight)),
                        Sprite = new Sprite(texture),
                        Velocity = new Vector2(_random.Next(-300, 300), _random.Next(-300, 300)),
                        Texture = texture,
                        Scale = new Vector2(_random.Next(25, 60), _random.Next(25, 60))
                    };
                    _collisionObjects.Add(newObj);
                }
            }

            _tweener.Update(elapsedSeconds);
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin();

            foreach (var collisionObject in _collisionObjects)
            {
                if (collisionObject is CircularCollisionObject)
                {
                    _spriteBatch.DrawEllipse(
                        collisionObject.Position, 
                        new Vector2(collisionObject.Bounds.Width / 2, collisionObject.Bounds.Height / 2), 
                        200, 
                        Color.Blue, 
                        thickness: collisionObject.Scale.X < collisionObject.Scale.Y ? collisionObject.Scale.X: collisionObject.Scale.Y);

                    // Add Outline
                    _spriteBatch.DrawEllipse(
                        collisionObject.Position,
                        new Vector2(collisionObject.Bounds.Width / 2, collisionObject.Bounds.Height / 2),
                        200,
                        Color.White,
                        thickness: 2
                        );
                }
                else
                {
                    _spriteBatch.FillRectangle(
                        collisionObject.Bounds,
                        Color.Red);

                    // Add Outline
                    _spriteBatch.DrawRectangle(
                        collisionObject.Bounds,
                        Color.White,
                        thickness: 2);
                }
            }

            _spriteBatch.End();
        }

        private void ConstrainObject(CollisionObject collisionObject)
        {

            var widthHalved = collisionObject.Bounds.Width / 2;
            var heightHalved = collisionObject.Bounds.Height / 2;
            if (collisionObject.Bounds.Center.X - widthHalved < 0)
            {
                collisionObject.Position.X = widthHalved;
                collisionObject.Velocity.X *= -1;
            }

            if (collisionObject.Bounds.Center.X + widthHalved > ScreenWidth)
            {
                collisionObject.Position.X = ScreenWidth - widthHalved;
                collisionObject.Velocity.X *= -1;
            }

            if (collisionObject.Bounds.Center.Y - heightHalved < 0)
            {
                collisionObject.Position.Y = heightHalved;
                collisionObject.Velocity.Y *= -1;
            }

            if (collisionObject.Bounds.Center.Y + collisionObject.Bounds.Height / 2 > ScreenHeight)
            {
                collisionObject.Position.Y = ScreenHeight - heightHalved;
                collisionObject.Velocity.Y *= -1;
            }
        }

        private bool HaveAlreadyCollided(CollisionObject collisionObject, CollisionObject otherObject)
        {
            return _collisions[collisionObject].Contains(otherObject) || _collisions[otherObject].Contains(otherObject);
        }

        private void HandleCollisions(CollisionObject collisionObject)
        {
            foreach (var otherObject in _collisionObjects)
            {
                if (collisionObject == otherObject) continue;

                if (otherObject.Bounds.Intersects(collisionObject.Bounds) && !HaveAlreadyCollided(collisionObject, otherObject))
                {
                    var otherVelocity = otherObject.Velocity;
                    var thisVelocity = collisionObject.Velocity;

                    var thisVelocityDelta = otherVelocity * (otherObject.Mass / collisionObject.Mass);
                    var otherVelocityDelta = thisVelocity * (collisionObject.Mass / collisionObject.Mass);
                    collisionObject.Velocity += thisVelocityDelta;
                    collisionObject.Velocity -= otherVelocityDelta;
                    otherObject.Velocity += otherVelocityDelta;
                    otherObject.Velocity -= thisVelocityDelta;

                    if(collisionObject.Bounds.Left < otherObject.Bounds.Left)
                    {
                        collisionObject.Position.X = otherObject.Bounds.Left - collisionObject.Bounds.Width / 2;
                    }

                    if(collisionObject.Bounds.Right > otherObject.Bounds.Right)
                    {
                        collisionObject.Position.X = otherObject.Bounds.Right + collisionObject.Bounds.Width / 2;
                    }
                    _collisions[collisionObject].Add(otherObject);
                }
            }
        }
    }
}
