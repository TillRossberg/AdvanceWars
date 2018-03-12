using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMode : MonoBehaviour
{
    List<float> coverRatings = new List<float>() { 0.9f, 0.85f,0.8f,0.7f,0.5f}; //Holds the damage reduction values: 0 = 10%, 1 = 20%, 2 = 30%, 3 = 40%, 4 = 50% ...

    // Use this for initialization
    void Start ()
    {
        
	}
	
    //Lets two units battle!! The fun part :)
    public void fight(Unit attacker, Unit defender)
    {
        //Get the tiles the Units are standing on.
        Tile attackerTile = this.GetComponent<MapCreator>().getTile(attacker.xPos, attacker.yPos);
        Tile defenderTile = this.GetComponent<MapCreator>().getTile(defender.xPos, defender.yPos);
        //1. the attacker shoots.
        int attackerDamage = calcDamage(attacker, defender, defenderTile);
        Debug.Log("Attacker: " + attacker.name +  " damage: " + attackerDamage);
        attacker.canFire = false;
        //2. The defender loses health.
        defender.subtractHealth(attackerDamage);
        //The defender only shoots back, if he is still alive and both units are direct attack units.
        if(defender.health > 0 && (attacker.directAttack && defender.directAttack))
        {
            //3. The defender shoots.
            int defenderDamage = calcDamage(defender, attacker, attackerTile);
            Debug.Log("Defender: " + defender.name + " damage: " + defenderDamage);
            //4. The attacker loses health.
            attacker.subtractHealth(defenderDamage);
        }
    }

    //Calculate the damage inflicted, based on the attacker, defender, the chosen General(will be added later!) and the cover of the tiles they stand on.
    public int calcDamage(Unit attacker, Unit defender, Tile defendingTile)
    {
        float Damage;//Damage that will be inflicted.
        int BaseDamage = getBaseDamage(attacker.myUnitType, defender.myUnitType);//Base damage of the unit. (Depends on the unit it fights against.)
        if(BaseDamage > 0)
        {
            float dmgModifierAttacker = 100; // Attacking CO attack value.(Will vary later ^^)
            int RandomNumber = Random.Range(0, 10); //Random number between 0-9 to vary the damage.
            int AttackerHP = attacker.health; //Attacker HP
            float dmgModifierDefender = 100; // Defending CO attack value.(Will vary later ^^)
            float dmgReductionCover = coverRatings[defendingTile.cover]; //Defending terrain stars.    
            float DefenderHp = defender.health; //HP of the defender.

            Debug.Log("Damage = (BaseDamage:" + BaseDamage + " + RandomNumber:" + RandomNumber + ") * AttackerHp/100:" + AttackerHP / 100 + " * dmgReductionCover:" + dmgReductionCover);
            return (int)(Damage = (BaseDamage + RandomNumber) * AttackerHP / 100 * dmgReductionCover);
        }
        else
        {
            Debug.Log("BattleMode: Invalid base damage, i.e. can't attack this unit!");
            return 0;
        }
    }

    //Returns the base damage of the attacking unit against the defending unit.
    public int getBaseDamage(Unit.type attackingUnit, Unit.type defendingUnit)
    {
        switch(attackingUnit)
        {
            case Unit.type.Infantry:
                switch(defendingUnit)
                {
                    case Unit.type.Infantry: return 55;
                    case Unit.type.Mech: return 45;

                    default:
                    Debug.Log("BattleMode: Invalid unit type for defending unit!");
                        return -1;
                }

            case Unit.type.Tank:
                switch (defendingUnit)
                {
                    case Unit.type.Infantry: return 75;
                    case Unit.type.Mech: return 70;
                    case Unit.type.Recon: return 85;
                    case Unit.type.Tank: return 55;
                    case Unit.type.MdTank: return 15;
                    case Unit.type.Titantank: return 15;
                    case Unit.type.APC: return 75;
                    case Unit.type.Artillery: return 70;
                    case Unit.type.Rockets: return 85;
                    case Unit.type.Flak: return 65;
                    case Unit.type.Missiles: return 85;
                    case Unit.type.BCopter: return 10;
                    case Unit.type.TCopter: return 40;
                    case Unit.type.Fighter: return -1;
                    case Unit.type.Bomber: return -1;
                    case Unit.type.Battleship: return 1;
                    case Unit.type.Lander: return 10;
                    case Unit.type.Cruiser: return 5;
                    case Unit.type.Sub: return 1;
                    case Unit.type.Pipe: return 15;

                    default:
                        Debug.Log("BattleMode: Invalid unit type for defending unit!");
                        return -1;
                }

            case Unit.type.MdTank:
                switch (defendingUnit)
                {
                    case Unit.type.Infantry: return 105;
                    case Unit.type.Mech: return 95;
                    case Unit.type.Recon: return 105;
                    case Unit.type.Tank: return 85;
                    case Unit.type.MdTank: return 55;
                    case Unit.type.Titantank: return 45;
                    case Unit.type.APC: return 105;
                    case Unit.type.Artillery: return 105;
                    case Unit.type.Rockets: return 105;
                    case Unit.type.Flak: return 105;
                    case Unit.type.Missiles: return 105;
                    case Unit.type.BCopter: return 12;
                    case Unit.type.TCopter: return 45;
                    case Unit.type.Fighter: return -1;
                    case Unit.type.Bomber: return -1;
                    case Unit.type.Battleship: return 10;
                    case Unit.type.Lander: return 35;
                    case Unit.type.Cruiser: return 45;
                    case Unit.type.Sub: return 10;
                    case Unit.type.Pipe: return 55;

                    default:
                        Debug.Log("BattleMode: Invalid unit type for defending unit!");
                        return -1;
                }


            case Unit.type.Rockets:
                switch (defendingUnit)
                {
                    case Unit.type.Infantry: return 95;
                    case Unit.type.Mech: return 90;
                    case Unit.type.Recon: return 90;
                    case Unit.type.Tank: return 80;
                    case Unit.type.MdTank: return 55;
                    case Unit.type.Titantank: return 50;
                    case Unit.type.APC: return 80;
                    case Unit.type.Artillery: return 80;
                    case Unit.type.Rockets: return 85;
                    case Unit.type.Flak: return 85;
                    case Unit.type.Missiles: return 90;
                    case Unit.type.BCopter: return -1;
                    case Unit.type.TCopter: return -1;
                    case Unit.type.Fighter: return -1;
                    case Unit.type.Bomber: return -1;
                    case Unit.type.Battleship: return 55;
                    case Unit.type.Lander: return 60;
                    case Unit.type.Cruiser: return 85;
                    case Unit.type.Sub: return 85;
                    case Unit.type.Pipe: return 55;

                    default:
                        Debug.Log("BattleMode: Invalid unit type for defending unit!");
                        return -1;
                }

            default:
                Debug.Log("BattleMode: Invalid unit type for attacking unit!");
                return -1;
        }
    }    
}
