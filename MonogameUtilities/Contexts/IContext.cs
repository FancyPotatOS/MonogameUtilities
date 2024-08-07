
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MonogameUtilities.Contexts
{
    public interface IContext
    {
        public Task Update();

        public void Draw();

        private static List<IContext> _contextStack = new();

        public static IContext PeekFromStack()
        {
            if (!HasContextInStack())
            {
                return null;
            }

            return _contextStack[0];
        }

        public static bool HasContextInStack()
        {
            return _contextStack.Count != 0;
        }

        public static IContext PopFromStack()
        {
            IContext top = PeekFromStack();
            _contextStack = _contextStack.GetRange(1, _contextStack.Count - 1);

            return top;
        }

        public static void PushToStack(IContext context)
        {
            _contextStack.Insert(0, context);
        }

        /// <summary>
        /// Gets the context lower on the stack
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IContext GetContextBefore(IContext context)
        {
            int index = _contextStack.IndexOf(context);

            // If it's the lowest item on the stack, just return null
            if (index == _contextStack.Count - 1)
            {
                return null;
            }

            return _contextStack[index + 1];
        }

        /// <summary>
        /// Continues to pop until reached specified context Type
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static void PopToContext(Type targetType)
        {
            while (HasContextInStack() && PeekFromStack().GetType() != targetType)
            {
                PopFromStack();
            }
        }
    }
}
