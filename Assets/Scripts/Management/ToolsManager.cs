using System;
using System.Collections.Generic;
using UnityEngine;
using GreenNacho.Common;

namespace MetaMonster
{
    public enum ToolType
    {
        Die,
        Roulette,
        Timer,
        Count
    }

    public class ToolsManager : MonoBehaviourSingleton<ToolsManager>
    {
        [Header("Screen References")]
        [SerializeField] MainScreen mainScreen = default;

        [Header("Tools' Settings")]
        [SerializeField] Color[] rouletteColors = default;

        public Color[] RouletteColors { get { return rouletteColors; } }

        List<uint> runningToolIDs = new List<uint>();
        uint lastAssignedID = 0;

        public bool IsToolRunning(uint toolID)
        {
            return runningToolIDs.Contains(toolID);
        }

        public void AddRunningTool(uint toolID)
        {
            runningToolIDs.Add(toolID);
        }

        public void RemoveRunningTool(uint toolID)
        {
            runningToolIDs.Remove(toolID);
        }

        public void AddDie(int faceCount, int buttonIndex)
        {
            lastAssignedID++;
            mainScreen.AddDie(faceCount, buttonIndex, lastAssignedID);
        }

        public void AddTimer(TimeSpan timeSpan, int buttonIndex)
        {
            lastAssignedID++;
            mainScreen.AddTimer(timeSpan, buttonIndex, lastAssignedID);
        }

        public void AddRoulette(int[] colorAmounts, int colorsAdded, int buttonIndex)
        {
            lastAssignedID++;
            mainScreen.AddRoulette(colorAmounts, colorsAdded, buttonIndex, lastAssignedID);
        }
    }
}