using Server.Data;
using Server.DB;
using Server.Game;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
	// 1. GameRoom 방식의 간단한 동기화 <- OK
	// 2. 더 넓은 영역 관리
	// 3. 심리스 MMO

	// 스레드 배치 현황
	// 1. Recv (N개)			서빙
	// 2. GameLogic (1개)	요리사
	// 3. Send (1개)			서빙
	// 4. DB (1개)			결제/장부

    class Program
	{
		static Listener _listener = new Listener();

		static void GameLogicTask()
		{
            while (true)
            {
                GameLogic.Instance.Update();
                Thread.Sleep(0);
            }
        }

		static void DbTask()
		{
            while (true)
            {
                DbTransaction.Instance.Flush();
				Thread.Sleep(0);
            }
        }

		static void NetworkTask()
		{
			while (true)
			{
				List<ClientSession> sessions = SessionManager.Instance.GetSessions();
				foreach (ClientSession session in sessions)
				{
					session.FlushSend();
				}
				Thread.Sleep(0);
			}
		}

		static void Main(string[] args)
		{
			// 클라이언트 내 원본 데이터를 로드합니다.
			ConfigManager.LoadConfig();
			DataManager.LoadData();

            GameLogic.Instance.Push(() => {
                GameLogic.Instance.Add(1);
            });

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

            // DbTask
            {
                Thread t = new Thread(DbTask);
				t.Name = "Db";
				t.Start();
			}

            // NetworkTask
            {
				// 태스크를 하나 스레드를 만드나 별 차이가 없다.
                Thread t = new Thread(NetworkTask);
                t.Name = "Network Send";
                t.Start();

                // 스레드를 하나 더 만든다.(pooling이 아니다.)
                //Task networkTask = new Task(NetworkTask, TaskCreationOptions.LongRunning);
                //networkTask.Start();
            }

			// GameLogicTask
			Thread.CurrentThread.Name = "GameLogic";
			GameLogicTask();
        }
	}
}