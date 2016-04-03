datablock AudioProfile(mineHitSound1)
{
	filename = "./sounds/hit1.wav";
	description = AudioClosest3d;
	preload = true;
};
datablock AudioProfile(mineHitSound2 : mineHitSound1) { filename = "./sounds/hit2.wav"; };
datablock AudioProfile(mineHitSound3 : mineHitSound1) { filename = "./sounds/hit3.wav"; };
datablock AudioProfile(mineHitSound4 : mineHitSound1) { filename = "./sounds/hit4.wav"; };
datablock AudioProfile(mineHitSound5 : mineHitSound1) { filename = "./sounds/hit5.wav"; };
datablock AudioProfile(mineHitSound6 : mineHitSound1) { filename = "./sounds/hit6.wav"; };
datablock AudioProfile(mineHitSound7 : mineHitSound1) { filename = "./sounds/hit7.wav"; };
datablock AudioProfile(mineHitSound8 : mineHitSound1) { filename = "./sounds/hit8.wav"; };
datablock AudioProfile(mineHitSound9 : mineHitSound1) { filename = "./sounds/hit9.wav"; };
datablock AudioProfile(mineHitSound10 : mineHitSound1) { filename = "./sounds/hit10.wav"; };
datablock AudioProfile(mineHitSound11 : mineHitSound1) { filename = "./sounds/hit11.wav"; };
datablock AudioProfile(mineHitSound12 : mineHitSound1) { filename = "./sounds/hit12.wav"; };
datablock AudioProfile(mineRemoveSound1 : mineHitSound1) { filename = "./sounds/remove1.wav"; };
datablock AudioProfile(mineRemoveSound2 : mineHitSound1) { filename = "./sounds/remove2.wav"; };
datablock AudioProfile(mineRemoveSound3 : mineHitSound1) { filename = "./sounds/remove3.wav"; };

// took this from an older gamemode of mine

function Player::miningLoop(%this) {
	if(%this.miningLoop) {
		cancel(%this.miningLoop);
	}

	if((%this.currTool !$= "" && %this.currTool != -1) || %this.getState() $= "Dead") {
		%this.stopMining();
		return;
	}

	if(!$Mining::CanPlay) {
		return;
	}

	%this.playThread(1,activate);

	%this.lastTriggered = getSimTime();

	%brick = %this.getLookingAt();

	if(isObject(%brick)) {
		if(%brick.getClassName() !$= "fxDTSBrick") {
			return;
		}

		if(%brick.withinLimits()) {
			%sound = getRandom(1, 12);
			serverPlay3D("mineHitSound" @ %sound, %brick.getPosition());
		
			if(%this.client.team != %brick.team) {
				%sound = getRandom(1, 3);
				serverPlay3D("mineRemoveSound" @ %sound, %brick.getPosition());
			}

			%brick.onMineBrick(%this.client);
		}

		%this.playThread(1,activate);
	}

	%this.miningLoop = %this.schedule($Mining::ActiveLoopData.tempoMS/2, miningLoop);
}

function Player::stopMining(%this) {
	cancel(%this.miningLoop);
}

function Player::getLookingAt(%this,%distance) {
	if(!%distance) {
		%distance = 5;
	}

	%eye = vectorScale(%this.getEyeVector(),%distance);
	%pos = %this.getEyePoint();
	%mask = $TypeMasks::FxBrickObjectType;
	%hit = firstWord(containerRaycast(%pos, vectorAdd(%pos, %eye), %mask, %this));
		
	if(!isObject(%hit)) {
		return;
	}
		
	return %hit;
}

package MiningPickaxePackage {
	function armor::onTrigger(%db, %obj, %slot, %val) {
		if(%obj.getClassName() $= "Player" && !%slot) {
			if(%val == 1) {
				if(%obj.currTool == -1 || %obj.currTool $= "") {
					if(getSimTime() - %obj.lastTriggered > $Mining::ActiveLoopData.tempoMS/2 && $Mining::CanPlay) {
						%obj.miningLoop();
					}
				}
			} else {
				%obj.stopMining();
			}
		}

		return Parent::onTrigger(%db,%obj,%slot,%val);
	}
};
activatePackage(MiningPickaxePackage);