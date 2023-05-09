
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static void Update()
        {
            Microsoft.Xna.Framework.Input.MouseState ms = Mouse.GetState();

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
    }
}
