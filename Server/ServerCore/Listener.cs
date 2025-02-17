using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class Listener
    {
        Socket _listenSocket;
        // 매개변수는 없고 Session을 리턴해주는 함수형식을 받는다
        Func<Session> _sessionFactory;

        public void Init(IPEndPoint endPoint, Func<Session> sessionFactory, int register = 10, int backlog = 100)
        {
            _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _sessionFactory += sessionFactory;

            // 문지기 교육
            _listenSocket.Bind(endPoint);

            // 영업 시작
            // backlog : 최대 대기수
            _listenSocket.Listen(backlog);

            for (int i = 0; i < register; i++)
            {
                // 비동기 함수 등록
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
                RegisterAccept(args);
            }
        }

        void RegisterAccept(SocketAsyncEventArgs args)
        {
            // 재사용 될때를 대비해 초기값으로 돌려놔준다.
            args.AcceptSocket = null;

            bool pending = _listenSocket.AcceptAsync(args);

            if (pending == false) // 바로 클라에게서 접속이 들어온다면
                OnAcceptCompleted(null, args);
        }

        // 멀티 스레드 환경에서 돌아가기 때문에 주의해야하는 함수다.
        void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success)
            {
                Session session = _sessionFactory.Invoke();
                session.Start(args.AcceptSocket);
                session.OnConnected(args.AcceptSocket.RemoteEndPoint);
            }
            else
                Console.WriteLine(args.SocketError.ToString());

            // 다음 접속을 위해 다시한번 호출
            RegisterAccept(args);
        }
    }
}
