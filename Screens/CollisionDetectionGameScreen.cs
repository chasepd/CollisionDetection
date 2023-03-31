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
        private SpriteFont _font;
        private Dictionary<CollisionObject, List<CollisionObject>> _collisions;
        private bool airResistanceEnabled;
        private bool consumeModeEnabled;
        List<CollisionObject> objectsToRemove;
        private readonly Vector2 airResisitance;
        private readonly FastRandom _random;    
      
        public int ScreenWidth => GraphicsDevice.Viewport.Width;
        public int ScreenHeight => GraphicsDevice.Viewport.Height;

        public CollisionDetectionGameScreen(Game game) :  base(game) 
        {
            _random = new FastRandom();
            _collisionObjects = new List<CollisionObject>();
            objectsToRemove = new List<CollisionObject>();
            _tweener = new Tweener();
            airResisitance = new Vector2(0.1f, 0.1f);
            airResistanceEnabled = false;
            consumeModeEnabled = false;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void LoadContent()
        {
            base.LoadContent();
            
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _font = Content.Load<SpriteFont>("Arial");
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
            

            foreach (var collisionObject in _collisionObjects)
            {
                _collisions[collisionObject] = new List<CollisionObject>();
            }
            foreach (var collisionObject in _collisionObjects)
            {
                collisionObject.Position += collisionObject.Velocity * elapsedSeconds;
                if (airResistanceEnabled)
                {
                    if (collisionObject.Velocity.X > 0)
                    {
                        collisionObject.Velocity.X -= airResisitance.X;
                    }
                    if (collisionObject.Velocity.X < 0)
                    {
                        collisionObject.Velocity.X += airResisitance.X;
                    }
                    if (collisionObject.Velocity.Y > 0)
                    {
                        collisionObject.Velocity.Y -= airResisitance.Y;
                    }
                    if (collisionObject.Velocity.Y < 0)
                    {
                        collisionObject.Velocity.Y += airResisitance.Y;
                    }
                }
                

                ConstrainObject(collisionObject);
                HandleCollisions(collisionObject);
            }

            foreach (var obj in objectsToRemove)
            {
                _collisionObjects.Remove(obj);
            }

            objectsToRemove.Clear();

            if (keyboardState.WasKeyJustDown(Keys.Escape))
            {
                Game.Exit();
            }
            
            if (keyboardState.WasKeyJustDown(Keys.Space))
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

            if (keyboardState.WasKeyJustDown(Keys.A))
            {
                airResistanceEnabled ^= true;
            }

            if (keyboardState.WasKeyJustDown(Keys.C))
            {
                consumeModeEnabled ^= true;
            }

            if (keyboardState.WasKeyJustDown(Keys.X))
            {
                consumeModeEnabled = false;
                airResistanceEnabled = false;
                _collisionObjects.Clear();
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

            _spriteBatch.DrawString(_font, "Press ESC to Quit", new Vector2(20, 20), Color.White);
            _spriteBatch.DrawString(_font, "Press Space to Spawn Objects", new Vector2(20, 40), Color.White);
            _spriteBatch.DrawString(_font, "Press A to Toggle Air Resistance; it is currently " + (airResistanceEnabled? "ON" : "OFF"), new Vector2(20, 60), Color.White);
            _spriteBatch.DrawString(_font, "Press C to Toggle Consume Mode; it is currently " + (consumeModeEnabled ? "ON" : "OFF"), new Vector2(20, 80), Color.White);
            _spriteBatch.DrawString(_font, "Press X to Clear", new Vector2(20, 100), Color.White);

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
            // Allow for energy loss as part of the collision
            var collisionTransferEfficency = 0.99f;

            foreach (var otherObject in _collisionObjects)
            {
                if (collisionObject == otherObject) continue;

                if (otherObject.Bounds.Intersects(collisionObject.Bounds) && !HaveAlreadyCollided(collisionObject, otherObject))
                {
                    if (!consumeModeEnabled)
                    {
                        var otherVelocity = otherObject.Velocity;
                        var thisVelocity = collisionObject.Velocity;

                        var thisVelocityDelta = otherVelocity * (otherObject.Mass / collisionObject.Mass);
                        var otherVelocityDelta = thisVelocity * (collisionObject.Mass / collisionObject.Mass);
                        collisionObject.Velocity += thisVelocityDelta * collisionTransferEfficency;
                        collisionObject.Velocity -= otherVelocityDelta;
                        otherObject.Velocity += otherVelocityDelta * collisionTransferEfficency;
                        otherObject.Velocity -= thisVelocityDelta;

                        _collisions[collisionObject].Add(otherObject);

                        while (otherObject.Bounds.Intersects(collisionObject.Bounds))
                        {
                            if (otherObject.Position.X > collisionObject.Position.X)
                            {
                                collisionObject.Position.X--;
                                otherObject.Position.X++;
                            }
                            if (otherObject.Position.X < collisionObject.Position.X)
                            {
                                collisionObject.Position.X++;
                                otherObject.Position.X--;
                            }
                            if (otherObject.Position.Y > collisionObject.Position.Y)
                            {
                                collisionObject.Position.Y--;
                                otherObject.Position.Y++;
                            }
                            if (otherObject.Position.Y < collisionObject.Position.Y)
                            {
                                collisionObject.Position.Y++;
                                otherObject.Position.Y--;
                            }
                        }
                    }
                    else
                    {                        
                        _collisions[collisionObject].Add(otherObject);
                        var objToBeConsumed = otherObject;
                        var objConsuming = collisionObject;
                        if(collisionObject.Mass < otherObject.Mass)
                        {
                            objToBeConsumed = collisionObject;
                            objConsuming = otherObject;
                        }

                        objConsuming.Position = (objConsuming.Position + objToBeConsumed.Position) / 2;
                        objConsuming.Scale.X += (float)Math.Sqrt(objToBeConsumed.Mass);
                        objConsuming.Scale.Y += (float)Math.Sqrt(objToBeConsumed.Mass);
                        objConsuming.Velocity += objToBeConsumed.Velocity * (objToBeConsumed.Mass / objConsuming.Mass) * collisionTransferEfficency;
                        objectsToRemove.Add(objToBeConsumed);
                        break;
                    }
                }
            }
        }
    }
}
