using CollisionDetection.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
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

            if (keyboardState.WasKeyJustDown(Keys.Escape))
            {
                Game.Exit();
            }
            else if (keyboardState.WasKeyJustDown(Keys.Space))
            {
                
                if (_random.Next(0, 2) % 2 == 0)
                {                    
                    _collisionObjects.Add(new CircularCollisionObject
                    {
                        Position = new Vector2(_random.Next(0, ScreenWidth), _random.Next(0, ScreenHeight)),
                        Velocity = new Vector2(_random.Next(5, 300), _random.Next(5, 300)),
                        Scale = new Vector2(_random.Next(25, 60), _random.Next(25, 60))                        
                    }
                    );
                }
                else
                {
                    Texture2D texture = new Texture2D(GraphicsDevice, 1, 1);
                    texture.SetData(new Color[] { Color.Red });
                    _collisionObjects.Add(new RectangularCollisionObject
                    {
                        Position = new Vector2(_random.Next(0, ScreenWidth), _random.Next(0, ScreenHeight)),
                        Sprite = new Sprite(texture),
                        Velocity = new Vector2(_random.Next(5, 300), _random.Next(5, 300)),
                        Texture = texture,
                        Scale = new Vector2(_random.Next(25, 60), _random.Next(25, 60))
                    }
                    );
                }
            }

            _tweener.Update(elapsedSeconds);
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();

            foreach (var collisionObject in _collisionObjects)
            {
                if (collisionObject is CircularCollisionObject)
                {
                    _spriteBatch.DrawEllipse(
                        collisionObject.Position, 
                        collisionObject.Scale, 
                        50, 
                        Color.Aquamarine, 
                        thickness: collisionObject.Scale.X < collisionObject.Scale.Y ? collisionObject.Scale.X: collisionObject.Scale.Y);

                    // Add Outline
                    _spriteBatch.DrawEllipse(
                        collisionObject.Position,
                        collisionObject.Scale,
                        50,
                        Color.White,
                        thickness: 2
                        );
                }
                else
                {
                    _spriteBatch.DrawRectangle(
                        collisionObject.Bounds,
                        Color.Red,
                        thickness: collisionObject.Scale.X < collisionObject.Scale.Y ? collisionObject.Scale.X : collisionObject.Scale.Y);

                    // Add Outline
                    _spriteBatch.DrawRectangle(
                        collisionObject.Bounds,
                        Color.White,
                        thickness: 2);
                }
            }

            _spriteBatch.End();
        }
    }
}
