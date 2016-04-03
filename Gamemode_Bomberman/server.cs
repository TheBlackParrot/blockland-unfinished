exec("./other/Player_TopDown.cs");
exec("./board.cs");

datablock ExplosionData(rocketNoDamageExplosion : rocketExplosion) {
	damageRadius = 0;
	radiusDamage = 0;

	impulseRadius = 0;
	impulseForce = 0;
};

datablock ProjectileData(rocketNoDamageProjectile : rocketLauncherProjectile) {
	directDamage = 0;
	impactImpulse = 0;
	verticalImpulse = 0;

	explosion = rocketNoDamageExplosion;

	uiName = "No Damage Rocket";
};


function fxDTSBrick::setHealth(%this, %amnt) {
	if(%this.isWall) {
		return;
	}
	
	if(!%amnt) {
		%this.killBrick();
		return;
	}

	%this.setColor(%amnt+3);
	%this.health = %amnt;
}

function Player::placeBomb(%this) {
	%brick = new fxDTSBrick() {
		angleID = 0;
		client = %this.client;
		colorFxID = 3;
		colorID = 0;
		dataBlock = "brick4xCubeData";
		isBasePlate = 1;
		isPlanted = 1;
		position = getWords(%this.getPosition(), 0, 1) SPC 2;
		printID = 0;
		rotation = "0 0 -1 90";
		scale = "1 1 1";
		shapeFxID = 0;
		stackBL_ID = %this.client.bl_id;
		isBomb = 1;
		ticks = 0;
	};
	%this.client.brickgroup.add(%brick);

	%brick.plant();
	%brick.setTrusted(1);

	%brick.setColliding(0);
	%brick.setRaycasting(0);

	%brick.doExplosion();
}

function fxDTSBrick::doExplosion(%this) {
	cancel(%this.explosionCountdown);
	if(%this.ticks < 8) {
		%this.explosionCountdown = %this.schedule(500, doExplosion);
	} else {
		%this.explode();
		return;
	}

	if(%this.colorID == 0) {
		%this.setColor(15);
	} else {
		%this.setColor(0);
	}

	%this.ticks++;
}

function fxDTSBrick::explode(%this) {
	%raylength = 24;
	%mask = $TypeMasks::FXBrickObjectType;
	%vectors = "1 0 0\t-1 0 0\t0 1 0\t0 -1 0";
	%point = %this.getPosition();

	%count = 0;
	for(%vector = 0; %vector < getFieldCount(%vectors); %vector++) {
		%raycast = containerRaycast(%point, VectorAdd(%point, VectorScale(getField(%vectors, %vector), %raylength)), %mask);

		%hit = getWord(%raycast, 0);
		if(isObject(%hit)) {
			if(%hit.getClassName() $= "fxDTSBrick") {
				%hit.setHealth(%hit.health - 1);
			}
		}
	}

	%this.spawnExplosion(rocketNoDamageProjectile);
	%this.killBrick();
}

package BombermanBombPackage {
	function Armor::onTrigger(%this, %player, %slot, %val) {
		if(%slot == 4 && %val) {
			%player.placeBomb();
		}
		return parent::onTrigger(%this, %player, %slot, %val);
	}
};
activatePackage(BombermanBombPackage);