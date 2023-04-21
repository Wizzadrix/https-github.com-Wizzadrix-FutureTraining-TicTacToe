using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Numerics;
using System.Security.Cryptography;

namespace TicTacToeChallenge
{
    internal class Program
    {

        static void Main(string[] args)
        {
            //DrawBoard(startingBoard);

            StartGame();

            Console.ReadKey();
        }

        //properties
        //initial board for new games
        public static string[,] startingBoard =
            {
                { "1", "2", "3" },
                { "4", "5", "6" },
                { "7", "8", "9" }
            };

        //board that is modified during the game
        public static string[,] board =
            {
                { "1", "2", "3" },
                { "4", "5", "6" },
                { "7", "8", "9" }
            };

        //Player 1 and 2 take turns. If Player 1 is not active, Player 2 is active
        public static bool isPlayerOneActive = true;
        public static string currentPlayer = "X";

        //Response for new game
        public static string response;

        //methods

        public static void StartGame()
        {
            //Potentially clear console from previous game and reset the board
            Console.Clear();
            //board = startingBoard;
            ResetBoard(board);
            isPlayerOneActive = true;

            Console.WriteLine("Welcome to Tic Tac Toe !");
            Console.WriteLine("Please choose a number where you would like to put your mark");

            DrawBoard(board);

            string playerInput = Console.ReadLine();

            ReplaceValueOnBoard(board, playerInput);


            var xxx = "";
        }
        public static void DrawBoard(string[,] board)
        {
            for (int i = 0; i < board.GetLength(0); i++)
            {
                Console.WriteLine("   |   |");
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    //print a row in three steps
                    Console.Write(" " + board[i, j]);

                    //add separation lines between columns
                    if (j < 2)
                        Console.Write(" |");
                }
                Console.WriteLine();

                //separation lines between rows
                if (i < board.GetLength(1) - 1)
                    Console.WriteLine("___|___|___");
            }
            Console.WriteLine("   |   |");
        }

        //Looks for a number to replace with "X" or "O". If no such number can be found, return false
        public static bool ReplaceValueOnBoard(string[,] board, string number)
        {
            //If the input is invalid, try again with different (hopefully correct) input
            string newInput = number;
            if (!CheckInputValidity(number))
            {
                newInput = Console.ReadLine();
                ReplaceValueOnBoard(board, newInput);
            }

            //Nested loop for setting the "X" or "O" at position number/newInput
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    //When the position on the board has been found, set it with "X" or "O"
                    if (board[i, j] == number)
                    {
                        //Place "X" for player one
                        //if (isPlayerOneActive)
                        //{
                        //    currentPlayer = "X";
                        //}
                        ////Place "O" for player two
                        //else
                        //{
                        //    currentPlayer = "O";
                        //}

                        //Set current player to "X" or "O"
                        currentPlayer = isPlayerOneActive ? "X" : "O";

                        if (!CheckFieldOccupied(board[i, j]))
                        {
                            board[i, j] = currentPlayer;
                        }
                        else //field already has a "X" or an "O"
                        {
                            newInput = Console.ReadLine();
                            ReplaceValueOnBoard(board, newInput);


                            //Check if the game is over
                            if (CheckWinner(board))
                            {

                                var winner = "";
                                if (isPlayerOneActive)
                                    winner = "X";
                                else
                                    winner = "O";

                                if (isPlayerOneActive)
                                {
                                    Console.WriteLine("Congratulations player {0}, you have won the game !", winner);

                                    //Ask for a new game
                                    AskForRematch();
                                }

                            }

                            DrawNewBoard(board);
                        }
                    }

                    //field to set X or O has been found
                    return true;
                }

            }
            //No such field exists
            return false;
        }

        //Returns true if field is "free" (number) otherwise false
        private static bool CheckFieldOccupied(string field)
        {
            int fieldValue = int.Parse(field);
            if (fieldValue >= 1 && fieldValue <= 9)
                return false;
            else
            {
                Console.WriteLine("The field {0} is already occupied ! Please choose an empty field instead.", fieldValue);
                return true;
            }
        }

        private static void AskForRematch()
        {
            Console.WriteLine("Would you like to play another game ?");
            Console.WriteLine("Type yes or no");
            response = Console.ReadLine();

            if (response == "yes")
            {
                Console.WriteLine("Okay, time for a rematch. Press any key to begin !");
                Console.ReadKey();
                StartGame();
            }
            else if (response == "no")
            {
                Console.WriteLine("GG, Farewell !");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("Only 'yes' or 'no' are accepted answers! Try again");
                response = Console.ReadLine();
                AskForRematch();
            }
        }

        //resets the board to the initial board with numbers 1 to 9
        private static void ResetBoard(string[,] board)
        {
            int boardValue = 1;
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    board[i, j] = boardValue.ToString();
                    boardValue++;
                }
            }
        }

        //Delete the old board, print the new board
        private static void DrawNewBoard(string[,] board)
        {
            Console.Clear();
            DrawBoard(board);

            //Change who is the active player
            isPlayerOneActive = !isPlayerOneActive;

            if (isPlayerOneActive)
                Console.WriteLine("Player X, please enter where you would like to put your mark");
            else
                Console.WriteLine("Player O, please enter where you would like to put your mark");

            //New player input
            string playerInput = Console.ReadLine();
            ReplaceValueOnBoard(board, playerInput);

        }

        //returns false, if anything is wrong with the input
        public static bool CheckInputValidity(string input)
        {
            //Handle invalid input
            int num = -1;
            if (!int.TryParse(input, out num))
            {
                Console.WriteLine("Input is not an integer ! Please enter another input.");
                return false;
            }
            //Only accept numbers between 1 and 9
            if (int.Parse(input) < 1 || int.Parse(input) > 9)
            {
                Console.WriteLine("You have to enter a number between 1 and 9 ! Please enter another input.");
                return false;
            }

            return true;
        }

        public static bool CheckWinner(string[,] board)
        {
            //X and O counter for diagonals left to right:
            int xCounterLr = 0;
            int oCounterLr = 0;

            //X and O counter for diagonals right to left:
            int xCounterRl = 0;
            int oCounterRl = 0;

            //all cases with a winner
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    //case horizontal
                    if ((board[i, 0] == "X" && board[i, 1] == "X" && board[i, 2] == "X") || (board[i, 0] == "O" && board[i, 1] == "O" && board[i, 2] == "O"))
                    {
                        Console.WriteLine("horizontal victory for " + board[i, 0]);
                        return true;
                    }
                    //case vertical
                    if ((board[0, j] == "X" && board[1, j] == "X" && board[2, j] == "X") || (board[0, j] == "O" && board[1, j] == "O" && board[2, j] == "O"))
                    {
                        Console.WriteLine("vertical victory for " + board[0, j]);
                        return true;
                    }
                    //case diagonal left to right
                    //else if ((board[1,1] == "X" && board[2,2] == "X" && board[3,3] == "X") || (board[1, 1] == "O" && board[2, 2] == "O" && board[3, 3] == "O"))

                    //case X diagonal left to righ
                    if ((board[i, j] == "X" && i == j))
                    {
                        xCounterLr++;
                        if (xCounterLr >= 3)
                        {
                            Console.WriteLine("left to right diagonal X victory");
                            return true;
                        }
                    }
                    //case O diagonal left to right
                    if (board[i, j] == "O" && i == j)
                    {
                        oCounterLr++;
                        if (oCounterLr >= 3)
                        {
                            Console.WriteLine("left to right diagonal O victory");
                            return true;
                        }
                    }

                    //case X diagonal right to left
                    if (board[j, i] == "X" && i + j == 2)
                    {
                        xCounterRl++;
                        if (xCounterRl >= 3)
                        {
                            Console.WriteLine("right to left diagonal X victory");
                            return true;
                        }
                    }
                    //case O diagonal right to left
                    if (board[j, i] == "O" && i + j == 2)
                    {
                        oCounterRl++;
                        if (oCounterRl >= 3)
                        {
                            Console.WriteLine("right to left diagonal O victory");
                            return true;
                        }
                    }
                    //note: diagonal cases could be combined

                }
            }

            //all cases without any winner
            return false;
        }


    }
}
