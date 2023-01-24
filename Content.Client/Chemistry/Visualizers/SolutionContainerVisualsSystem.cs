﻿using Content.Shared.Chemistry;
using Robust.Client.GameObjects;

namespace Content.Client.Chemistry.Visualizers;

public sealed class SolutionContainerVisualsSystem : VisualizerSystem<SolutionContainerVisualsComponent>
{
    protected override void OnAppearanceChange(EntityUid uid, SolutionContainerVisualsComponent component, ref AppearanceChangeEvent args)
    {
        if (!args.Component.TryGetData(SolutionContainerVisuals.VisualState, out SolutionContainerVisualState state))
            return;

        if (args.Sprite == null)
            return;

        if (!args.Sprite.LayerMapTryGet(component.Layer, out var fillLayer))
            return;

<<<<<<< HEAD
        var fillPercent = state.FilledVolumePercent;
        var closestFillSprite = (int) Math.Round(fillPercent * component.MaxFillLevels);
=======
        // Currently some solution methods such as overflowing will try to update appearance with a
        // volume greater than the max volume. We'll clamp it so players don't see
        // a giant error sign and error for debug.
        if (fraction > 1f)
        {
            Logger.Error("Attempted to set solution container visuals volume ratio on " + ToPrettyString(uid) + " to a value greater than 1. Volume should never be greater than max volume!");
            fraction = 1f;
        }

        var closestFillSprite = (int) Math.Round(fraction * component.MaxFillLevels);
>>>>>>> fb3df9665 (log error when trying to set invalid solution ratio and clamp it (#13675))

        if (closestFillSprite > 0)
        {
            if (component.FillBaseName == null)
                return;

            args.Sprite.LayerSetVisible(fillLayer, true);

            var stateName = component.FillBaseName + closestFillSprite;
            args.Sprite.LayerSetState(fillLayer, stateName);

            if (component.ChangeColor)
                args.Sprite.LayerSetColor(fillLayer, state.Color);
        }
        else
        {
            if (component.EmptySpriteName == null)
                args.Sprite.LayerSetVisible(fillLayer, false);
            else
            {
                args.Sprite.LayerSetState(fillLayer, component.EmptySpriteName);
                if (component.ChangeColor)
                    args.Sprite.LayerSetColor(fillLayer, component.EmptySpriteColor);
            }
        }
    }
}
