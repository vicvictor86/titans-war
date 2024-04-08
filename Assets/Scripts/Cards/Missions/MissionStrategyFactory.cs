using Domain;
using static Assets.Scripts.Cards.Missions.MissionCardStrategies;

namespace Assets.Scripts.Cards.Missions
{
    public class MissionStrategyFactory
    {
        private static MissionCardStrategy ConquestRiver = new ConquestTerritory(TerrainType.RIVER);
        private static MissionCardStrategy ConquestMountains = new ConquestTerritory(TerrainType.MOUNTAINS);
        private static MissionCardStrategy ConquestPlains = new ConquestTerritory(TerrainType.PLAINS);
        private static MissionCardStrategy ConquestDesert = new ConquestTerritory(TerrainType.DESERT);
        private static MissionCardStrategy ConquestAtenas = new ConquestCity("Atenas");
        private static MissionCardStrategy ConquestPlateia = new ConquestCity("Plateia");
        private static MissionCardStrategy ConquestMicenas = new ConquestCity("Micenas");
        private static MissionCardStrategy ConquestEfeso = new ConquestCity("Efeso");
        private static MissionCardStrategy ConquestTwoCities = new ConquestNumberOfCities(2);
        private static MissionCardStrategy ConquestThreeCities = new ConquestNumberOfCities(3);
        private static MissionCardStrategy ConquestFourCities = new ConquestNumberOfCities(4);
        private static MissionCardStrategy ConquestThreeTerritoryPerCity = new ConquestNumberOfTerritoriesPerCity(3);

        public MissionCardStrategy GetMissionCardStrategy(MissionType missionType)
        {
            return missionType switch
            {
                MissionType.River => ConquestRiver,
                MissionType.Mountain => ConquestMountains,
                MissionType.Plains => ConquestPlains,
                MissionType.Desert => ConquestDesert,
                MissionType.Atenas => ConquestAtenas,
                MissionType.Plateia => ConquestPlateia,
                MissionType.Micenas => ConquestMicenas,
                MissionType.Efeso => ConquestEfeso,
                MissionType.TwoCities => ConquestTwoCities,
                MissionType.ThreeCities => ConquestThreeCities,
                MissionType.FourCities => ConquestFourCities,
                MissionType.ThreeTerritoriesPerCity => ConquestThreeTerritoryPerCity,
                _ => null
            };
        }
    }
}
