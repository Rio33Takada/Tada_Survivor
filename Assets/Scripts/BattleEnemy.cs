namespace takada
{
    public class BattleEnemy
    {

        public int hp { get; private set; }

        public bool isAlive => hp > 0;
        public void TakeDamage(int amount)
        {
            hp -= amount;
            if (!isAlive)
            {
                Death();
            }
        }

        public void Death()
        {

        }
    }
}