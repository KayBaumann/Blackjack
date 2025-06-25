namespace BlackjackApp
{
    public class Player
    {
        public string Name { get; }
        public Hand Hand { get; }

        public Player(string name)
        {
            Name = name;
            Hand = new Hand();
        }
    }
}
