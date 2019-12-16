namespace GameplayEvents
{
    public struct Answered
    {
        public readonly bool Correct;

        public Answered(bool correct) => Correct = correct;
    } 

    public struct TableReady
    {

    }

    public struct LiveLost
    {

    }

    public struct DeathTimePassed
    {

    }

    public struct GameOver
    {

    }
}