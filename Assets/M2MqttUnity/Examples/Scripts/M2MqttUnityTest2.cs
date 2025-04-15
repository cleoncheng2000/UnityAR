/*
The MIT License (MIT)

Copyright (c) 2018 Giovanni Paolo Vigano'

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using M2MqttUnity;
using Newtonsoft.Json;
using System.ComponentModel.Composition;

/// <summary>
/// Examples for the M2MQTT library (https://github.com/eclipse/paho.mqtt.m2mqtt),
/// </summary>
namespace M2MqttUnity.Examples
{
    /// <summary>
    /// Script for testing M2MQTT with a Unity UI
    /// </summary>
    /// 

    public class M2MqttUnityCapstone : M2MqttUnityClient
    {
        [Tooltip("Set this to true to perform a testing cycle automatically on startup")]
        public bool autoTest = false;
        [Header("User Interface")]
        public InputField consoleInputField;
        public Toggle encryptedToggle;
        public InputField addressInputField;
        public InputField portInputField;
        public Button connectButton;
        public Button disconnectButton;
        public Button testPublishButton;
        public Button clearButton;
        public ProjectileEventManager projectileEventManager;
        public HUDManager HUDManager;
        public PlayerManager playerManager;
        public Image connectionStatusImage;
        public AudioManagerVuforia audioManagerVuforia;
        private List<string> eventMessages = new List<string>();
        private bool updateUI = false;
        public Button sendButton;

        public void TestPublish()
        {
            client.Publish("group21/response", System.Text.Encoding.UTF8.GetBytes("Test message"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
            Debug.Log("Test message published");
            AddUiMessage("Test message published.");
        }

        public void SetBrokerAddress(string brokerAddress)
        {
            if (addressInputField && !updateUI)
            {
                this.brokerAddress = brokerAddress;
            }
        }

        public void SetBrokerPort(string brokerPort)
        {
            if (portInputField && !updateUI)
            {
                int.TryParse(brokerPort, out this.brokerPort);
            }
        }

        public void SetEncrypted(bool isEncrypted)
        {
            this.isEncrypted = isEncrypted;
        }


        public void SetUiMessage(string msg)
        {
            if (consoleInputField != null)
            {
                consoleInputField.text = msg;
                updateUI = true;
            }
        }

        public void AddUiMessage(string msg)
        {
            if (consoleInputField != null)
            {
                consoleInputField.text += msg + "\n";
                updateUI = true;
            }
        }

        protected override void OnConnecting()
        {
            base.OnConnecting();
            SetUiMessage("Connecting to broker on " + brokerAddress + ":" + brokerPort.ToString() + "...\n");
            updateConectionStatus("connecting");
        }

        protected override void OnConnected()
        {
            base.OnConnected();
            SetUiMessage("Connected to broker on " + brokerAddress + "\n");
            updateConectionStatus("connected");
            if (autoTest)
            {
                TestPublish();
            }
        }

        protected override void SubscribeTopics()
        {
            client.Subscribe(new string[] { "group21/game_state" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            client.Subscribe(new string[] { "group21/query" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
        }

        protected override void UnsubscribeTopics()
        {
            client.Unsubscribe(new string[] { "group21/game_state" });
            client.Unsubscribe(new string[] { "group21/query" });
        }

        protected override void OnConnectionFailed(string errorMessage)
        {
            AddUiMessage("CONNECTION FAILED! " + errorMessage);
        }

        protected override void OnDisconnected()
        {
            AddUiMessage("Disconnected.");
            updateConectionStatus("disconnected");
        }

        protected override void OnConnectionLost()
        {
            AddUiMessage("CONNECTION LOST!");
            updateConectionStatus("disconnected");
        }

        private void UpdateUI()
        {
            if (client == null)
            {
                if (connectButton != null)
                {
                    connectButton.interactable = true;
                    disconnectButton.interactable = false;
                    testPublishButton.interactable = false;
                }
            }
            else
            {
                if (testPublishButton != null)
                {
                    testPublishButton.interactable = client.IsConnected;
                }
                if (disconnectButton != null)
                {
                    disconnectButton.interactable = client.IsConnected;
                }
                if (connectButton != null)
                {
                    connectButton.interactable = !client.IsConnected;
                }
            }
            if (addressInputField != null && connectButton != null)
            {
                addressInputField.interactable = connectButton.interactable;
                addressInputField.text = brokerAddress;
            }
            if (portInputField != null && connectButton != null)
            {
                portInputField.interactable = connectButton.interactable;
                portInputField.text = brokerPort.ToString();
            }
            if (encryptedToggle != null && connectButton != null)
            {
                encryptedToggle.interactable = connectButton.interactable;
                encryptedToggle.isOn = isEncrypted;
            }
            if (clearButton != null && connectButton != null)
            {
                clearButton.interactable = connectButton.interactable;
            }
            updateUI = false;
        }

        protected override void Start()
        {
            SetUiMessage("Ready.");
            updateUI = true;
            base.Start();
            //sendButton.onClick.AddListener(SendJson);
        }

        public void SendJson()
        {
            string response = "{\"type\": \"visibility_and_snowbomb_update\"" +
             ", \"players_visibility\": " + "true" +
             ", \"snow_bombs_hit_p1\": " + 0 +
             ", \"snow_bombs_hit_p2\": " + 0 + "}";
            Debug.Log("Response: " + response);
            client.Publish("group21/response", System.Text.Encoding.UTF8.GetBytes(response), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
        }

        protected override void DecodeMessage(string topic, byte[] message)
        {
            string msg = System.Text.Encoding.UTF8.GetString(message);
            Debug.Log("Received: " + msg);
            StoreMessage(msg);

            if (topic == "group21/query")
            {
                bool isVisible = true;
                if (projectileEventManager.trackedTarget != null)
                {
                    isVisible = projectileEventManager.IsTargetInView(projectileEventManager.trackedTarget.transform);
                }
                else
                {
                    Debug.LogWarning("Tracked target is null. Setting isVisible to false.");
                    isVisible = false;
                }

                int snowBombs = projectileEventManager.GetSnowParticleCount();
                Player player = playerManager.GetCurrentPlayer();

                // Create a JSON object
                var responseObject = new
                {
                    type = "visibility_and_snowbomb_update",
                    player_id = (player == playerManager.p1) ? 1 : 2,
                    opponent_visibility = isVisible,
                    snow_bombs_hit = snowBombs,
                };

                // Serialize the object to a JSON string
                string response = JsonConvert.SerializeObject(responseObject);

                Debug.Log("Response: " + response);

                // Publish the JSON response
                client.Publish("group21/response", System.Text.Encoding.UTF8.GetBytes(response), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
                Debug.Log("Published response: " + response);
                AddUiMessage("Published response: " + response);
            }
            if (topic == "group21/game_state")
            { //parse game state stuff 
                try
                {
                    // Parse game state stuff
                    //msg = JsonConvert.DeserializeObject<string>(msg);
                    GameData gameData = JsonConvert.DeserializeObject<GameData>(msg);

                    // Access P1's attributes
                    Player p1State = gameData.game_state.p1;
                    Player p2State = gameData.game_state.p2;

                    Player p1 = playerManager.p1;
                    Player p2 = playerManager.p2;

                    // Update HUD with correct values
                    HUDManager.UpdatePlayer(p1, p1State.hp, p1State.shields, p1State.shield_hp, p1State.bullets, p1State.bombs, p1State.deaths, p2State.deaths);
                    HUDManager.UpdatePlayer(p2, p2State.hp, p2State.shields, p2State.shield_hp, p2State.bullets, p2State.bombs, p1State.deaths, p2State.deaths);

                    //Shows projectile for current player
                    if (playerManager.GetCurrentPlayer() == p1)
                    {
                        projectileEventManager.UpdateAction(gameData.p1_action, p2State.shield_hp);
                        if (gameData.p1_action == "shield") {
                            audioManagerVuforia.PlayShieldSound();
                        }
                    }
                    else
                    {
                        projectileEventManager.UpdateAction(gameData.p2_action, p1State.shield_hp);
                        if (gameData.p2_action == "shield") {
                            audioManagerVuforia.PlayShieldSound();
                        }
                    }

                }
                catch (ArgumentException ex)
                {
                    Debug.LogError("Error converting JSON to GameData: " + ex.Message);
                }
                catch (JsonSerializationException ex)
                {
                    Debug.LogError("Error deserializing GameData: " + ex.Message);
                }
                catch (Exception ex)
                {
                    Debug.LogError("Unexpected error: " + ex.Message);
                }

            }
            if (topic == "M2MQTT_Unity/test")
            {
                if (autoTest)
                {
                    autoTest = false;
                    Disconnect();
                }
            }
        }

        private void StoreMessage(string eventMsg)
        {
            eventMessages.Add(eventMsg);
        }

        private void ProcessMessage(string msg)
        {
            AddUiMessage("Received: " + msg);
        }

        protected override void Update()
        {
            base.Update(); // call ProcessMqttEvents()

            if (eventMessages.Count > 0)
            {
                foreach (string msg in eventMessages)
                {
                    ProcessMessage(msg);
                }
                eventMessages.Clear();
            }
            if (updateUI)
            {
                UpdateUI();
            }
        }

        private void OnDestroy()
        {
            Disconnect();
        }

        private void OnValidate()
        {
            if (autoTest)
            {
                autoConnect = true;
            }
        }

        public void updateConectionStatus(string status)
        {
            switch (status)
            {
                case "connected":
                    connectionStatusImage.color = Color.green;
                    break;
                case "disconnected":
                    connectionStatusImage.color = Color.red;
                    break;
                case "connecting":
                    connectionStatusImage.color = Color.yellow;
                    break;
                default:
                    connectionStatusImage.color = Color.red;
                    break;
            }
        }
    }
}