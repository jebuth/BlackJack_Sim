using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Windows.Forms.DataVisualization.Charting;


namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        DataTable Strategy_Card = new DataTable();
        Deck d = null;
        Card Dealer_Down_Card = null;
        int Click_Count = 0;
        bool Player_Turn = true;
        bool New_Hand = true;
        int Player_Hand_Count = 0;
        int Player_Current_Total = 0;
        bool Player_Ace = false;
        bool Dealer_Ace = false;
        int Dealer_Hand_Count = 0;
        int Dealer_Current_Total = 0;
        int First_Value_Ace = 1;
        int Sec_Value_Ace = 11;
        int Player_Win_Count = 0;
        int Player_Loss_Count = 0;
        int Dealer_Win_Count = 0;
        int Dealer_Loss_Count = 0;
        int Push_Count = 0;
        int Hand_Count = 0;
        bool Face_Down_Ten = false;
        double Starting_Bet = 25;
        double Current_Bet = 0;
        double Max_Bet = 500;
        double Starting_Bankroll = 1000;
        double Bank = 1000;
        int Player_First_Card;
        int Player_Second_Card;
        bool Doubled_Previous_Hand = false;
        bool Decision_Reached = false;
        int DELAY_TASK_TIME = 20;

        public Form1()
        {
            InitializeComponent();
            Init_Graph();
            // Disable all buttons at start
            Deal_Button.Enabled = false;
            Hit_Button.Enabled = false;
            Stand_Button.Enabled = false;
            Clear_Button.Enabled = false;
            Double_Button.Enabled = false;

            Starting_Bet = 25;
            Current_Bet = Starting_Bet;
            Bet_TextBox.Text = string.Format("{0:0.00}", Current_Bet);
            Bankroll_TextBox.Text = string.Format("{0:0.00}", Starting_Bankroll);
            Player_Result_TextBox.Text = "";

            

        }

        private void Create_Deck_Button_Click_1(object sender, EventArgs e)
        {
            d = new Deck();
            d.Display_Deck();

            textBox1.ScrollBars = ScrollBars.Vertical;
            textBox1.Text = d.Display_Deck_ToString();

            Deal_Button.Enabled = true;
            Create_Deck_Button.Enabled = false;
        }

        // Dealing of initial 4 cards. 
        async private void Deal_Button_Click_1(object sender, EventArgs e)
        {
            Status_Label.Text = "Status: Round in Progress.";
            Decision_Reached = false;
            int Deal_Value = 0;
            Console.WriteLine("New hand ================================");
            while (Deal_Value < 4)
            {
                if (!d.Is_Empty())
                {
                    Card c = d.Draw();



                    // Draw facedown on dealer's first turn
                    if ((Dealer_Hand_Count == 0) && (!Player_Turn))
                    {
                        Dealer_Down_Card = c;
                        // If the facedown card is a 10, flag it
                        if (Dealer_Down_Card.Get_Integer_Value() == 10)
                            Face_Down_Ten = true;
                        Dealer_Card_1.Image = c.Get_Image_FaceDown();
                        Dealer_Hand_Count++;
                        Player_Turn = true;
                    }

                    else if (Player_Turn)
                    {
                        if (Player_Hand_Count == 0)
                        {
                            Player_Card_1.Image = c.Get_Image();
                            Player_First_Card = c.Get_Integer_Value();
                        }
                        else if (Player_Hand_Count == 1)
                        {
                            Player_Card_2.Image = c.Get_Image();
                            Player_Second_Card = c.Get_Integer_Value();

                            // pause to display card
                            await Task.Delay(DELAY_TASK_TIME);
                        }

                        // Check for ACE
                        if (c.Get_Integer_Value() == 1)
                        {
                            //Console.WriteLine("WTF");
                            Player_Ace = true;
                            // If first card is Ace
                            if (Player_Hand_Count == 0)
                            {
                                Player_Current_Total += 1;
                                Player_Result_TextBox.Text = "1/11";
                            }
                            // Condition (A, A) - total will be 0
                            else if (Player_Current_Total == 0)
                            {
                                Player_Current_Total += 1;
                            }
                            // (x, A) condition
                            else
                            {
                                Player_Current_Total += 1;
                            }
                            // Create string ex. (5/15)
                            string Possible_Sums = "";
                            //Player_Current_Total += c.Get_Integer_Value();

                            Possible_Sums += Player_Current_Total.ToString() + "/" + (Player_Current_Total + 10).ToString();
                            Player_Result_TextBox.Text = Possible_Sums;

                            // if BJ with (x, A)
                            if ((Player_Current_Total + 10) == 21)
                            {
                                Decision(false, false, true, false);
                                return;
                            }
                            // If soft 18+ - Stand
                            else if (Player_Current_Total + 10 >= 18)
                            {
                                Console.WriteLine("236");
                                Player_Result_TextBox.Text = (Player_Current_Total + 10).ToString();
                                //Stand_Button.PerformClick(); premature stand: stand before dealer dealt his 2nd
                                // instead of standing, just end player's turn
                                Player_Turn = false;
                                //return;//
                            }


                        }
                        // (A, X) Condition
                        else
                        {

                            //Console.WriteLine("214");
                            if (Player_Result_TextBox.Text.Contains('/'))
                            {
                                string Possible_Sums = "";
                                Player_Current_Total += c.Get_Integer_Value();
                                // BJ (A, 10) condition
                                if ((Player_Current_Total + 10) == 21)
                                {
                                    Player_Result_TextBox.Text = Player_Current_Total.ToString();

                                    if (d.Is_Empty())
                                    {
                                        // draw dealer's second card before calling Decision() to end turn;
                                        Dealer_Card_2.Image = d.Draw().Get_Image();

                                        Decision(false, false, true, false);

                                        return;
                                    }
                                    // empty shoe
                                    else
                                    {
                                        Display_Empty_Message();
                                        return;
                                    }


                                    //Clear_Button.PerformClick();

                                }

                                Possible_Sums += Player_Current_Total.ToString() + "/" + (Player_Current_Total + 10).ToString();
                                Player_Result_TextBox.Text = Possible_Sums;
                            }
                            else
                            {
                                Player_Current_Total += c.Get_Integer_Value(); // (this value is calculated, this needs to be calculated)
                                Player_Result_TextBox.Text = Player_Current_Total.ToString();
                            }
                        }
                        Player_Hand_Count++;
                        Player_Turn = false;
                    }
                    // Dealer's turn
                    else
                    {
                        if (Dealer_Hand_Count == 1)
                            Dealer_Card_2.Image = c.Get_Image();

                        //Console.WriteLine("287: wtf is it doing");

                        // (-, A) Condition
                        if (c.Get_Integer_Value() == 1)
                        {
                            // check for BJ outright
                            //if (Dealer_Down_Card.Get_Integer_Value() + 11 == 21)
                            if (Face_Down_Ten)
                            {
                                Dealer_Card_1.Image = Dealer_Down_Card.Get_Image();
                                Decision(false, false, false, true);
                                return;
                            }

                            //string Possible_Values = "";
                            Dealer_Current_Total += 1; // 11
                            Dealer_Result_TextBox.Text = "1/11";// dealer still has a face down. 
                            Dealer_Hand_Count++;
                            Player_Turn = true;
                        }
                        else
                        {
                            Dealer_Current_Total += c.Get_Integer_Value();
                            Dealer_Result_TextBox.Text = Dealer_Current_Total.ToString();
                            Dealer_Hand_Count++;
                            Player_Turn = true;
                        }
                    }

                    //textBox2.Text = d.Display_Discard_Pile_ToString();
                    //textBox1.Text = d.Display_Deck_ToString();
                }
                else
                {
                    textBox2.Text = "Empty deck.";
                    return;
                }

                //Remaining_Cards_Count_TextBox.Text = d.Get_Remaining_Cards().ToString();
                Deal_Value++;
                //Console.WriteLine("Deal_Value: " + Deal_Value);
                // delay
                await Task.Delay(DELAY_TASK_TIME);
            }

            //Console.WriteLine("251: Deal_Value: " + Deal_Value);
            New_Hand = false;

            // Keep buttons disabled during initial dealing
            if (Dealer_Hand_Count >= 2)
            {
                Hit_Button.Enabled = true;
                Stand_Button.Enabled = true;
                Deal_Button.Enabled = false;
                Double_Button.Enabled = true;

                //
                if (Double_Down(Player_First_Card, Player_Second_Card))
                {
                    Player_First_Card = Player_Second_Card = 0;
                    Double_Button.PerformClick();
                    // Stand to end player turn after double

                    Stand_Button.PerformClick();
                    //return; // not tested
                    // reset bet from doubling
                    //Current_Bet /= 2;
                }
                Player_First_Card = Player_Second_Card = 0;

            }

            //Console.WriteLine("Decision Reached: " + Decision_Reached.ToString());

            // make decision if player_decision_Textbox is empty (no decision)
            // reasons that it would not be empty are in player blackjack or outright WIN/LOSS
            // changed from if to while to allow multiple hits by player
            //while (Player_Decision_TextBox.Text.Equals("") && Deal_Value > 3) // added second condition 11:23 am 10/17
            while((!Decision_Reached) && Player_Decision_TextBox.Text.Equals(""))
            {
                // why is playeR_current_Total = if Deal_Value is 4????
                //Console.WriteLine("INSIDE DEAL FUNCTION");

                // if no aces, pass int values
                if (!Player_Result_TextBox.Text.Contains('/') && (!Dealer_Result_TextBox.Text.Contains('/')))
                {
                    // if the decision is NOT empty, a winner has been decided.
                   
                    
                    //Console.WriteLine("This portion should only be executed at the end of a hand.");
                    //Console.WriteLine("Deal_Value: " + Deal_Value);
                    Console.WriteLine("314: Player_Result_TextBox: " + Player_Result_TextBox.Text + " Dealer_Result_TextBox: " + Dealer_Result_TextBox.Text);

                    Decide(Convert.ToInt32(Player_Result_TextBox.Text), Convert.ToInt32(Dealer_Result_TextBox.Text));
                    
                    
                }
                // if there are aces, 
                else
                {
                    Decide(Player_Result_TextBox.Text, Dealer_Result_TextBox.Text);
                }

                // update Player_Decision_TextBox value to exit loop
                // Maybe update player deciion textbox inside Decide();
               

            }
            //Decision_Reached = false;
            // tried to click clear here, doesn't work

            // delay before dealing next hand
            /*if (!d.Is_Empty())
            {

                await Task.Delay(DELAY_TASK_TIME);
                Deal_Button.PerformClick();
            }
            else
            {
                Display_Empty_Message();
            }*/

        }

        // decide based on basic strategy, (aces present)
        private void Decide(string Player_Value, string Dealer_Value)
        {
            //Console.WriteLine("Deciding with aces..");
            // DEALER ACE CASE and player no ace
            if (Dealer_Value.Contains('/') && !Player_Value.Contains('/'))
            {
                int Value = Convert.ToInt32(Player_Value);

                // stand on 17+ and dealer Ace
                if(Value > 17)
                {
                    Stand_Button.PerformClick();
                } 
                // if player has <17, hit
                else
                {
                    Hit_Button.PerformClick();
                }
            }
            // PLAYER ACE CASE and Dealer does not
            else if(!Dealer_Value.Contains('/') && Player_Value.Contains('/'))
            {
                int Value = Convert.ToInt32(Dealer_Value);

                // always stand on soft 19+
                if (Player_Value.Contains("9/19") || Player_Value.Contains("10/20"))
                {
                    Stand_Button.PerformClick();
                }
                // stand on soft 18 when dealer shoes 8-
                else if (Player_Value.Contains("8/18") && Value < 9)
                {
                    Stand_Button.PerformClick();
                }
                // hit on soft 18 when dealer shoes 9+
                else if (Player_Value.Contains("8/18") && Value > 9)
                {
                    Stand_Button.PerformClick();
                }
                // hit rest of soft hands (3/13 - 7/17) (double downs have been taken care of before this function call)
                else if (Player_Value.Contains("2/12") || (Player_Value.Contains("3/13")) || (Player_Value.Contains("4/14")) ||
                         (Player_Value.Contains("5/15")) || (Player_Value.Contains("6/16")) || (Player_Value.Contains("7/17")))
                {
                    Hit_Button.PerformClick();
                }
            }

            //Player_Decision_TextBox.Text = Player_Current_Total.ToString();
            
        }

        // Decide based on basic strategy
        private void Decide(int Player_Value, int Dealer_Value)
        {
            Console.WriteLine("Deciding..");
            // Stand on player value 17+
            if(Player_Value > 16 && Player_Value < 22)
            {
                Stand_Button.PerformClick();
            }
            // Stand on player 13-16 and dealer up card < 7
            else if ((Player_Value > 12 && Player_Value < 17) && (Dealer_Value < 7))
            {
                Stand_Button.PerformClick();
            }
            // Hit on player 13-16 when dealer shows 7+
            else if ((Player_Value > 12 && Player_Value < 17) && (Dealer_Value > 6))
            {
                Hit_Button.PerformClick();
            }
            // Hit on player 12 when dealer shows 2-3
            else if ((Player_Value == 12) && ((Dealer_Value == 2 && Dealer_Value == 3) || Dealer_Value > 6))
            {
                Hit_Button.PerformClick();
            }
            // stand on player 12 and dealer shows 4-6
            else if ((Player_Value == 12) && (Dealer_Value > 3 && Dealer_Value < 7))
            {
                Hit_Button.PerformClick();
            }
            // Hit on player 11 and dealer Ace
            else if ((Player_Value == 11) && (Dealer_Value == 1))
            {
                Hit_Button.PerformClick();
            }
            // Hit on player 10 and dealer 10 or Ace
            else if ((Player_Value == 10) && (Dealer_Value == 1 || Dealer_Value == 10))
            {
                Hit_Button.PerformClick();
            }

            // Hit on player 9 and dealer 2 or 7+
            else if ((Player_Value == 9) && (Dealer_Value == 2 || Dealer_Value == 1 || Dealer_Value > 6))
            {
                Hit_Button.PerformClick();
            }
            // Hit on player 5-8
            else if (Player_Value > 4 && Player_Value < 9)
            {
                Hit_Button.PerformClick();
            }

           // Player_Decision_TextBox.Text = Player_Current_Total.ToString();
        }

        private void Hit_Button_Click(object sender, EventArgs e)
        {
            Player_Turn = true;

            if (!d.Is_Empty())
            {
                Card c = d.Draw();

                //update textbox
                Remaining_Cards_Count_TextBox.Text = d.Get_Remaining_Cards().ToString();

                if (Player_Hand_Count == 2)
                {
                    Player_Card_3.Image = c.Get_Image();
                }
                else if (Player_Hand_Count == 3)
                {
                    Player_Card_4.Image = c.Get_Image();
                }
                else if (Player_Hand_Count == 4)
                {
                    Player_Card_5.Image = c.Get_Image();
                }
                else if (Player_Hand_Count == 5)
                {
                    Player_Card_6.Image = c.Get_Image();
                }
                else if (Player_Hand_Count == 6)
                {
                    Player_Card_7.Image = c.Get_Image();
                }


                // Check for ace
                if (Player_Result_TextBox.Text.Contains('/'))
                {
                    // check for 21
                    if (Player_Current_Total + 11 == 21)
                    {
                        Player_Current_Total = 21;
                        Player_Result_TextBox.Text = "21";
                        Stand_Button.PerformClick();
                    }
                    //no 21 - perform calculation with ace in hand
                    else
                    {
                        int Possible_Value_1 = Player_Current_Total + c.Get_Integer_Value();
                        int Possible_Value_2 = Possible_Value_1 + 10;
                        Player_Current_Total = Possible_Value_1;
                        Player_Result_TextBox.Text = Possible_Value_1.ToString() + "/" + Possible_Value_2.ToString();
                        Player_Hand_Count++;

                        // end hand if player busts and reveal dealer card
                        if (Player_Current_Total > 21)
                        {
                            Dealer_Card_1.Image = Dealer_Down_Card.Get_Image();
                            // Dealer results not calculated since player busted
                            Decision(true, false, false, false);
                            return;
                        }
                    }
                }
                else
                {
                    Player_Current_Total += c.Get_Integer_Value();
                    Player_Result_TextBox.Text = Player_Current_Total.ToString();
                    Player_Hand_Count++;

                    // end hand if player busts and reveal dealer card
                    if (Player_Current_Total > 21)
                    {
                        Dealer_Card_1.Image = Dealer_Down_Card.Get_Image();
                        // Dealer results not calculated since player busted
                        Decision(true, false, false, false);
                        return;
                    }
                }

            }
            // else deck is empty
            else
            {
                Display_Empty_Message();
                return;
            }
        }

        private void Stand_Button_Click(object sender, EventArgs e)
        {
            Player_Turn = false;
            // ======================
            int Possible_Value_1 = 0;  // Assigned when an ace is present
            int Possible_Value_2 = 0; //
            // ======================

            Dealer_Card_1.Image = Dealer_Down_Card.Get_Image();
            Dealer_Current_Total += Dealer_Down_Card.Get_Integer_Value();
            // check for face down ace
            if (Dealer_Down_Card.Get_Integer_Value() == 1)
            {
                Dealer_Ace = true;
                string Possible_Sums = "";
                Possible_Value_1 = Dealer_Current_Total;
                Possible_Value_2 = Dealer_Current_Total + 10;
                Dealer_Current_Total = Possible_Value_1;
                Possible_Sums += Possible_Value_1.ToString() + "/" + Possible_Value_2.ToString();
                Dealer_Result_TextBox.Text = Possible_Sums;

            }
            // else if 2nd card is ACE
            else if (Dealer_Result_TextBox.Text.Equals("1/11"))
            {
                Possible_Value_1 = Dealer_Current_Total;
                Possible_Value_2 = Dealer_Current_Total + 10;

                //Console.WriteLine(Possible_Value_1 + " " + Possible_Value_2);
                
                //Check for soft18+
                if(Possible_Value_2 > 17 && Possible_Value_2 < 22)
                {
                    Dealer_Current_Total = Possible_Value_2;
                    Dealer_Result_TextBox.Text = Dealer_Current_Total.ToString();
                    Decision();
                    return; // not tested yet
                }
                
            }
            else
            {
                Dealer_Result_TextBox.Text = Dealer_Current_Total.ToString();
            }

            //Console.WriteLine(Dealer_Current_Total);
            // Dealer keeps drawing until passes 21

            while (Dealer_Current_Total < 17) // 17
            {
               // Console.WriteLine("424");

                

                // Check for soft18+ before drawing 3rd card
                if ((Dealer_Ace) && (Dealer_Current_Total + 10 > 17) && (Dealer_Current_Total < 22))
                {
                    //Console.WriteLine(Dealer_Current_Total+10);
                    Dealer_Current_Total += 10;
                    Dealer_Ace = false;
                    Decision();
                    return; // added 2:16
                }
                else
                {
                    if (!d.Is_Empty())
                    {

                        Card c = d.Draw();

                        Remaining_Cards_Count_TextBox.Text = d.Get_Remaining_Cards().ToString();
                        if (Dealer_Hand_Count == 2)
                            Dealer_Card_3.Image = c.Get_Image();
                        else if (Dealer_Hand_Count == 3)
                            Dealer_Card_4.Image = c.Get_Image();
                        else if (Dealer_Hand_Count == 4)
                            Dealer_Card_5.Image = c.Get_Image();
                        else if (Dealer_Hand_Count == 5)
                            Dealer_Card_6.Image = c.Get_Image();
                        else if (Dealer_Hand_Count == 6)
                            Dealer_Card_7.Image = c.Get_Image();
                        Dealer_Hand_Count++;

                        Dealer_Current_Total += c.Get_Integer_Value();
                        Possible_Value_1 = Dealer_Current_Total;
                        Possible_Value_2 = Possible_Value_1 + 10;

                        // only display Possible_Value_1 if Value_2 is bust
                        if (Possible_Value_2 > 21)
                        {
                            Dealer_Result_TextBox.Text = Possible_Value_1.ToString();
                        }
                        // if Possible_Value_1 bust
                        else if (Possible_Value_1 > 21)
                        {
                            Decision(false, true, false, false);
                            return; // added 2:17
                        }
                        // if dealer reached 21, go to decision
                        else if (Possible_Value_1 == 21 || Possible_Value_2 == 21)
                        {
                            Decision();
                            return; // 5:02
                        }
                        // dealer hits soft 17
                        else if (Possible_Value_1 == 17)
                        {

                        }
                        // stand on soft 18+
                        else if (Possible_Value_2 > 17 && Possible_Value_2 < 22)
                        {
                            Dealer_Current_Total = Possible_Value_2;
                            Dealer_Result_TextBox.Text = Dealer_Current_Total.ToString();
                            Decision();
                            return; // added 2:17
                        }
                    }
                    // empty shoe
                    else
                    {
                        if(d.Get_Counter() == 0)
                            Display_Empty_Message();
                    }
                }

                //Dealer_Result_TextBox.Text = Possible_Value_1.ToString() + "/" + Possible_Value_2.ToString();
                

            }

            // Dealer bust
            if (Dealer_Current_Total > 21)
            {
                Decision(false, true, false, false);
                return; // added 2:17
            }
            // Compare hands
            else
            {
               // Console.WriteLine("Player: " + Player_Result_TextBox.Text + "\r\nDealer: " + Dealer_Result_TextBox.Text);
                // Check for player ace
                
                //if (Player_Result_TextBox.Text.Contains('/'))
                if(Player_Ace)
                {
                    Player_Current_Total += 10;
                    Player_Result_TextBox.Text = Player_Current_Total.ToString();
                }
                // Check for dealer ace
                if (Dealer_Result_TextBox.Text.Contains('/'))
                {
                    //Dealer_Current_Total += 10;
                    Dealer_Result_TextBox.Text = Dealer_Current_Total.ToString();
                }
                //Console.WriteLine("THIS DECISION: 703");
                Decision();
                return; // added 2:17
            }



        }

        private void Manage_Bet(double Current_Bet_Amount, bool Player_Win, bool Player_BlackJack, bool Push = false, bool Double = false)
        {

            if (Doubled_Previous_Hand)
            {
               // Console.WriteLine("Double down. \r\nabout to reset bet from " + Current_Bet + " to " + Current_Bet/2 );
            }

            // If player loses, reset bet amount
            if ((!Player_Win) && (!Push))
            {
                Bank -= Current_Bet;
                //Console.WriteLine("Resetting bet due to loss:");
                // reset bet on loss
                Current_Bet = 25.00;
                Bet_TextBox.Text = string.Format("{0:0.00}", Current_Bet);
                Bankroll_TextBox.Text = Bank.ToString();
                
            }

            // do nothing if push
            else if (Push)
            {
                // do nothing to bet amount
            }
            // player wins without blackjack
            else if (Player_Win && (!Player_BlackJack))
            {
                //Console.WriteLine("596 " + Current_Bet.ToString());
                Bank += Current_Bet;
                // double bet on win
                //Console.WriteLine("Doubling bet");
                
                Bet_TextBox.Text = string.Format("{0:0.00}", Current_Bet);

                Current_Bet *= 2;// here

                Bankroll_TextBox.Text = Bank.ToString();
                
                
            }

            // Pay 3 to 2 on Blackjack
            else if (Player_BlackJack)
            {
                Bank += (Current_Bet*(1.5));
                // double bet on win
                //Console.WriteLine("Doubling bet");
                
                Bet_TextBox.Text = string.Format("{0:0.00}", Current_Bet);


                Current_Bet *= 2; // here

                Bankroll_TextBox.Text = Bank.ToString();
                
            }

            string Profit_String = "";
            double Difference = 0;
            if(Bank > Starting_Bankroll)
            {
                Difference = Bank - Starting_Bankroll;
                Profit_String += "+" + Difference.ToString();
            } else if (Bank < Starting_Bankroll)
            {
                Difference = Starting_Bankroll - Bank;
                Profit_String += "-" + Difference.ToString();
            }
            // if break even
            else if(Bank == Starting_Bankroll)
            {
                Profit_String += "+0"; 
            }
            Profit_TextBox.Text = Profit_String;

            // div bet by 2 if previous hand was doubled
            if (Doubled_Previous_Hand)
            {
                Current_Bet /= 2;
                Doubled_Previous_Hand = false;
            }
            Bet_TextBox.Text = Current_Bet.ToString();

        }

        private void Display_Empty_Message()
        {
            MessageBox.Show("Empty shoe.");
        }

        async public void Decision(bool Player_Bust = false, bool Dealer_Bust = false, bool Player_BJ = false, bool Dealer_BJ = false)
        {

            //Console.WriteLine("796: Player: " + Player_Current_Total + " Dealer: " + Dealer_Current_Total);
            if (Player_Bust)
            {
                // reset bet on loss 
                Manage_Bet(Current_Bet, false, false);
                
                Player_Decision_TextBox.Text = "BUST";
                Player_Loss_Count++;
                Dealer_Win_Count++;
            }
            else if (Dealer_Bust)
            {
                Manage_Bet(Current_Bet, true, false);

                Dealer_Result_TextBox.Text = "BUST";
                Player_Decision_TextBox.Text = "WIN";
                Player_Win_Count++;
                Dealer_Loss_Count++;
            }
            else if (Player_BJ)
            {
                Manage_Bet(Current_Bet, true, true);
                Player_Decision_TextBox.Text = "BLACKJACK";
                Player_Win_Count++;
                Dealer_Loss_Count++;
            }
            else if (Dealer_BJ)
            {
                Manage_Bet(Current_Bet, false, false);

                Dealer_Result_TextBox.Text = "BLACKJACK";
                Player_Decision_TextBox.Text = "LOSS";
                Player_Loss_Count++;
                Dealer_Win_Count++;
            } 
            // If method was called with no parameters (decision)
            else
            {
                if (Player_Current_Total > Dealer_Current_Total)
                {

                    Manage_Bet(Current_Bet, true, false);

                    Player_Decision_TextBox.Text = "WIN";
                    Player_Win_Count++;
                    Dealer_Loss_Count++;
                }
                else if (Player_Current_Total == Dealer_Current_Total)
                {

                    Manage_Bet(Current_Bet, false, false, true);

                    Player_Decision_TextBox.Text = "PUSH";
                    Push_Count++;
                   
                }
                else
                {
                    Manage_Bet(Current_Bet, false, false);
                   // Console.WriteLine("854: " + "Player: " + Player_Current_Total + " Dealer: " + Dealer_Current_Total);
                    Player_Decision_TextBox.Text = "LOSS 2";
                    Player_Loss_Count++;
                    Dealer_Win_Count++;
                }
                
            }

            // delay to show table at the end of hand
            await Task.Delay(DELAY_TASK_TIME);

            //Thread.Sleep(1000);

            Clear_Button.Enabled = true;
            Hit_Button.Enabled = false;
            Double_Button.Enabled = false;
            Stand_Button.Enabled = false;
            Deal_Button.Enabled = false;

            Hand_Count++;

            Player_Wins_TextBox.Text = Player_Win_Count.ToString();
            Player_Losses_TextBox.Text = Dealer_Win_Count.ToString();
            Dealer_Wins_TextBox.Text = Dealer_Win_Count.ToString();
            Dealer_Losses_TextBox.Text = Player_Win_Count.ToString();
            Pushes_TextBox.Text = Push_Count.ToString();
            No_Hands_TextBox.Text = Hand_Count.ToString();

           // Console.WriteLine("         ===================================");
            Console.WriteLine("         Hands Dealt: " + Hand_Count);
            Console.WriteLine("         Current Bet: " + Current_Bet);
            Console.WriteLine("         Bet_TextBox: " + Bet_TextBox.Text);
          //  Console.WriteLine("         ===================================");


            //Decision_Reached = true;

            // updategraph
            Update_Graph();

            //Decision_Reached = true;

            Status_Label.Text = "Status: Round over.";
            //Console.WriteLine("Player Result TextBox: " + Player_Result_TextBox.Text);

            // perform clear_button 
            //Console.WriteLine("CLEAR BUTTON CLICKED here 893");
            if (Status_Label.Text.Equals("Status: Round over."))
            {
                Console.WriteLine("End of hand ============================");
                // Console.WriteLine("DECISION REACHED!");
                //Thread.Sleep(2000);
                Clear_Button.PerformClick();
                Decision_Reached = true;

                // delay before dealing new hand
                if (!d.Is_Empty())
                {
                    await Task.Delay(DELAY_TASK_TIME);
                    Deal_Button.PerformClick();
                }         
                else
                {
                    Display_Empty_Message();
                    return;
                }

            }
        }
        // Clear table
        private void Clear_Button_Click(object sender, EventArgs e)
        {
            Dealer_Card_1.Image = null;
            Dealer_Card_2.Image = null;
            Dealer_Card_3.Image = null;
            Dealer_Card_4.Image = null;
            Dealer_Card_5.Image = null;
            Dealer_Card_6.Image = null;
            Dealer_Card_7.Image = null;
            Dealer_Result_TextBox.Text = "";
            Dealer_Hand_Count = 0;
            Dealer_Current_Total = 0;
            Dealer_Ace = false;

            Player_Card_1.Image = null;
            Player_Card_2.Image = null;
            Player_Card_3.Image = null;
            Player_Card_4.Image = null;
            Player_Card_5.Image = null;
            Player_Card_6.Image = null;
            Player_Card_7.Image = null;
            Player_Result_TextBox.Text = "";
            Player_Hand_Count = 0;
            Player_Current_Total = 0;
            Player_Decision_TextBox.Text = "";
            Player_Turn = true;
            Player_Ace = false;

            Deal_Button.Enabled = true;
            // deal after click (automation)
            //Deal_Button.PerformClick();
        }

        // Player doubles bet and draws 1 more card
        private void Double_Button_Click(object sender, EventArgs e)
        {
            Doubled_Previous_Hand = true;
            Current_Bet *= 2;
            //Console.WriteLine("Double Button Click: " + Current_Bet);
            Bet_TextBox.Text = string.Format("{0:0.00}", Current_Bet);

            if (!d.Is_Empty())
            {
                Card c = d.Draw();
                Player_Card_3.Image = c.Get_Image();

                // Check for ace
                if (Player_Result_TextBox.Text.Contains('/'))
                {
                    // check for 21
                    if (Player_Current_Total + 11 == 21)
                    {
                        Player_Current_Total = 21;
                        Player_Result_TextBox.Text = "21";
                        Stand_Button.PerformClick();
                    }
                    //no 21 - perform calculation with ace in hand
                    else
                    {
                        int Possible_Value_1 = Player_Current_Total + c.Get_Integer_Value();
                        int Possible_Value_2 = Possible_Value_1 + 10;
                        Player_Current_Total = Possible_Value_1;
                        Player_Result_TextBox.Text = Possible_Value_1.ToString() + "/" + Possible_Value_2.ToString();
                        Player_Hand_Count++;

                        // end hand if player busts and reveal dealer card
                        if (Player_Current_Total > 21)
                        {
                            Dealer_Card_1.Image = Dealer_Down_Card.Get_Image();
                            // Dealer results not calculated since player busted
                            Decision(true, false, false, false);
                            return;
                        }
                    }
                }
                else
                {
                    Player_Current_Total += c.Get_Integer_Value();
                    Player_Result_TextBox.Text = Player_Current_Total.ToString();
                    Player_Hand_Count++;

                    // end hand if player busts and reveal dealer card
                    if (Player_Current_Total > 21)
                    {
                        Dealer_Card_1.Image = Dealer_Down_Card.Get_Image();
                        // Dealer results not calculated since player busted
                        Decision(true, false, false, false);
                        return;
                    }

                }
            }
            // empty shoe
            else
            {
                Display_Empty_Message();
                return;
            }
            // reduce current bet from double amount
            //Console.WriteLine("div");
            //Current_Bet /= 2;
        }

        private bool Double_Down(int Value_1, int Value_2)
        {

            // handle simple hands, (non-aces) // if Dealer and Player don't have aces
            if (!Dealer_Result_TextBox.Text.Contains('/') && !Player_Result_TextBox.Text.Contains('/'))
            {
                // return true if player has 11 and dealer shows 2-10
                if ((Dealer_Current_Total > 1 && Dealer_Current_Total < 11) && (Player_Current_Total == 11))
                {
                    Console.WriteLine("Player has 11!");
                    return true;
                }
                // return true if player has 10 and dealer shows 2-9
                else if ((Dealer_Current_Total > 1 && Dealer_Current_Total < 10) && (Player_Current_Total == 10))
                {
                    Console.WriteLine("Player has 10!");
                    return true;
                }
                // return true if player has 9 and dealer shows 3-6
                else if ((Dealer_Current_Total > 2 && Dealer_Current_Total < 7) && (Player_Current_Total == 9))
                {
                    Console.WriteLine("Dealer 3-6 and player has 9");
                    return true;
                }
            }

            // otherwise ACES are present:
            // Check for soft 17/18, and dealer shows 3-6
            else if (((Value_1 == 1 && Value_2 == 7) ||
                     (Value_1 == 1 && Value_2 == 6) ||
                     (Value_1 == 7 && Value_2 == 1) ||
                     (Value_1 == 6 && Value_2 == 1)) && (Dealer_Current_Total > 2 && Dealer_Current_Total < 7))
            {
                Console.WriteLine("soft 17/18 and dealer 3-6");
                return true;
            }
            // check for soft 15/16, and dealer shows 4-6
            else if (((Value_1 == 1 && Value_2 == 5) ||
                     (Value_1 == 1 && Value_2 == 4) ||
                     (Value_1 == 5 && Value_2 == 1) ||
                     (Value_1 == 4 && Value_2 == 1)) && (Dealer_Current_Total > 3 && Dealer_Current_Total < 7))
            {
                Console.WriteLine("soft 15/16 and dealer 4-6");
                return true;
            }
            // check for soft 13/14, and dealer shows 5 OR 6
            else if (((Value_1 == 1 && Value_2 == 3) ||
                     (Value_1 == 1 && Value_2 == 2) ||
                     (Value_1 == 3 && Value_2 == 1) ||
                     (Value_1 == 2 && Value_2 == 1)) && (Dealer_Current_Total == 5 || Dealer_Current_Total == 6))
            {
                Console.WriteLine("soft 13/14 and dealer has 5 OR 6");
                return true;
            }

            

            return false;
        }

        private void Init_Graph()
        {
            this.Graph_1.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0;
            this.Graph_1.ChartAreas[0].AxisY.MajorGrid.LineWidth = 0;
            this.Graph_1.Series[0].BorderWidth = 2;

            this.Graph_1.Palette = ChartColorPalette.SeaGreen;
            this.Graph_1.Titles.Add("Bankroll");
            this.Graph_1.ChartAreas[0].AxisX.ScrollBar.Enabled = true;

            // Plot first point: initial bankroll
            this.Graph_1.Series[0].Points.AddXY(0, Starting_Bankroll);

            this.Graph_1.ChartAreas[0].AxisY.Title = "Bankroll";
            this.Graph_1.ChartAreas[0].AxisX.Title = "Hand Count";

            this.Graph_1.ChartAreas[0].AxisX.Minimum = 0;
            this.Graph_1.ChartAreas[0].AxisX.Maximum = 6;

            this.Graph_1.ChartAreas[0].AxisY.Minimum = Bank - ((Bank/2)-250);
            this.Graph_1.ChartAreas[0].AxisY.Maximum = Bank + ((Bank/2)-250);

            this.Graph_1.ChartAreas[0].AxisX.LabelStyle.Enabled = false;

        }

        private void Update_Graph()
        {
            // ============================================
            if (!backgroundWorker1.IsBusy)
            {
                //backgroundWorker1.RunWorkerAsync();
            }
            // ============================================


            
            this.Graph_1.Series[0].Points.AddXY(Hand_Count, Bank);

            if (Hand_Count > 1)
            {
                //this.Graph_1.ChartAreas[0].AxisX.Minimum = Hand_Count - 6;
                this.Graph_1.ChartAreas[0].AxisX.Maximum = Hand_Count + 6;
            }
            

        }

        private void Update_Graph(int x, int y)
        {
            // plot new data                        (x    ,y)
            this.Graph_1.Series[0].Points.AddXY(x, y);


            if (Hand_Count > 1)
            {
                //this.Graph_1.ChartAreas[0].AxisX.Minimum = Hand_Count - 6;
                this.Graph_1.ChartAreas[0].AxisX.Maximum = y + 6;
            }

        }

        // Button to run simulation
        private void Run_Button_Click(object sender, EventArgs e)
        {
            //Deal_Button.PerformClick();
        }

        // Whened RunWorkerAsync() is called
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

           // backgroundWorker1.ReportProgress((int)Convert.ToDouble(Bankroll_TextBox.Text.ToString()));

        }

        // Everytime a target value changes, this function will be called
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //this.Graph_1.Series[0].Points.AddXY(Hand_Count, Bank);


        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //MessageBox.Show("BackgroundWorker completed");
        }
    }
}
