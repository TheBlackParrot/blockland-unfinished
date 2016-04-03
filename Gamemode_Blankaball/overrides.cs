package BlankaBallOverrides {
	function serverCmdSuicide(%client) {
		if(%client.scored) {
			return;
		}
		
		if($Sim::Time - %client.suicideSP < 1) {
			return;
		}
		%client.suicideSP = $Sim::Time;

		if(isObject(%client.blanka)) {
			%client.blanka.explode();
		}

		if(isObject(%client.player)) {
			%client.player.instantRespawn();
		} else {
			%client.spawnPlayer();
		}
	}

	function Player::instantRespawn(%this) {
		if(isObject(%this.blanka)) {
			%this.blanka.explode();
		}

		parent::instantRespawn(%this);
	}

	function GameConnection::spawnPlayer(%this) {
		parent::spawnPlayer(%this);

		%this.scored = 0;
	}

	function GameConnection::onClientLeaveGame(%this) {
		$DefaultMinigame.checkRemaining();

		return parent::onClientLeaveGame(%this);
	}
};
activatePackage(BlankaBallOverrides);