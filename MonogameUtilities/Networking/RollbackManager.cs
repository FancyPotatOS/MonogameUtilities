

using MonogameUtilities.Util;
using System;
using Microsoft.AspNetCore.SignalR.Client;
using System.Text.Json.Nodes;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.ComponentModel.Design;

namespace MonogameUtilities.Networking
{
    public static class RollbackManager<T> where T : IStringableGeneric
    {

        public enum InputExtrapolateStrategy
        {
            LastInput,
            DefaultValue,
            ProvidedValue
        }

        private static HubConnection _connection = null;
        private static Guid _userId;

        private static List<JsonWrapper> _privateMessages = new();
        private static List<JsonWrapper> _publicMessages = new();

        private static Dictionary<Guid, FramedInputs<T>> _networkInputs;
        private static Dictionary<Guid, FramedInputs<T>> _localInputs = new();

        private static Func<string, T> _forwardConverterMethod = null;
        private static Func<T, string> _backwardConverterMethod = null;

        private static List<IRollbackObject<T>> _rollbackObjects = new();
        private static long _latestCompleteFrame;

        private static InputExtrapolateStrategy _inputExtrapolateStrategy = InputExtrapolateStrategy.LastInput;
        private static (List<T> newInp, List<T> preInp, List<T> accInp) _inputDefault = default;

        /// <summary>
        /// Connect to a server
        /// </summary>
        /// <param name="userId">The current user ID</param>
        /// <param name="serverName">The name of the game/server</param>
        /// <param name="address">the IP address of the server, including the <em>http://</em> or <em>https://</em></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Thrown when the connection is running or being run</exception>
        public static async Task Connect(Guid userId, string serverName, string address)
        {
            if (_connection != null)
            {
                throw new InvalidOperationException("Cannot connect once a connection has been made.");
            }

            _connection = new HubConnectionBuilder()
                .WithUrl(address + "/" + serverName)
                .Build();

            _connection.On<string, string>(nameof(ReceiveMessage), ReceiveMessage);

            await _connection.StartAsync();
            _userId = userId;

            _networkInputs = new();
        }

        public static async Task Disconnect()
        {
            if (_connection == null)
            {
                throw new InvalidOperationException("Cannot disconnect when no connection has been made.");
            }

            await _connection.DisposeAsync();

            _connection = null;
            _userId = Guid.Empty;
            _networkInputs = null;
        }

        /// <summary>
        /// Returns true if completely connected
        /// </summary>
        /// <returns></returns>
        public static bool IsConnected()
        {
            return GetConnectionState() == HubConnectionState.Connected;
        }

        /// <summary>
        /// Callback method to when network message is received
        /// </summary>
        /// <param name="user"></param>
        /// <param name="json"></param>
        /// <exception cref="Exception"></exception>
        private static void ReceiveMessage(string user, string json)
        {
            JsonWrapper jsonParsed = new(JsonNode.Parse(json));

            if (jsonParsed["type"] == "private")
            {
                _privateMessages.Add(jsonParsed);
            }
            else if (jsonParsed["type"] == "public")
            {
                _publicMessages.Add(jsonParsed);
            }
            else
            {
                throw new Exception($"Unexpected message type '{jsonParsed["type"]}' in rollback netcode!");
            }
        }

        /// <summary>
        /// Returns the current connection state
        /// </summary>
        /// <returns></returns>
        public static HubConnectionState GetConnectionState() { return _connection == null ? HubConnectionState.Disconnected : _connection.State; }

        /// <summary>
        /// Registers a rollback object. Will automatically remove reference to object when specified to delete and past latest complete frame
        /// </summary>
        /// <param name="rollbackObject"></param>
        public static void RegisterRollbackObject(IRollbackObject<T> rollbackObject)
        {
            _rollbackObjects.Add(rollbackObject);
        }

        /// <summary>
        /// Registers the conversion methods between string and input
        /// </summary>
        /// <param name="forwardConverter"></param>
        /// <param name="backwardConverter"></param>
        public static void RegisterConverter(Func<string, T> forwardConverter, Func<T, string> backwardConverter)
        {
            _forwardConverterMethod = forwardConverter;

            _backwardConverterMethod = backwardConverter;
        }

        /// <summary>
        /// Updates the connection messages
        /// </summary>
        /// <returns></returns>
        public static bool ConnectionUpdate()
        {
            while (0 < _privateMessages.Count)
            {
                JsonWrapper privateMessage = _privateMessages[0];
                _privateMessages.RemoveAt(0);

                if (privateMessage["message"] == "disconnect")
                {
                    Disconnect();
                }
                else if (privateMessage["message"] == "input")
                {
                    JsonWrapper input = privateMessage["input"];

                    _privateMessages.Add(input);
                }
            }

            return !IsConnected();
        }

        /// <summary>
        /// Sends a json object through the connection.
        /// </summary>
        /// <param name="message"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public static void Send(string message)
        {
            if (!IsConnected())
            {
                throw new InvalidOperationException("Cannot send a message if no connection is established to the server");
            }

            _connection.InvokeAsync(nameof(ReceiveMessage), _userId.ToString(), message);
        }

        /// <summary>
        /// Gets the next provided message from the connection
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static JsonWrapper GetNext()
        {
            if (!HasAvailableMessage())
            {
                throw new Exception("No messages available to grab.");
            }

            JsonWrapper publicMessage = _publicMessages[0];
            _publicMessages.RemoveAt(0);

            return publicMessage;
        }

        /// <summary>
        /// Returns true if there is an available message
        /// </summary>
        /// <returns></returns>
        public static bool HasAvailableMessage()
        {
            return 0 < _publicMessages.Count;
        }

        /// <summary>
        /// Returns true when there is a private rollback message
        /// </summary>
        /// <returns></returns>
        private static bool HasAvailablePrivateMessage()
        {
            return 0 < _privateMessages.Count;
        }

        public static void SetInputExtrapolateStrategy(InputExtrapolateStrategy strategy, (List<T> newInp, List<T> preInp, List<T> accInp) defaultValue = default)
        {
            if (strategy == InputExtrapolateStrategy.LastInput)
            {
                _inputExtrapolateStrategy = strategy;
            }
            else if (strategy == InputExtrapolateStrategy.DefaultValue)
            {
                _inputExtrapolateStrategy = strategy;

                _inputDefault = default;
            }
            else if (strategy == InputExtrapolateStrategy.ProvidedValue)
            {
                _inputDefault = defaultValue;
            }
        }

        /// <summary>
        /// Registers an input as locally managed
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="input"></param>
        public static void RegisterLocalInput(Guid guid, FramedInputs<T> input)
        {
            _localInputs[guid] = input;
        }

        /// <summary>
        /// Performs a whole update routine on every input, then on every rollback object with extrapolated inputs.<br/>
        /// Clears all rollback objects that are marked for deletion and is passed the newest completed input frame
        /// </summary>
        /// <param name="currentFrame"></param>
        public static void UpdateRollback(long currentFrame)
        {
            UpdateLocalInputs(currentFrame);

            UpdateRemoteInputs();

            // The newest frame that's inputs are all known
            long newestCompleteFrame = Math.Min(_localInputs.Select(input => input.Value.GetLatestCompletedFrame()).DefaultIfEmpty(currentFrame).Min(), (_networkInputs ?? new()).Select(input => input.Value.GetLatestCompletedFrame()).DefaultIfEmpty(currentFrame).Min());
            //long newestCompleteFrame = Math.Min(_localInputs.Select(input => input.Value.FrameOffset).DefaultIfEmpty(0).Min(), (_networkInputs ?? new()).Select(input => input.Value.FrameOffset).DefaultIfEmpty(0).Min());

            // The amount of frames that have been completed increased
            long difference = newestCompleteFrame - _latestCompleteFrame;
            if (0 < difference)
            {
                // Rollback the objects
                _rollbackObjects.ForEach(rbobj => rbobj.RestoreState());

                // Update iteratively until caught up to completed frame
                for (long i = 0; i < difference; i++)
                {
                    long intermittentUpdateFrame = _latestCompleteFrame + i;

                    // Get all the inputs for the frame
                    Dictionary<Guid, (List<T> newInp, List<T> preInp, List<T> accInp)> realInputs = new();
                    foreach (KeyValuePair<Guid, FramedInputs<T>> kvp in _localInputs)
                    {
                        if (!kvp.Value.HasInputAt(intermittentUpdateFrame))
                        {
                            realInputs[kvp.Key] = default;
                        }
                        else
                        {
                            realInputs[kvp.Key] = kvp.Value.GetAt(intermittentUpdateFrame);
                        }
                    }
                    foreach (KeyValuePair<Guid, FramedInputs<T>> kvp in _networkInputs ?? new())
                    {
                        realInputs[kvp.Key] = kvp.Value.GetAt(intermittentUpdateFrame);
                    }

                    _rollbackObjects.ForEach(rbobj => rbobj.Update(realInputs));
                }

                _rollbackObjects.ForEach(rbobj => rbobj.SaveState());
            }

            // Save the newest completed frame as the latest minus one, to preserve the last input
            _latestCompleteFrame = newestCompleteFrame;

            // Clear all the old inputs that are now passed completion
            _localInputs.ToList().ForEach(kvp => kvp.Value.ClearBefore(_latestCompleteFrame));
            _networkInputs?.ToList().ForEach(kvp => kvp.Value.ClearBefore(_latestCompleteFrame));

            for (long i = _latestCompleteFrame; i < currentFrame; i++)
            {
                // Get all the inputs for the frame, faking where needed
                Dictionary<Guid, (List<T> newInp, List<T> preInp, List<T> accInp)> fakedInputs = GetFakedInputs(i);

                _rollbackObjects.ForEach(rbobj => rbobj.Update(fakedInputs));
            }
        }

        /// <summary>
        /// Returns all user inputs, using the strategy to fill in network inputs where needed
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        private static Dictionary<Guid, (List<T> newInp, List<T> preInp, List<T> accInp)> GetFakedInputs(long frame)
        {
            // Get all the inputs for the frame
            Dictionary<Guid, (List<T> newInp, List<T> preInp, List<T> accInp)> fakeInputs = new();

            // Populate easy local inputs
            foreach (KeyValuePair<Guid, FramedInputs<T>> kvp in _localInputs)
            {
                if (!kvp.Value.HasInputAt(frame))
                {
                    fakeInputs[kvp.Key] = default;
                    continue;
                }

                fakeInputs[kvp.Key] = kvp.Value.GetAt(frame);
            }

            // Populate network inputs, extrapolating where needed
            foreach (KeyValuePair<Guid, FramedInputs<T>> kvp in _networkInputs ?? new())
            {
                Guid userId = kvp.Key;
                FramedInputs<T> input = kvp.Value;

                (List<T> newInp, List<T> preInp, List<T> accInp) currentFake;
                // Not faked
                if (input.HasInputAt(frame))
                {
                    currentFake = input.GetAt(frame);
                }
                // Faked
                else
                {
                    currentFake = GetInputByStrategy(userId, frame);
                }

                fakeInputs[userId] = currentFake;
            }

            return fakeInputs;
        }

        /// <summary>
        /// Gets input of user, faking by strategy
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="frame"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static (List<T> newInp, List<T> preInp, List<T> accInp) GetInputByStrategy(Guid userId, long frame)
        {
            long targetFrame = frame - 1;

            if (_inputExtrapolateStrategy == InputExtrapolateStrategy.DefaultValue)
            {
                return default;
            }
            else if (_inputExtrapolateStrategy == InputExtrapolateStrategy.ProvidedValue)
            {
                return _inputDefault;
            }
            else if (_inputExtrapolateStrategy == InputExtrapolateStrategy.LastInput)
            {
                FramedInputs<T> userInput = _networkInputs[userId];

                return userInput.GetLatestInput(frame);
            }
            else
            {
                throw new Exception($"Unknown input extrapolation strategy {_inputExtrapolateStrategy}.");
            }
        }

        private static void UpdateLocalInputs(long frame)
        {
            if (_networkInputs == null || !_networkInputs.Any())
            {
                return;
            }

            foreach (KeyValuePair<Guid, FramedInputs<T>> kvp in _localInputs)
            {
                FramedInputs<T> input = kvp.Value;

                if (!input.HasInputAt(frame))
                {
                    continue;
                }

                (List<T> newInputs, List<T> preInputs, List<T> accInputs) = input.GetAt(frame);

                string guid = kvp.Key.ToString();

                string newInputString = "";
                if (newInputs.Any())
                {
                    newInputString = '"' + string.Join("\",\"", newInputs.Select(t => _backwardConverterMethod.Invoke(t))) + '"';
                }

                string preInputString = "";
                if (preInputs.Any())
                {
                    preInputString = '"' + string.Join("\",\"", preInputs.Select(t => _backwardConverterMethod.Invoke(t))) + '"';
                }

                string accInputString = "";
                if (accInputs.Any())
                {
                    accInputString = '"' + string.Join("\",\"", accInputs.Select(t => _backwardConverterMethod.Invoke(t))) + '"';
                }

                string json = $"" +
                    $"{{" +
                        $"\"type\": \"private\"," +
                        $"\"message\": \"input\"," +
                        $"\"input\": {{" +
                            $"\"owner\": \"{guid}\"," +
                            $"\"frame\":\"{frame}\"," +
                            $"\"new\": [ {newInputString} ]," +
                            $"\"pre\": [ {preInputString} ]," +
                            $"\"acc\": [ {accInputString} ]" +
                        $"}}" +
                    $"}}";

                Send(json);
            }
        }

        private static void UpdateRemoteInputs()
        {
            if (!IsConnected())
            {
                return;
                throw new InvalidOperationException("Cannot update inputs if no connection is established to the server.");
            }
            else if (_forwardConverterMethod == null || _backwardConverterMethod == null)
            {
                throw new InvalidOperationException($"Cannot update inputs if no conversion methods are provided. Provide conversion methods to {nameof(RegisterConverter)}");
            }

            while (HasAvailablePrivateMessage())
            {
                JsonWrapper message = GetNext();

                Guid owner = message["owner"];

                long frame = long.Parse(message["frame"]);

                FramedInputs <T> input = _networkInputs[owner];

                List<T> newT = message["new"].AsArray<string>().Select(val => _forwardConverterMethod.Invoke(val)).ToList();
                List<T> preT = message["pre"].AsArray<string>().Select(val => _forwardConverterMethod.Invoke(val)).ToList();
                List<T> accT = message["acc"].AsArray<string>().Select(val => _forwardConverterMethod.Invoke(val)).ToList();

                input.SetAt(frame, (newT, preT, accT));
            }
        }

    }
}
