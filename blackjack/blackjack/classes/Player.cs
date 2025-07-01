using System.Collections.Generic;

namespace BlackjackApp
{
    public class Player
    {
        public string Name { get; }
        public List<Hand> Hands { get; set; }
        public int ActiveHandIndex { get; set; }

        public Hand Hand => Hands[ActiveHandIndex];

        public Player(string name)
        {
            Name = name;
            Hands = new List<Hand> { new Hand() };
            ActiveHandIndex = 0;
        }
    }
}
