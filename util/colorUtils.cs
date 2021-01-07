function clampRGBColorToPercent(%color)
{
	%r = getWord(%color, 0); %g = getWord(%color, 1); %b = getWord(%color, 2);

	if (%r > 1 || %g > 1 || %b > 1)
	{
		%r /= 255; %g /= 255; %b /= 255;
	}
	%r = getMax(getMin(%r, 1), 0) + 0;
	%g = getMax(getMin(%g, 1), 0) + 0;
	%b = getMax(getMin(%b, 1), 0) + 0;

	return mFloatLength(%r, 2) SPC mFloatLength(%g, 2) SPC mFloatLength(%b, 2);
}

function hexFromRGB(%color)
{
	%color = clampRGBColorToPercent(%color);
	
	%rh = intToHex(mFloor(getWord(%color, 0) * 255 + 0.5));
	%gh = intToHex(mFloor(getWord(%color, 1) * 255 + 0.5));
	%bh = intToHex(mFloor(getWord(%color, 2) * 255 + 0.5));

	if (strLen(%rh) == 1) { %rh = "0" @ %rh; }
	if (strLen(%gh) == 1) { %gh = "0" @ %gh; }
	if (strLen(%bh) == 1) { %bh = "0" @ %bh; }

	return %rh @ %gh @ %bh;
}