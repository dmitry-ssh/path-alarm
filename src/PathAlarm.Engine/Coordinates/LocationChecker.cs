using NetMQ;
using NetMQ.Sockets;
using PathAlarm.Models.Communication;

namespace PathAlarm.Engine.Coordinates;

public class LocationChecker
{
    private SubscriberSocket? socket;

    public void Start()
    {
        socket = new SubscriberSocket();
        socket.Options.ReceiveHighWatermark = 1000;
        socket.Connect($"tcp://localhost:{CommunicationConstants.LocationPort}");
        socket.Subscribe(CommunicationConstants.LocationTopic);
    }
    public string GetMessage()
    {
        if (socket == null)
        {
            return String.Empty;
        }
        socket.ReceiveFrameString();
        string messageReceived = socket.ReceiveFrameString();
        return messageReceived;
    }
}