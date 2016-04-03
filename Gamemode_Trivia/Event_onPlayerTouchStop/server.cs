package StopTouch
{
	function fxDtsBrick::OnPlayerTouch(%this, %obj)
	{
		if(isEventPending(%this.stopTouchSched[%obj]))
		{
			cancel(%this.stopTouchSched[%obj]);
		}
		else
		{
			%this.playersTouching++;
		}
		%this.stopTouchSched[%obj] = %this.schedule(350, "OnPlayerTouchStop", %obj);
		return parent::OnPlayerTouch(%this, %obj);
	}
};
activatepackage(StopTouch);


function fxDtsBrick::OnPlayerTouchStop(%this, %obj)
{
	if(isEventPending(%this.stopTouchSched[%obj]))
	{
		cancel(%obj.stopTouchSched[%obj]);
	}
	
	$InputTarget_["Self"] = %this;

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
			$InputTarget_["MiniGame"] = getMiniGameFromObject(%this);
		}
		else

		{
			$InputTarget_["MiniGame"] = 0;

		}
	}
	%this.processInputEvent("OnPlayerTouchStop", %obj.client);

	%this.playersTouching--;
	if(%this.playersTouching < 0)
	{
		%this.playersTouching = 0;
	}
	if(!%this.playersTouching)
	{
		%this.processInputEvent("OnNoPlayerTouch", %obj.client);
	}
}

registerInputEvent("FxDTSBrick","OnPlayerTouchStop","Self fxDTSBrick" TAB "Player Player" TAB "Client GameConnection" TAB "MiniGame MiniGame");
registerInputEvent("FxDTSBrick","OnNoPlayerTouch","Self fxDTSBrick" TAB "Player Player" TAB "Client GameConnection" TAB "MiniGame MiniGame");