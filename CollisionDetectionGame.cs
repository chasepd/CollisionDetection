using CollisionDetection.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;
using System;

namespace CollisionDetection
{
    public class CollisionDetectionGame : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private readonly ScreenManager _screenManager;

        public CollisionDetectionGame()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1920,
                PreferredBackBufferHeight = 1080
            };
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
            IsFixedTimeStep = true;
            TargetElapsedTime = TimeSpan.FromSeconds(1f / 60f);

            _screenManager = Components.Add<ScreenManager>();
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            _screenManager.LoadScreen(new CollisionDetectionGameScreen(this), new ExpandTransition(GraphicsDevice, Color.Black, 0.2f));
        }
    }
}