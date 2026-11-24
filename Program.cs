
/**************************************************************
 * Course: DEV110
 * Instructor: Phil Duncan
 * Term: Fall 2026
 *
 * Programmer: 
 * Assignment: Avatar Battle System
 *
 * Description:
 * Imports two completed avatar cards and conducts a
 * turn-based battle of up to seven exchanges.
 * Determines a winner and supports repeated playtesting
 * of Attack, Defense, and Health allocations.
 *
 * Revision  Date        Comment
 * --------  ----------  -------------------------------
 * 1.0       11/18/2026  Initial prototype
 **************************************************************/

using static System.Console;
using static System.IO.File;

namespace AvatarBattleArena
{
    class BattleSystem
    {
        /*
		 * Controls avatar selection, battle execution,
		 * and the option to play additional battles.
		 */
        static void Main(string[] args)
        {
            string[] avatarOne;
            string[] avatarTwo;
            bool keepPlaying = true;
            bool avatarOneStarts = true;
            int menuChoice;

            WriteLine(
                "================================\n" +
                "       AVATAR BATTLE ARENA      \n" +
                "================================"
            );

            avatarOne = ImportAvatar("Avatar 1");
            avatarTwo = ImportAvatar("Avatar 2");

            while (keepPlaying)
            {
                RunBattle(
                    avatarOne,
                    avatarTwo,
                    avatarOneStarts
                );

                //Each time a battle ends, the program switches which avatar gets to go first next time.
                avatarOneStarts = !avatarOneStarts;

                WriteLine(
                    "\n1. Fight again\n" +
                    "2. Import different avatars\n" +
                    "3. Exit"
                );

                menuChoice = ReadMenuChoice();

                if (menuChoice == 1)
                {

                    WriteLine(
                        "\nPreparing another battle..."
                    );
                }
                else if (menuChoice == 2)
                {
                    avatarOne = ImportAvatar("Avatar 1");
                    avatarTwo = ImportAvatar("Avatar 2");
                }
                else
                {
                    keepPlaying = false;
                }
            }

            WriteLine(
                "\nThanks for playing!\n" +
                "Press a key to continue..."
            );

            ReadKey();
        }

        /*
		 * Reads and validates an avatar card from a file.
		 *
		 * Input: A label identifying the avatar.
		 * Output: An array containing five avatar fields.
		 *
		 * Card format:
		 * 0 = Name, 1 = Description, 2 = Attack,
		 * 3 = Defense, 4 = Health.
		 */
        static string[] ImportAvatar(string avatarLabel)
        {
            string[] avatar = new string[5];
            string[] fileContents;
            string fileName;
            bool validCard = false;
            int attack;
            int defense;
            int health;

            while (!validCard)
            {
                Write(
                    "\nEnter file name for " +
                    avatarLabel + ": "
                );

                fileName = ReadLine() ?? "";

                if (Exists(fileName))
                {
                    try
                    {
                        fileContents =
                            ReadAllLines(fileName);

                        if (fileContents.Length == 5)
                        {
                            validCard = ValidateAvatar(
                                fileContents
                            );

                            if (validCard)
                            {
                                avatar = fileContents;

                                WriteLine(
                                    "\nAvatar imported: " +
                                    avatar[0]
                                );
                            }
                            else
                            {
                                WriteLine(
                                    "Invalid avatar attributes."
                                );
                            }
                        }
                        else
                        {
                            WriteLine(
                                "Invalid card format."
                            );
                        }
                    }
                    catch (IOException)
                    {
                        WriteLine(
                            "Unable to read the file."
                        );
                    }
                    catch (UnauthorizedAccessException)
                    {
                        WriteLine(
                            "Access to the file was denied."
                        );
                    }
                }
                else
                {
                    WriteLine(
                        "File not found. Try again."
                    );
                }
            }

            return avatar;
        }

        /*
		 * Verifies that an imported card contains valid
		 * attributes totaling exactly 100 points.
		 *
		 * Input: An array containing avatar information.
		 * Output: True when the card is valid.
		 */
        static bool ValidateAvatar(string[] avatar)
        {
            const int TotalPoints = 100;
            const int MinimumAttribute = 5;
            const int MaximumAttribute = 90;

            //const int MinimumAttack = 10;
            //const int MaximumAttack = 70;

            //const int MinimumDefense = 10;
            //const int MaximumDefense = 80;

            //const int MinimumHealth = 20;
            //const int MaximumHealth = 80;

            bool valid = false;
            bool validAttack;
            bool validDefense;
            bool validHealth;
            int attack;
            int defense;
            int health;

            validAttack = int.TryParse(
                avatar[2], out attack
            );

            validDefense = int.TryParse(
                avatar[3], out defense
            );

            validHealth = int.TryParse(
                avatar[4], out health
            );

            if (validAttack &&
                validDefense &&
                validHealth &&
                !string.IsNullOrWhiteSpace(avatar[0]) &&
                !string.IsNullOrWhiteSpace(avatar[1]))
            {
                if (attack >= MinimumAttribute &&
                    attack <= MaximumAttribute &&
                    defense >= MinimumAttribute &&
                    defense <= MaximumAttribute &&
                    health >= MinimumAttribute &&
                    health <= MaximumAttribute


                    //attack >= MinimumAttack &&
                    //attack <= MaximumAttack &&
                    //defense >= MinimumDefense &&
                    //defense <= MaximumDefense &&
                    //health >= MinimumHealth &&
                    //health <= MaximumHealth

                    )
                {
                    if (attack + defense + health ==
                        TotalPoints)
                    {
                        valid = true;
                    }
                }
            }

            return valid;
        }

        /*
		 * Conducts a battle between two imported avatars.
		 *
		 * Each exchange allows two attacks unless an
		 * avatar is defeated.
		 *
		 * The starting attacker alternates between
		 * successive battles.
		 */
        static void RunBattle(
            string[] avatarOne,
            string[] avatarTwo,
            bool avatarOneStarts)
        {
            const int MaximumExchanges = 7;

            string[] names = new string[2];
            int[] attacks = new int[2];
            int[] defenses = new int[2];
            int[] remainingHealth = new int[2];

            int exchange = 1;
            int turn;
            int attacker;
            int defender;
            int damage;
            int winner = -1;
            int loser;
            bool battleOver = false;

            names[0] = avatarOne[0];
            names[1] = avatarTwo[0];

            attacks[0] = int.Parse(avatarOne[2]);
            attacks[1] = int.Parse(avatarTwo[2]);

            defenses[0] = int.Parse(avatarOne[3]);
            defenses[1] = int.Parse(avatarTwo[3]);

            remainingHealth[0] = int.Parse(avatarOne[4]);
            remainingHealth[1] = int.Parse(avatarTwo[4]);

            WriteLine(
                "\n================================\n" +
                "          BATTLE BEGINS!        \n" +
                "================================"
            );

            DisplayAvatar(avatarOne);
            DisplayAvatar(avatarTwo);

            if (avatarOneStarts)
            {
                WriteLine(
                    "\nFirst attacker: " + names[0]
                );
            }
            else
            {
                WriteLine(
                    "\nFirst attacker: " + names[1]
                );
            }

            /*
			 * Each exchange contains up to two turns.
			 * Combat stops when health reaches zero or
			 * the seventh exchange is completed.
			 */
            while (exchange <= MaximumExchanges &&
                !battleOver)
            {
                WriteLine(
                    "\n---------- EXCHANGE " +
                    exchange + " ----------"
                );

                for (turn = 0;
                    turn < 2 && !battleOver;
                    turn++)
                {
                    if (avatarOneStarts)
                    {
                        attacker = turn;
                    }
                    else
                    {
                        attacker = 1 - turn;
                    }

                    defender = 1 - attacker;

                    damage = CalculateDamage(
                        attacks[attacker],
                        defenses[defender]
                    );

                    remainingHealth[defender] =
                        ApplyDamage(
                            names[attacker],
                            names[defender],
                            remainingHealth[defender],
                            damage
                        );

                    if (remainingHealth[defender] == 0)
                    {
                        winner = attacker;
                        battleOver = true;
                    }
                }

                if (!battleOver)
                {
                    WriteLine("\nPress a key to bring on the next exchange...");
                    ReadKey(true);
                }

                exchange++;
            }

            /*
			 * If both avatars survive seven exchanges,
			 * remaining health determines the winner.
			 * Equal health initiates sudden death.
			 */
            if (!battleOver)
            {
                WriteLine(
                    "\nSeven exchanges completed."
                );

                if (remainingHealth[0] >
                    remainingHealth[1])
                {
                    winner = 0;
                }
                else if (remainingHealth[1] >
                    remainingHealth[0])
                {
                    winner = 1;
                }
                else
                {
                    winner = SuddenDeath(
                        names,
                        attacks,
                        defenses
                    );
                }
            }

            loser = 1 - winner;

            AnnounceWinner(
                names[winner],
                names[loser],
                remainingHealth[winner]
            );
        }

        /*
		 * Calculates attack damage after defense.
		 *
		 * Input: Attack and Defense attribute values.
		 * Output: Damage dealt by one attack.
		 *
		 * This experimental formula guarantees at least
		 * one damage point and is intended for playtesting.
		 */
        static int CalculateDamage(
            int attack,
            int defense)
        {
            const int AttackDivisor = 5;
            const int DefenseDivisor = 10;
            const int MinimumDamage = 1;

            int attackPower;
            int defenseReduction;
            int damage;

            attackPower = attack / AttackDivisor;
            defenseReduction = defense / DefenseDivisor;

            damage = attackPower - defenseReduction;

            if (damage < MinimumDamage)
            {
                damage = MinimumDamage;
            }

            return damage;
        }

        /*
		 * Applies one attack and displays its result.
		 *
		 * Input: Combatant names, current health,
		 * and calculated damage.
		 * Output: Defender's updated remaining health.
		 */
        static int ApplyDamage(
            string attackerName,
            string defenderName,
            int defenderHealth,
            int damage)
        {
            defenderHealth -= damage;

            if (defenderHealth < 0)
            {
                defenderHealth = 0;
            }

            WriteLine(
                "\nAttacker: " + attackerName +
                "\n\nDefender: " + defenderName +
                "\nDamage: " + damage +
                "\nRemaining health: " + defenderHealth
            );

            return defenderHealth;
        }

        /*
		 * Resolves a battle tied after seven exchanges.
		 *
		 * Each avatar's potential attack damage is
		 * compared. A random selection resolves an
		 * otherwise equal result.
		 *
		 * Input: Names, Attack, and Defense arrays.
		 * Output: Index of the winning avatar.
		 */
        static int SuddenDeath(
            string[] names,
            int[] attacks,
            int[] defenses)
        {
            Random random = new Random();
            int damageOne;
            int damageTwo;
            int winner;

            damageOne = CalculateDamage(
                attacks[0], defenses[1]
            );

            damageTwo = CalculateDamage(
                attacks[1], defenses[0]
            );

            WriteLine(
                "\n================================\n" +
                "          SUDDEN DEATH!         \n" +
                "================================\n" +
                names[0] + " damage: " + damageOne +
                "\n" + names[1] + " damage: " + damageTwo
            );

            if (damageOne > damageTwo)
            {
                winner = 0;
            }
            else if (damageTwo > damageOne)
            {
                winner = 1;
            }
            else
            {
                WriteLine(
                    "\nEqual damage. Random tiebreaker."
                );

                winner = random.Next(0, 2);
            }

            return winner;
        }

        /*
		 * Displays an imported avatar's information.
		 *
		 * Input: The five fields of an avatar card.
		 */
        static void DisplayAvatar(string[] avatar)
        {
            WriteLine(
                "\nName: " + avatar[0] +
                "\nDescription: " + avatar[1] +
                "\nAttack: " + avatar[2] +
                "\nDefense: " + avatar[3] +
                "\nHealth: " + avatar[4]
            );
        }

        /*
		 * Displays the final battle result.
		 *
		 * Input: Winner, loser, and winner's health.
		 */
        static void AnnounceWinner(
            string winner,
            string loser,
            int remainingHealth)
        {
            WriteLine(
                "\n================================\n" +
                "           BATTLE OVER!         \n" +
                "================================\n" +
                "Winner: " + winner +
                "\nLoser: " + loser +
                "\nWinner remaining health: " +
                remainingHealth +
                "\n================================"
            );
        }

        /*
		 * Accepts a menu selection between 1 and 3.
		 *
		 * Output: A validated integer menu choice.
		 */
        static int ReadMenuChoice()
        {
            int choice = 0;
            bool validChoice = false;
            string input;

            while (!validChoice)
            {
                Write("\nYour choice (1-3): ");
                input = Console.ReadLine() ?? "";

                validChoice = int.TryParse(
                    input, out choice
                );

                if (validChoice)
                {
                    validChoice =
                        choice >= 1 && choice <= 3;
                }

                if (!validChoice)
                {
                    WriteLine(
                        "Please enter 1, 2, or 3."
                    );
                }
            }

            return choice;
        }
    }
}
