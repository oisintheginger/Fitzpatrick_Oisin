﻿using System;
using System.Collections.Generic;

namespace Fitzpatrick_Oisin
{
    /*
     
    A first-person strategy game has the following description. There is one player and many
    non-player characters (NPC). The player is able to shoot an NPC. If the NPC is an enemy,
    their health is reduced. If the NPC is a friend, the players health is reduced. The player can
    query an NPCs backstory to help determine if they are a friend or an enemy. If they think
    they are friendly, they can request friendship, however if they are an enemy the player
    loses health points.
    The player has a name, a position, a health value (5 points) and ammunition value (10
    bullets). To shoot an enemy the player must be within 30m. The player can move but will
    lose health points each time.
    Each NPC has a name, a type, an ally status (public) and a health value (5 points). Type
    defines whether the NPC is friend or enemy (private). Each time the player successfully
    shoots an enemy, their health value is reduced.
    The goal of the game is to kill or recruit all NPCs while maintaining the maximum health
    value possible. When the last NPC has been recruited OR killed, the game should finish,
    printing the player health value.

    Requirements
    Based on the Game Description, write a C# Console application (Game) that uses classes
    as design templates for the creation of Players and Non-Player Characters (NPC). The
    user should also be able to run the following commands from the command line:
    1. Create Player: When this command is called, ask for name of the player and create a
    player object with a randomly generated position on a 100 x 100 grid. Only 1 player can
    be created.
    2. Create NPC: When this command is called, ask for the name of the NPC and create
    an NPC object with a randomly generated position on a 100 x 100 grid and randomly
    generated type and backstory values (the type and backstory should be related). A
    maximum of 5 NPC objects can be created.
    3. Shoot: When this command is called, ask for the name of the NPC to shoot. If the NPC
    is within distance, is an enemy and has health points remaining, then reduce their
    health by 1. If the NPC is a friend, then reduce the players health points by 2. When an
    enemy’s health reaches 0, the enemy is dead. Each time an enemy is killed, the status
    of all friends and enemies has to be checked to determine if the game is over.
    4. Move: When this command is called, ask for a new position, update the players
    position and reduce their health by 1.

    5. Query NPC: When this command is called, ask for an NPC’s name and print the NPC’s
    backstory.
    6. Request Alliance: When this command is called, ask for the NPC’s name. If the NPC’s
    type is enemy, then the player loses 2 health points and their allegiance status is set to
    “enemy”. If the NPC’s type is friend, then their allegiance status is set to “friend”.
    7. Print Player Status: When this command is called, print the current status of the
    player. This should include the players health value, ammunition value and position.
    8. Print NPC Status: When this commend is called, print the current status of all NPCs.
    This should include health values, their allegiance status and their distance from the
    player.

    */

    //using struct to contain a win condition
    public struct WinCondition
    {
        public int recruits;
        public int deadEnemies;
    }

    //Using structs for position generation allows x and y position values to be returned as one value by the position generator method
    public struct position
    {
        public int PositionX;
        public int PositionY;
    }
    
    //Using structs for player/NPC status/type/allegiance allows it to be returned from a method
    public struct PlayerStatus
    {
        public position pos;
        public string playerName;
        public int playerHealth;
        public int playerAmmo;

    }

    public struct NPCtype
    {
        public string Type;
    }

    public struct npcStatus
    {
        public position npcPos;
        public string npcName, disposition;
        public npcAllegiance allegiance;
        public int NPChealth;
        

    }

    public struct npcAllegiance
    {
        public string allegiance;
    }

    

    public class Program
    {
        public static Program p = new Program();
        public List<NPCObject> npcList = new List<NPCObject>();
        public PlayerObject Player;

        public static int enemyKillCount;
        public static int allyRecruitCount;

        public static void Main(string[] args)
        {
            
            enemyKillCount=0;
            allyRecruitCount=0;
            
            Console.WriteLine("Welcome. What is your name?");
            Console.WriteLine();
            String PlayerName = Console.ReadLine();
            Console.WriteLine();
            position generatedPosition = PositionGenerator();
            p.Player = new PlayerObject(PlayerName, 5, 10, generatedPosition);
            Console.WriteLine("Greetings {0}, you are at ({1},{2})", PlayerName.ToUpper(), generatedPosition.PositionX, generatedPosition.PositionY);
            Console.WriteLine("Your leg has been injured after a fall and you need help back to town.");
            Console.WriteLine("If you move, you hurt yourself (lose HP)");
            Console.WriteLine("You also have a slingshot to defend yourself with. You have a range of 30 metres with it.");
            Console.WriteLine();
            Console.WriteLine("There are entities around you. Who are they?");
            Console.WriteLine();

            //Loop for creating NPC's
            for (int i = 0; i < 5; i++)
            {
                string createdNPCName = Console.ReadLine();
                if (i > 0 && createdNPCName == p.npcList[i-1].NPCname)   //making sure that the name of the npc is unique on the list
                {
                    Console.WriteLine("Please enter a unique name");
                    createdNPCName = Console.ReadLine();
                }
                position NPCPositionGenerator = PositionGenerator();
                NPCtype NPCTypeGenerator = TypeGenerator();//generating a type of enemy (friend or foe)

                npcAllegiance NPCAllegiance = allegianceGenerator(NPCTypeGenerator); // based on enemy type the allegiance status is set to enemy for foes and neutral for friends.
                NPCObject newNPC = new NPCObject(createdNPCName, 1, NPCAllegiance, NPCPositionGenerator, NPCTypeGenerator);
                Console.WriteLine("{0} is at ({1},{2}). They have 1HP. ", createdNPCName.ToUpper(), NPCPositionGenerator.PositionX, NPCPositionGenerator.PositionY);
                Console.WriteLine();
                p.npcList.Add(newNPC);
            }

            //using function to create a win condition after all the enemies are spawned
            int enemyCreatedCount =0, allyCreatedCount =0;
            foreach(NPCObject npc in p.npcList)
            {
                if(npc.typeOfNPC.Type=="Foe")
                {
                    enemyCreatedCount += 1;
                }
                if(npc.typeOfNPC.Type =="Friend")
                {
                    allyCreatedCount += 1;
                }
            }
            WinCondition winningCondition = WinCalculator(enemyCreatedCount, allyCreatedCount);

            //Check the win/lose condition after every action, if not won or lost another action is given
            BeginAction:
            bool Win = WinConditionChecker(winningCondition, enemyKillCount,allyRecruitCount);
            if(Win==true)
            {
                Console.WriteLine("You Win! Your Score is: {0}", p.Player.playerHealth);
                Console.WriteLine("Please relaunch application to play again!");
                goto End;
            }
            bool lose = LoseCondtionChecker(p.Player);
            if (lose == true)
            {
                Console.WriteLine("You Lose! Your Score is: {0}", p.Player.playerHealth);
                Console.WriteLine("Please relaunch application to play again!");
                goto End;
            }
            playerAction();
            goto BeginAction;

            //looping end game
            End:
            goto End;
        }

        //method for generating positions for both NPCs and PCs
        static public position PositionGenerator()
        {
            Random rnd = new Random();

            position newPosition = new position();

            newPosition.PositionX = rnd.Next(1, 100);
            newPosition.PositionY = rnd.Next(1, 100);

            return newPosition;

        }

        //method for generatiing the type of NPC: Friend or Foe
        static public NPCtype TypeGenerator()
        {
            Random rnd = new Random();
            NPCtype friendEnemy = new NPCtype();
            int selector = rnd.Next(0, 99);
            if (selector < 50)
            {
                friendEnemy.Type = "Foe";
            }
            if (selector >= 50)
            {
                friendEnemy.Type = "Friend";
            }

            return friendEnemy;

        }

        //generating allegeiance status
        public static npcAllegiance allegianceGenerator(NPCtype friendFoe)
        {
            
            npcAllegiance status = new npcAllegiance();
            status.allegiance = "Unknown Allegiance";
            return status;
        }

        //Player Action Method
        public static void playerAction()
        {
       
            Console.WriteLine("What do I want to do?");
            Console.WriteLine();
            Console.WriteLine("1) Move to a new location 2) Shoot an NPC, 3) Check my status, 4) Check NPC status, 5)Talk to an NPC, 6) Request alliance with an NPC");
            Console.WriteLine();


            string answer = Console.ReadLine();
            if (answer.ToLower() == "1")
            {
                position NewPlayerPosition = Mover();
                p.Player.PlayerPosition = NewPlayerPosition;
                
            }

            else if (answer.ToLower() == "2")
            {
                ShootNPC();
                
            }

            else if (answer.ToLower() == "3")
            {
                CheckPlayerStatus();
                
            }

            else if (answer.ToLower() == "4")
            {
                CheckNPCStatus();
                
            }

            else if( answer.ToLower() == "5")
            {
                QueryNPC();
            }

            else if (answer.ToLower() == "6")
            {
                RequestAlliance();
            }

            else
            {
                Console.WriteLine("Please type instruction option.");
                Console.WriteLine();
            }
        }

        //Method for Moving the player
        static public position Mover()
        {
            position newPosition = new position();
            Console.WriteLine();
            Console.WriteLine("Where do you want to go horizontally?");
            int h;
            int v;


        //Xposition
        WrongInputX:
            Console.WriteLine();
            string playerChosenPosX = Console.ReadLine();
            Console.WriteLine();
            if (String.IsNullOrEmpty(playerChosenPosX))
            {
                Console.WriteLine("Please give a number for where to move to.");
                goto WrongInputX;
            }
            else if (int.TryParse(playerChosenPosX, out h))
            {
                if (h <= 100 && h >= 1)
                {
                    newPosition.PositionX = h;
                }
                else
                {
                    Console.WriteLine("Give a number between 1 and 100");
                    goto WrongInputX;
                }
            }
            else if(playerChosenPosX.ToLower() == "cancel")
            {
                Console.WriteLine("Action Cancelled");
                Console.WriteLine();
                newPosition.PositionX = p.Player.PlayerPosition.PositionX;
                newPosition.PositionY = p.Player.PlayerPosition.PositionY;
                goto End;
            }
            else
            {
                Console.WriteLine("Please give a number for where to move to");
                goto WrongInputX;
            }

            Console.WriteLine();
            Console.WriteLine("Where do you want to go vertically?");
            Console.WriteLine();
        //Yposition
        WrongInputY:
            string playerChosenPosY = Console.ReadLine();
            Console.WriteLine();
            if (String.IsNullOrEmpty(playerChosenPosY))
            {
                Console.WriteLine("Please give a number for where to move to.");
                goto WrongInputY;
            }
            else if (int.TryParse(playerChosenPosY, out v))
            {
                if (v <= 100 && v >= 1)
                {
                    newPosition.PositionY = v;
                    
                }
                else
                {
                    Console.WriteLine("Give a number between 1 and 100");
                    goto WrongInputY;
                }
            }
            else if (playerChosenPosY.ToLower() == "cancel")
            {
                Console.WriteLine("Action Cancelled");
                Console.WriteLine();
                newPosition.PositionX = p.Player.PlayerPosition.PositionX;
                newPosition.PositionY = p.Player.PlayerPosition.PositionY;
                goto End;
            }
            else
            {
                Console.WriteLine("Please give a number for where to move to.");
                goto WrongInputY;
            }

            Console.WriteLine();
            Console.WriteLine("Your new position is ({0},{1})", newPosition.PositionX, newPosition.PositionY);
            Console.WriteLine();
            p.Player.playerHealth -= 2;
            End:
            return newPosition;
            
            
        }

        //Shooting Method
        public static int ShootNPC()
        {
            int healthLoss = 0;
            Console.WriteLine("Which entity do you want to shoot?");
            string target = Console.ReadLine();
            if(target.ToLower() == "cancel")
            {
                Console.WriteLine("Action cancelled");
                goto End;
            }
            foreach (NPCObject npc in p.npcList)
            {

                if (target == npc.NPCname)
                {
                    
                    if (MathF.Abs(npc.NPCposition.PositionX - p.Player.PlayerPosition.PositionX) < 30 && MathF.Abs(npc.NPCposition.PositionY - p.Player.PlayerPosition.PositionY) < 30&& npc.NPCHealth>0)
                    {
                        
                        if (npc.typeOfNPC.Type == "Friend")
                        {
                            Console.WriteLine("You just shot a friend! You lose HP as punishment");
                            p.Player.playerHealth -= 2;
                            p.Player.playerAmmo -= 1; 
                            Console.WriteLine();
                        }
                        if(npc.typeOfNPC.Type== "Foe")
                        {
                            Console.WriteLine("Target shot");
                            healthLoss = -1;
                            enemyKillCount += 1;
                            Console.WriteLine(enemyKillCount);
                            p.Player.playerAmmo -= 1;
                            npc.NPCHealth += healthLoss; //taking away the health from the enemy
                            Console.WriteLine();
                        }

                    }
                    else if (MathF.Abs(npc.NPCposition.PositionX - p.Player.PlayerPosition.PositionX) > 30 || MathF.Abs(npc.NPCposition.PositionY - p.Player.PlayerPosition.PositionY) > 30)
                    {
                        Console.WriteLine("Target is too far away!");
                        Console.WriteLine();
                    } 

                    else if(npc.NPCHealth<1)
                    {
                        Console.WriteLine("The target is already dead, don't waste ammo!");
                        Console.WriteLine();
                    }
                    

                }
                
            }
            End:
            return healthLoss;
        }

        //Player Status Method
        public static void CheckPlayerStatus()
        {
            PlayerStatus playerStats = new PlayerStatus();
            playerStats.playerName = p.Player.nameP;
            playerStats.playerHealth = p.Player.playerHealth;
            playerStats.playerAmmo = p.Player.playerAmmo;
            playerStats.pos = p.Player.PlayerPosition;
            Console.WriteLine("{0} has: {1}HP, {2} slingshot ammo, and is at ({3},{4})", playerStats.playerName.ToUpper(), playerStats.playerHealth,
                                                    playerStats.playerAmmo, playerStats.pos.PositionX, playerStats.pos.PositionY);

        }

        //NPC Status Method
        public static void CheckNPCStatus()
        {
            foreach (NPCObject npc in p.npcList)
            {
                npcStatus npcStats = new npcStatus();
                npcStats.npcName = npc.NPCname;
                npcStats.NPChealth = npc.NPCHealth;
                npcStats.npcPos = npc.NPCposition;
                npcStats.allegiance = npc.Allystatus;
                npcStats.disposition = npc.friendFoe;

                Console.WriteLine("{0} is at ({1},{2}). They have {3}HP and are of a{4} disposition. They are ({5},{6}) metres away from the player. " + npcStats.allegiance.allegiance, npcStats.npcName.ToUpper(),
                    npcStats.npcPos.PositionX, npcStats.npcPos.PositionY, npcStats.NPChealth, npcStats.disposition, MathF.Abs(npc.NPCposition.PositionX-p.Player.PlayerPosition.PositionX), MathF.Abs(npc.NPCposition.PositionY - p.Player.PlayerPosition.PositionY));
            }
        }

        //NPC Query
        public static void QueryNPC()
        {
            Console.WriteLine("Which entity do you want to talk to?");
            Console.WriteLine();
            string target = Console.ReadLine();
            Console.WriteLine();

            
            foreach (NPCObject npc in p.npcList)
            {
                if(target== npc.NPCname && npc.NPCHealth>0)
                {
                    if(npc.typeOfNPC.Type=="Friend")
                    {
                        Console.WriteLine("{0} : 'Hello friend, I am a travelling merchant. Do you know the way back to town?'", npc.NPCname.ToUpper());
                        Console.WriteLine("This person is of good character, perhaps they could be my ally");
                        Console.WriteLine();

                        if (npc.friendFoe != " friendly"&& npc.Allystatus.allegiance!= "This person is my Ally")
                        {
                            npc.friendFoe = " friendly";
                            npc.Allystatus.allegiance = "Neutral";
                        }

                    }

                    if(npc.typeOfNPC.Type == "Foe")
                    {
                        if (npc.Allystatus.allegiance != "Enemy")
                        {
                            Console.WriteLine("{0} : 'Hey there, you have some nice boots. Would you like me to show you back to town?'", npc.NPCname.ToUpper());
                            Console.WriteLine("This person is of a strange ilk, mayhaps a bandit in disguise.");
                            Console.WriteLine();
                            npc.friendFoe = " strange";
                        }
                        if (npc.Allystatus.allegiance == "Enemy")
                        {
                            Console.WriteLine("{0} : 'ARGH I'M GONNA KILL AND ROB YOU!'", npc.NPCname.ToUpper());
                            Console.WriteLine("Oh no! This person is a damnable bandit!");
                            Console.WriteLine();
                            npc.friendFoe = "n angry";
                        }
                    }
                }
                else if(npc.NPCHealth<=0)
                {
                    Console.WriteLine("You can't talk to the dead.");
                }
                if (target.ToLower() == "cancel")
                {
                    Console.WriteLine("Action cancelled");
                    return;
                }
                
            }
            
        }

        //NPC Aliiance Request
        public static void RequestAlliance()
        {
            Console.WriteLine("Which NPC do you want to team up with?");
            Console.WriteLine();
            string target = Console.ReadLine();

            foreach(NPCObject npc in p.npcList)
            {
                if(target == npc.NPCname)
                {
                    if(npc.typeOfNPC.Type=="Friend")
                    {
                        if (npc.Allystatus.allegiance == "Ally")
                        {
                            Console.WriteLine("This person is already your ally.");
                        }

                        if (npc.Allystatus.allegiance=="Neutral")
                        {
                            npc.Allystatus.allegiance = "Ally";
                            Console.WriteLine("{0} is now your ally.", npc.NPCname.ToUpper());
                            allyRecruitCount += 1;
                        }
                        
                        if(npc.Allystatus.allegiance!="Ally"&& npc.Allystatus.allegiance != "Neutral")
                        {
                            Console.WriteLine("Perhaps I should know this person before I ask them to become my ally.");
                        }
                    }

                    if(npc.typeOfNPC.Type =="Foe")
                    {
                        Console.WriteLine("Ouch! This person wants to hurt me. They are not my ally");
                        npc.Allystatus.allegiance = "Enemy";
                        npc.friendFoe = "n angry";
                        p.Player.playerHealth -= 2;
                    }

                }
                if (target.ToLower() == "cancel")
                {
                    Console.WriteLine("Action cancelled");
                    return;
                }
            }


        }

        //calculating necessary win condition
        public static WinCondition WinCalculator(int enemyAdded, int allyAdded)
        {
            WinCondition goal;
            goal.deadEnemies = enemyAdded;
            goal.recruits = allyAdded;
            return goal;
        }
        
        //Win Condition Checker
        public static bool WinConditionChecker(WinCondition requirements, int enemies, int allies)
        {
            bool winLose ;
            if(enemyKillCount >= requirements.deadEnemies && allyRecruitCount >= requirements.recruits)
            {
                Console.WriteLine("You win!");
                winLose = true;
            }
            else
            {
                winLose = false;
            }
            return winLose;
        }

        //Lose Condition Checker
        public static bool LoseCondtionChecker(PlayerObject playerHealth)
        {
            bool lose =false;
            if(playerHealth.playerHealth>0)
            {
                lose = false;
            }
            else if(playerHealth.playerHealth<=0)
            {
                lose = true;
            }
            foreach(NPCObject npc in p.npcList)
            {
                if( npc.typeOfNPC.Type =="Friend")
                {
                    if(npc.NPCHealth<=0)
                    {
                        lose = true;
                    }
                }
            }
            return lose;
        }
    }

    //Character Objects
    //describing the player object
    public class PlayerObject
    {
        public int playerHealth, playerAmmo;
        public position PlayerPosition;
        public string nameP;
        public PlayerObject(string name, int Health, int Ammo, position playerPos) //player object constructor
        {
            nameP = name;
            playerHealth = Health;
            playerAmmo = Ammo;
            PlayerPosition = playerPos;
        }
    }

    //describing the NPC object
    public class NPCObject
    {
        public string NPCname, friendFoe;
        public position NPCposition;
        public int NPCHealth;
        public NPCtype typeOfNPC;
        public npcAllegiance Allystatus;

        public NPCObject(string name, int Health, npcAllegiance Status, position NPCPos, NPCtype npcType, string defaultAnswer = "n unknown" ) //npc object constructor
        {
            NPCname = name;
            NPCHealth = Health;
            Allystatus = Status;
            NPCposition = NPCPos;
            typeOfNPC = npcType;
            friendFoe = defaultAnswer;
        }

    }
}
