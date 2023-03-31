using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;
using MonoGame.Extended.Screens;
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
        private Texture2D _texture;

        public CollisionDetectionGameScreen(Game game) :  base(game) { }

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

            if (keyboardState.WasKeyJustDown(Keys.Escape))
            {
                Game.Exit();
            }
        }

        public override void Draw(GameTime gameTime)
        {            
        }
    }
}
