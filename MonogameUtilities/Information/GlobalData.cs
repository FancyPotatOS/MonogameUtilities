﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonogameUtilities.Information
{
    public static class GlobalData
    {
        public static int Scale = 1;
        public static int WindowWidth = 512;
        public static int WindowHeight = 512;

        public static readonly TimeSpan TargetElapsedTime = TimeSpan.FromSeconds(1d / 60d);

        public static SpriteBatch StaticSpriteBatchReference;
        public static Texture2D Cursor;

        public static void DrawCursor()
        {
            Point mousePos = MouseManager.point;
            StaticSpriteBatchReference.Draw(Cursor, new Rectangle(mousePos.X * Scale, mousePos.Y * Scale, Cursor.Width * Scale, Cursor.Width * Scale), Color.White);
        }

        public static void Draw(Texture2D tex, Rectangle bound, Color color, SpriteBatch spriteBatch)
        {
            bound.X *= Scale;
            bound.Y *= Scale;
            bound.Width *= Scale;
            bound.Height *= Scale;
            spriteBatch.Draw(tex, bound, color);
        }
    }
}