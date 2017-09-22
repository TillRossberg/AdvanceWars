using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMode : MonoBehaviour
{
    List<float> coverRatings = new List<float>(); //Holds the damage reduction values: 0 = 10%, 1 = 20%, 2 = 30%, 3 = 40%, 4 = 50% ...

    // Use this for initialization
    void Start ()
    {
        initCoverList();
	}
	
    public void findAttackableTiles()
    {
        this.GetComponent<MainFunctions>().activateFireMode();
        this.GetComponent<MainFunctions>().selectedUnit.findAttackableTiles();
        this.GetComponent<Graph>().resetReachableTiles();
    }

	public void testBattle()
    {
        Unit attacker = this.GetComponent<UnitManager>().teamRed[0].GetComponent<Unit>();
        Unit defender = this.GetComponent<UnitManager>().teamBlue[0].GetComponent<Unit>();
        fight(attacker, defender);
    }
    //Lets two units battle!! The fun part :)
    public void fight(Unit attacker, Unit defender)
    {
        //Get the tiles the Units are standing on.
        Tile attackerTile = this.GetComponent<Graph>().getTile(attacker.xPos, attacker.yPos);
        Tile defenderTile = this.GetComponent<Graph>().getTile(defender.xPos, defender.yPos);
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

    //Calc the damage inflicted, based on the attacker, defender, the chosen General(will be added later!) and the cover of the tiles they stand on.
    public int calcDamage(Unit attacker, Unit defender, Tile defendingTile)
    {
        float Damage;//Damage that will be inflicted.
        int BaseDamage = getBaseDamage(attacker.myUnitType, defender.myUnitType);//Base damage of the unit. (Depends on the unit it fights against.)
        float dmgModifierAttacker = 100; // Attacking CO attack value.(Will vary later ^^)
        int RandomNumber = Random.Range(0, 10); //Random number to between 0-9 to vary the damage.
        int AttackerHP = attacker.health; //Attacker HP
        float dmgModifierDefender = 100; // Defending CO attack value.(Will vary later ^^)
        float dmgReductionCover = coverRatings[defendingTile.cover]; //Defending terrain stars.    
        float DefenderHp = defender.health; //HP of the defender.

        Debug.Log("Damage = (BaseDamage:" + BaseDamage + " + RandomNumber:" + RandomNumber + ") * AttackerHp/100:" + AttackerHP / 100 + " * dmgReductionCover:" + dmgReductionCover);
        return (int)(Damage = (BaseDamage + RandomNumber) * AttackerHP / 100 * dmgReductionCover);
    }

    //Returns the base damage of the attacking unit against the defending unit.
    public int getBaseDamage(Unit.unitType attackingUnit, Unit.unitType defendingUnit)
    {
        switch(attackingUnit)
        {
            case Unit.unitType.Infantry:
                switch(defendingUnit)
                {
                    case Unit.unitType.Infantry: return 55;
                    case Unit.unitType.Mech: return 45;

                    default:
                    Debug.Log("Invalid unit type for defending unit!");
                        return -1;
                }

            case Unit.unitType.Tank:
                switch (defendingUnit)
                {
                    case Unit.unitType.Infantry: return 75;
                    case Unit.unitType.Mech: return 70;
                    case Unit.unitType.Recon: return 85;
                    case Unit.unitType.Tank: return 55;
                    case Unit.unitType.MdTank: return 15;
                    case Unit.unitType.Neotank: return 15;
                    case Unit.unitType.APC: return 75;
                    case Unit.unitType.Artillery: return 70;
                    case Unit.unitType.Rockets: return 85;
                    case Unit.unitType.AntiAir: return 65;
                    case Unit.unitType.Missiles: return 85;
                    case Unit.unitType.BCopter: return 10;
                    case Unit.unitType.TCopter: return 40;
                    case Unit.unitType.Fighter: return -1;
                    case Unit.unitType.Bomber: return -1;
                    case Unit.unitType.Battleship: return 1;
                    case Unit.unitType.Lander: return 10;
                    case Unit.unitType.Cruiser: return 5;
                    case Unit.unitType.Sub: return 1;
                    case Unit.unitType.Pipe: return 15;

                    default:
                        Debug.Log("Invalid unit type for defending unit!");
                        return -1;
                }

            case Unit.unitType.Rockets:
                switch (defendingUnit)
                {
                    case Unit.unitType.Infantry: return 95;
                    case Unit.unitType.Mech: return 90;
                    case Unit.unitType.Recon: return 90;
                    case Unit.unitType.Tank: return 80;
                    case Unit.unitType.MdTank: return 55;
                    case Unit.unitType.Neotank: return 50;
                    case Unit.unitType.APC: return 80;
                    case Unit.unitType.Artillery: return 80;
                    case Unit.unitType.Rockets: return 85;
                    case Unit.unitType.AntiAir: return 85;
                    case Unit.unitType.Missiles: return 90;
                    case Unit.unitType.BCopter: return -1;
                    case Unit.unitType.TCopter: return -1;
                    case Unit.unitType.Battleship: return 55;
                    case Unit.unitType.Lander: return 60;
                    case Unit.unitType.Cruiser: return 85;
                    case Unit.unitType.Sub: return 85;
                    case Unit.unitType.Pipe: return 55;

                    default:
                        Debug.Log("Invalid unit type for defending unit!");
                        return -1;
                }

            default:
                Debug.Log("Invalid unit type for attacking unit!");
                return -1;
        }
    }
    
    private void initCoverList()
    {
        coverRatings.Add(0.9f);
        coverRatings.Add(0.85f);
        coverRatings.Add(0.8f);
        coverRatings.Add(0.7f);
        coverRatings.Add(0.5f);       
    }
}
