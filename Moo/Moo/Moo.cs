using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moo
{
    class Moo
    {
        public static byte numberOfDigits;
        
        static void Main(string[] args)
        {
            while (true)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("Колко цифри да съдържа числото, измислено от компютъра? ");
                if(!byte.TryParse(Console.ReadLine(), out numberOfDigits))
                {
                    ErrorMessage("\nВъвели сте грешни данни!");
                    continue;
                }
                if (numberOfDigits<3)
                {
                    ErrorMessage("\nБроят на цифрите не може да бъде по-малък от 3!");
                    continue;
                }
                else if (numberOfDigits>10)
                {
                    ErrorMessage("\nБроят на цифрите не може да бъде по-голям от 10!");
                    continue;
                }

                uint computersNumber = GenerateNumber(); // Компютъра си измисля число 
                uint countTries = 0; // Брои опитите на играча да познае числото

                ComputerMessage(String.Format("Намислих си едно {0}-цифрено число 3:-O", numberOfDigits));
                while (true)
                {                   
                    Console.Write("\nИграч:");
                    string input = Console.ReadLine();

                    if (input == "exit")
                    {
                        Exit();
                    }
                    else if (input == "help")
                    {
                        Help();
                    }
                    else if(input == "rules")
                    {
                        Rules();
                    } 
                    else if (input == "clear")
                    {
                        Console.Clear();
                    }
                    else if (input == "log")
                    {
                        Log();
                    }
                    else if (input=="surrender")
                    {
                        if (Surrender(computersNumber))
                        {
                            Console.Clear();
                            break;
                        }
                    }
                    else if (input=="joker")
                    {
                        Joker();
                    }
                    else if (input=="newgame")
                    {

                        Console.Write("Сигурни ли сте, че искате да започнете нова игра?(y|N):");
                        if (Console.ReadKey().Key == ConsoleKey.Y)
                        {
                            Console.Clear();
                            break;
                        }
                        Console.WriteLine();
                    }
                    else
                    {
                        //Тук ще проверяваме броя на кравите и броя на биковете,ще следим предположенията на играча и дали той не е познал числото :)

                        /* 
                         * Опитваме се да парснем числото, въведено от играча. 
                         * Ако играча не е въвел число или пък числото,което е въвел, не се побира в типа uint - връщаме му съобщение за грешка.
                         */
                        uint gamersNumber = 0;
                        if (!uint.TryParse(input,out gamersNumber))
                        {
                            if (String.IsNullOrEmpty(input) || String.IsNullOrWhiteSpace(input))
                            {
                                continue;
                            }
                            ErrorMessage(String.Format("\n\"{0}\" не е валидно число! Трябва да въведете {1}-цифрено положително число.",input,numberOfDigits));
                            continue;
                        }

                        //Проверяваме дали числото не е по-голямо или по-малко от избраната, от играча, дължина в началото на играта
                        if (input.Trim().Length > numberOfDigits)
                        {
                            ErrorMessage(String.Format("Въведеното от вас число има повече от {0} цифри!",numberOfDigits));
                            continue;
                        }
                        else if (input.Trim().Length < numberOfDigits)
                        {
                            ErrorMessage(String.Format("Въведеното от вас число има по-малко от {0} цифри!", numberOfDigits));
                            continue;
                        }

                        //Правим проверка и за повтарящи се цифри
                        if (HasEqualCharacters(input.Trim()))
                        {
                            ErrorMessage("Числото не трябва да съдържа повтарящи се цифри!");
                            continue;
                        }

                        // Вземаме броя на биковете и броя на кравите, и ги отпечатваме на конзолата
                        byte[] bullsAndCows = GetBullsAndCows(computersNumber,gamersNumber);
                        if (bullsAndCows[0]==numberOfDigits) // Ако биковете са равни на цифрите, играча печели играта
                        {
                            Console.Beep();
                            ComputerMessage(String.Format("Вие разкрихте тайното ми число с {0} опита. :)",countTries++));
                            Console.Write("Искате ли да започнете нова игра?(Y|n):");
                            if (Console.ReadKey().Key == ConsoleKey.N)
                            {
                                return;
                            }
                            Console.Clear();
                            break;
                        }
                        else
                        {
                            countTries++;
                            ComputerMessage(String.Format("Имате {0} бика и {1} крави",bullsAndCows[0],bullsAndCows[1]));
                            continue;
                        }
                    }
                    Console.WriteLine();
                }
            }
        }

        static void Exit()
        {
            Console.Write("Наистина ли искате да излезете от играта?(y|N):");
            if (Console.ReadKey().Key == ConsoleKey.Y)
            {
                Console.WriteLine();
                Environment.Exit(Environment.ExitCode);
            }
            Console.WriteLine();
        }

        static bool Surrender(uint secretNumber)
        {
            ComputerMessage("Предаваш се, а? Слабак!");
            Console.Write("Сигурни ли сте, че искате да се предадете?(y|N):");
            if (Console.ReadKey().Key == ConsoleKey.Y)
            {
                Console.WriteLine();
                ComputerMessage(String.Format("Тайното ми число беше {0}", secretNumber));
                Console.WriteLine();
                Console.Write("Искате ли да започнете нова игра?(Y|n):");
                if (Console.ReadKey().Key == ConsoleKey.N)
                {
                    Environment.Exit(Environment.ExitCode);
                }
                return true;
            }
            Console.WriteLine();
            return false;
        }

        static void Joker()
        {

        }

        static void Log()
        {

        }

        static void Help()
        {

        }

        static void Rules()
        {

        }
        
        static byte[] GetBullsAndCows(uint compNumber, uint gamersNumber) // Връща броя на биковете и кравите
        {
            byte[] computersNumberDigits = ExtractDigits(compNumber);
            byte[] gamersNumberDigits = ExtractDigits(gamersNumber);
            byte[] numberBullsAndCows = {0,0}; // На индекс 0 е броят на биковете

            for (int i = 0; i < numberOfDigits; i++)
            {
                if (computersNumberDigits[i] == gamersNumberDigits[i])
                {
                    numberBullsAndCows[0]++;
                }
                else
                {
                    for (int j = 0; j < numberOfDigits; j++)
                    {
                        if (gamersNumberDigits[i]==computersNumberDigits[j])
                        {
                            numberBullsAndCows[1]++;
                        }
                    }
                }
            }
            
            return numberBullsAndCows;
        }

        static bool HasEqualCharacters(string gamersInput)
        {            
            for (int i = 0; i < gamersInput.Length; i++)
            {
                for (int j = 0; j < gamersInput.Length; j++)
                {
                    if (gamersInput[i].Equals(gamersInput[j]) && j!=i)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        static byte[] ExtractDigits(uint number) // Разделя числото на отделни цифри
        {
            byte[] digits = new byte[numberOfDigits];
            for (int i = 0; i < numberOfDigits; i++)
            {
                digits[i] = (byte)((number / (uint)Math.Pow(10, i)) % 10);
            }
            return digits;
        }

        static uint GenerateNumber()
        {
            Random randomGenerator = new Random();
            int bottomBoundary = (int)Math.Pow(10, numberOfDigits - 1);
            int topBoundary = (int)Math.Pow(10, numberOfDigits) - 1;
            uint compNumber = (uint)randomGenerator.Next(bottomBoundary, topBoundary);
            while (HasEqualCharacters(compNumber.ToString()))
            {
                compNumber = (uint)randomGenerator.Next(bottomBoundary, topBoundary);
            }
            return compNumber;
        }

        static void ComputerMessage(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Компютър: {0}",msg);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
        
        static void ErrorMessage(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(msg);
            Console.ForegroundColor = ConsoleColor.Gray;            
        }
    }
}
