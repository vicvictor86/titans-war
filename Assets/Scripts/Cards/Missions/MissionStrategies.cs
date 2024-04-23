using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Cards.Missions
{
    public static class MissionCardStrategies
    {
        public class ConquestTerritory : MissionCardStrategy
        {
            public TerrainType TerrainType { get; set; }
            public ConquestTerritory(TerrainType type)
            {
                TerrainType = type;
            }

            public bool IsComplete(PlayerDeck player)
            {
                return ValidTerritoryTypeMission(player, TerrainType);
            }
        }

        public class ConquestCity : MissionCardStrategy
        {
            public string CityName { get; set; }

            public ConquestCity(string cityName)
            {
                CityName = cityName;
            }

            public bool IsComplete(PlayerDeck player)
            {
                return GetCityByName(CityName)
                    .Owner == player;
            }
        }

        public class ConquestNumberOfCities : MissionCardStrategy
        {
            public int NumberOfCities { get;}

            public ConquestNumberOfCities(int numberOfCities)
            {
                NumberOfCities = numberOfCities;
            }

            public bool IsComplete(PlayerDeck player)
            {
                return ValidNumberOfCities(player, NumberOfCities);
            }
        }

        public class ConquestNumberOfTerritoriesPerCity : MissionCardStrategy
        {
            public int NumberOfTerritoriesPerCity { get; set; }

            public ConquestNumberOfTerritoriesPerCity(int numberOfTerritoriesPerCity)
            {
                NumberOfTerritoriesPerCity = numberOfTerritoriesPerCity;
            }

            public bool IsComplete(PlayerDeck player)
            {
                return ValidNumberOfTerritoriesPerCity(player, NumberOfTerritoriesPerCity);
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

        private static bool ValidNumberOfCities(PlayerDeck player, int numberOfCities)
        {
            return GetAllCities().Where(city => city.Owner == player).Count() >= numberOfCities;
        }

        private static bool ValidNumberOfTerritoriesPerCity(PlayerDeck player, int number)
        {
            return GameObject.FindGameObjectsWithTag("Territory")
                    .Select(gameObject => gameObject.GetComponent<Territory>())
                    .GroupBy(territory => territory.City)
                    .All(group => group.Where(territory => territory.Owner == player).Count() >= number);
        }

    }
}
