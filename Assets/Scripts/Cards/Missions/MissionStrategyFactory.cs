using Domain;
using static Assets.Scripts.Cards.Missions.MissionCardStrategies;

namespace Assets.Scripts.Cards.Missions
{
    public class MissionStrategyFactory
    {
        private static MissionCardStrategy ConquestRiver = new ConquestRiver();
        private static MissionCardStrategy ConquestMountains = new ConquestMountains();
        private static MissionCardStrategy ConquestPlains = new ConquestPlains();
        private static MissionCardStrategy ConquestDesert = new ConquestDesert();

        public MissionCardStrategy GetMissionCardStrategy(MissionType missionType)
        {
            return missionType switch
            {
                MissionType.River => ConquestRiver,
                MissionType.Mountain => ConquestMountains,
                MissionType.Plains => ConquestPlains,
                MissionType.Desert => ConquestDesert,
                _ => null
            };
        }
    }
}
