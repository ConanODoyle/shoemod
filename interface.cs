package ShoeMod_Interface
{
	function GameConnection::displayCenterprintMenu(%cl, %diff)
	{
		%ret = parent::displayCenterprintMenu(%cl, %diff);

		%shoeName = %cl.centerprintMenu.shoe[%cl.currOption];
		if (%shoeName $= "Random")
		{
			randomShoeLoop(%cl);
		}
		else
		{
			stopRandomShoeLoop(%cl);
			%cl.wearShoes(%shoeName);
		}
		return %ret;
	}

	function GameConnection::exitCenterprintMenu(%cl)
	{
		if ($Pref::Server::ShoeMod::ShoeAccess || %cl.ownsShoe(%cl.getSavedShoes()))
		{
			return parent::exitCenterprintMenu(%cl);
		}

		if (!$Pref::Server::ShoeMod::ShoeAccess && !%cl.ownsShoe(%cl.getSavedShoes()))
		{
			%cl.wearShoes("None");
		}
		else
		{
			%cl.wearShoes(%cl.getSavedShoes());
		}
		stopRandomShoeLoop(%cl);
		return parent::exitCenterprintMenu(%cl);
	}

	function GameConnection::onRemove(%cl)
	{
		if (isObject(%cl.shoeMenu))
		{
			%cl.shoeMenu.delete();
		}
		return parent::onRemove(%cl);
	}
};
activatePackage(ShoeMod_Interface);

function serverCmdShoe(%cl, %a, %b, %c, %d, %e, %f, %g)
{
	%shoeName = trim(%a SPC %b SPC %c SPC %d SPC %e SPC %f SPC %g);

	//prefs checks
	if (!$Pref::Server::ShoeMod::EnableCommand)
	{
		messageClient(%cl, '', "\c5/shoe is disabled. You cannot wear or remove shoes manually.");
		return;
	}
	if ($Pref::Server::ShoeMod::ForceRandom)
	{
		messageClient(%cl, '', "\c5Force random shoes is enabled. You cannot remove or change your shoes.");
		return; //do not want to allow people to `/shoe random` continuously after spawning to get the shoes they want
	}
	else if ((%registered = isRegisteredShoe(%shoeName)) && !%cl.ownsShoe(%shoeName) && !$Pref::Server::ShoeMod::ShoeAccess)
	{
		messageClient(%cl, '', "\c5You do not own a pair of these shoes!");
		return;
	}


	if (%shoeName !$= "None" && %shoeName !$= "Random" && !isRegisteredShoe(%shoeName))
	{
		openShoeMenu(%cl);
	}
	else
	{
		%cl.setCurrentShoes(%shoeName);
		%cl.wearShoes(%shoeName);
	}
}

function serverCmdShoes(%cl, %a, %b, %c, %d, %e, %f, %g)
{
	serverCmdShoe(%cl, %a, %b, %c, %d, %e, %f, %g);
}

function randomShoeLoop(%cl)
{
	cancel(%cl.randomShoeSchedule);
	
	%cl.wearShoes("Random");

	%cl.randomShoeSchedule = schedule(300, %cl, randomShoeLoop, %cl);
}

function stopRandomShoeLoop(%cl)
{
	cancel(%cl.randomShoeSchedule);
}

function openShoeMenu(%cl)
{
	%menu = getCenterprintShoeMenu(%cl);

	%shoe = %cl.getCurrentShoes();

	%cl.startCenterprintMenu(%menu);
	%cl.displayCenterprintMenu(%menu.menuOptionIDX[getSafeVariableName(%shoe)]);
}

function GameConnection::isViewingShoeMenu(%cl)
{
	return isObject(%cl.centerprintMenu) && %cl.centerprintMenu.isShoeMenu;
}

function GameConnection::refreshShoeMenu(%cl)
{
	%cl.shoeMenu.lastValidatedTime = 0;
	getCenterprintShoeMenu(%cl);
	%cl.displayCenterprintMenu(0);
}

function getCenterprintShoeMenu(%cl)
{
	if (!isObject(%cl.shoeMenu))
	{
		%cl.shoeMenu = new ScriptObject(ShoeMenu)
		{
			isCenterprintMenu = 1;

			isShoeMenu = 1;

			menuOption[0] = "None";
			menuFunction[0] = "confirmShoe";
			shoe[0] = "None";
			menuOption[1] = "Random";
			menuFunction[1] = "confirmShoe";
			shoe[1] = "Random";
			menuOptionCount = 2;
		};
	}

	if (%cl.shoeMenu.lastValidatedTime > ((getSimTime() - 200 | 0) | 0))
	{
		return %cl.shoeMenu;
	}
	
	for (%i = 2; %i < %cl.shoeMenu.menuOptionCount; %i++)
	{
		%cl.shoeMenu.menuOption[%i] = "";
	}

	if ($Pref::Server::ShoeMod::ShoeAccess)
	{
		for (%i = 0; %i < $ShoeSet.getCount(); %i++)
		{
			%cl.shoeMenu.menuOption[%i + 2] = $ShoeSet.getObject(%i).shoeName;
			%cl.shoeMenu.shoe[%i + 2] = $ShoeSet.getObject(%i).shoeName;
			%cl.shoeMenu.menuFunction[%i + 2] = "confirmShoe";
			%cl.shoeMenu.menuOptionIDX[getSafeVariableName($ShoeSet.getObject(%i).shoeName)] = %i + 2;
		}
	}
	else
	{
		%settings = %cl.shoeSettings;
		for (%i = 0; %i < %cl.shoeSettings.ownedShoeListCount; %i++)
		{
			%shoeName = getField(%settings.ownedShoeList[%i], 0);
			%count = getField(%settings.ownedShoeList[%i], 1);

			if (%count > 0)
			{
				%cl.shoeMenu.menuOption[%currSlot + 2] = %shoeName @ " (" @ %count @ ")";
				%cl.shoeMenu.shoe[%currSlot + 2] = %shoeName;
				%cl.shoeMenu.menuFunction[%currSlot + 2] = "confirmShoe";
				%cl.shoeMenu.menuOptionIDX[getSafeVariableName(%shoeName)] = %currSlot + 2;
				%currSlot++;
			}
		}
		%i = %currSlot;
	}
	%cl.shoeMenu.menuName = "-Enter to confirm- <br>\c5Use /setShoeNodeColor to recolor shoes";
	%cl.shoeMenu.menuOptionCount = %i + 2;
	%cl.shoeMenu.lastValidatedTime = getSimTime() | 0;

	return %cl.shoeMenu;
}

function confirmShoe(%cl, %menu, %option)
{
	serverCmdShoe(%cl, %menu.shoe[%option]);
}