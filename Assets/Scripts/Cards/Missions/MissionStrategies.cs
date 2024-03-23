using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Cards.Missions
{
    public static class MissionCardStrategies
    {
        public class ConquestRiver : MissionCardStrategy
        {
            public bool IsComplete(PlayerDeck player)
            {
                return ValidTerritoryTypeMission(player, TerrainType.RIVER);
            }
        }

        public class ConquestMountains : MissionCardStrategy
        {
            public bool IsComplete(PlayerDeck player)
            {
                return ValidTerritoryTypeMission(player, TerrainType.MOUNTAINS);
            }
        }

        public class ConquestPlains : MissionCardStrategy
        {
            public bool IsComplete(PlayerDeck player)
            {
                return ValidTerritoryTypeMission(player, TerrainType.PLAINS);
            }
        }

        public class ConquestDesert : MissionCardStrategy
        {
            public bool IsComplete(PlayerDeck player)
            {
                return ValidTerritoryTypeMission(player, TerrainType.DESERT);
            }
        }

        public class ConquestEspartha : MissionCardStrategy
        {
            public bool IsComplete(PlayerDeck player)
            {
                return GetCityByName("Espartha")
                    .Owner == player;
            }
        }

        public class Salamina : MissionCardStrategy
        {
            public bool IsComplete(PlayerDeck player)
            {
                return GetCityByName("Salamina")
                    .Owner == player;
            }
        }

        private static IEnumerable<Territory> GetAllTerritoryByTerrainType(TerrainType terrainType)
        {
            return GameObject.FindGameObjectsWithTag("Territory")
                    .Select(gameObject => gameObject.GetComponent<Territory>())
                    .Where(territory => territory.Type == terrainType);
        }

        private static bool ValidTerritoryTypeMission(PlayerDeck player, TerrainType terrainType)
        {
            var allTerrain = GetAllTerritoryByTerrainType(terrainType);

            return allTerrain.Where(territory => territory.Owner == player).Count() >= (allTerrain.Count() / 2) + 1;
        }

        private static City GetCityByName(string cityName)
        {
            return GameObject.FindGameObjectsWithTag("City")
                .Select(gameObject => gameObject.GetComponent<City>())
                .FirstOrDefault(city => city.name == cityName);
        }

        private static IEnumerable<City> GetAllCities()
        {
            return GameObject.FindGameObjectsWithTag("City")
                .Select(gameObject => gameObject.GetComponent<City>());
        }

    }
}
