using MonogameUtilities.Hitboxes;
using MonogameUtilities.Information;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MonogameUtilities.Elements
{
    public class Canvas : IElement
    {
        internal bool WasClicking = false;

        internal List<IElement> elements;
        internal List<IElement> RemovingElements;

        public IElement focus;

        public int TopLayer { get; set; }

        public Canvas() {
            elements = new List<IElement>();
            RemovingElements = new List<IElement>();
        }

        public virtual void AddElement(IElement element)
        {
            element.SetParent(this);

            elements.Add(element);
        }

        public virtual void RemoveElement(IElement element)
        {
            element.SetParent(null);

            elements.Remove(element);
        }

        /// <summary>
        /// Updates each element by descending layer
        /// </summary>
        public virtual bool Update()
        {
            bool leftClickConsumed = false;

            foreach (var element in elements.OrderBy(ele => ele.GetLayer()).Reverse())
            {
                if (element is Draggable draggable)
                {
                    if (MouseManager.leftClick)
                    {
                        if (MouseManager.newLeftClick && !leftClickConsumed)
                        {
                            leftClickConsumed = draggable.DragStart();
                            if (leftClickConsumed)
                                focus = draggable;
                        }
                        if (!MouseManager.newLeftClick && !leftClickConsumed)
                        {
                            leftClickConsumed = draggable.DragMid();
                            if (leftClickConsumed)
                                focus = draggable;
                        }
                    }
                    else if (WasClicking)
                    {
                        if (!leftClickConsumed)
                        {
                            leftClickConsumed = draggable.DragEnd();
                            if (leftClickConsumed)
                                focus = draggable;
                        }
                    }
                }
            }

            if (MouseManager.leftClick && !leftClickConsumed)
            {
                focus = null;
            }

            WasClicking = MouseManager.leftClick;

            UpdateElements();

            elements.RemoveAll(x => RemovingElements.Contains(x));
            RemovingElements.ForEach(element => element.SetParent(null));
            RemovingElements = new List<IElement>();

            return false;
        }

        private void UpdateElements()
        {

            foreach (var element in elements.OrderBy(ele => ele.GetLayer()).Reverse())
            {
                if (element.Update())
                {
                    RemovingElements.Add(element);
                }
            }
        }

        /// <summary>
        /// Draws each element by ascending layer
        /// </summary>
        public virtual void Draw()
        {
            foreach (var element in elements.OrderBy(ele => ele.GetLayer()))
            {
                element.Draw();
            }
        }

        public virtual int GetLayer() { return -1; }

        public void Bound(Hitbox bound) { }
    }
}
