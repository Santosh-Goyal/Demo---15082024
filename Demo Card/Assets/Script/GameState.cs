using System;
using System.Collections.Generic;

[Serializable]
public class CardData
{
    public int cardID;
    public int positionIndex;
    public bool isFaceUp;
    public bool isMatched;
}

[Serializable]
public class GameState
{
    public int score;
    public int highScore;
    public int matchedPairs;
    public List<CardData> cards;
}