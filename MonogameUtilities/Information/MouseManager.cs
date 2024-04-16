
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonogameUtilities.Information
{
    public class MouseManager
    {
        public static Texture2D cursorTexture = null;
        public static Point cursorSize = new();
        public static Point cursorScale = new(1);
        public static Point cursorOffset = new(0);
        public static Color cursorColor = Color.White;
        public static SpriteBatch spriteBatch = null;

        public static Point point;

        public static int scrollWheelValue = Mouse.GetState().ScrollWheelValue;
        public static int dScrollWheelValue = 0;

        public static bool newLeftClick;
        public static bool newMiddleClick;
        public static bool newRightClick;

        public static bool leftClick;
        public static bool middleClick;
        public static bool rightClick;

        /// <summary>
        /// Updates all values based on last update
        /// </summary>
        public static void Update()
        {
            MouseState ms = Mouse.GetState();

            point = ms.Position;

            int swv = Mouse.GetState().ScrollWheelValue;
            dScrollWheelValue = swv - scrollWheelValue;
            scrollWheelValue = swv;

            newLeftClick = !leftClick && ms.LeftButton == ButtonState.Pressed;
            newMiddleClick = !middleClick && ms.MiddleButton == ButtonState.Pressed;
            newRightClick = !rightClick && ms.RightButton == ButtonState.Pressed;

            leftClick = ms.LeftButton == ButtonState.Pressed;
            middleClick = ms.MiddleButton == ButtonState.Pressed;
            rightClick = ms.RightButton == ButtonState.Pressed;
        }

        public static void DrawCursor()
        {
            spriteBatch.Draw(cursorTexture, new Rectangle((point + cursorOffset) * cursorScale, cursorSize * cursorScale), cursorColor);
        }
    }
}
