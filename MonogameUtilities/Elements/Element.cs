

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonogameUtilities.Elements.ElementEventArgs;
using MonogameUtilities.Information;
using MonogameUtilities.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MonogameUtilities.Elements
{
    public partial class Element : GenericAttributes
    {
        private Texture2D _texture;

        private readonly Hitbox _bounds;
        public Hitbox Bounds { get { return _bounds; } }

        private readonly List<Element> _strictBounds = null;
        public IReadOnlyList<Element> StrictBounds { get { return _strictBounds; } }

        private Element _parent;
        public Element Parent { get { return _parent; } }
        private readonly List<Element> _children;
        public IReadOnlyList<Element> Children { get { return _children; } }

        private static bool _onClickEventHandled = false;
        public static bool OnClickEventHandled { get {  return _onClickEventHandled; } }
        public delegate bool OnClickEventHandler(object sender, ClickEventArgs e);
        /// <summary>
        /// Event triggered when the mouse is over this element on mouse down. Returns true if the event should be consumed.<br/>
        /// Consumed events will not call any other <see cref="OnClick"/> events.
        /// </summary>
        public event OnClickEventHandler OnClick;

        private static bool _offClickEventHandled = false;
        public static bool OffClickEventHandled { get { return _offClickEventHandled; } }
        public delegate bool OffClickEventHandler(object sender, ClickEventArgs e);
        /// <summary>
        /// Event triggered when the mouse is over this element on mouse up. Returns true if the event should be consumed.<br/>
        /// Consumed events will not call any other <see cref="OffClick"/> events.
        /// </summary>
        public event OnClickEventHandler OffClick;

        private Dictionary<string, Action> _updateActions;
        public ReadOnlyDictionary<string, Action> UpdateActions
        {
            get
            {
                return new ReadOnlyDictionary<string, Action>(_updateActions);
            }
        }

        public Element(int x, int y, int width, int height, Element parent = null, Action updateAction = null)
        {
            _bounds = new Hitbox(x, y, width, height);

            SetParent(parent);

            _children = new List<Element>();
            _updateActions = new();
        }

        public static void StaticUpdate()
        {
            _onClickEventHandled = false;
            _offClickEventHandled = false;
        }

        /// <summary>
        /// Updates the element<br/>
        /// This includes click events, update actions, and children.
        /// </summary>
        /// <returns>Whether should be removed from parent</returns>
        public virtual bool Update()
        {
            List<Element> shouldRemove = new();

            if (!_onClickEventHandled && MouseManager.AnyClick && _bounds.Contains(MouseManager.MousePos))
            {
                _onClickEventHandled = OnClick?
                    .Invoke(
                        this,
                        new ClickEventArgs(
                            MouseManager.MousePos.X,
                            MouseManager.MousePos.Y,
                            MouseManager.LeftClick,
                            MouseManager.MiddleClick,
                            MouseManager.RightClick
                        )
                    ) ?? false;
            }
            if (!_offClickEventHandled && _bounds.Contains(MouseManager.MousePos))
            {
                _offClickEventHandled = OffClick?
                    .Invoke(
                        this,
                        new ClickEventArgs(
                            MouseManager.MousePos.X,
                            MouseManager.MousePos.Y,
                            MouseManager.LeftClick,
                            MouseManager.MiddleClick,
                            MouseManager.RightClick
                        )
                    ) ?? false;
            }

            _updateActions.Values.ToList().ForEach(action => action.Invoke());

            foreach (Element child in _children)
            {
                if (child.Update())
                {
                    shouldRemove.Add(child);
                }
            }

            _children.RemoveAll(child => shouldRemove.Contains(child));
            return false;
        }

        /// <summary>
        /// Adds an update action by name.
        /// </summary>
        /// <param name="actionName">The name of the action.</param>
        /// <param name="action">The action to perform.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="actionName"/> is null or empty, or when an action with the same name already exists.
        /// </exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="action"/> is null.</exception>
        public void AddUpdateAction(string actionName, Action action)
        {
            if (string.IsNullOrEmpty(actionName))
            {
                throw new ArgumentException("Input cannot be null or empty.", nameof(actionName));
            }
            else if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            else if (_updateActions.ContainsKey(actionName))
            {
                throw new ArgumentException($"An action with the name '{actionName}' already exists.", nameof(actionName));
            }

            _updateActions.Add(actionName, action);
        }

        /// <summary>
        /// Removes an update action by name.
        /// </summary>
        /// <param name="actionName">The name of the action to remove.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="actionName"/> is null or empty.</exception>
        /// <exception cref="KeyNotFoundException">Thrown when an action with the provided name does not exist.</exception>
        public void RemoveUpdateAction(string actionName)
        {
            if (string.IsNullOrEmpty(actionName))
            {
                throw new ArgumentException("Input cannot be null or empty.", nameof(actionName));
            }
            else if (!_updateActions.ContainsKey(actionName))
            {
                throw new KeyNotFoundException($"An action with the name '{actionName}' does not exist.");
            }

            _updateActions.Remove(actionName);
        }

        /// <summary>
        /// Draws the element and its children using the specified sprite batch and scale.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch used to draw the element. Cannot be null.</param>
        /// <param name="scale">The scaling factor to apply to the drawing.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="spriteBatch"/> is null.</exception>
        public virtual void Draw(SpriteBatch spriteBatch, float scale)
        {
            spriteBatch = spriteBatch ?? throw new ArgumentNullException(nameof(spriteBatch));

            if (_texture != null)
            {
                Rectangle bound = _bounds.GetRectangle();

                spriteBatch.Draw(_texture, bound, Color.White);
            }

            foreach (Element child in _children)
            {
                child.Draw(spriteBatch, scale);
            }
        }

        /// <summary>
        /// Returns a reference to the element's hitbox
        /// </summary>
        /// <returns></returns>
        public virtual Hitbox GetBounds() { return _bounds; }

        /// <summary>
        /// Forces the element within the provided bounds
        /// </summary>
        /// <param name="bounding"></param>
        public virtual void Bound(Hitbox bounding)
        {
            if (!bounding.Contains(_bounds))
            {
                _bounds.X = Math.Max(_bounds.X, bounding.X);
                _bounds.Y = Math.Max(_bounds.Y, bounding.Y);

                if (bounding.RightSide < _bounds.RightSide)
                {
                    _bounds.X = bounding.RightSide - _bounds.Width;
                }
                if (bounding.BottomSide < _bounds.BottomSide)
                {
                    _bounds.Y = bounding.BottomSide - _bounds.Height;
                }
            }
        }

        /// <summary>
        /// Ensure this bounding hitbox intersects the given hitbox
        /// </summary>
        /// <param name="bounding">Hitbox to intersect with</param>
        public virtual void Intersect(Hitbox bounding, int margin)
        {
            Hitbox boundWithMargins = new(_bounds.X + margin, _bounds.Y + margin, _bounds.Width - 2 * margin, _bounds.Height - 2 * margin);
            if (boundWithMargins.Outside(bounding))
            {
                int dX = 0;
                int dY = 0;
                if (bounding.RightSide <= _bounds.X + margin)
                {
                    dX = bounding.RightSide - margin - _bounds.X;
                }
                else if (_bounds.RightSide - margin <= bounding.X)
                {
                    dX = bounding.X + margin - _bounds.Width - _bounds.X;
                }
                if (bounding.BottomSide <= _bounds.Y + margin)
                {
                    dY = bounding.BottomSide - margin - _bounds.Y;
                }
                else if (_bounds.BottomSide - margin <= bounding.Y)
                {
                    dY = bounding.Y + margin - _bounds.Height - _bounds.Y;
                }

                _bounds.X += dX;
                _bounds.Y += dY;
            }
        }

        /// <summary>
        /// </summary>
        /// <returns>the element position offset by the parent element</returns>
        public virtual Point GetPos()
        {
            return _bounds.Position;
        }

        /// <summary>
        /// Sets the position of the element. <br/>
        /// This operation complies to any <see cref="_strictBounds"/> set.
        /// </summary>
        /// <param name="pos">The new position to set for the element.</param>
        public virtual void SetPos(Point pos)
        {
            if (_strictBounds?.Any() ?? false)
            {
                Element currentBounding = _strictBounds.Where(element => element._bounds.Contains(this)).First();

                int oldX = _bounds.X;
                int oldY = _bounds.Y;

                // No bounds catches the element
                if (!_strictBounds.Exists(strictBound => strictBound._bounds.Contains(_bounds)))
                {
                    _bounds.ForceContainedBy(currentBounding._bounds);
                }
            }
            else
            {
                _bounds.X = pos.X;
                _bounds.Y = pos.Y;
            }
        }

        /// <summary>
        /// Offsets the element by the specified amount. <br/>
        /// This operation complies to any strict bounds set.
        /// </summary>
        /// <param name="offset"></param>
        public virtual void ShiftBy(Point offset)
        {
           SetPos(_bounds.Position + offset);
        }

        public virtual void ShiftAllBy(Point offset)
        {
            _bounds.ShiftBy(offset);
            _children?.ForEach(child => child.ShiftAllBy(offset));
        }

        /// <summary>
        /// Adds the child to the element's children. Sets the child's parent to this.
        /// </summary>
        /// <param name="child"></param>
        /// <exception cref="ArgumentNullException">Child must not be null.</exception>
        public virtual void AddChild(Element child)
        {
            _ = child ?? throw new ArgumentNullException(nameof(child));

            _children.Add(child);
            child.SetParent(this);
        }

        /// <summary>
        /// Removes the specified child element from the list of children and clears its parent reference.
        /// </summary>
        /// <param name="child">The child element to be removed.</param>
        public virtual void RemoveChild(Element child)
        {
            child?.ClearParent();
            _children.Remove(child);
        }

        /// <summary>
        /// Sets the parent of this element.
        /// </summary>
        /// <param name="element">The element to set as the parent.</param>
        public virtual void SetParent(Element element)
        {
            _parent = element;
        }

        /// <summary>
        /// Clears the parent reference of this element.
        /// </summary>
        public virtual void ClearParent()
        {
            _parent = null;
        }

        /// <summary>
        /// Sets the texture for this element.
        /// </summary>
        /// <param name="texture">The texture to set for this element.</param>
        public virtual void SetTexture(Texture2D texture)
        {
            _texture = texture;
        }

        public static implicit operator Hitbox(Element e) => e.GetBounds();
    }
}
