function strContainsWord(%str, %word, %offset)
{
	%offset = getMax(%offset, 0);
	for (%i = %offset; %i < getWordCount(%str); %i++)
	{
		%curr = getWord(%str, %i);
		if (%word $= %curr)
		{
			return 1 SPC %i;
		}
	}
	return 0;
}

function strContainsField(%str, %field, %offset)
{
	%offset = getMax(%offset, 0);
	for (%i = %offset; %i < getFieldCount(%str); %i++)
	{
		%curr = getField(%str, %i);
		if (%field $= %curr)
		{
			return 1 SPC %i;
		}
	}
	return 0;
}