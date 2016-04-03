exec("./scoring.cs");
exec("./board.cs");
exec("./system.cs");
exec("./overrides.cs");

package BlankaBallGamePackage {
	function blankaBallImage::onFire(%this, %obj, %slot) {
		%client = %obj.client;
		%client.scored = 0;

		return parent::onFire(%this, %obj, %slot);
	}

	function Projectile::onAdd(%this, %obj) {
		if(%obj.getDatablock().getName() $= "blankaBallProjectile") {
			%client = %obj.client;
			%client.blanka = %obj;

			%transform = %obj.getTransform();
			%client.camera.setOrbitMode(%obj, %transform, 0.5, 8, 8, 1);
			%client.camera.mode = "Orbit";
			%client.setControlObject(%client.camera);

			if(isObject(%client.player)) {
				%client.player.schedule(33, delete);
			}
		}

		return parent::onAdd(%this, %obj);
	}

	function blankaBallProjectile::onCollision(%this, %obj, %col, %fade, %pos, %normal) {
		%client = %obj.client;

		if(%col.getClassName() $= "fxDTSBrick") {
			if(%col.getName() $= "_scoreBrick") {
				%col.doBlankaScore(%client);
				return parent::onCollision(%this, %obj, %col, %fade, %pos, %normal);
			}
		}

		%client.instantRespawn();
		return parent::onCollision(%this, %obj, %col, %fade, %pos, %normal);
	}

	function MinigameSO::checkLastManStanding(%this) {
		// custom
		return;
	}
};
activatePackage(BlankaBallGamePackage);