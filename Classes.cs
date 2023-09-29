using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleships_WPF
{
    public class Classes
    {
        class Match
        {
            List<int> players;
            int winner, turnid, playerBoats, enemyBoats;
            public Match(int winner, int turnid, int playerBoats, int enemyBoats)
            {
                this.players = new List<int>();
                this.winner = winner;
                this.turnid = turnid;
                this.playerBoats = playerBoats;
                this.enemyBoats = enemyBoats;
            }
            public int checkturn()
            {
                return turnid;
            }
            public int checkBoatAmount(Player player)
            {
                return player.AliveBoats(); 
            }
            public void AddPlayer(Player player)
            {
                players.Add(player.playerID);
            }
        }

        public class Player
        {
            public int playerID, PlayerBoats;
            public List<Boat> boats;
            public Player(int playerID, int playerBoats, List<Boat> boats)
            {
                this.playerID = playerID;
                PlayerBoats = playerBoats;
                this.boats = boats;
            }
            public int AliveBoats()
            {
                int count = 0;
                foreach(Boat b in boats)
                {
                    if(b.destroyed == false)
                    {
                        count++;
                    }
                }
                return count;
            }

        }

        public class Boat
        {
            int size, damagedParts;
            public bool destroyed;
            public List<BoatParts> parts;
            public Boat(int size, int damagedParts)
            {
                parts = new List<BoatParts>();
                this.size = size;
                this.damagedParts = damagedParts;
                this.destroyed = false;
            }
            public void checkDamage()
            {
                int damage = 0;
                foreach(BoatParts p in parts)
                {
                    if (p.damaged == true)
                    {
                        damage++;
                    }
                }
                damagedParts = damage;
            }
        }

        public class BoatParts
        {
            public int colPos, rowPos;
            public bool damaged;
            public BoatParts(int colPos, int rowPos)
            {
                this.colPos = colPos;
                this.rowPos = rowPos;
                this.damaged = false;
            }
        }

        /*public class Attack
        {
            public int playerID, targetRow, targetCol;
            public string ButtonID;
            public bool attackHit;
            public 
        }*/

        public bool Attack(Player player,int row,int col)
        {
            foreach(Boat b in player.boats)
            {
                foreach(BoatParts parts in b.parts)
                {
                    if (parts.rowPos == row && parts.colPos == col)
                    {
                        parts.damaged = true;
                        return true;
                    }
                }
            }
            return false;
        }



    }
}
