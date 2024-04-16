
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonogameUtilities.Information
{
    public class MouseManager
    {
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
            point.X /= GlobalData.Scale;
            point.Y /= GlobalData.Scale;

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

        /// <summary>
        /// Returns how much the mouse was rounded by
        /// </summary>
        /// <returns></returns>
        public static Point GetMouseRounded()
        {
            MouseState ms = Mouse.GetState();
            Point rounded = new Point(ms.Position.X % GlobalData.Scale, ms.Position.Y % GlobalData.Scale);
            return rounded;
        }
    }
}
