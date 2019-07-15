using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calculations_Battle
{
    List<float> coverRatings = new List<float>() { 0.9f, 0.85f, 0.8f, 0.7f, 0.5f };

    //Lets two units battle!! The fun part :)
    public void Fight(Unit attacker, Unit defender)
    {
        //Get the tiles the Units are standing on.
        Tile attackerTile = Core.Model.GetTile(attacker.Position);
        Tile defenderTile = Core.Model.GetTile(defender.Position);
        //1. the attacker shoots.
        int attackerDamage = CalcDamage(attacker, defender, defenderTile);
        Debug.Log("Attacker: " + attacker.name + " damage: " + attackerDamage);
        attacker.CanFire = false;
        //2. The defender loses health.
        defender.SubtractHealth(attackerDamage);
        //The defender only shoots back, if he is still alive and both units are direct attack units.
        if (defender.health > 0 && (attacker.data.directAttack && defender.data.directAttack))
        {
            //3. The defender shoots.
            int defenderDamage = CalcDamage(defender, attacker, attackerTile);
            Debug.Log("Defender: " + defender.name + " damage: " + defenderDamage);
            //4. The attacker loses health.
            attacker.SubtractHealth(defenderDamage);
            if (attacker.health < 0)
            {
                defender.team.Data.IncUnitsKilledCount();
            }
        }
        else
        {
            attacker.team.Data.IncUnitsKilledCount();
        }
    }
    
    //Calculate the damage inflicted, based on the attacker, defender, the chosen General(will be added later!) and the cover of the tiles they stand on.
    public int CalcDamage(Unit attacker, Unit defender, Tile defendingTile)
    {
        float Damage;//Damage that will be inflicted.
        float BaseDamage = attacker.data.GetDamageAgainst(defender.data.type);//Base damage of the unit. (Depends on the unit it fights against.)
        if(BaseDamage > 0)
        {
            float dmgModifierAttacker = 1; // Attacking CO attack value.(Will vary later ^^)
            int RandomNumber = Random.Range(0, 10); //Random number between 0-9 to vary the damage.
            int AttackerHP = attacker.health; //Attacker HP
            float dmgModifierDefender = 1; // Defending CO attack value.(Will vary later ^^)
            float dmgReductionCover = coverRatings[defendingTile.data.cover]; //Defending terrain stars.    
            float DefenderHp = defender.health; //HP of the defender.

            Debug.Log("Damage = (BaseDamage:" + BaseDamage + " + RandomNumber:" + RandomNumber + ") * AttackerHp/100:" + AttackerHP / 100 + " * dmgReductionCover:" + dmgReductionCover);
            return (int)(Damage = (BaseDamage + RandomNumber) * AttackerHP / 100 * dmgReductionCover * dmgModifierAttacker * dmgModifierDefender);
        }
        else
        {
            Debug.Log("BattleMode: Invalid base damage, i.e. can't attack this unit!");
            return 0;
        }
    }    
}
