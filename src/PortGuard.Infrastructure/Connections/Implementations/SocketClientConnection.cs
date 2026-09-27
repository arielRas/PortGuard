using PortGuard.Application.Abstractions.Connections;
using System.Net;
using System.Net.Sockets;

namespace PortGuard.Infrastructure.Connections.Implementations
{
    public class SocketClientConnection : IClientConnection
    {
        internal Socket Socket { get; }
        public string Id { get; }
        public int IncomingPort { get; }
        public string RemoteIpAddress { get; }
        public int TargetPort { get; }
        public string TargetHost { get; }

        public SocketClientConnection(Socket socket, int incomingPort, string targetHost, int targetPort)
        {
            Id = Guid.CreateVersion7().ToString();
            Socket = socket;
            IncomingPort = incomingPort;
            TargetHost = targetHost;
            TargetPort = targetPort;
            RemoteIpAddress = Socket.RemoteEndPoint is IPEndPoint ip
                ? ip.Address.ToString()
                : "Unknown";
        }

        public async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken token = default)
        {
            return await Socket.ReceiveAsync(buffer, SocketFlags.None, token);
        }

        public async ValueTask DropConnectionAsync(string reason, CancellationToken token = default)
        {
            try
            {
                Socket.LingerState = new LingerOption(true, 0);
                Socket.Close();
            }
            catch (Exception)
            {
                //Ignore closed-client exception
            }
        }

        public Stream GetStream()
        {
            return new NetworkStream(Socket, ownsSocket: false);
        }

        public async ValueTask DisposeAsync()
        {
            try
            {
                if (Socket.Connected)
                {
                    Socket.Shutdown(SocketShutdown.Both);
                }
            }
            catch
            {
                // Ignorado intencionalmente al cerrar
            }
            finally
            {
                Socket.Dispose();
            }

            await ValueTask.CompletedTask;
        }
    }
}
