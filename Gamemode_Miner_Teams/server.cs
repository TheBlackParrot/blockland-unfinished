// = TODO =
// [DONE] arbitrary limits on X/Y/Z axis, 120 per player, highest amount kept until end of round
//		- just let people mine a block infinitely with a warning mentioning the arbitrary limit
// [DONE] increase gun radius maximum by 1
// spear shrapnel bombs
//		- similar to bomb shrapnel in brickochet
// make a bouncy projectile gun out of the printer
// let the wrench reconfigure bombs to blow up using the color of the team
//		- balance this, of course, let explosions be less extreme
// let health slow down players
//		- set suicides to respawn players after 21 seconds
// [DONE] team costumes
// get the sword in-sync with tempo
// teams aren't being resumed correctly, just defaults to red?
// [DONE] rename the "Mining Gun" "Eraser"
// people are crashing and i have no idea why
//		- maybe just resort to making own projectiles/etc.
//			- [DONE] gun
//			- sword
//		- i think changing the filename of sounds /after/ they're initiated is crashing people
// [DONE] increase size of spawn area
// [DONE] make arbitray walls black, cancel anything thrown at it
// prevent team killing
// figure out why $Mining::CanPlay is being ignored
// [FIXED] winners aren't determined correctly

exec("./system.cs");
exec("./bricks.cs");
exec("./pickaxe.cs");
exec("./gun.cs");
exec("./loops.cs");
exec("./player.cs");

$Mining::DefaultColor = 20;

function player::addTool(%this, %item, %slot) {
	%this.tool[%slot] = %item;
	messageClient(%this.client, 'MsgItemPickup', '', %slot, %item);
}

package MiningWorkarounds {
	function GameConnection::spawnPlayer(%this) {
		%r = parent::spawnPlayer(%this);

		%mg = $DefaultMinigame;

		if(isObject(%this.minigame)) {
			if(%this.team $= "") {
				if(%mg.numMembers <= 1) {
					%mg.checkForResetDone();
					return %r;
				}

				for(%i=0;%i<%mg.numMembers;%i++) {
					%team = %mg.member[%i].team;
					%count[%team]++;
				}
				%lowest[amount] = 999999;
				%lowest[team] = 0;
				%equal = 0;
				for(%i=0;%i<%mg.teamCount;%i++) {
					if(!%mg.team[%i, inUse]) {
						continue;
					}

					if(%count[%i] < %lowest) {
						%lowest[amount] = %count[%i];
						%lowest[team] = %i;
					}
					if(%count[%i] == %lowest) {
						%equal++;
					}
				}

				if(%equal == %mg.teamCount-1) {
					%this.camera.setMode("Observer");
					%this.camera.setTransform("0 0 12000");
					%this.setControlObject(%this.camera);
					%this.player.delete();

					return %r;
				} else {
					%this.team = %lowest[team];
				}
			}
		}

		//if(isObject(%this.minigame)) {
		//	%this.player.addTool(nameToID("HammerItem"), 0);
		//}

		%this.player.setUniform();

		%this.centerPrint("\c6You are on the <color:" @ %mg.team[%this.team, colorHex] @ ">" @ strUpr(%mg.team[%this.team, name]) SPC "TEAM", 3);
		%this.player.setTransform(getRandom(-7, 7) SPC getRandom(-7, 7) SPC getRandom(0, 7)+12000);
		%this.player.setVelocity(getRandom(-10, 10) SPC getRandom(-10, 10) SPC getRandom(5, 10));

		return %r;
	}

	function GameConnection::onClientLeaveGame(%this) {
		%mg = $DefaultMinigame;

		if(%this.team !$= "") {
			for(%i=0;%i<%mg.numMembers;%i++) {
				if(%mg.member[%i].team $= "") {
					%mg.member[%i].spawnPlayer();
					break;
				}
			}
		}

		return parent::onClientLeaveGame(%this);
	}

	function onServerDestroyed() {
		%mg = $DefaultMinigame;

		cancel(%mg.resetSched);
		for(%i=0;%i<6;%i++) {
			cancel(%mg.timeLimitSched[%i]);
		}
		cancel(%mg.chainDeleteSched);
		cancel(%mg.resetCheckSched);

		deleteVariables("$Mining::*");

		return parent::onServerDestroyed();
	}
};
activatePackage(MiningWorkarounds);