using Google.Protobuf.Protocol;
using Server.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public class Player : GameObject
    {
        public int PlayerDbId { get; set; }
        public ClientSession Session { get; set; }
        public Inventory Inven { get; private set; } = new Inventory();

        public Player()
        {
            ObjectType = GameObjectType.Player;
        }

        public override void OnDamaged(GameObject attacker, int damage)
        {
            base.OnDamaged(attacker, damage);
        }

        public override void OnDead(GameObject attacker)
        {
            base.OnDead(attacker);
        }

        public void OnLeaveGame()
        {
            // DB에 접근은 부하가 많이 걸린다.
            // 1. 서버가 다운되면 아직 저장되지 않은 정보가 날아갈 수 있다.
            // 2. 코드 흐름을 다 막아버린다!!!
            // 이 함수는 jobserializer를 상속받는 게임 룸에서 호출되기 때문에 db접근은 하나의 스레드가 여기에 너무 오래 머무르게 될 수 있다.
            // 해결법?
            // - 비동기 방법 사용
            // - 다른 스레드로 db일감을 던져버리면? -> 결과를 받아 이어서 처리를 해야하는 경우에 또 문제가생김(예 : 아이템 생성)
            DbTransaction.SavePlayerStatus_Step1(this, Room);
        }
    }
}
