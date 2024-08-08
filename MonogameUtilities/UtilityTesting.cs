using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonogameUtilities.Demo.Rollback;
using MonogameUtilities.Elements;
using MonogameUtilities.Information;
using MonogameUtilities.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private long _frame;

        private Guid _p1Id = Guid.NewGuid();
        private FramedInputs<NumericalInput> _p1Input;
        private Guid _p2Id = Guid.NewGuid();
        private FramedInputs<NumericalInput> _p2Input;

        private List<Keys> _newKeys = new();
        private List<Keys> _preKeys = new();
        private List<Keys> _accKeys = new();


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

            _p1Input = new(0);
            _p2Input = new(0);

            RollbackManager<NumericalInput>.RegisterConverter(val => NumericalInput.StaticFromString(val), number => number.Number.ToString());
            RollbackManager<NumericalInput>.RegisterLocalInput(_p1Id, _p1Input);
            RollbackManager<NumericalInput>.RegisterLocalInput(_p2Id, _p2Input);

            _frame = 0;
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

        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _totalSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            MouseManager.Update();

            _preKeys = Keyboard.GetState().GetPressedKeys().ToList();
            _newKeys = _preKeys.Where(key => !_accKeys.Contains(key)).ToList();
            _accKeys = _preKeys;

            List<NumericalInput> p1preInp = _preKeys.Select(key => (int)key).Where(key => (int)Keys.D0 <= key && key <= (int)Keys.D9).Select(key => new NumericalInput((NumericalInput.Numbers)(key - (int)Keys.D0))).ToList();
            List<NumericalInput> p1newInp = _newKeys.Select(key => (int)key).Where(key => (int)Keys.D0 <= key && key <= (int)Keys.D9).Select(key => new NumericalInput((NumericalInput.Numbers)(key - (int)Keys.D0))).ToList();
            List<NumericalInput> p1accInp = _accKeys.Select(key => (int)key).Where(key => (int)Keys.D0 <= key && key <= (int)Keys.D9).Select(key => new NumericalInput((NumericalInput.Numbers)(key - (int)Keys.D0))).ToList();

            List<NumericalInput> p2preInp = _preKeys.Select(key => (int)key).Where(key => (int)Keys.NumPad0 <= key && key <= (int)Keys.NumPad9).Select(key => new NumericalInput((NumericalInput.Numbers)(key - (int)Keys.NumPad0))).ToList();
            List<NumericalInput> p2newInp = _newKeys.Select(key => (int)key).Where(key => (int)Keys.NumPad0 <= key && key <= (int)Keys.NumPad9).Select(key => new NumericalInput((NumericalInput.Numbers)(key - (int)Keys.NumPad0))).ToList();
            List<NumericalInput> p2accInp = _accKeys.Select(key => (int)key).Where(key => (int)Keys.NumPad0 <= key && key <= (int)Keys.NumPad9).Select(key => new NumericalInput((NumericalInput.Numbers)(key - (int)Keys.NumPad0))).ToList();

            _p1Input.Update((p1newInp, p1preInp, p1accInp));
            _p2Input.Update((p1newInp, p1preInp, p1accInp));

            RollbackManager<NumericalInput>.UpdateRollback(_frame);

            _frame++;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointWrap);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}