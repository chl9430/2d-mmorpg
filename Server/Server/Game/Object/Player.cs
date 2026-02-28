using Google.Protobuf.Protocol;
using Server.DB;
using Server.Game.Room;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public class Player : GameObject
    {
        public int PlayerDbId { get; set; }
        public ClientSession Session { get; set; }
        public VisionCube Vision { get; private set; }

        public Inventory Inven { get; private set; } = new Inventory();

        public int WeaponDamage { get; private set; }
        public int ArmorDefense { get; private set; }

        public override int TotalAttack { get { return Stat.Attack + WeaponDamage; } }
        public override int TotalDefense { get { return ArmorDefense; } }

        public Player()
        {
            ObjectType = GameObjectType.Player;
            Vision = new VisionCube(this);
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

        public void HandleEquipItem(C_EquipItem equipPacket)
        {
            Item item = Inven.Get(equipPacket.ItemDbId);
            if (item == null)
                return;

            if (item.ItemType == ItemType.Consumable)
                return;

            // 착용 요청 시, 겹치는 부위는 해제
            if (equipPacket.Equipped)
            {
                Item unequipItem = null;

                if (item.ItemType == ItemType.Weapon)
                {
                    unequipItem = Inven.Find(
                        i => i.Equipped && i.ItemType == ItemType.Weapon);
                }
                else if (item.ItemType == ItemType.Armor)
                {
                    ArmorType armorType = ((Armor)item).ArmorType;
                    unequipItem = Inven.Find(
                        i => i.Equipped && i.ItemType == ItemType.Armor
                        && ((Armor)i).ArmorType == armorType
                        );
                }

                if (unequipItem != null)
                {
                    // 메모리 선적용
                    unequipItem.Equipped = false;

                    // DB Noti
                    DbTransaction.EquipItemNoti(this, unequipItem);

                    // 클라에 통보
                    S_EquipItem equipOkItem = new S_EquipItem();
                    equipOkItem.ItemDbId = unequipItem.ItemDbId;
                    equipOkItem.Equipped = unequipItem.Equipped;
                    Session.Send(equipOkItem);
                }
            }

            {
                // 메모리 선적용
                item.Equipped = equipPacket.Equipped;

                // DB Noti
                DbTransaction.EquipItemNoti(this, item);

                // 클라에 통보
                S_EquipItem equipOkItem = new S_EquipItem();
                equipOkItem.ItemDbId = equipPacket.ItemDbId;
                equipOkItem.Equipped = equipPacket.Equipped;
                Session.Send(equipOkItem);
            }

            RefeshAdditionalStat();
        }

        public void RefeshAdditionalStat()
        {
            WeaponDamage = 0;
            ArmorDefense = 0;

            foreach (Item item in Inven.Items.Values)
            {
                if (item.Equipped == false)
                    continue;
                
                switch (item.ItemType)
                {
                    case ItemType.Weapon:
                        {
                            WeaponDamage += ((Weapon)item).Damage;
                        }
                        break;
                    case ItemType.Armor:
                        {
                            ArmorDefense += ((Armor)item).Defense;
                        }
                        break;
                }
            }
        }
    }
}
