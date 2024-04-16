using System;
using System.Collections.Generic;

namespace MonogameUtilities.Util
{
    internal class Dice
    {
        public enum Operator
        {
            Add,
            Subtract,
            Multiply,
            Divide
        }

        public int Rolls;
        public int Pips;

        public Dice NextDice;
        public Operator NextDiceOperator;

        public static Random RNG = new Random();

        public Dice(int rolls, int pips)
        {
            Rolls = rolls;
            Pips = pips;
        }

        public Dice(int rolls, int pips, Dice nextDice, Operator nextOperator)
        {
            Rolls = rolls;
            Pips = pips;

            NextDice = nextDice;
            NextDiceOperator = nextOperator;
        }

        public int Roll()
        {
            int nextDiceRoll = 0;
            bool hasNextDice = NextDice != null;
            if (hasNextDice)
            {
                nextDiceRoll = NextDice.Roll();
            }

            int coll = 0;
            if (Pips == 1)
            {
                coll = Rolls;
            }
            else
            {
                for (int i = 0; i < Rolls; i++)
                {
                    coll += RNG.Next(Pips) + 1;
                }
            }

            if (hasNextDice)
            {
                return NextDiceOperator switch
                {
                    (Operator.Add) => coll + nextDiceRoll,
                    (Operator.Subtract) => coll - nextDiceRoll,
                    (Operator.Multiply) => coll * nextDiceRoll,
                    (Operator.Divide) => coll / nextDiceRoll,
                    _ => throw new Exception("Invalid dice operation (" + NextDiceOperator.ToString() + ")"),
                };
            }
            else
            {
                return coll;
            }
        }

        public static Dice Parse(string str)
        {
            str = str.Replace(" ", "");

            int index = 0;

            Stack<Dice> dice = new();

            Operator lastOperator = Operator.Add;
            while (index < str.Length)
            {
                if (str[index] == '+')
                {
                    lastOperator = Operator.Add;
                    index++;
                    continue;
                }
                else if (str[index] == '-')
                {
                    lastOperator = Operator.Subtract;
                    index++;
                    continue;
                }
                else if (str[index] == '*')
                {
                    lastOperator = Operator.Multiply;
                    index++;
                    continue;
                }
                else if (str[index] == '/')
                {
                    lastOperator = Operator.Divide;
                    index++;
                    continue;
                }

                Tuple<int, int> ret = GetNextInt(str, index);
                int num = ret.Item1;
                index = ret.Item2;

                if (index < str.Length)
                {
                    if (str[index] == 'd')
                    {
                        index += 1;

                        ret = GetNextInt(str, index);
                        int pips = ret.Item1;
                        index = ret.Item2;

                        dice.Push(new Dice(num, pips, null, lastOperator));
                    }
                    else
                    {
                        dice.Push(new Dice(num, 1, null, lastOperator));
                    }
                }
                else
                {
                    dice.Push(new Dice(num, 1, null, lastOperator));

                    break;
                }
            }

            Dice lastDice = dice.Pop();

            while (dice.Count > 0)
            {
                Dice newDice = dice.Pop();
                newDice.NextDice = lastDice;

                lastDice = newDice;
            }

            return lastDice;
        }

        private static Tuple<int, int> GetNextInt(string str, int index)
        {
            string coll = "";

            while (index < str.Length && 48 <= (byte)str[index] && (byte)str[index] <= 57)
            {
                coll += str[index];

                index++;
            }

            return new(int.Parse(coll), index);
        }

        public override string ToString()
        {
            string coll = Rolls.ToString();
            if (Pips > 1)
            {
                coll = Rolls + "d" + Pips;
            }

            if (NextDice != null)
            {
                string op = "";
                switch (NextDiceOperator)
                {
                    case (Operator.Add): op += "+"; break;
                    case (Operator.Subtract): op += "-"; break;
                    case (Operator.Multiply): op += "*"; break;
                    case (Operator.Divide): op += "/"; break;
                }

                coll += op + NextDice.ToString();
            }

            return coll;
        }

        public static int D20()
        {
            return RNG.Next(1, 21);
        }

        public static int RollDice(int diceFace)
        {
            return RNG.Next(1, diceFace + 1);
        }

        // Easy conversions
        public static implicit operator Dice(string notation) => Parse(notation);
        public static implicit operator Dice(int val) => new(val, 1);
        public static implicit operator int(Dice dice) => dice.Roll();

    }
}
