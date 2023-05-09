using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonogameUtilities.Elements;
using MonogameUtilities.Hitboxes;
using MonogameUtilities.Information;
using System;

namespace MonogameUtilities
{
    public class UtilityTesting : Game
    {
        static GraphicsDeviceManager _graphics;
        static SpriteBatch _spriteBatch;

        public const float scale = 1;
        public const int windowWidth = 600;
        public const int windowHeight = 400;

        readonly BoundingCanvas canvas = new(50, 50, 200, 100);

        public static Texture2D pixel;

        public UtilityTesting()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.PreferredBackBufferHeight = windowHeight;
            _graphics.PreferredBackBufferWidth = windowWidth;
            IsFixedTimeStep = true;
            TargetElapsedTime = TimeSpan.FromSeconds(1d / 60d);
            _graphics.ApplyChanges();

            canvas.AddElement(new ClickableElement(10, 10, 50, 20, canvas));
            canvas.AddElement(new DraggableElement(10, 10, 50, 20, Color.Gray, canvas));
            canvas.AddElement(new DraggableElement(10, 10, 50, 20, Color.Gray, canvas));
            canvas.AddElement(new DraggableElement(10, 10, 50, 20, Color.Gray, canvas));
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            pixel = Content.Load<Texture2D>("pixel");
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            MouseManager.Update();

            canvas.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointWrap, null);

            canvas.Draw();

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public static void Draw(Texture2D tex, Rectangle bound, Color color)
        {
            bound.X = (int)(bound.X * scale);
            bound.Y = (int)(bound.Y * scale);
            bound.Width = (int)(bound.Width * scale);
            bound.Height = (int)(bound.Height * scale);
            _spriteBatch.Draw(tex, bound, color);
        }
    }
}