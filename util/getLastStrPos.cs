function getLastStrPos(%str, %search)
{
	if (%search $= "")
	{
		error("getLastStrPos: ERROR - Cannot search for an empty string!");
		return -1;
	}
	%curr = strPos(%str, %search);
	while ((%next = strPos(%str, %search, %curr + 1)) >= 0)
	{
		%curr = %next;
	}
	return %curr;
}