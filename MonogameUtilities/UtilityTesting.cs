using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonogameUtilities.Elements;
using MonogameUtilities.Information;
using System.IO.Pipes;

namespace MonogameUtilities
{
    public class UtilityTesting : Game
    {
        static GraphicsDeviceManager _graphics;
        static SpriteBatch _spriteBatch;

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

            /**/
            Texture2D spyglass = Content.Load<Texture2D>("spyglass32");
            ImageElement spyglassElement = new(0, 0, spyglass.Width, spyglass.Height, null, spyglass)
            {
                Layer = 1
            };
            canvas.AddChild(spyglassElement);
            /**/

            /**/
            Texture2D map = Content.Load<Texture2D>("map");
            ImageElement mapElement = new(5, 5, map.Width, map.Height, null, map);
            /**/

            Draggable dragElement = new(5, 5, map.Width, map.Height, canvas, true, spyglassElement.Bounds)
            {
                Layer = 2,
                dragScale = -1
            };
            canvas.AddChild(dragElement);

            IntersectingElement intersectElement = new(5, 5, spyglass.Width, spyglass.Height, dragElement, spyglass.Width / 2, spyglassElement.Bounds);
            intersectElement.AddChild(mapElement);
            dragElement.AddChild(intersectElement);

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
            GlobalData.DrawCursor();

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}