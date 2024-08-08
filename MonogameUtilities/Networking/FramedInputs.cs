

using System;
using System.Collections.Generic;

namespace MonogameUtilities.Networking
{
    public class FramedInputs<T>
    {

        private (List<T> newInputs, List<T> preInputs, List<T> accInputs)[] _inputs;
        private bool[] _isInput;

        private long _frameOffset;
        public long FrameOffset => _frameOffset;

        public FramedInputs(long frameOffset)
        {
            _inputs = Array.Empty<(List<T> newInputs, List<T> preInputs, List<T> accInputs)>();
            _isInput = Array.Empty<bool>();
            _frameOffset = frameOffset;
        }

        private FramedInputs(long frameOffset, (List<T> newInputs, List<T> preInputs, List<T> accInputs)[] inputs, bool[] isInput)
        {
            _inputs = inputs;
            _isInput = isInput;
            _frameOffset = frameOffset;
        }

        public (List<T> newInputs, List<T> preInputs, List<T> accInputs) GetAt(long frame)
        {
            long index = GetIndex(frame);

            return _inputs[index];
        }

        public bool HasInputAt(long frame)
        {
            long index = GetIndex(frame);

            if (index < 0 || _isInput.Length <= index)
            {
                return false;
            }

            return _isInput[index];
        }

        public bool FrameInRange(long frame)
        {
            return _frameOffset <= frame && frame <= _frameOffset + _inputs.LongLength;
        }

        public void SetAt(long frame, (List<T> newInputs, List<T> preInputs, List<T> accInputs) value)
        {
            long index = GetIndex(frame);

            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index cannot be negative.");
            }

            // Outside the bounds of the inputs
            if (_inputs.Length <= index)
            {
                // New size
                long extra = 1 + index;

                // Swap out _inputs and fill at 0 with the old values
                (List<T> newInputs, List<T> preInputs, List<T> accInputs)[] oldElements = _inputs;
                bool[] oldIsElements = _isInput;
                _inputs = new (List<T> newInputs, List<T> preInputs, List<T> accInputs)[extra];
                _isInput = new bool[extra];
                oldElements.CopyTo(_inputs, 0);
                oldIsElements.CopyTo(_isInput, 0);
            }

            // Set the value at the index
            _inputs[index] = value;
            _isInput[index] = true;
        }

        public void ClearBefore(long frame)
        {
            long cutoff = GetIndex(frame);

            if (cutoff <= 0)
            {
                return;
            }

            _frameOffset = frame;

            long newArrayLen = _inputs.Length - 1 - cutoff;

            // If we are removing everything, just create an empty array
            if (newArrayLen <= 0)
            {
                _inputs = Array.Empty<(List<T> newInputs, List<T> preInputs, List<T> accInputs)>();
                _isInput = Array.Empty<bool>();

                return;
            }

            (List<T> newInputs, List<T> preInputs, List<T> accInputs)[] oldElements = _inputs;
            bool[] oldIsElements = _isInput;
            _inputs = new (List<T> newInputs, List<T> preInputs, List<T> accInputs)[newArrayLen];
            _isInput = new bool[newArrayLen];
            Array.Copy(oldElements, cutoff, _inputs, 0, newArrayLen);
            Array.Copy(oldIsElements, cutoff, _isInput, 0, newArrayLen);
        }

        public FramedInputs<T> Clone()
        {
            (List<T> newInputs, List<T> preInputs, List<T> accInputs)[] newElements = new (List<T> newInputs, List<T> preInputs, List<T> accInputs)[_inputs.Length];
            Array.Copy(_inputs, newElements, _inputs.Length);

            bool[] newIsElements = new bool[_isInput.Length];
            Array.Copy(_isInput, newIsElements, _isInput.Length);

            return new FramedInputs<T>(_frameOffset, newElements, newIsElements);
        }

        /// <summary>
        /// Returns the latest input, with a provided max frame
        /// </summary>
        /// <param name="maxFrame"></param>
        /// <returns></returns>
        public (List<T> newInputs, List<T> preInputs, List<T> accInputs) GetLatestInput(long maxFrame)
        {
            long maxIndex = maxFrame - _frameOffset;

            (List<T> newInputs, List<T> preInputs, List<T> accInputs) foundFrame = default;
            for (int i = 0; i < maxIndex; i++)
            {
                if (HasInputAt(i + _frameOffset))
                {
                    foundFrame = _inputs[i];
                }
            }

            return foundFrame;
        }

        public long GetLatestCompletedFrame()
        {
            long latest = _frameOffset;
            for (long i = _frameOffset; i < _frameOffset + _inputs.LongLength; i++)
            {
                if (!HasInputAt(i))
                {
                    break;
                }

                latest = i;
            }
            return latest;
        }

        public void Update((List<T> newInputs, List<T> preInputs, List<T> accInputs) value)
        {
            SetAt(_frameOffset + _inputs.LongLength, value);
        }

        private long GetIndex(long frame)
        {
            return frame - _frameOffset;
        }

    }
}
