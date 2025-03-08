using System;
using System.Text;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

public class MqttTest : MonoBehaviour
{
    private MqttClient client;
    private string brokerAddress = "0cc00bbd72c94b7589723d139d527c9c.s1.eu.hivemq.cloud"; // HiveMQ broker
    private int brokerPort = 8883; // MQTT over SSL/TLS port
    private string username = "hivemq.webclient.1741332185289";
    private string password = "3#.2!*01IJQZbafAcdgC"; // Your actual password
    private string topic = "group21/game_state";

    void Start()
    {
        ConnectToBroker();
    }

    void ConnectToBroker()
    {
        try
        {
            // Initialize MQTT client
            client = new MqttClient(brokerAddress, brokerPort, true, null, null, MqttSslProtocols.TLSv1_2); // Enable TLS

            // Set credentials
            client.Connect(Guid.NewGuid().ToString(), username, password);

            if (client.IsConnected)
            {
                Debug.Log("Connected to HiveMQ broker!");
                
                // Subscribe to topic
                client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
                client.MqttMsgPublishReceived += OnMessageReceived;

                // Publish a test message
                PublishMessage("test!");
            }
            else
            {
                Debug.LogError("Failed to connect to MQTT broker.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("MQTT Connection Error: " + ex.Message);
        }
    }

    void PublishMessage(string message)
    {
        if (client.IsConnected)
        {
            client.Publish(topic, Encoding.UTF8.GetBytes(message), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
            Debug.Log("Message Published: " + message);
        }
        else
        {
            Debug.LogError("Not connected to MQTT broker.");
        }
    }

    void OnMessageReceived(object sender, MqttMsgPublishEventArgs e)
    {
        string receivedMessage = Encoding.UTF8.GetString(e.Message);
        Debug.Log($"Received Message on {e.Topic}: {receivedMessage}");
    }

    void OnDestroy()
    {
        if (client != null && client.IsConnected)
        {
            client.Disconnect();
            Debug.Log("Disconnected from MQTT broker.");
        }
    }
}