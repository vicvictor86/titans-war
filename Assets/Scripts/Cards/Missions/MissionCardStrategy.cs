namespace Assets.Scripts.Cards.Missions
{
    public interface MissionCardStrategy
    {
        bool IsComplete(PlayerDeck player);
    }
}
