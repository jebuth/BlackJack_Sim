using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;


namespace WindowsFormsApplication1
{
    class Card
    {
        private int Integer_Value;
        private string Suit = null;
        private Image Card_Image = null;
        private Image Face_Down_Image = null;

        // Suits
        private const int DIAMOND = 0;
        private const int HEART = 1;
        private const int SPADE = 2;
        private const int CLUB = 3;

        // Ranks
        private int ACE;
        private const int DEUCE = 2;
        private const int THREE = 3;
        private const int FOUR = 4;
        private const int FICE = 5;
        private const int SIX = 6;
        private const int SEVEN = 7;
        private const int EIGHT = 8;
        private const int NINE = 9;
        private const int TEN = 10;
        private const int JACK = 10;
        private const int QUEEN = 10;
        private const int KING = 10;

        public Card(int Value, int Suit)
        {
            // keep real value for J, Q, K
            string Real_Value = null;

            // Assign a suit
            switch (Suit)
            {
                case 1: this.Suit = "Spades";
                    break;
                case 2: this.Suit = "Clubs";
                    break;
                case 3: this.Suit = "Hearts";
                    break;
                case 4: this.Suit = "Diamonds";
                    break;
                default: this.Suit = null;
                    break;
            }

            // Assign value
            switch (Value)
            {   
                // ACE
                case 1: this.Integer_Value = 1;
                    break;
                case 2: this.Integer_Value = 2;
                    break;
                case 3: this.Integer_Value = 3;
                    break;
                case 4:
                    this.Integer_Value = 4;
                    break;
                case 5:
                    this.Integer_Value = 5;
                    break;
                case 6:
                    this.Integer_Value = 6;
                    break;
                case 7:
                    this.Integer_Value = 7;
                    break;
                case 8:
                    this.Integer_Value = 8;
                    break;
                case 9:
                    this.Integer_Value = 9;
                    break;
                case 10:
                    this.Integer_Value = 10;
                    break;
                case 11:
                    Real_Value = "jack";
                    this.Integer_Value = 10;
                    break;
                case 12:
                    Real_Value = "queen";
                    this.Integer_Value = 10;
                    break;
                case 13:
                    Real_Value = "king";
                    this.Integer_Value = 10;
                    break;
                default:
                    this.Integer_Value = 0;
                    break;
            }

            Set_Image(this.Suit, this.Integer_Value, Real_Value);
            Set_FaceDown_Image();
        }

        private void Set_FaceDown_Image()
        {
            this.Face_Down_Image = Properties.Resources.ResourceManager.GetObject("rear") as Bitmap;
        }

        private void Set_Image(string Suit, int Value, string Real_Value)
        {
            string LowerCase_Suit = Suit.ToLower();
            // string Path = null;
            string Path = null;

            if (Value == 1)
                Path = "ace_of_" + LowerCase_Suit;
            else if (Real_Value == "jack")
                Path = "jack_of_" + LowerCase_Suit;
            else if (Real_Value == "queen")
                Path = "queen_of_" + LowerCase_Suit;
            else if (Real_Value == "king")
                Path = "king_of_" + LowerCase_Suit;
            else
                Path = "_" + Value.ToString() + "_of_" + LowerCase_Suit;

            this.Card_Image = Properties.Resources.ResourceManager.GetObject(Path) as Bitmap;



        }

        public Image Get_Image()
        {
           
            return this.Card_Image;
        }

        public Image Get_Image_FaceDown()
        {

            return this.Face_Down_Image;
        }

        public Card Draw()
        {
            return this;
        }

        public int Get_Integer_Value()
        {
            return this.Integer_Value;
        }

        public void Display_Card()
        {
            Console.WriteLine(this.Integer_Value + " " + this.Suit);
        }

        public string Display_Card_ToString()
        {
            string str = "";
            str += this.Integer_Value + " " + this.Suit + "\r\n";
            return str;
        }

    }
}
