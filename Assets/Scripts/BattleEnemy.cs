using UnityEngine;

public enum EnemyType
{
    knight,
    archer,
    bomber,
}

namespace takada
{
    public abstract class BattleEnemy
    {
        public virtual int MaxHp { get; }
        public int Hp { get; private set; }

        public Vector2Int gridPosition;

        public bool IsAlive => Hp > 0;

        public BattleEnemy()
        {
            Hp = MaxHp;
        }

        public void TakeDamage(int amount)
        {
            Hp -= amount;
            if (!IsAlive)
            {
                Death();
            }
        }

        public void Death()
        {

        }

        public virtual void Move(Vector2Int playerPos)
        {

        }

        public virtual void Attack()
        {

        }

        public virtual bool SearchPlayer()
        {
            return false;
        }
    }
}