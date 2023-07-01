using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonogameUtilities.Elements;
using MonogameUtilities.Information;
using MonogameUtilities.Demos;

namespace MonogameUtilities
{
    public class UtilityTesting : Game
    {
        static GraphicsDeviceManager _graphics;
        static SpriteBatch _spriteBatch;

        public static float TotalSeconds;

        readonly Canvas canvas = new(0, 0, 0, 0);

        public static Texture2D pixel;

        public UtilityTesting()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.PreferredBackBufferHeight = GlobalData.WindowHeight * GlobalData.Scale;
            _graphics.PreferredBackBufferWidth = GlobalData.WindowWidth * GlobalData.Scale;
            IsFixedTimeStep = true;
            TargetElapsedTime = GlobalData.TargetElapsedTime;
            IsMouseVisible = false;
            _graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            GlobalData.StaticSpriteBatchReference = _spriteBatch;
            GlobalData.Cursor = Content.Load<Texture2D>("cursor");

            pixel = Content.Load<Texture2D>("pixel");
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            TotalSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            MouseManager.Update();

            canvas.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointWrap);

            canvas.Draw(_spriteBatch);
            GlobalData.DrawCursor();

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}