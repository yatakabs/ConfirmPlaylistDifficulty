﻿using System.Linq;
using ConfirmPlaylistDifficulty.Configuration;
using HarmonyLib;
using HMUI;
using UnityEngine;

/// <summary>
/// See https://github.com/pardeike/Harmony/wiki for a full reference on Harmony.
/// </summary>
namespace ConfirmPlaylistDifficulty.HarmonyPatches
{
    /// <summary>
    /// This patches ClassToPatch.MethodToPatch(Parameter1Type arg1, Parameter2Type arg2)
    /// </summary>
    [HarmonyPatch(typeof(StandardLevelDetailView), nameof(StandardLevelDetailView.RefreshContent))]
    public class Patches
    {
        /// <summary>
        /// This code is run after the original code in MethodToPatch is run.
        /// </summary>
        /// <param name="__instance">The instance of ClassToPatch</param>
        /// <param name="arg1">The Parameter1Type arg1 that was passed to MethodToPatch</param>
        /// <param name="____privateFieldInClassToPatch">Reference to the private field in ClassToPatch named '_privateFieldInClassToPatch', 
        ///     added three _ to the beginning to reference it in the patch. Adding ref means we can change it.</param>
        private static void Postfix(IBeatmapLevel ____level, IDifficultyBeatmap ____selectedDifficultyBeatmap, LevelParamsPanel ____levelParamsPanel, StandardLevelDetailView __instance)
        {
            var actionButton = DataModel._actionButton = __instance.actionButton;

            DataModel.difficultyBeatmap = ____selectedDifficultyBeatmap;
            DataModel.selectedLevelCategory = Resources.FindObjectsOfTypeAll<SelectLevelCategoryViewController>()
                .FirstOrDefault()
                ?.selectedLevelCategory
                ?? SelectLevelCategoryViewController.LevelCategory.None;

            if (actionButton?.gameObject == null)
            {
                Plugin.Log.Error("Failed to obtain actionButton.");
                return;
            }


            if (DataModel.defaultColor == default)
            {
                DataModel.defaultColor = actionButton
                        .GetComponentsInChildren<ImageView>()
                        .SingleOrDefault(x => x.name == "BG")
                        ?.color
                        ?? default;
            }

            if (PluginConfig.Instance.Enable)
            {
                DataModel.RefreshButtonColor();
            }
        }
    }
}