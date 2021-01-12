exec("./lib/Support_CenterprintMenuSystem.cs");
exec("./lib/datablockCreation.cs");
exec("./lib/disableMountSound.cs");
// exec("./lib/jerseyCompatibility.cs");

exec("./util/colorUtils.cs");
exec("./util/getLastStrPos.cs");
exec("./util/hexToInt.cs");
exec("./util/objectVariables.cs");
exec("./util/strContains.cs");

exec("./register.cs");
exec("./preferences.cs");
exec("./shoes.cs");
exec("./owning.cs");
exec("./customization.cs");
exec("./interface.cs");

//development
// function serverCmdReExecShoeMod(%cl)
// {
// 	if (%cl.isSuperAdmin)
// 	{
// 		setmodpaths(getmodpaths());
// 		exec("Add-ons/Server_ShoeMod/server.cs");
// 	}
// }

function serverCmdShoeHelp(%cl)
{
	messageClient(%cl, '', "\c5Shoe Commands:");
	messageClient(%cl, '', "\c3/shoes [shoe name]\c6 - leave blank to open shoe list");
	messageClient(%cl, '', "\c3/setShoeNodeColor [node] [color - hex or R, G, B]");
	messageClient(%cl, '', "\c3/giveShoes [clientName] [shoe name]");
	messageClient(%cl, '', "\c3/clearShoes \c6 - clears all owned shoes");
	if (%cl.isAdmin)
	{
		messageClient(%cl, '', "\c4Admin-only:");
		messageClient(%cl, '', "\c4/clearShoes [client name]");
		messageClient(%cl, '', "\c4/grantShoes [clientName] [shoe name]");
		messageClient(%cl, '', "\c4/listShoes");
	}
}

function serverCmdShoesHelp(%cl)
{
	serverCmdShoeHelp(%cl);
}