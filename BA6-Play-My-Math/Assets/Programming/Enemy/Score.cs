using Programming.Enemy;
using Programming.Fraction_Engine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class keeps track of the score. It's saved as a static variable.
/// </summary>
public class Score : MonoBehaviour
{
    float difficultyWeight = 2f;
    float numberOfOperationsWeight = -0.5f;
    float numberOfCardsDrawnWeight = -0.2f;
    
    /// <summary>
    /// The entire score of the current playthrough. When starting a new playthrough call resetScore().
    /// Do not change this variable! Do it via the functions in the class.
    /// </summary>
    static int score = 0;
    
    /// <summary>
    /// Resets the score to zero. Save the score, before you reset it. Use getScore() get the current score.
    /// </summary>
    public static void resetScore() {  score = 0; }
    
    /// <summary>
    /// Returns the score as an int.
    /// </summary>
    /// <returns></returns>
    public static int getScore() {  return score; }

    /// <summary>
    /// Adds a value to the score, based on the monster provided. Call addMonsterToScore(justDefeatedMonster)
    /// Returns the value that has been added to the score. Use getScore() get the updated total score.
    /// </summary>
    public static int addFractionToScore(Fraction fraction) 
    {
        // This function requires a monster type where I can save which difficulty a fraction has. Currently that is not in the game.
        // So instead I will just add a static value buuuhuuu
        int valueToAdd = fraction.difficulty;
        
        score += valueToAdd;
        Debug.Log("Score: " + score.ToString());

        return valueToAdd;
    }

    /// <summary>
    /// Adds an exact value to the score. Use getScore() get the updated total score.
    /// </summary>
    public static void addValueToScore(int valueToAdd)
    {
        score += valueToAdd;
    }

    public class Enemy
    {
        public Fraction fraction;
        public int difficultyClass;

        public Enemy(Fraction fraction, int difficultyClass)
        {
            this.fraction = fraction;
            this.difficultyClass = difficultyClass;
        }
    }
}
