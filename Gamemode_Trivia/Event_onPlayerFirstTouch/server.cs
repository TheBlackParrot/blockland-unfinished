registerInputEvent("FxDTSBrick","onPlayerFirstTouch","Self fxDTSBrick" TAB "Player Player" TAB "Client GameConnection" TAB "MiniGame MiniGame");

datablock TriggerData(onPlayerFirstTouchTriggerData)
{
	tickPeriodMS = 150;
};

function onPlayerFirstTouchTriggerData::onEnterTrigger(%this, %trigger, %obj)
{
	if(%obj.getClassName() !$= "Player")
	{
		return;
	}

	if(!%trigger.brick.isColliding())
	{
		return;
	}

	$InputTarget_["Self"] = %trigger.brick;
	$InputTarget_["Player"] = %obj;
	$InputTarget_["Client"] = %obj.client;

	if($Server::LAN)
	{
		$InputTarget_["MiniGame"] = getMiniGameFromObject(%obj.client);
	}

	else
	{
		if(getMiniGameFromObject(%this) == getMiniGameFromObject(%obj.client))
		{
			$InputTarget_["MiniGame"] = getMiniGameFromObject(%obj);
		}

		else
		{
			$InputTarget_["MiniGame"] = 0;
		}
	}

	%trigger.brick.processInputEvent("onPlayerFirstTouch", %obj.client);
}

function fxDTSBrick::createOnPlayerFirstTouchTrigger(%this, %data)
{
	//credits to Space Guy for showing how to create triggers

	%t = new Trigger()
	{
		datablock = %data;
		polyhedron = "0 0 0 1 0 0 0 -1 0 0 0 1"; //this determines the shape of the trigger
	};

	missionCleanup.add(%t);
	
	%boxMax = getWords(%this.getWorldBox(), 3, 5);
	%boxMin = getWords(%this.getWorldBox(), 0, 2);
	%boxDiff = vectorSub(%boxMax, %boxMin);
	%boxDiff = vectorAdd(%boxDiff, "0.2 0.2 0.2"); 
	%t.setScale(%boxDiff);
	%posA = %this.getWorldBoxCenter();
	%posB = %t.getWorldBoxCenter();
	%posDiff = vectorSub(%posA, %posB);
	%t.setTransform(%posDiff);
	//All this makes a trigger that's slightly bigger than the brick
	//on all sides

	%this.onPlayerFirstTouchTrigger = %t;
	%t.brick = %this;

	return %t;
}

package onPlayerFirstTouch
{
	function serverCmdAddEvent(%client, %delay, %input, %target, %a, %b, %output, %par1, %par2, %par3, %par4)
	{
		if(%input == inputEvent_GetInputEventIdx("onPlayerFirstTouch") && !isObject(%brick.onPlayerFirstTouchTrigger))
		{
			%client.wrenchBrick.createOnPlayerFirstTouchTrigger(onPlayerFirstTouchTriggerData);
		}

		return parent::serverCmdAddEvent(%client, %delay, %input, %target, %a, %b, %output, %par1, %par2, %par3, %par4);
	}

	function serverCmdClearEvents(%client)
	{
		if(isObject(%client.wrenchBrick.onPlayerFirstTouchTrigger))
		{
		    %client.wrenchBrick.onPlayerFirstTouchTrigger.delete();
		}

		parent::serverCmdClearEvents(%client);
	}

	function fxDTSBrick::onDeath(%brick)
	{
		if(isObject(%brick.onPlayerFirstTouchTrigger))
		{
			%brick.onPlayerFirstTouchTrigger.delete();
		}

		parent::onDeath(%brick);
	}

	function fxDTSBrick::onRemove(%brick)
	{
		if(isObject(%brick.onPlayerFirstTouchTrigger))
		{
			%brick.onPlayerFirstTouchTrigger.delete();
		}

		parent::onRemove(%brick);
	}
};
activatePackage(onPlayerFirstTouch);