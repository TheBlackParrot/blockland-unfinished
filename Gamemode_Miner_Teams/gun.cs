datablock AudioProfile(mineGunSound1)
{
	filename = "./sounds/gun1.wav";
	description = AudioClose3d;
	preload = true;
};
datablock AudioProfile(mineGunSound2 : mineGunSound1) { filename = "./sounds/gun2.wav"; };
datablock AudioProfile(mineGunSound3 : mineGunSound1) { filename = "./sounds/gun3.wav"; };
datablock AudioProfile(mineGunSound4 : mineGunSound1) { filename = "./sounds/gun4.wav"; };
datablock AudioProfile(mineGunSound5 : mineGunSound1) { filename = "./sounds/gun5.wav"; };
datablock AudioProfile(mineGunSound6 : mineGunSound1) { filename = "./sounds/gun6.wav"; };
datablock AudioProfile(mineGunSound7 : mineGunSound1) { filename = "./sounds/gun7.wav"; };
datablock AudioProfile(mineGunSound8 : mineGunSound1) { filename = "./sounds/gun8.wav"; };
datablock AudioProfile(mineBulletSound1 : mineGunSound1) { filename = "./sounds/bullet1.wav"; };
datablock AudioProfile(mineBulletSound2 : mineGunSound1) { filename = "./sounds/bullet2.wav"; };
datablock AudioProfile(mineBulletSound3 : mineGunSound1) { filename = "./sounds/bullet3.wav"; };
datablock AudioProfile(mineBulletSound4 : mineGunSound1) { filename = "./sounds/bullet4.wav"; };
datablock AudioProfile(mineBulletSound5 : mineGunSound1) { filename = "./sounds/bullet5.wav"; };
datablock AudioProfile(mineBulletSound6 : mineGunSound1) { filename = "./sounds/bullet6.wav"; };
datablock AudioProfile(mineBulletSound7 : mineGunSound1) { filename = "./sounds/bullet7.wav"; };
datablock AudioProfile(mineBulletSound8 : mineGunSound1) { filename = "./sounds/bullet8.wav"; };
datablock AudioProfile(mineSilence : mineGunSound1) { filename = "./sounds/silence.wav"; };

datablock ExplosionData(MiningGunExplosion : gunExplosion) {
	soundProfile = mineSilence;
};
datablock ProjectileData(MiningGunProjectile : gunProjectile) {
	directDamage = 10;
	explosion = MiningGunExplosion;
	uiName = "Eraser Projectile";
};

datablock shapeBaseImageData(MiningGunImage : gunImage) {
	colorShiftColor = "1.000 1.000 1.000 1.000";

	stateName[0] = "Activate";
	stateTimeoutValue[0] = 0;
	stateTransitionOnTimeout[0] = "waitCheckA";

	stateName[1] = "Ready";
	stateTransitionOnTriggerDown[1] = "Fire";
	stateTimeoutValue[1] = 0;

	stateName[2] = "Fire";
	stateScript[2] = "onFire";
	stateTimeoutValue[2] = 0;
	stateAllowImageChange[2] = 1;
	stateSound[2] = mineSilence;

	stateTimeoutValue[3] = 0;
	stateTransitionOnTimeout[3] = "waitCheckA";

	stateTimeoutValue[4] = 0;

	stateTimeoutValue[5] = 0;

	stateName[6] = "waitCheckA";
	stateScript[6] = "onWaitCheck";
	stateTimeoutValue[6] = 0.01;
	stateTransitionOnTimeout[6] = "waitCheckB";

	stateName[7] = "waitCheckB";
	stateTransitionOnNoAmmo[7] = "waitCheckA";
	stateTransitionOnAmmo[7] = "Ready";
	stateTimeoutValue[7] = 0;

	projectile = MiningGunProjectile;
};

datablock ItemData(MiningGunItem : GunItem) {
	uiName = "Eraser";
	colorShiftColor = "1.000 1.000 1.000 1.000";

	image = MiningGunImage;
	canDrop = true;
};


function MiningGunImage::onFire(%this, %obj, %slot) {
	%obj.imageNameFireTime = getSimTime();

	%sound = getRandom(1, 8);
	serverPlay3D("mineGunSound" @ %sound, %obj.getTransform());

	%obj.client.lastFire = getSimTime();

	parent::onFire(%this, %obj, %slot);
}

function MiningGunImage::onWaitCheck(%this, %obj, %slot) {
	if(getSimTime() - %obj.imageNameFireTime >= $Mining::ActiveLoopData.tempoMS-($Mining::ActiveLoopData.tempoMS/10) && $Mining::CanPlay) {
		%obj.setImageAmmo(%slot, 1);
	} else {
		%obj.setImageAmmo(%slot, 0);
	}
}

function MiningGunProjectile::onExplode(%this, %obj) {
	%sound = getRandom(1, 8);
	serverPlay3D("mineBulletSound" @ %sound, %obj.getTransform());

	parent::onExplode(%this, %obj);
}

function MiningGunProjectile::onCollision(%this, %obj, %col, %fade, %pos, %normal, %velocity) {
	if(%col.getClassName() !$= "fxDTSBrick") {
		parent::onCollision(%this, %obj, %col, %fade, %pos, %normal, %velocity);
		return;
	}

	%radius = getRandom(1, 3);
	%col.mineExplode(%radius);

	%obj.client.updateBottomPrint();

	parent::onCollision(%this, %obj, %col, %fade, %pos, %normal, %velocity);
}

swordProjectile.directDamage = 10;