function Player::setUniform(%this) {
	if(%this.client.team $= "") {
		talk(%this SPC "doesn't have a team set!");
		return;
	}

	%nodes = "femchest chest pants helmet copHat knitHat scoutHat bicorn pointyHelmet flareHelmet armor cape bucket skirtHip SkirtTrimRight SkirtTrimLeft";

	%color = getColorIDTable(%this.client.team);

	%this.setShapeNameColor(%color);
	for(%i=0;%i<getWordCount(%nodes);%i++) {
		%node = getWord(%nodes, %i);
		%this.setNodeColor(%node, %color);
	}
}