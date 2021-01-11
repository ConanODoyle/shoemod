package ShoeMod_Interface
{
	function GameConnection::displayCenterprintMenu(%cl, %diff)
	{
		%ret = parent::displayCenterprintMenu(%cl, %diff);

		%shoeName = %cl.centerprintMenu.menuOption[%cl.currOption];
		%cl.wearShoes(%shoeName);
		return %ret;
	}

	function GameConnection::exitCenterprintMenu(%cl)
	{
		%cl.wearShoes(%cl.getSavedShoes());
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
	if ($Pref::Server::ShoeMod::ForceRandom)
	{
		messageClient(%cl, '', "\c2Force random shoes is enabled. You cannot remove or change your shoes.");
		%shoeName = "Random";
		%cl.setCurrentShoes(%shoeName);
		return; //do not want to allow people to `/shoe random` continuously to get the shoes they want
	}
	else if ((%registered = isRegisteredShoe(%shoeName)) && !%cl.ownsShoe(%shoeName) && !$Pref::Server::ShoeMod::ShoeAccess)
	{
		messageClient(%cl, '', "\c2You do not own a pair of these shoes!");
		%shoeName = ""; //sends the player to shoe menu to select a pair
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

function openShoeMenu(%cl)
{
	%menu = getCenterprintShoeMenu(%cl);

	%shoe = %cl.getCurrentShoes();

	%cl.startCenterprintMenu(%menu);
	%cl.displayCenterprintMenu(%menu.menuOptionIDX[getSafeVariableName(%shoe)]);
}

function getCenterprintShoeMenu(%cl)
{
	if (!isObject(%cl.shoeMenu))
	{
		%cl.shoeMenu = new ScriptObject(ShoeMenu)
		{
			isCenterprintMenu = 1;

			menuOption[0] = "None";
			menuFunction[0] = "confirmShoe";
			menuOption[1] = "Random";
			menuFunction[1] = "confirmShoe";
			menuOptionCount = 2;
		};
	}

	if (%cl.shoeMenu.lastValidatedTime > ((getSimTime() - 200 | 0) | 0))
	{
		return %cl.shoeMenu;
	}
	
	for (%i = 1; %i < %cl.shoeMenu.menuOptionCount; %i++)
	{
		%cl.shoeMenu.menuOption[%i] = "";
	}

	for (%i = 0; %i < $ShoeSet.getCount(); %i++)
	{
		%cl.shoeMenu.menuOption[%i + 2] = $ShoeSet.getObject(%i).shoeName;
		%cl.shoeMenu.menuFunction[%i + 2] = "confirmShoe";
		%cl.shoeMenu.menuOptionIDX[getSafeVariableName($ShoeSet.getObject(%i).shoeName)] = %i + 2;
	}
	%cl.shoeMenu.menuName = "-Enter to confirm- <br>\c5Use /setShoeNodeColor to recolor shoes";
	%cl.shoeMenu.menuOptionCount = %i + 2;
	%cl.shoeMenu.lastValidatedTime = getSimTime() | 0s;

	return %cl.shoeMenu;
}

function confirmShoe(%cl, %menu, %option)
{
	serverCmdShoe(%cl, %menu.menuOption[%option]);
}