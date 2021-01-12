function GameConnection::giveRandomShoe(%cl, %count, %duplicateAllowed)
{
	if (%count $= "")
	{
		%count = 1;
	}
	else if (%count > 20)
	{
		%count = 20;
	}
	else if (%count <= 0)
	{
		return 0;
	}

	for (%i = 0; %i < $ShoeSet.getCount(); %i++)
	{
		%set = %set SPC %i;
	}
	%set = trim(%set);

	while (getWordCount(%set) > 0)
	{
		%idx = getRandom(getWordCount(%set) - 1);
		%select = getWord(%set, %idx);
		%set = removeWord(%set, %idx);

		%shoeName = $ShoeSet.getObject(%select).shoeName;
		if (%cl.ownsShoe(%shoeName) && !%duplicateAllowed)
		{
			continue;
		}
		else
		{
			%cl.addOwnedShoe(%shoeName);
			%cl.recentRandomShoes = trim(%cl.recentRandomShoes TAB getShoeScriptObject(%shoeName).shoeName);
			cancel(%cl.clearRecentRandomShoeSchedule);
			%cl.clearRecentRandomShoeSchedule = %cl.schedule(1000, clearRecentRandomShoes);
			%added = 1;
			break;
		}
	}

	return %cl.giveRandomShoe(%count - 1, %duplicateAllowed) + %added;
}

function GameConnection::clearRecentRandomShoes(%cl)
{
	%cl.recentRandomShoes = "";
}

function GameConnection::addOwnedShoe(%cl, %shoeName, %count)
{
	if (!isObject(%cl.shoeSettings) || !isRegisteredShoe(%shoeName))
	{
		return 0;
	}

	%count = getMax(%count, 1);

	for (%i = 0; %i < %cl.shoeSettings.ownedShoeListCount; %i++)
	{
		%select = %cl.shoeSettings.ownedShoeList[%i];
		%listShoeName = getField(%select, 0);
		%shoeCount = getField(%select, 1);

		if (%listShoeName $= %shoeName)
		{
			%cl.shoeSettings.ownedShoeList[%i] = %listShoeName TAB (%shoeCount + 1);
			if (%cl.isViewingShoeMenu())
			{
				%cl.refreshShoeMenu();
			}
			return 1;
		}
	}
	//shoe not in list, append
	%cl.shoeSettings.ownedShoeList[%i] = getShoeScriptObject(%shoeName).shoeName TAB 1;
	%cl.shoeSettings.ownedShoeListCount = %i + 1;

	if (%cl.isViewingShoeMenu())
	{
		%cl.refreshShoeMenu();
	}
	return 1;
}

function GameConnection::removeOwnedShoe(%cl, %shoeName, %count)
{
	if (!isObject(%cl.shoeSettings) || !isRegisteredShoe(%shoeName))
	{
		return 0;
	}

	%count = getMax(%count, 1);

	for (%i = 0; %i < %cl.shoeSettings.ownedShoeListCount; %i++)
	{
		%select = %cl.shoeSettings.ownedShoeList[%i];
		%listShoeName = getField(%select, 0);
		%shoeCount = getField(%select, 1);

		if (%listShoeName $= %shoeName)
		{
			%cl.shoeSettings.ownedShoeList[%i] = %listShoeName TAB getMax((%shoeCount - %count), 0);
			if (%cl.isViewingShoeMenu())
			{
				%cl.refreshShoeMenu();
			}
			if (%shoeCount > 0)
			{
				return 1 SPC getMin(%shoeCount, %count);
			}
			else
			{
				return 0;
			}
		}
	}
	//shoe not in list
	if (%cl.isViewingShoeMenu())
	{
		%cl.refreshShoeMenu();
	}
	return 0;
}

function GameConnection::ownsShoe(%cl, %shoeName)
{
	if (!isObject(%cl.shoeSettings))
	{
		return 0;
	}

	for (%i = 0; %i < %cl.shoeSettings.ownedShoeListCount; %i++)
	{
		%select = %cl.shoeSettings.ownedShoeList[%i];
		%listShoeName = getField(%select, 0);
		%shoeCount = getField(%select, 1);

		if (%listShoeName $= %shoeName && %shoeCount > 0)
		{
			return 1;
		}
	}
	return 0;
}






function serverCmdGrantShoes(%cl, %target, %a, %b, %c, %d, %e, %f, %g)
{
	%shoeName = trim(%a SPC %b SPC %c SPC %d SPC %e SPC %f SPC %g);
	
	if (!%cl.isAdmin)
	{
		%errorString = "You need to be admin to use /grantShoes!";
	}
	else if (%shoeName $= "" || %target $= "")
	{
		%errorString = "\c6Usage: \c3/grantShoes [clientName] [shoe name]";
	}
	else if ($Pref::Server::ShoeMod::ShoeAccess)
	{
		%errorString = "\c5Players have access to all shoes already!";
	}
	else if (!isObject(%target = findClientByName(%target)))
	{
		%errorString = "\c5No client found! \c6Usage: \c3/grantShoes [clientName] [shoe name]";
	}
	else if (!isRegisteredShoe(%shoeName))
	{
		%errorString = "\c5Shoe '" @ %shoeName @ "' does not exist! \c6Usage: \c3/grantShoes [clientName] [shoe name]";
	}
	else if (%cl.lastGrantShoesTarget != %target || %cl.repeatTime < $Sim::Time)
	{
		%trueShoeName = getShoeScriptObject(%shoeName).shoeName;
		%errorString = "\c6Are you sure you want to grant \c3" @ %target.name @ "\c6 a pair of \c3'" @ %trueShoeName @ "'\c6 shoes? Repeat the command to confirm.";
		%cl.lastGrantShoesTarget = %target;
		%cl.repeatTime = $Sim::Time + 5;
	}

	if (%errorString !$= "")
	{
		messageClient(%cl, '', %errorString);
		return;
	}

	%cl.lastGrantShoesTarget = "";
	%target.addOwnedShoe(%shoeName, 1);
	%trueShoeName = getShoeScriptObject(%shoeName).shoeName;
	messageClient(%cl, '', "\c6You have granted \c3" @ %target.name @ " a pair of '" @ %trueShoeName @ "' shoes!");
	messageClient(%target, '', "\c3" @ %cl.name @ "\c6 has granted you a pair of '" @ %trueShoeName @ "' shoes!");
}

function serverCmdGrantShoe(%cl, %target, %a, %b, %c, %d, %e, %f, %g)
{
	serverCmdGrantShoes(%cl, %target, %a, %b, %c, %d, %e, %f, %g);
}

function serverCmdGiveShoes(%cl, %target, %a, %b, %c, %d, %e, %f, %g)
{
	%shoeName = trim(%a SPC %b SPC %c SPC %d SPC %e SPC %f SPC %g);
	
	if (!$Pref::Server::ShoeMod::ShoeSharing)
	{
		%errorString = "\c5/giveShoes is disabled. You cannot give shoes to other players.";
	}
	else if (%shoeName $= "" || %target $= "")
	{
		%errorString = "\c6Usage: \c3/giveShoes [clientName] [shoe name]";
	}
	else if ($Pref::Server::ShoeMod::ShoeAccess)
	{
		%errorString = "\c5Players have access to all shoes already!";
	}
	else if ((%target = findClientByName(%target)) == %cl)
	{
		%errorString = "\c5You cannot give yourself your own shoes!";
	}
	else if (!isObject(%target))
	{
		%errorString = "\c5No client found! \c6Usage: \c3/giveShoes [clientName] [shoe name]";
	}
	else if (!isRegisteredShoe(%shoeName))
	{
		%errorString = "\c5Shoe '" @ %shoeName @ "' does not exist! \c6Usage: \c3/giveShoes [clientName] [shoe name]";
	}
	else if (!%cl.ownsShoe(%shoeName))
	{
		%errorString = "\c5You do not own a pair of these shoes!";
	}
	else if (%cl.lastGiveShoesTarget != %target || %cl.repeatTime < $Sim::Time)
	{
		%trueShoeName = getShoeScriptObject(%shoeName).shoeName;
		%errorString = "\c6Are you sure you want to give \c3" @ %target.name @ "\c6 a pair of \c3'" @ %trueShoeName @ "'\c6 shoes? Repeat the command to confirm.";
		%cl.lastGiveShoesTarget = %target;
		%cl.repeatTime = $Sim::Time + 5;
	}

	if (%errorString !$= "")
	{
		messageClient(%cl, '', %errorString);
		return;
	}

	%cl.lastGiveShoesTarget = "";
	%cl.removeOwnedShoe(%shoeName, 1);
	%target.addOwnedShoe(%shoeName, 1);
	%trueShoeName = getShoeScriptObject(%shoeName).shoeName;
	messageClient(%cl, '', "\c6You have given \c3" @ %target.name @ " a pair of '" @ %trueShoeName @ "' shoes!");
	messageClient(%target, '', "\c3" @ %cl.name @ "\c6 has given you a pair of '" @ %trueShoeName @ "' shoes!");
}

function serverCmdGiveShoe(%cl, %target, %a, %b, %c, %d, %e, %f, %g)
{
	serverCmdGiveShoes(%cl, %target, %a, %b, %c, %d, %e, %f, %g);
}

function serverCmdClearShoes(%cl, %a, %b, %c, %d, %e, %f, %g)
{	
	%target = trim(%a SPC %b SPC %c SPC %d SPC %e SPC %f SPC %g);
	
	if (!$Pref::Server::ShoeMod::ClearShoes)
	{
		%errorString = "\c5/clearShoes is disabled. You cannot give shoes to other players.";
	}
	else if (%target !$= %cl.name && %target !$= "" && !%cl.isAdmin)
	{
		%errorString = "You need to be admin to clear other people's owned shoes!";
	}
	else if (%target $= "")
	{
		if (!%cl.isAdmin)
		{
			return serverCmdClearShoes(%cl, %cl.name);
		}
		else
		{
			%errorString = "\c6Usage: \c3/clearShoes [client name]";
		}
	}
	else if ($Pref::Server::ShoeMod::ShoeAccess)
	{
		%errorString = "\c5Players have access to all shoes already!";
	}
	else if (!isObject(%target = findClientByName(%target)))
	{
		%errorString = "\c5No client found! \c6Usage: \c3/clearShoes [client name]";
	}
	else if (%cl.lastClearShoesTarget != %target || %cl.repeatTime < $Sim::Time)
	{
		if (%target != %cl)
		{
			%errorString = "\c6Are you sure you want to clear all of \c3" @ %target.name @ "\c6's owned shoes? Repeat the command to confirm.";
		}
		else
		{
			%errorString = "\c6Are you sure you want to clear all your owned shoes? Repeat the command to confirm.";

		}
		%cl.lastClearShoesTarget = %target;
		%cl.repeatTime = $Sim::Time + 5;
	}

	if (%errorString !$= "")
	{
		messageClient(%cl, '', %errorString);
		return;
	}

	%cl.lastClearShoesTarget = "";
	%target.shoeSettings.ownedShoeListCount = 0;
	%target.setCurrentShoes("None");
	%target.unwearShoes();

	messageClient(%cl, '', "\c6You have cleared \c3" @ %target.name @ "\c6's owned shoes.");
	if (%target != %cl)
	{
		messageClient(%target, '', "\c3" @ %cl.name @ "\c6 has cleared your owned shoes.");
	}
	if (%target.isViewingShoeMenu())
	{
		%target.refreshShoeMenu();
	}
}

function serverCmdClearShoe(%cl)
{
	serverCmdClearShoes(%cl);
}