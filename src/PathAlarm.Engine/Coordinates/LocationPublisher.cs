using NetMQ;
using NetMQ.Sockets;
using PathAlarm.Models.Communication;

namespace PathAlarm.Engine.Coordinates;

public class LocationPublisher : IDisposable
{
    private PublisherSocket? publisherSocket;

    public void Start()
    {
        publisherSocket = new PublisherSocket();
        publisherSocket.Options.SendHighWatermark = 1000;
        publisherSocket.Bind($"tcp://*:{CommunicationConstants.LocationPort}");
    }

    public void Send(string text)
    {
        publisherSocket?.SendMoreFrame(CommunicationConstants.LocationTopic).SendFrame(text);
    }
    public void Dispose()
    {
        publisherSocket?.Dispose();
    }
}