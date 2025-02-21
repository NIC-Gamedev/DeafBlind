using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using System;
using Unity.VisualScripting.YamlDotNet.Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace FishNet.Component.Spawning
{

    /// <summary>
    /// Spawns a player object for clients when they connect.
    /// Must be placed on or beneath the NetworkManager object.
    /// </summary>
    [AddComponentMenu("FishNet/Component/PlayerSpawner")]
    public class PlayerSpawner : MonoBehaviour
    {
        #region Public.
        /// <summary>
        /// Called on the server when a player is spawned.
        /// </summary>
        public event Action<NetworkObject> OnSpawned;
        #endregion

        #region Serialized.
        /// <summary>
        /// Prefab to spawn for the player.
        /// </summary>
        [Tooltip("Prefab to spawn for the player.")]
        
        [SerializeField] private NetworkObject _deafPrefab; // Префаб первого персонажа
        [SerializeField] private NetworkObject _blindPrefab; // Префаб второго персонажа

        private bool _isDeafAChosen = false; // Флаг выбора первого персонажа
        private bool _isBlindAChosen = false; // Флаг выбора второго персонажа
        //private NetworkObject _playerPrefab;
        /// <summary>
        /// Sets the PlayerPrefab to use.
        /// </summary>
        /// <param name="nob"></param>
        /// <summary>
        /// True to add player to the active scene when no global scenes are specified through the SceneManager.
        /// </summary>
        [Tooltip("True to add player to the active scene when no global scenes are specified through the SceneManager.")]
        [SerializeField]
        private bool _addToDefaultScene = true;
        /// <summary>
        /// Areas in which players may spawn.
        /// </summary>
        [Tooltip("Areas in which players may spawn.")]
        public Transform[] Spawns = new Transform[0];
        #endregion

        #region Private.
        /// <summary>
        /// NetworkManager on this object or within this objects parents.
        /// </summary>
        private NetworkManager _networkManager;
        /// <summary>
        /// Next spawns to use.
        /// </summary>
        private int _nextSpawn;
        #endregion

        private void Start()
        {
            InitializeOnce();
        }

        private void OnDestroy()
        {
            if (_networkManager != null)
                _networkManager.SceneManager.OnClientLoadedStartScenes -= SceneManager_OnClientLoadedStartScenes;
        }
 

        /// <summary>
        /// Initializes this script for use.
        /// </summary>
        private void InitializeOnce()
        {
            _networkManager = InstanceFinder.NetworkManager;
            if (_networkManager == null)
            {
                NetworkManagerExtensions.LogWarning($"PlayerSpawner on {gameObject.name} cannot work as NetworkManager wasn't found on this object or within parent objects.");
                return;
            }

            _networkManager.SceneManager.OnClientLoadedStartScenes += SceneManager_OnClientLoadedStartScenes;
        }

        /// <summary>
        /// Вызывается, когда клиент загрузил начальные сцены после подключения.
        /// </summary>
        private void SceneManager_OnClientLoadedStartScenes(NetworkConnection conn, bool asServer)
        {
            if (!asServer)
                return;

            NetworkObject selectedPrefab = null;

            // Получаем выбор игрока из настроек
            bool isBlind = Convert.ToBoolean(PlayerPrefs.GetString("IsBlind"));
            Debug.Log("Isblind" + isBlind);
            // Определяем, является ли подключение локальным (с того же компьютера)
            // Здесь предполагается, что локальный клиент подключается через "127.0.0.1"
            bool isLocalClient = conn.GetAddress().Equals("127.0.0.1");

            if (isLocalClient)
            {
                // Если клиент с того же компьютера – используем выбор игрока
                if (isBlind)
                {
                    selectedPrefab = _blindPrefab;
                    _isDeafAChosen = false;
                    _isBlindAChosen = true; 

                }
                else
                {
                    selectedPrefab = _deafPrefab;
                    _isDeafAChosen = true;
                    _isBlindAChosen = false; 


                }
            }
            else
            {
                // Если клиент с другого компьютера в сети – игнорируем выбор игрока
                // и используем оставшуюся логику выбора персонажа
                if (_isDeafAChosen == false)
                {
                    selectedPrefab = _deafPrefab;
                    _isDeafAChosen = true;
                }
                if(_isBlindAChosen == false)
                {
                    selectedPrefab = _blindPrefab;
                    _isBlindAChosen = true; // Второй игрок получает второго персонаж
                }
            }

            if (selectedPrefab == null)
            {
                NetworkManagerExtensions.LogWarning($"Player prefab is empty and cannot be spawned for connection {conn.ClientId}.");
                return;
            }

            Vector3 position;
            Quaternion rotation;
            SetSpawn(selectedPrefab.transform, out position, out rotation);

            NetworkObject nob = _networkManager.GetPooledInstantiated(selectedPrefab, position, rotation, true);
            _networkManager.ServerManager.Spawn(nob, conn);

            // Если не используются глобальные сцены, добавляем в дефолтную
            if (_addToDefaultScene)
                _networkManager.SceneManager.AddOwnerToDefaultScene(nob);

            OnSpawned?.Invoke(nob);
        }



        /// <summary>
        /// Sets a spawn position and rotation.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        private void SetSpawn(Transform prefab, out Vector3 pos, out Quaternion rot)
        {
            //No spawns specified.
            if (Spawns.Length == 0)
            {
                SetSpawnUsingPrefab(prefab, out pos, out rot);
                return;
            }

            Transform result = Spawns[_nextSpawn];
            if (result == null)
            {
                SetSpawnUsingPrefab(prefab, out pos, out rot);
            }
            else
            {
                pos = result.position;
                rot = result.rotation;
            }

            //Increase next spawn and reset if needed.
            _nextSpawn++;
            if (_nextSpawn >= Spawns.Length)
                _nextSpawn = 0;
        }

        /// <summary>
        /// Sets spawn using values from prefab.
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        private void SetSpawnUsingPrefab(Transform prefab, out Vector3 pos, out Quaternion rot)
        {
            pos = prefab.position;
            rot = prefab.rotation;
        }

    }


}