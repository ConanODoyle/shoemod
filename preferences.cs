if (isFunction(RTB_registerPref))
{
	RTB_registerPref("Minutes between ticks",	"Shoe Mod",	"$Pref::Server::ShoeMod::ShoeTickTime",		"int 1 60",		"Server_ShoeMod",	10,			0,			0);
	RTB_registerPref("Chance to get Shoe (%)",	"Shoe Mod",	"$Pref::Server::ShoeMod::ShoeChance",		"int 1 20",		"Server_ShoeMod",	5,			0,			0);
	RTB_registerPref("/giveShoe command",		"Shoe Mod",	"$Pref::Server::ShoeMod::ShoeSharing",		"bool",			"Server_ShoeMod",	1,			0,			0);
	RTB_registerPref("/clearShoes command",		"Shoe Mod",	"$Pref::Server::ShoeMod::ClearShoes",		"bool",			"Server_ShoeMod",	1,			0,			0);
	RTB_registerPref("Enable duplicate Shoes",	"Shoe Mod",	"$Pref::Server::ShoeMod::DuplicateShoes",	"bool",			"Server_ShoeMod",	1,			0,			0);
	RTB_registerPref("Access to all Shoes",		"Shoe Mod",	"$Pref::Server::ShoeMod::ShoeAccess",		"bool",			"Server_ShoeMod",	1,			0,			0);
	RTB_registerPref("Random shoes",			"Shoe Mod",	"$Pref::Server::ShoeMod::RandomShoes",		"bool",			"Server_ShoeMod",	0,			0,			0);
	RTB_registerPref("Force wear random Shoe",	"Shoe Mod",	"$Pref::Server::ShoeMod::ForceRandom",		"bool",			"Server_ShoeMod",	0,			0,			0,			"ShoeMod_ForceRandom");
} else {
	if($Pref::Server::ShoeMod::ShoeTickTime		$= "")	$Pref::Server::ShoeMod::ShoeTickTime = 10;
	if($Pref::Server::ShoeMod::ShoeChance		$= "")	$Pref::Server::ShoeMod::ShoeChance = 5;
	if($Pref::Server::ShoeMod::ShoeSharing		$= "")	$Pref::Server::ShoeMod::ShoeSharing = 1;
	if($Pref::Server::ShoeMod::ClearShoes		$= "")	$Pref::Server::ShoeMod::ClearShoes = 1;
	if($Pref::Server::ShoeMod::DuplicateShoes	$= "")	$Pref::Server::ShoeMod::DuplicateShoes = 1;
	if($Pref::Server::ShoeMod::ShoeAccess		$= "")	$Pref::Server::ShoeMod::ShoeAccess = 1;
	if($Pref::Server::ShoeMod::RandomShoes		$= "")	$Pref::Server::ShoeMod::RandomShoes = 0;
	if($Pref::Server::ShoeMod::ForceRandom		$= "")	$Pref::Server::ShoeMod::ForceRandom = 0;
}

function ShoeMod_ForceRandom() {
	if (!$Pref::Server::ShoeMod::ForceRandom)
		return;

	%count = ClientGroup.getCount();
	for(%i=0; %i < %count; %i++) {
		%client = ClientGroup.getObject(%i);
		serverCmdShoe(%client, "random");
	}
}
