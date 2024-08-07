using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonogameUtilities.Elements;
using MonogameUtilities.Information;
using System;
using System.Reflection;

namespace MonogameUtilities
{
    public class UtilityTesting : Game
    {
        public static readonly float SCALE = 4.0f;
        public static readonly int WINDOW_WIDTH = 200;
        public static readonly int WINDOW_HEIGHT = 200;

        static GraphicsDeviceManager _graphics;
        static SpriteBatch _spriteBatch;

        private static float _totalSeconds;

        private static Texture2D _pixel;

        private Element _baseElement;

        public UtilityTesting()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.PreferredBackBufferHeight = (int)(WINDOW_WIDTH * SCALE);
            _graphics.PreferredBackBufferWidth = (int)(WINDOW_HEIGHT* SCALE);
            IsFixedTimeStep = true;
            // 60 FPS
            TargetElapsedTime = TimeSpan.FromMilliseconds(1000f / 60);
            IsMouseVisible = false;
            _graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            Texture2D cursorTexture = Content.Load<Texture2D>("cursor");
            Point cursorSize = new(9, 9);

            MouseManager.SetMouseData(cursorTexture, SCALE, cursorSize, new Point(0), Color.White, _spriteBatch);

            _pixel = Content.Load<Texture2D>("pixel");

            _baseElement = new(10, 10, 20, 20);
            _baseElement.OnClick += _baseElement_OnClick;
            _baseElement.AddUpdateAction("DragAction", () => {
                Element sourceElement = _baseElement;

                if (!sourceElement.HasAttribute($"{nameof(MonogameUtilities)}.dragging"))
                {
                    return;
                }

                Point deltaPos = (Point)sourceElement.GetAttribute($"{nameof(MonogameUtilities)}.deltaPos");

                Point currentMousePos = MouseManager.MousePos;

                sourceElement.SetPos(currentMousePos + deltaPos);
            });
            _baseElement.OffClick += _baseElement_OffClick;
            _baseElement.SetTexture(_pixel);
        }

        private bool _baseElement_OffClick(object sender, Elements.ElementEventArgs.ClickEventArgs e)
        {
            if (MouseManager.LeftClick) { return false; }

            Element sourceElement = sender as Element;

            if (!sourceElement.HasAttribute($"{nameof(MonogameUtilities)}.dragging"))
            {
                return false;
            }

            sourceElement.SetAttribute($"{nameof(MonogameUtilities)}.deltaPos", null);

            sourceElement.RemoveAttribute($"{nameof(MonogameUtilities)}.dragging");
            sourceElement.RemoveAttribute($"{nameof(MonogameUtilities)}.deltaPos");

            return true;
        }

        private bool _baseElement_OnClick(object sender, Elements.ElementEventArgs.ClickEventArgs e)
        {
            if (!MouseManager.LeftClick) { return false; }

            Element sourceElement = sender as Element;

            if (sourceElement.HasAttribute($"{nameof(MonogameUtilities)}.dragging"))
            {
                return false;
            }

            Point initalMousePos = MouseManager.MousePos;

            sourceElement.SetAttribute($"{nameof(MonogameUtilities)}.deltaPos", sourceElement.GetPos() - initalMousePos);
            sourceElement.SetAttribute($"{nameof(MonogameUtilities)}.dragging", true);

            return true;
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _totalSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            MouseManager.Update();
            Element.StaticUpdate();
            _baseElement.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointWrap);

            _baseElement.Draw(_spriteBatch, SCALE);
            MouseManager.DrawCursor();

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}