using ServerCore;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DummyClient
{
    

    class Program
    {
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

            Connector connector = new Connector();

            // 클라이언트를 여러개 생성
            connector.Connect(endPoint, 
                () => { return SessionManager.Instance.Generate(); },
                10);

            while (true)
            {
                try
                {
                    SessionManager.Instance.SendForEach();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

                Thread.Sleep(250);
            }
        }
    }
}