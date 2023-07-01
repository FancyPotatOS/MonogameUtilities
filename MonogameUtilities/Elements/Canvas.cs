using MonogameUtilities.Hitboxes;
using MonogameUtilities.Information;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Xml.Linq;

namespace MonogameUtilities.Elements
{
    public class Canvas : Element
    {
        internal bool WasClicking = false;

        internal List<IElement> RemovingElements;
        public IElement focus;


        public Canvas(int x = 0, int y = 0, int width = 0, int height = 0, IElement parent = null) : base(x, y, width, height, parent) {
            Children = new List<IElement>();
            RemovingElements = new List<IElement>();
        }

        public virtual void AddElement(IElement element)
        {
            element.SetParent(this);

            Children.Add(element);
        }

        public virtual void RemoveElement(IElement element)
        {
            element.SetParent(null);

            Children.Remove(element);
        }

        /// <summary>
        /// Updates each element by descending layer
        /// </summary>
        public override bool Update()
        {
            bool leftClickConsumed = false;

            foreach (var element in Children.OrderBy(ele => ele.GetLayer()).Reverse())
            {
                if (element is Draggable draggable)
                {
                    if (MouseManager.leftClick)
                    {
                        if (MouseManager.newLeftClick && !leftClickConsumed)
                        {
                            leftClickConsumed = draggable.DragStart();
                            /** /
                            if (leftClickConsumed)
                                focus = draggable;
                            /**/
                        }
                        if (!MouseManager.newLeftClick && !leftClickConsumed)
                        {
                            leftClickConsumed = draggable.DragMid();
                            /** /
                            if (leftClickConsumed)
                                focus = draggable;
                            /**/
                        }
                    }
                    else if (WasClicking)
                    {
                        if (!leftClickConsumed)
                        {
                            leftClickConsumed = draggable.DragEnd();
                            /** /
                            if (leftClickConsumed)
                                focus = draggable;
                            /**/
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

            Children.RemoveAll(x => RemovingElements.Contains(x));
            RemovingElements.ForEach(element => element.SetParent(null));
            RemovingElements = new List<IElement>();

            base.Update();

            return false;
        }

        private void UpdateElements()
        {

            foreach (var element in Children.OrderBy(ele => ele.GetLayer()).Reverse())
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
        public override void Draw()
        {
            foreach (var element in Children.OrderBy(ele => ele.GetLayer()))
            {
                element.Draw();
            }

            base.Draw();
        }

        public override int GetLayer() { return -1; }

        public override void Bound(Hitbox bound) { base.Bound(bound); }
    }
}
