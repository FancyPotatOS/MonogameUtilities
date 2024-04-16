
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonogameUtilities.Information
{
    internal interface IGlobalDataProvider
    {
        public static IGlobalDataProvider Instance { get; }

        public int GetWindowWidth();
        public void SetWindowWidth(int width);

        public int GetWindowHeight();
        public void SetWindowHeight(int height);

        public TimeSpan GetTargetElapsedTime();
        public void SetTargetElapsedTime(TimeSpan targetElapsedTime);

        public GraphicsDevice GetGraphicsDevice();
        public void SetGraphicsDevice(GraphicsDevice grahicsDevice);

        public ContentManager GetContentManager();
        public void SetContentManager(ContentManager contentManager);
    }
}
