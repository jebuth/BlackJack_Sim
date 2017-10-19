using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
    class Deck
    {
        //private int Deck_Size = 52;
        private int Number_Of_Decks = 1;
        private List<Card> Remaining_Shoe = null;
        private List<Card> Discard_Pile = null;
        private int Counter = 0;

        public Deck()
        {
            Remaining_Shoe = new List<Card>();
            Discard_Pile = new List<Card>();
            Initialize_Shoe();
        }

        public void Initialize_Shoe()
        {
            for (int k = 0; k < 6; k++) {
                // Iterate 4 for quits
                for (int j = 1; j < 5; j++)
                {
                    // Iterate 13 for values
                    for (int i = 1; i < 14; i++)
                    {
                        // Necesarry!!!
                        Remaining_Shoe.Add(new Card(i, j));

                        // Add bunch of aces for test
                        //Remaining_Shoe.Add(new Card(1, j));
                        Counter++;
                    }
                }
            }

  


            // Custom deck for test
            /* Remaining_Shoe.Add(new Card(10, 1)); // 6
             Remaining_Shoe.Add(new Card(6, 1)); // 4 ////// was 1
             Remaining_Shoe.Add(new Card(1, 1)); // 5
             Remaining_Shoe.Add(new Card(7, 1)); // 6

             Remaining_Shoe.Add(new Card(7, 1)); // 5
             Remaining_Shoe.Add(new Card(7, 1)); // 5
             Remaining_Shoe.Add(new Card(8, 1)); // 6
             Remaining_Shoe.Add(new Card(2, 1)); // 4 ////// was 1
             Remaining_Shoe.Add(new Card(7, 1)); // 5
             Remaining_Shoe.Add(new Card(8, 1)); // 6
             Remaining_Shoe.Add(new Card(8, 1)); // 6
             Remaining_Shoe.Add(new Card(7, 3)); // ///////// 1 not ok. dont change any other values
             Remaining_Shoe.Add(new Card(1, 1)); // 5
             Remaining_Shoe.Add(new Card(8, 1)); // 6
             Remaining_Shoe.Add(new Card(2, 1)); // 4 ////// was 1
             Remaining_Shoe.Add(new Card(7, 1)); // 5
             Remaining_Shoe.Add(new Card(8, 1)); // 6
             Remaining_Shoe.Add(new Card(8, 1)); // 6
             Remaining_Shoe.Add(new Card(2, 1)); // 4 ////// was 1
             Remaining_Shoe.Add(new Card(7, 1)); // 5
             Remaining_Shoe.Add(new Card(8, 1)); // 6
             Remaining_Shoe.Add(new Card(1, 1)); // 5
             Remaining_Shoe.Add(new Card(1, 1)); // 5
             Remaining_Shoe.Add(new Card(8, 1)); // 6
             Remaining_Shoe.Add(new Card(2, 1)); // 4 ////// was 1
             Remaining_Shoe.Add(new Card(7, 1)); // 5
             Remaining_Shoe.Add(new Card(8, 1)); // 6
             Remaining_Shoe.Add(new Card(8, 1)); // 6

             //Counter = 28;
             */

            Console.WriteLine("SHOE COUNTER: " + Counter);
            // Shuffle deck
            //Remaining_Shoe = Shuffle_List<Card>(Remaining_Shoe);
            

        }


        public int Get_Counter()
        {
            return this.Counter;
        }

        public void Display_Deck()
        {
            foreach(Card c in Remaining_Shoe)
            {
                c.Display_Card();
            }
        }

        public void Display_Discard_Pile()
        {
            foreach (Card c in Discard_Pile)
            {
                c.Display_Card();
            }
        }

        public string Display_Discard_Pile_ToString()
        {
            string str = "";

            foreach (Card c in Discard_Pile)
            {
                str += c.Display_Card_ToString() + "\n";
            }
            return str;
        }


        public string Display_Deck_ToString()
        {
            string str = "";

            foreach (Card c in Remaining_Shoe)
            {
                str += c.Display_Card_ToString() + "\n";
            }
            return str;
        }

        public int Get_Remaining_Cards()
        {
            return this.Remaining_Shoe.Count;
        }

        public Card Draw()
        {
            if (!Is_Empty())
            {
                Card victim = Remaining_Shoe[0];
                // Add card to discard pile
                Discard_Pile.Add(victim);
                // Remove card from shoe
                Remaining_Shoe.RemoveAt(0);
                Counter--;
                return victim;
            }
            else
            {
                MessageBox.Show("Empty shoe. Exiting simulator.");
                Application.Exit();
                return null;
            }
            

        }
        

        public bool Is_Empty()
        {
            return (Counter <= 0) ? true : false;
        }

        private List<Card> Shuffle_List<E>(List<Card> inputList)
        {
            List<Card> randomList = new List<Card>();

            Random r = new Random();
            int randomIndex = 0;
            while (inputList.Count > 0)
            {
                randomIndex = r.Next(0, inputList.Count); //Choose a random object in the list
                randomList.Add(inputList[randomIndex]); //add it to the new, random list
                inputList.RemoveAt(randomIndex); //remove to avoid duplicates
            }

            return randomList; //return the new random list
        }


    }
}
