namespace takada
{
    public class BattleEnemy
    {

        public int hp { get; private set; }

        public bool IsAlive => hp > 0;
        public void TakeDamage(int amount)
        {
            hp -= amount;
            if (!IsAlive)
            {
                Death();
            }
        }

        public void Death()
        {

        }
    }
}