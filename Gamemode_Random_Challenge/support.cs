function StructureContainer::plantBrick(%this, %datablock, %pos, %rot, %color) {
	%brick = new fxDTSBrick() {
		angleID = 0;
		colorFxID = 0;
		colorID = %color;
		dataBlock = %datablock;
		isBasePlate = 1;
		isPlanted = 1;
		position = %pos;
		printID = 0;
		scale = "1 1 1";
		shapeFxID = 0;
		stackBL_ID = %this.client.bl_id;
		enableTouch = 1;
		rotation = %rot;
	};

	//talk(%brick);

	%brick.plant();
	%brick.setTrusted(1);

	%this.client.brickgroup.add(%brick);

	return %brick.getWorldBox() TAB %rot TAB %datablock.brickSizeX/2 SPC %datablock.brickSizeY/2 SPC %datablock.brickSizeZ/5 TAB %brick.getPosition();
}