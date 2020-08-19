#region Copyright & License Information
/*
 * Copyright 2019-2020 The OpenHV Developers (see CREDITS)
 * This file is part of OpenHV, which is free software. It is made
 * available to you under the terms of the GNU General Public License
 * as published by the Free Software Foundation, either version 3 of
 * the License, or (at your option) any later version. For more
 * information, see COPYING.
 */
#endregion

using System;
using OpenRA.Graphics;
using OpenRA.Mods.Common.Widgets;
using OpenRA.Widgets;

namespace OpenRA.Mods.HV.Widgets
{
	public class SpritePowerMeterWidget : Widget
	{
		[Desc("The name of the Container Widget to tie the Y axis to.")]
		[FieldLoader.Require]
		public readonly string MeterAlongside = "";

		[Desc("The name of the Container with the items to get the height from.")]
		[FieldLoader.Require]
		public readonly string ParentContainer = "";

		[FieldLoader.Require]
		public readonly int BarStride = 4;

		[FieldLoader.Require]
		public readonly int PowerUnitsPerBar = 25;

		[FieldLoader.Require]
		public readonly string ImageCollection = "";

		[FieldLoader.Require]
		public readonly string NoPowerImage = "";

		[FieldLoader.Require]
		public readonly string AvailablePowerImage = "";

		[FieldLoader.Require]
		public readonly string UsedPowerImage = "";

		[FieldLoader.Require]
		public readonly string OverUsedPowerImage = "";

		[FieldLoader.Require]
		public readonly string FlashPowerImage = "";

		[FieldLoader.Require]
		public readonly int WarningFlashDuration = 10;

		[FieldLoader.Require]
		public readonly int WarningFlashBlinkRate = 80;

		public int NumberOfBars;

		public int WarningFlash;
		public int TotalPowerDisplay;
		public int LastTotalPowerDisplay;

		public float TotalPowerStep;
		public float PowerUsedStep;
		public float PowerAvailableStep;

		public bool LowPower;

		[ObjectCreator.UseCtor]
		public SpritePowerMeterWidget() { }

		public override void Draw()
		{
			NumberOfBars = Bounds.Height / BarStride;

			// Cache it here, because DPI changes can happen
			var noPowerImage = ChromeProvider.GetImage(ImageCollection, NoPowerImage);
			var availablePowerImage = ChromeProvider.GetImage(ImageCollection, AvailablePowerImage);
			var usedPowerImage = ChromeProvider.GetImage(ImageCollection, UsedPowerImage);
			var overusedPowerImage = ChromeProvider.GetImage(ImageCollection, OverUsedPowerImage);
			var flashPowerImage = ChromeProvider.GetImage(ImageCollection, FlashPowerImage);

			// Create a list of new bars
			for (var i = 0; i < NumberOfBars; i++)
			{
				var image = noPowerImage;

				var targetIcon = availablePowerImage;

				if (i < PowerUsedStep)
					targetIcon = usedPowerImage;

				if (i > PowerAvailableStep)
					targetIcon = overusedPowerImage;

				if (i == TotalPowerStep && LowPower)
					targetIcon = overusedPowerImage;

				// Flash the top bar if something is wrong
				if (i == TotalPowerStep)
				{
					if (WarningFlash % WarningFlashBlinkRate != 0)
						targetIcon = flashPowerImage;
					if (WarningFlash > 0)
						WarningFlash--;
				}

				if (image != targetIcon)
					image = targetIcon;

				var bounds = new int2(Bounds.X, -(i * BarStride) + Bounds.Height + Bounds.Y);
				WidgetUtils.DrawRGBA(image, bounds);

				Bounds.Width = image.Bounds.Width;
			}
		}
	}
}
