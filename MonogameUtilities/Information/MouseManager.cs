
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace MonogameUtilities.Information
{
    public class MouseManager
    {
        private static Texture2D _cursorTexture = null;
        private static Point _cursorSize = new();
        private static float _cursorScale = 1.0f;
        private static Point _cursorOffset = new(0);
        private static Color _cursorColor = Color.White;
        private static SpriteBatch _spriteBatch = null;
        private static bool _canDrawMouse = false;
        public static bool CanDrawMouse { get => _canDrawMouse; }

        /// <summary>
        /// Clears all references and resets <see cref="CanDrawMouse"/>
        /// </summary>
        public static void Unload()
        {
            _spriteBatch = null;
            _cursorTexture = null;

            _canDrawMouse = false;
        }

        private static Point _point;
        public static Point MousePos { get => _point; }

        private static int _scrollWheelValue = Mouse.GetState().ScrollWheelValue;
        private static int _dScrollWheelValue = 0;
        public static int ScrollWheelValue { get => _scrollWheelValue; }
        public static int DScrollWheelValue { get => _dScrollWheelValue; }

        private static bool _newLeftClick;
        private static bool _newMiddleClick;
        private static bool _newRightClick;
        private static bool _anyNewClick;
        public static bool NewLeftClick { get => _newLeftClick; }
        public static bool NewMiddleClick { get => _newMiddleClick; }
        public static bool NewRightClick { get => _newRightClick; }
        public static bool AnyNewClick { get => _anyNewClick; }

        private static bool _removedLeftClick;
        private static bool _removedMiddleClick;
        private static bool _removedRightClick;
        private static bool _anyRemovedClick;
        public static bool RemovedLeftClick { get => _removedLeftClick; }
        public static bool RemovedMiddleClick { get => _removedMiddleClick; }
        public static bool RemovedRightClick { get => _removedRightClick; }
        public static bool AnyRemovedClick { get => _anyRemovedClick; }

        private static bool _leftClick;
        private static bool _middleClick;
        private static bool _rightClick;
        private static bool _anyClick;
        public static bool LeftClick { get => _leftClick; }
        public static bool MiddleClick { get => _middleClick; }
        public static bool RightClick { get => _rightClick; }
        public static bool AnyClick { get => _anyClick; }


        /// <summary>
        /// Updates all values based on last update
        /// </summary>
        public static void Update()
        {
            MouseState ms = Mouse.GetState();

            _point = ms.Position;

            int swv = Mouse.GetState().ScrollWheelValue;
            _dScrollWheelValue = swv - _scrollWheelValue;
            _scrollWheelValue = swv;

            _removedLeftClick = ms.LeftButton != ButtonState.Pressed && _leftClick;
            _removedMiddleClick = ms.MiddleButton != ButtonState.Pressed && _middleClick;
            _removedRightClick = ms.RightButton != ButtonState.Pressed && _rightClick;
            _anyRemovedClick = _removedLeftClick || _removedMiddleClick || _removedRightClick;

            _newLeftClick = !_leftClick && ms.LeftButton == ButtonState.Pressed;
            _newMiddleClick = !_middleClick && ms.MiddleButton == ButtonState.Pressed;
            _newRightClick = !_rightClick && ms.RightButton == ButtonState.Pressed;
            _anyNewClick = _newLeftClick || _newMiddleClick || _newRightClick;

            _leftClick = ms.LeftButton == ButtonState.Pressed;
            _middleClick = ms.MiddleButton == ButtonState.Pressed;
            _rightClick = ms.RightButton == ButtonState.Pressed;
            _anyClick = _leftClick || _middleClick || _rightClick;
        }

        /// <summary>
        /// Sets the data required to display the mouse.
        /// </summary>
        /// <param name="cursorTexture">The texture of the cursor. Cannot be null.</param>
        /// <param name="cursorScale">The scaling factor of the mouse texture. Must be greater than 0.</param>
        /// <param name="cursorSize">The size of the mouse texture. Both coordinates must be greater than 0. This value is scaled with <paramref name="cursorScale"/>.</param>
        /// <param name="cursorOffset">The offset position of the cursor. This value is scaled with <paramref name="cursorScale"/>.</param>
        /// <param name="cursorColor">The color of the cursor.</param>
        /// <param name="spriteBatch">The sprite batch used to draw the cursor. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="cursorTexture"/> or <paramref name="spriteBatch"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="cursorSize"/> is less than or equal to 0, or <paramref name="cursorScale"/> is less than or equal to 0.</exception>
        public static void SetMouseData(Texture2D cursorTexture, float cursorScale, Point cursorSize, Point cursorOffset, Color cursorColor, SpriteBatch spriteBatch)
        {
            _cursorTexture = cursorTexture ?? throw new ArgumentNullException(nameof(cursorTexture));
            _spriteBatch = spriteBatch ?? throw new ArgumentNullException(nameof(spriteBatch));

            if (cursorSize.X < 1) throw new ArgumentOutOfRangeException(nameof(cursorSize.X), $"{nameof(cursorSize)}.{nameof(cursorSize.X)} must be greater than 0.");
            if (cursorSize.Y < 1) throw new ArgumentOutOfRangeException(nameof(cursorSize.Y), $"{nameof(cursorSize)}.{nameof(cursorSize.Y)} must be greater than 0.");
            _cursorSize = cursorSize;

            if (cursorScale <= 0) throw new ArgumentOutOfRangeException(nameof(cursorScale), $"{nameof(cursorScale)} must be greater than 0.");
            _cursorScale = cursorScale;
            
            _cursorOffset = cursorOffset;

            _cursorColor = cursorColor;

            _canDrawMouse = true;
        }

        /// <summary>
        /// Draws the mouse cursor based on the last <see cref="Update"/> call.<br/>
        /// Requires <see cref="SetMouseData"/> to be run first.
        /// </summary>
        /// <exception cref="InvalidOperationException">Mouse data must be set with <see cref="SetMouseData"/> before execution.</exception>
        public static void DrawCursor()
        {
            if (!_canDrawMouse)
            {
                throw new InvalidOperationException($"{nameof(SetMouseData)} must be run before drawing the mouse.");
            }

            Point topLeft = (_point + _cursorOffset);

            Point size = _cursorSize;
            size.X = (int)(size.X * _cursorScale);
            size.Y = (int)(size.Y * _cursorScale);

            Rectangle bound = new(topLeft, size);

            _spriteBatch.Draw(_cursorTexture, bound, _cursorColor);
        }
    }
}
