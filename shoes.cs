
function ShoeMod_removeShoes(%obj, %cl)
{
	if (isObject(%obj.rShoe))
	{
		%obj.rShoe.delete();
	}
	if (isObject(%obj.lShoe))
	{
		%obj.lShoe.delete();
	}
	if (isObject(%obj.dummyBot))
	{
		%obj.dummyBot.delete();
	}
}

function ShoeMod_wearShoes(%obj, %shoeName, %cl)
{
	%safeShoeName = getSafeVariableName(%shoeName);
	%scriptObj = getShoeScriptObject(%shoeName);

	%ldb = %scriptObj.lShoeDB;
	%rdb = %scriptObj.rShoeDB;

	if (!isObject(%ldb) || !isObject(%rdb))
	{
		error("ShoeMod_wearShoes: ERROR - Shoe '" @ %shoeName @ "' is missing shoe datablocks!");
		return 0;
	}

	if (isObject(%obj.rShoe))
	{
		%obj.rShoe.delete();
	}
	if (isObject(%obj.lShoe))
	{
		%obj.lShoe.delete();
	}
	if (!isObject(%obj.dummyBot))
	{
		%dummyBot = new AIPlayer(Shoes) {dataBlock = %ldb;};
		%dummyBot.kill();
		%dummyBot.setScale(%obj.getScale());
		%dummyBot.hideNode("ALL");
		%obj.mountObject(%dummyBot, 2);
		%obj.dummyBot = %dummyBot;
	}

	%rShoe = new AIPlayer(Shoes) {dataBlock = %rdb;};
	%lShoe = new AIPlayer(Shoes) {dataBlock = %ldb;};

	%rShoe.setScale(%obj.getScale());
	%lShoe.setScale(%obj.getScale());

	%rShoe.kill();
	%lShoe.kill();

	applyShoeColors(%rShoe, %cl);
	applyShoeColors(%lShoe, %cl);

	%obj.mountObject(%rShoe, 3);
	%obj.mountObject(%lShoe, 4);

	if (%cl.rLeg == 0)
		%obj.rShoe = %rShoe;
	else
		%rShoe.delete();

	if (%cl.lLeg == 0)
		%obj.lShoe = %lShoe;
	else
		%lShoe.delete();

	return 1;
}