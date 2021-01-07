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
		%cl.wearShoes(%cl.getSavedShoes())
		return parent::exitCenterprintMenu(%cl);
	}
};
activatePackage(ShoeMod_Interface);

function serverCmdShoe(%cl, %a, %b, %c, %d, %e, %f, %g)
{
	%shoeName = trim(%a SPC %b SPC %c SPC %d SPC %e SPC %f SPC %g);
	if (!isRegisteredShoe(%shoeName) && %shoeName !$= "None")
	{
		openShoeMenu(%cl);
	}
	else
	{
		%cl.setCurrentShoes(%shoeName);
		%cl.wearShoes(%shoeName);
	}
}

function openShoeMenu(%cl)
{
	%menu = getCenterprintShoeMenu();


}

function getCenterprintShoeMenu()
{
	if (!isObject($ShoeSet.centerprintMenu))
	{
		$ShoeSet.centerprintMenu = new ScriptObject(ShoeMenu)
		{
			isCenterprintMenu = 1;
			menuName = "-Enter to confirm-";
		};
	}

	if ($ShoeSet.centerprintMenu.lastValidatedCount == $ShoeSet.getCount())
	{
		return $ShoeSet.centerprintMenu;
	}
	
	for (%i = 0; %i < $ShoeSet.centerprintMenu.menuOptionCount; %i++)
	{
		%ShoeSet.centerprintMenu.menuOption[%i] = "";
	}

	for (%i = 0; %i < $ShoeSet.getCount(); %i++)
	{
		$ShoeSet.centerprintMenu.menuOption[%i] = $ShoeSet.getObject(%i).shoeName;
		$ShoeSet.centerprintMenu.menuFunction[%i] = "confirmShoe";
	}
	$ShoeSet.centerprintMenu.lastValidatedCount = %i;
}

function confirmShoe(%cl, %menu, %option)
{
	serverCmdShoe(%cl, %menu.menuOption[%option]);
}