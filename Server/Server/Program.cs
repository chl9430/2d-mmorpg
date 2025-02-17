using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ServerCore;

namespace Server
{
    class Program
    {
        static Listener _listener = new Listener();
        public static GameRoom Room = new GameRoom();

        static void FlushRoom()
        {
            Room.Push(() => Room.Flush());
            JobTimer.Instance.Push(FlushRoom, 250);
        }

        static void Main(string[] args)
        {
            // dns(domain name service)
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            // 구글 같이 트래픽이 많은 사이트는 부화 분산을 위해 하나의 도메인당 아이피가 여러개일 수 있다.
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);
            // ipAddr : 식당주소, 7777: 정문인지 후문인지?
            // www.rookiss.com => ip가 변경되어도 도메인으로 관리가능

            _listener.Init(endPoint, () => { return SessionManager.Instance.Generate(); });
            Console.WriteLine("Listening...");

            // FlushRoom();
            JobTimer.Instance.Push(FlushRoom);

            while (true)
            {
                JobTimer.Instance.Flush();
            }
        }
    }
}