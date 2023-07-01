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
            GlobalData.StaticGraphicsDeviceReference = GraphicsDevice;
            GlobalData.StaticContentReference = Content;
            GlobalData.Cursor = Content.Load<Texture2D>("cursor");

            Texture2D rainWorldTex = Content.Load<Texture2D>("rain_world");
            Texture2D rainWorldMask = Content.Load<Texture2D>("rain_world_mask");
            Mask maskElement = new Mask(10, 10, rainWorldTex.Width, rainWorldTex.Height, null, rainWorldMask);
            ImageElement imgElement = new ImageElement(10, 10, rainWorldTex.Width, rainWorldTex.Height, null, rainWorldTex);

            maskElement.AddChild(imgElement);
            canvas.AddChild(maskElement);

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
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointWrap);

            canvas.Draw(_spriteBatch);
            GlobalData.DrawCursor(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}