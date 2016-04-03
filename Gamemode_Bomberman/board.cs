function generateBoard(%length, %width, %density) {
	%density = mClamp(mAbs(%density - 10), 3, 10);
	%datablock = "brick8xCubeData";
	// 16
	for(%x=0;%x<%length;%x++) {
		for(%y=0;%y<%width;%y++) {
			if(%x == 0 || %y == 0 || %x == %length-1 || %y == %width-1) {
				%color = 16;
				%health = -1;
				%isWall = 1;
			} else {
				%color = getRandom(4, 7);
				%health = %color - 3;
				%isWall = 0;
			}

			if(getRandom(0, 10) > %density || %isWall) {
				%brick = new fxDTSBrick() {
					angleID = 0;
					client = 0;
					colorFxID = 0;
					colorID = %color;
					dataBlock = %datablock;
					isBasePlate = 1;
					isPlanted = 1;
					position = %x*4 SPC %y*4 SPC 2;
					printID = 0;
					rotation = "0 0 -1 90";
					scale = "1 1 1";
					shapeFxID = 0;
					stackBL_ID = -1;
					isWall = %isWall;
					health = %health;
				};
				BrickGroup_888888.add(%brick);

				%brick.plant();
				%brick.setTrusted(1);
			}
		}
	}

	%raylength = 4;
	%mask = $TypeMasks::FXBrickObjectType;
	%vectors = "1 0 0\t-1 0 0\t0 1 0\t0 -1 0";

	for(%x=0;%x<%length;%x++) {
		for(%y=0;%y<%width;%y++) {
			%point = %x*4 SPC %y*4 SPC 2;

			%count = 0;
			for(%vector = 0; %vector < getFieldCount(%vectors); %vector++) {
				%raycast = containerRaycast(%point, VectorAdd(%point, VectorScale(getField(%vectors, %vector), %raylength)), %mask);

				%hit = getWord(%raycast, 0);
				if(isObject(%hit)) {
					if(%hit.getClassName() $= "fxDTSBrick") {
						%obj[%count] = %hit;
						%count++;
					}
				}
			}

			initContainerBoxSearch(%point, "1 1 1", %mask);
			%hit = containerSearchNext();

			if(isObject(%hit)) {
				if(%hit.getClassName() $= "fxDTSBrick") {
					%obj[%count] = %hit;
					%count++;
				}
			}

			if(%count >= 4) {
				%remove = %obj[getRandom(0, %count-1)];
				while(%remove.isWall) {
					%remove = %obj[getRandom(0, %count-1)];
				}
				%remove.delete();
			}
		}
	}
}