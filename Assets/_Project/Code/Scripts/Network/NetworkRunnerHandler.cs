namespace BlackFire
{
    using System;
    using System.Collections.Generic;
    using Cinemachine;
    using Fusion;
    using Fusion.Sockets;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public struct NetworkInputData : INetworkInput
    {
        public Vector3 Direction;
    }

    [DisallowMultipleComponent]
    public class NetworkRunnerHandler : MonoBehaviour, INetworkRunnerCallbacks
    {
        [SerializeField]
        private NetworkPrefabRef _playerPrefab;

        [SerializeField]
        private GameMode _mode = GameMode.Host;

        private NetworkRunner _runner = default;
        private InputHandler _inputHandler = default;

        private Dictionary<PlayerRef, NetworkObject> _spawnedPlayers = new Dictionary<PlayerRef, NetworkObject>();

        public void OnConnectedToServer(NetworkRunner runner)
        {
            Debug.Log($"Connected to server: {runner.LocalPlayer.PlayerId}");
        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {
        }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {
        }

        public void OnDisconnectedFromServer(NetworkRunner runner)
        {
        }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            if (_inputHandler == null && PlayerController.Local)
                _inputHandler = PlayerController.Local.GetComponent<InputHandler>();

            if (_inputHandler != null)
                input.Set(_inputHandler.GetInput());
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (_runner.IsServer)
            {
                Vector3 spawnPos = new Vector3((player.RawEncoded % runner.Config.Simulation.DefaultPlayers) * 3.0f, 1.0f, 0.0f);
                NetworkObject networkPlayerObj = runner.Spawn(_playerPrefab, spawnPos, Quaternion.identity, player);

                _spawnedPlayers.Add(player, networkPlayerObj);
            }
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            if (_spawnedPlayers.TryGetValue(player, out NetworkObject playerObj))
            {
                runner.Despawn(playerObj);
                _spawnedPlayers.Remove(player);
            }
        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
        {
        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
        }

        private async void StartGame(GameMode mode)
        {
            _runner = gameObject.AddComponent<NetworkRunner>();
            _runner.ProvideInput = true;

            await _runner.StartGame(new StartGameArgs()
            {
                GameMode = mode,
                SessionName = "MotionMatchingTest",
                Scene = SceneManager.GetActiveScene().buildIndex,
                SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
            });
        }

        private void OnGUI()
        {
            if (_runner != null) { return; }

            if (GUI.Button(new Rect(0, 0, 200, 40), "Host"))
                StartGame(_mode);
            else if (GUI.Button(new Rect(0, 40, 200, 40), "Join"))
                StartGame(GameMode.Client);
        }
    }
}
