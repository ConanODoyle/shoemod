if (isFunction(RTB_registerPref))
{
	RTB_registerPref("Enable /shoes",				"Shoe Mod",	"$Pref::Server::ShoeMod::EnableCommand",	"bool",			"Server_ShoeMod",	1,	0,	0);
	RTB_registerPref("Access to all Shoes",			"Shoe Mod",	"$Pref::Server::ShoeMod::ShoeAccess",		"bool",			"Server_ShoeMod",	1,	0,	0);
	RTB_registerPref("Force wear random shoes",		"Shoe Mod",	"$Pref::Server::ShoeMod::ForceRandom",		"bool",			"Server_ShoeMod",	0,	0,	0,	"ShoeMod_ForceRandom");
	RTB_registerPref("/giveShoes command",			"Shoe Mod",	"$Pref::Server::ShoeMod::ShoeSharing",		"bool",			"Server_ShoeMod",	1,	0,	0);
	RTB_registerPref("/clearShoes command",			"Shoe Mod",	"$Pref::Server::ShoeMod::ClearShoes",		"bool",			"Server_ShoeMod",	1,	0,	0);
	RTB_registerPref("Enable random shoe drops",	"Shoe Mod",	"$Pref::Server::ShoeMod::ShoeDrops",		"bool",			"Server_ShoeMod",	0,	0,	0,	"ShoeMod_ShoeDropLoop");
	RTB_registerPref("Allow duplicate shoe drops",	"Shoe Mod",	"$Pref::Server::ShoeMod::DuplicateShoes",	"bool",			"Server_ShoeMod",	1,	0,	0);
	RTB_registerPref("Announce shoe drops",			"Shoe Mod",	"$Pref::Server::ShoeMod::AnnounceDrops",	"bool",			"Server_ShoeMod",	1,	0,	0);
	RTB_registerPref("Chance to get Shoe (%)",		"Shoe Mod",	"$Pref::Server::ShoeMod::ShoeChance",		"int 0 100",	"Server_ShoeMod",	5,	0,	0);
	RTB_registerPref("Minutes between ticks",		"Shoe Mod",	"$Pref::Server::ShoeMod::ShoeTickTime",		"int 1 60",		"Server_ShoeMod",	10,	0,	0,	"ShoeMod_ShoeDropLoop");
}
else
{
	if ($Pref::Server::ShoeMod::EnableCommand	$= "")	$Pref::Server::ShoeMod::EnableCommand = 1;
	if ($Pref::Server::ShoeMod::ShoeAccess		$= "")	$Pref::Server::ShoeMod::ShoeAccess = 1;
	if ($Pref::Server::ShoeMod::ForceRandom		$= "")	$Pref::Server::ShoeMod::ForceRandom = 0;
	if ($Pref::Server::ShoeMod::ShoeSharing		$= "")	$Pref::Server::ShoeMod::ShoeSharing = 1;
	if ($Pref::Server::ShoeMod::ClearShoes		$= "")	$Pref::Server::ShoeMod::ClearShoes = 1;
	if ($Pref::Server::ShoeMod::ShoeDrops		$= "")	$Pref::Server::ShoeMod::ShoeDrops = 0;
	if ($Pref::Server::ShoeMod::DuplicateShoes	$= "")	$Pref::Server::ShoeMod::DuplicateShoes = 1;
	if ($Pref::Server::ShoeMod::AnnounceDrops	$= "")	$Pref::Server::ShoeMod::AnnounceDrops = 1;
	if ($Pref::Server::ShoeMod::ShoeChance		$= "")	$Pref::Server::ShoeMod::ShoeChance = 5;
	if ($Pref::Server::ShoeMod::ShoeTickTime	$= "")	$Pref::Server::ShoeMod::ShoeTickTime = 10;
}

function ShoeMod_ForceRandom()
{
	if (!$Pref::Server::ShoeMod::ForceRandom)
	{
		return;
	}

	%count = ClientGroup.getCount();
	for (%i = 0; %i < %count; %i++)
	{
		ClientGroup.getObject(%i).wearShoes("random");
	}
}

function ShoeMod_ShoeDropLoop()
{
	cancel($ShoeDropSchedule);
	if (!$Pref::Server::ShoeMod::ShoeDrops)
	{
		return;
	}

	for (%i = 0; %i < ClientGroup.getCount(); %i++)
	{
		if (getRandom() < $Pref::Server::ShoeMod::ShoeChance / 100)
		{
			%cl = ClientGroup.getObject(%i);
			%cl.giveRandomShoe(1, $Pref::Server::ShoeMod::DuplicateShoes);
			%last = getField(%cl.recentRandomShoes, getFieldCount(%Cl.recentRandomShoes) - 1);
			if ($Pref::Server::ShoeMod::AnnounceDrops)
			{
				messageAll('', "\c3" @ %cl.name @ "\c6 found a pair of \c3" @ %last @ "\c6 shoes!");
			}
			else
			{
				messageAll('', "\c6You found a pair of \c3" @ %last @ "\c6 shoes!");
			}
		}
	}

	$ShoeDropSchedule = schedule($Pref::Server::ShoeMod::ShoeTickTime * 60 * 1000, MissionCleanup, ShoeMod_ShoeDropLoop);
}

schedule(1000, 0, ShoeMod_ShoeDropLoop);