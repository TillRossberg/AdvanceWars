using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMode : MonoBehaviour
{
    //References
    private Manager _manager;
    //Fields
    List<float> coverRatings = new List<float>() { 0.9f, 0.85f,0.8f,0.7f,0.5f}; 

    public void init()
    {
		_manager = GameObject.FindGameObjectWithTag ("LevelManager").GetComponent<Manager>();
    }

    //Lets two units battle!! The fun part :)
    public void fight(Unit attacker, Unit defender)
    {
        //Get the tiles the Units are standing on.
        Tile attackerTile = _manager.getMapCreator().getTile(attacker.xPos, attacker.yPos);
        Tile defenderTile = _manager.getMapCreator().getTile(defender.xPos, defender.yPos);
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
            if(attacker.health < 0)
            {
                defender.myTeam.incUnitsKilledCount();
            }
        }
        else
        {
            attacker.myTeam.incUnitsKilledCount();
        }
    }

    //Calculate the damage inflicted, based on the attacker, defender, the chosen General(will be added later!) and the cover of the tiles they stand on.
    public int calcDamage(Unit attacker, Unit defender, Tile defendingTile)
    {
        float Damage;//Damage that will be inflicted.
        int BaseDamage = _manager.getDatabase().getBaseDamage(attacker.myUnitType, defender.myUnitType);//Base damage of the unit. (Depends on the unit it fights against.)
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
}
