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
    class Program
	{
		static Listener _listener = new Listener();
		static List<System.Timers.Timer> _timers = new List<System.Timers.Timer>();

		static void TickRoom(GameRoom room, int tick = 100)
		{
			var timer = new System.Timers.Timer();
			timer.Interval = tick;

			// Interval이 끝난 뒤 실행할 함수
			timer.Elapsed += ((s, e) => { room.Update(); });
			timer.AutoReset = true;
			timer.Enabled = true;

			_timers.Add(timer);
		}

		static void Main(string[] args)
		{
			// 클라이언트 내 원본 데이터를 로드합니다.
			ConfigManager.LoadConfig();
			DataManager.LoadData();

			// Test Code
			using (AppDbContext db = new AppDbContext())
			{
                // FirstOrDefault : 없으면 null을 반환
                PlayerDb player = db.Players.FirstOrDefault();
				if (player != null)
				{
					db.Items.Add(new ItemDb()
					{
						TemplateId = 1,
						Count = 1,
						Slot = 0,
						Owner = player
					});

                    db.Items.Add(new ItemDb()
                    {
                        TemplateId = 100,
                        Count = 1,
                        Slot = 1,
                        Owner = player
                    });

                    db.Items.Add(new ItemDb()
                    {
                        TemplateId = 101,
                        Count = 1,
                        Slot = 2,
                        Owner = player
                    });

                    db.Items.Add(new ItemDb()
                    {
                        TemplateId = 200,
                        Count = 0,
                        Slot = 5,
                        Owner = player
                    });

					db.SaveChangesEx();
                }
			}
			
			GameRoom room = RoomManager.Instance.Add(1);
			TickRoom(room, 50);

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
			// JobTimer.Instance.Push(FlushRoom);

			while (true)
			{
				DbTransaction.Instance.Flush();
			}
		}
	}
}